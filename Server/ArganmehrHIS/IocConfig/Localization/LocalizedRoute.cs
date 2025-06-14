﻿using Common.Infrastructure;
using ServiceLayer.Congfiguration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IocConfig.Localization
{
    public class LocalizedRoute : Route, ICloneable<LocalizedRoute>
    {
        #region Fields

        private bool? _seoFriendlyUrlsForLanguagesEnabled;
        private string _leftPart;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern and handler class.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LocalizedRoute(string url, IRouteHandler routeHandler)
            : base(url, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class and default parameter values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LocalizedRoute(string url, RouteValueDictionary defaults, IRouteHandler routeHandler)
            : base(url, defaults, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values and constraints.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LocalizedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, IRouteHandler routeHandler)
            : base(url, defaults, constraints, routeHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the System.Web.Routing.Route class, using the specified URL pattern, handler class, default parameter values, 
        /// constraints,and custom values.
        /// </summary>
        /// <param name="url">The URL pattern for the route.</param>
        /// <param name="defaults">The values to use if the URL does not contain all the parameters.</param>
        /// <param name="constraints">A regular expression that specifies valid values for a URL parameter.</param>
        /// <param name="dataTokens">Custom values that are passed to the route handler, but which are not used to determine whether the route matches a specific URL pattern. The route handler might need these values to process the request.</param>
        /// <param name="routeHandler">The object that processes requests for the route.</param>
        public LocalizedRoute(string url, RouteValueDictionary defaults, RouteValueDictionary constraints, RouteValueDictionary dataTokens, IRouteHandler routeHandler)
            : base(url, defaults, constraints, dataTokens, routeHandler)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns information about the requested route.
        /// </summary>
        /// <param name="httpContext">An object that encapsulates information about the HTTP request.</param>
        /// <returns>
        /// An object that contains the values from the route definition.
        /// </returns>
        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            if ( this.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var helper = new LocalizedUrlHelper(httpContext.Request);
                if (helper.IsLocalizedUrl())
                {
                    helper.StripSeoCode();
                    httpContext.RewritePath("~/" + helper.RelativePath, true);
                }
            }

            if (_leftPart == null)
            {
                var url = this.Url;
                int idx = url.IndexOf('{');
                _leftPart = "~/" + (idx >= 0 ? url.Substring(0, idx) : url).TrimEnd('/');
            }

            // Perf
            if (!httpContext.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_leftPart, true, CultureInfo.InvariantCulture))
                return null;

            RouteData data = base.GetRouteData(httpContext);
            return data;
        }

        /// <summary>
        /// Returns information about the URL that is associated with the route.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the requested route.</param>
        /// <param name="values">An object that contains the parameters for a route.</param>
        /// <returns>
        /// An object that contains information about the URL that is associated with the route.
        /// </returns>
        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            VirtualPathData data = base.GetVirtualPath(requestContext, values);

            if (data != null &&  this.SeoFriendlyUrlsForLanguagesEnabled)
            {
                var helper = new LocalizedUrlHelper(requestContext.HttpContext.Request, true);
                string cultureCode;
                if (helper.IsLocalizedUrl(out cultureCode))
                {
                    if (!requestContext.RouteData.Values.ContainsKey("StripInvalidSeoCode"))
                    {
                        data.VirtualPath = String.Concat(cultureCode, "/", data.VirtualPath);
                    }
                }
            }
            return data;
        }

        public virtual void ClearSeoFriendlyUrlsCachedValue()
        {
            _seoFriendlyUrlsForLanguagesEnabled = null;
        }

        #endregion

        #region Properties

        public bool IsClone { get; private set; }

        protected bool SeoFriendlyUrlsForLanguagesEnabled
        {
            get
            {
                if (!_seoFriendlyUrlsForLanguagesEnabled.HasValue)
                    _seoFriendlyUrlsForLanguagesEnabled = ProjectObjectFactory.Container.GetInstance<LocalizationSettings>().SeoFriendlyUrlsForLanguagesEnabled;

                return _seoFriendlyUrlsForLanguagesEnabled.Value;
            }
        }

        #endregion

        #region Clone Members

        public LocalizedRoute Clone()
        {
            var clone = new LocalizedRoute(this.Url,
                new RouteValueDictionary(this.Defaults),
                new RouteValueDictionary(this.Constraints),
                new RouteValueDictionary(this.DataTokens),
                new MvcRouteHandler());
            clone.RouteExistingFiles = this.RouteExistingFiles;
            clone._seoFriendlyUrlsForLanguagesEnabled = this._seoFriendlyUrlsForLanguagesEnabled;
            clone.IsClone = true;
            return clone;
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        #endregion
    }
}
