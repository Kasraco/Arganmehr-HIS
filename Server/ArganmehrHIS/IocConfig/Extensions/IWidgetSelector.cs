using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IocConfig.Extensions
{
    public interface IWidgetSelector
    {
        IEnumerable<WidgetRouteInfo> GetWidgets(string widgetZone, object model);
    }
}