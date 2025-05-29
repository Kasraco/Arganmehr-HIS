using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers.Extentions;

namespace IocConfig.UI
{
    public static class ComponentRendererUtils
    {

        public static void AppendCssClass(this IDictionary<string, object> attributes, string @class)
        {
            attributes.AppendInValue("class", " ", @class);
        }

        public static void PrependCssClass(this IDictionary<string, object> attributes, string @class)
        {
            attributes.PrependInValue("class", " ", @class);
        }

    }
}
