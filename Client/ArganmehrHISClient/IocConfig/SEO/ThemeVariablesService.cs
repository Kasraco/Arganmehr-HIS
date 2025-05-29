using Common.Caching;
using Common.Helpers.Extentions;
using DataLayer.Context;
using DomainClasses.Entities.Theme;
using DomainClasses.Theme;
using ServiceLayer.Contracts.AM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;



namespace IocConfig.SEO
{
    public class ThemeVariablesService : IThemeVariablesService
    {
        private const string THEMEVARS_BY_THEME_KEY = "Arganmehr.themevars.theme-{0}-{1}";
        private const string THEMEVARS_PATTERN_KEY = "Arganmehr.themevars.";

        private readonly IDbSet<ThemeVariable> _rsVariables;
        private readonly IThemeRegistry _themeRegistry;
        private readonly ICacheManagerKRB _cacheManager;
        private readonly IUnitOfWork _uow;

        public ThemeVariablesService(
            IUnitOfWork uow,
            IThemeRegistry themeRegistry,
            ICacheManagerKRB cacheManager)
        {
            this._uow = uow;
            this._rsVariables = _uow.Set<ThemeVariable>();
            this._themeRegistry = themeRegistry;
            this._cacheManager = cacheManager;
        }

        public virtual ExpandoObject GetThemeVariables(string themeName, int storeId)
        {
            if (themeName.IsEmpty())
                return null;

            if (!_themeRegistry.ThemeManifestExists(themeName))
                return null;

            string key = string.Format(THEMEVARS_BY_THEME_KEY, themeName, storeId);
            return _cacheManager.Get(key, () =>
            {
                var result = new ExpandoObject();
                var dict = result as IDictionary<string, object>;

                // first get all default (static) var values from manifest...
                var manifest = _themeRegistry.GetThemeManifest(themeName);
                manifest.Variables.Values.Each(v =>
                {
                    dict.Add(v.Name, v.DefaultValue);
                });

                // ...then merge with persisted runtime records
                var query = from v in _rsVariables.AsNoTracking()
                            where v.StoreId == storeId && v.Theme.Equals(themeName, StringComparison.OrdinalIgnoreCase)
                            select v;
                query.Each(v =>
                {
                    if (v.Value.HasValue() && dict.ContainsKey(v.Name))
                    {
                        dict[v.Name] = v.Value;
                    }
                });

                return result;
            });
        }

        public virtual void DeleteThemeVariables(string themeName, int storeId)
        {


            var query = from v in _rsVariables.AsNoTracking()
                        where v.StoreId == storeId && v.Theme.Equals(themeName, StringComparison.OrdinalIgnoreCase)
                        select v;

            if (query.Any())
            {
                
                    query.Each(v =>
                    {
                        _rsVariables.Remove(v);
                    });

                    _cacheManager.Remove(THEMEVARS_BY_THEME_KEY.FormatInvariant(themeName, storeId));

                    _uow.SaveChanges();
                
            }
        }

        public virtual int SaveThemeVariables(string themeName, int storeId, IDictionary<string, object> variables)
        {


            if (!variables.Any())
                return 0;

            var count = 0;
            var infos = _themeRegistry.GetThemeManifest(themeName).Variables;

            
                var unsavedVars = new List<string>();
                var savedThemeVars = _rsVariables.AsNoTracking().Where(v => v.StoreId == storeId && v.Theme.Equals(themeName, StringComparison.OrdinalIgnoreCase)).ToList();
                bool touched = false;

                foreach (var v in variables)
                {
                    ThemeVariableInfo info;
                    if (!infos.TryGetValue(v.Key, out info))
                    {
                        // var not specified in metadata so don't save
                        // TODO: (MC) delete from db also if it exists
                        continue;
                    }

                    var value = v.Value == null ? string.Empty : v.Value.ToString();

                    var savedThemeVar = savedThemeVars.FirstOrDefault(x => x.Name == v.Key);
                    if (savedThemeVar != null)
                    {
                        if (value.IsEmpty() || String.Equals(info.DefaultValue, value, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // it's either null or the default value, so delete
                            _rsVariables.Remove(savedThemeVar);
                            touched = true;
                            count++;
                        }
                        else
                        {
                            // update entity
                            if (!savedThemeVar.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                            {
                                savedThemeVar.Value = value;
                                touched = true;
                                count++;
                            }
                        }
                    }
                    else
                    {
                        if (value.HasValue() && !String.Equals(info.DefaultValue, value, StringComparison.CurrentCultureIgnoreCase))
                        {
                            // insert entity (only when not default value)
                            unsavedVars.Add(v.Key);
                            savedThemeVar = new ThemeVariable
                            {
                                Theme = themeName,
                                Name = v.Key,
                                Value = value,
                                StoreId = storeId
                            };
                            _rsVariables.Add(savedThemeVar);
                            touched = true;
                            count++;
                        }
                    }
                }

                if (touched)
                {
                    _uow.SaveChanges();
                }
            

            return count;
        }

        public int ImportVariables(string themeName, int storeId, string configurationXml)
        {

            var dict = new Dictionary<string, object>();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(configurationXml);

            string forTheme = xmlDoc.DocumentElement.GetAttribute("for");
            if (!forTheme.Equals(themeName, StringComparison.CurrentCultureIgnoreCase))
            {
                throw new Exception("The theme reference in the import file ('{0}') does not match the current theme '{1}'.".FormatCurrent(forTheme.ToSafe(), themeName));
            }

            var xndVars = xmlDoc.DocumentElement.SelectNodes("Var").Cast<XmlElement>();
            foreach (var xel in xndVars)
            {
                string name = xel.GetAttribute("name");
                string value = xel.InnerText;

                if (name.IsEmpty() || value.IsEmpty())
                {
                    continue;
                }

                dict.Add(name, value);
            }

            return this.SaveThemeVariables(themeName, storeId, dict);
        }

        public string ExportVariables(string themeName, int storeId)
        {

            var vars = this.GetThemeVariables(themeName, storeId) as IDictionary<string, object>;

            if (vars == null || !vars.Any())
                return null;

            var infos = _themeRegistry.GetThemeManifest(themeName).Variables;

            var sb = new StringBuilder();

            using (var xmlWriter = XmlWriter.Create(sb))
            {

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("ThemeVars");
                xmlWriter.WriteAttributeString("for", themeName);

                foreach (var kvp in vars)
                {
                    string name = kvp.Key;
                    string value = kvp.Value.ToString();

                    ThemeVariableInfo info;
                    if (!infos.TryGetValue(name, out info))
                    {
                        // var not specified in metadata so don't export
                        continue;
                    }

                    xmlWriter.WriteStartElement("Var");
                    xmlWriter.WriteAttributeString("name", name);
                    xmlWriter.WriteAttributeString("type", info.TypeAsString);
                    xmlWriter.WriteString(value);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
            }

            return sb.ToString();
        }
    }
}
