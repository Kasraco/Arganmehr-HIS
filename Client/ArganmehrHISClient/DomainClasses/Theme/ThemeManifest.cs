using Common.Infrastructure;
using Common.Helpers.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using Common.Collections;

namespace DomainClasses.Theme
{
    public class ThemeManifest : ComparableObject<ThemeManifest>
    {

        public ThemeManifest()
        {
        }

        #region Methods

        public static ThemeManifest Create(string themePath, string virtualBasePath = "~/Themes/")
        {

            var folderData = CreateThemeFolderData(themePath, virtualBasePath);
            if (folderData != null)
            {
                return Create(folderData);
            }

            return null;
        }

        public static ThemeManifest Create(ThemeFolderData folderData)
        {

            var materializer = new ThemeManifestMaterializer(folderData);
            var manifest = materializer.Materialize();

            return manifest;
        }

        public static ThemeFolderData CreateThemeFolderData(string themePath, string virtualBasePath = "~/Themes/")
        {
            if (themePath.IsEmpty())
                return null;

            virtualBasePath = virtualBasePath.EnsureEndsWith("/");
            var themeDirectory = new DirectoryInfo(themePath);
            var themeConfigFile = new FileInfo(System.IO.Path.Combine(themeDirectory.FullName, "theme.config"));

            if (themeConfigFile.Exists)
            {
                var doc = new XmlDocument();
                doc.Load(themeConfigFile.FullName);

          
                var root = doc.DocumentElement;

                var baseTheme = root.GetAttribute("baseTheme").TrimSafe().NullEmpty();
                if (baseTheme != null && baseTheme.IsCaseInsensitiveEqual(themeDirectory.Name))
                {
                    // Don't let theme point to itself!
                    baseTheme = null;
                }

                return new ThemeFolderData
                {
                    FolderName = themeDirectory.Name,
                    FullPath = themeDirectory.FullName,
                    Configuration = doc,
                    VirtualBasePath = virtualBasePath,
                    BaseTheme = baseTheme
                };
            }

            return null;
        }

        #endregion

        #region Properties

        public XmlElement ConfigurationNode
        {
            get;
             set;
        }

        /// <summary>
        /// Gets the virtual theme base path (e.g.: ~/Themes)
        /// </summary>
        public string Location
        {
            get;
             set;
        }

        /// <summary>
        /// Gets the physical path of the theme
        /// </summary>
        public string Path
        {
            get;
             set;
        }

        public string PreviewImageUrl
        {
            get;
             set;
        }

        public string PreviewText
        {
            get;
             set;
        }

        public bool SupportRtl
        {
            get;
             set;
        }

        public bool MobileTheme
        {
            get;
            set;
        }

        [ObjectSignature]
        public string ThemeName
        {
            get;
            set;
        }

        public string BaseThemeName
        {
            get;
            set;
        }

        public ThemeManifest BaseTheme
        {
            get;
            set;
        }

        public string ThemeTitle
        {
            get;
             set;
        }

        public string Author
        {
            get;
            set;
        }

        public string Url
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        private IDictionary<string, ThemeVariableInfo> _variables;
        public IDictionary<string, ThemeVariableInfo> Variables
        {
            get
            {
                if (this.BaseTheme == null)
                {
                    return _variables;
                }

                var baseVars = this.BaseTheme.Variables;
                var merged = new Dictionary<string, ThemeVariableInfo>(baseVars, StringComparer.OrdinalIgnoreCase);
                foreach (var localVar in _variables)
                {
                    if (merged.ContainsKey(localVar.Key))
                    {
                        // Overridden var in child: update existing.
                        var baseVar = merged[localVar.Key];
                        merged[localVar.Key] = new ThemeVariableInfo
                        {
                            Name = baseVar.Name,
                            Type = baseVar.Type,
                            SelectRef = baseVar.SelectRef,
                            DefaultValue = localVar.Value.DefaultValue,
                            Manifest = localVar.Value.Manifest
                        };
                    }
                    else
                    {
                        // New var in child: add to list.
                        merged.Add(localVar.Key, localVar.Value);
                    }
                }

                return merged;
            }
            set
            {
                _variables = value;
            }
        }

        private Multimap<string, string> _selects;
        public Multimap<string, string> Selects
        {
            get
            {
                if (this.BaseTheme == null)
                {
                    return _selects;
                }

                var baseSelects = this.BaseTheme.Selects;
                var merged = new Multimap<string, string>();
                baseSelects.Each(x => merged.AddRange(x.Key, x.Value));
                foreach (var localSelect in _selects)
                {
                    if (!merged.ContainsKey(localSelect.Key))
                    {
                        // New Select in child: add to list.
                        merged.AddRange(localSelect.Key, localSelect.Value);
                    }
                    else
                    {
                        // Do nothing: we don't support overriding Selects
                    }
                }

                return merged;
            }
            set
            {
                _selects = value;
            }
        }

        private ThemeManifestState _state;
        public ThemeManifestState State
        {
            get
            {
                if (_state == ThemeManifestState.Active)
                {
                    // active state does not mean, that it actually IS active: check state of base themes!
                    var baseTheme = this.BaseTheme;
                    while (baseTheme != null)
                    {
                        if (baseTheme.State != ThemeManifestState.Active)
                        {
                            return baseTheme.State;
                        }
                        baseTheme = baseTheme.BaseTheme;
                    }
                }

                return _state;
            }
            set
            {
                _state = value;
            }
        }

        public string FullPath
        {
            get { return System.IO.Path.Combine(this.Path, "theme.config"); }
        }

        public override string ToString()
        {
            return "{0} (Parent: {1}, State: {2})".FormatInvariant(ThemeName, BaseThemeName ?? "-", State.ToString());
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                BaseTheme = null;
                if (_variables != null)
                {
                    foreach (var pair in _variables)
                    {
                        pair.Value.Dispose();
                    }
                    _variables.Clear();
                }
            }
        }

        ~ThemeManifest()
        {
            Dispose(false);
        }

        #endregion
    }

    public enum ThemeManifestState
    {
        MissingBaseTheme = -1,
        Active = 0,
    }
}
