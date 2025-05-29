using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IocConfig;
using DomainClasses.Localization;
using ServiceLayer.ThemService;
using ServiceLayer.Context;
using DomainClasses.Theme;
using IocConfig.Theming;
using System.Dynamic;
using Common.Helpers.Extentions;
using Common.Data;
using System.Web.WebPages;
using System.IO;
using System.Web.Mvc;

namespace Arganmehr.Web.Themeing  //IocConfig.Theming
{
    public abstract class WebViewPage<TModel> : System.Web.Mvc.WebViewPage<TModel>
    {
        private IText _text;
        private IWorkContext _workContext;

      
        private IThemeRegistry _themeRegistry;
        private IThemeContext _themeContext;
        private ExpandoObject _themeVars;
        private bool? _isHomePage;
        private int? _currentCategoryId;
        private int? _currentManufacturerId;
        private int? _currentProductId;

        protected int CurrentCategoryId
        {
            get
            {
                if (!_currentCategoryId.HasValue)
                {
                    int id = 0;
                    var routeValues = this.Url.RequestContext.RouteData.Values;
                    if (routeValues["controller"].ToString().IsCaseInsensitiveEqual("catalog") && routeValues["action"].ToString().IsCaseInsensitiveEqual("category"))
                    {
                        id = Convert.ToInt32(routeValues["categoryId"].ToString());
                    }
                    _currentCategoryId = id;
                }

                return _currentCategoryId.Value;
            }
        }

        protected int CurrentManufacturerId
        {
            get
            {
                if (!_currentManufacturerId.HasValue)
                {
                    var routeValues = this.Url.RequestContext.RouteData.Values;
                    int id = 0;
                    if (routeValues["controller"].ToString().IsCaseInsensitiveEqual("catalog") && routeValues["action"].ToString().IsCaseInsensitiveEqual("manufacturer"))
                    {
                        id = Convert.ToInt32(routeValues["manufacturerId"].ToString());
                    }
                    _currentManufacturerId = id;
                }

                return _currentManufacturerId.Value;
            }
        }

        protected int CurrentProductId
        {
            get
            {
                if (!_currentProductId.HasValue)
                {
                    var routeValues = this.Url.RequestContext.RouteData.Values;
                    int id = 0;
                    if (routeValues["controller"].ToString().IsCaseInsensitiveEqual("product") && routeValues["action"].ToString().IsCaseInsensitiveEqual("productdetails"))
                    {
                        id = Convert.ToInt32(routeValues["productId"].ToString());
                    }
                    _currentProductId = id;
                }

                return _currentProductId.Value;
            }
        }

        protected bool IsHomePage
        {
            get
            {
                if (!_isHomePage.HasValue)
                {
                    var routeData = this.Url.RequestContext.RouteData;
                    _isHomePage = routeData.GetRequiredString("controller").IsCaseInsensitiveEqual("Home") &&
                        routeData.GetRequiredString("action").IsCaseInsensitiveEqual("Index");
                }

                return _isHomePage.Value;
            }
        }

      

     
      

        /// <summary>
        /// Get a localized resource
        /// </summary>
        public Localizer T
        {
            get
            {
                return _text.Get;
            }
        }

        public IWorkContext WorkContext
        {
            get
            {
                return _workContext;
            }
        }

        public override void InitHelpers()
        {
            base.InitHelpers();

            //if (DataSettings.DatabaseIsInstalled())
            //{
        
//
                _text = ProjectObjectFactory.Container.GetInstance<IText>();
                _workContext = ProjectObjectFactory.Container.GetInstance<IWorkContext>();
            //}
        }

        public HelperResult RenderWrappedSection(string name, object wrapperHtmlAttributes)
        {
            Action<TextWriter> action = delegate(TextWriter tw)
            {
                var htmlAttributes = HtmlHelper.AnonymousObjectToHtmlAttributes(wrapperHtmlAttributes);
                var tagBuilder = new TagBuilder("div");
                tagBuilder.MergeAttributes(htmlAttributes);

                var section = this.RenderSection(name, false);
                if (section != null)
                {
                    tw.Write(tagBuilder.ToString(TagRenderMode.StartTag));
                    section.WriteTo(tw);
                    tw.Write(tagBuilder.ToString(TagRenderMode.EndTag));
                }
            };
            return new HelperResult(action);
        }

        public HelperResult RenderSection(string sectionName, Func<object, HelperResult> defaultContent)
        {
            return this.IsSectionDefined(sectionName) ? this.RenderSection(sectionName) : defaultContent(new object());
        }

        public override string Layout
        {
            get
            {
                var layout = base.Layout;

                if (!string.IsNullOrEmpty(layout))
                {
                    var filename = System.IO.Path.GetFileNameWithoutExtension(layout);
                    ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindView(this.ViewContext.Controller.ControllerContext, filename, "");

                    if (viewResult.View != null && viewResult.View is RazorView)
                    {
                        layout = (viewResult.View as RazorView).ViewPath;
                    }
                }

                return layout;
            }
            set
            {
                base.Layout = value;
            }
        }

        /// <summary>
        /// Return a value indicating whether the working language and theme support RTL (right-to-left)
        /// </summary>
        /// <returns></returns>
        public bool ShouldUseRtlTheme()
        {
            var supportRtl = _workContext.WorkingLanguage.Rtl;
            if (supportRtl)
            {
                //ensure that the active theme also supports it
                supportRtl = this.ThemeManifest.SupportRtl;
            }
            return supportRtl;
        }

        /// <summary>
        /// Gets the manifest of the current active theme
        /// </summary>
        protected ThemeManifest ThemeManifest
        {
            get
            {
                EnsureThemeContextInitialized();
                return _themeContext.CurrentTheme;
            }
        }

        /// <summary>
        /// Gets the current theme name. Override this in configuration views.
        /// </summary>
        [Obsolete("The theme name gets resolved automatically now. No need to override anymore.")]
        protected virtual string ThemeName
        {
            get
            {
                EnsureThemeContextInitialized();
                return _themeContext.WorkingDesktopTheme;
            }
        }

        /// <summary>
        /// Gets the runtime theme variables as specified in the theme's config file
        /// alongside the merged user-defined variables
        /// </summary>
        public dynamic ThemeVariables
        {
            get
            {
                if (_themeVars == null)
                {
                    var storeContext = ProjectObjectFactory.Container.GetInstance<ICAContext>();
                    var repo = new ThemeVarsRepository();
                    _themeVars = repo.GetRawVariables(this.ThemeManifest.ThemeName, storeContext.CurrentCA.Id);
                }

                return _themeVars;
            }
        }

        public string GetThemeVariable(string varname, string defaultValue = "")
        {
            return GetThemeVariable<string>(varname, defaultValue);
        }

        /// <summary>
        /// Gets a runtime theme variable value
        /// </summary>
        /// <param name="varName">The name of the variable</param>
        /// <param name="defaultValue">The default value to return if the variable does not exist</param>
        /// <returns>The theme variable value</returns>
        public T GetThemeVariable<T>(string varName, T defaultValue = default(T))
        {
            

            var vars = this.ThemeVariables as IDictionary<string, object>;
            if (vars != null && vars.ContainsKey(varName))
            {
                string value = vars[varName] as string;
                if (!value.HasValue())
                {
                    return defaultValue;
                }
                return (T)value.Convert(typeof(T));
            }

            return defaultValue;
        }

        public static string GenerateHelpUrl(string path) 
        {
            return ArganmehrVersion.GenerateHelpUrl(path);
        }

        private void EnsureThemeContextInitialized()
        {
            
            if (_themeContext == null)
                _themeContext = ProjectObjectFactory.Container.GetInstance<IThemeContext>();
            if (_themeRegistry == null)
                _themeRegistry = ProjectObjectFactory.Container.GetInstance<IThemeRegistry>();
        }

    }

    public abstract class WebViewPage : WebViewPage<dynamic>
    {
    }
}
