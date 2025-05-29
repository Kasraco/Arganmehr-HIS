using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace Common.Library
{
    public class RouteInfo
    {
        public RouteInfo(RouteInfo cloneFrom)
            : this(cloneFrom.Action, cloneFrom.Controller, new RouteValueDictionary(cloneFrom.RouteValues))
        {
           
        }

        public RouteInfo(string action, string controller, object routeValues)
            : this(action, controller, new RouteValueDictionary(routeValues))
        {
            
        }

        public RouteInfo(string action, string controller, IDictionary<string, object> routeValues)
            : this(action, controller, new RouteValueDictionary(routeValues))
        {
            
        }

        public RouteInfo(string action, string controller, RouteValueDictionary routeValues)
        {
          
            this.Action = action;
            this.Controller = controller;
            this.RouteValues = routeValues;
        }

        public string Action
        {
            get;
            private set;
        }

        public string Controller
        {
            get;
            private set;
        }

        public RouteValueDictionary RouteValues
        {
            get;
            private set;
        }

    }
}
