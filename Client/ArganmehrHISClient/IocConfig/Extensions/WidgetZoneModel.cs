using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace IocConfig.Extensions
{
    public class WidgetZoneModel : ModelBase
    {
        public IEnumerable<WidgetRouteInfo> Widgets { get; set; }
        public string WidgetZone { get; set; }
        public object Model { get; set; }
    }
}
