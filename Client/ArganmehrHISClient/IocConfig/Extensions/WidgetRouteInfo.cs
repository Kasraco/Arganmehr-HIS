using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using ViewModel;

namespace IocConfig.Extensions
{
    public partial class WidgetRouteInfo : ModelBase
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
        public int Order { get; set; }
    }
}
