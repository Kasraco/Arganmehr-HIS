using IocConfig.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IocConfig.Extensions
{
    public static class HtmlHelperExtensions
    {

        public static ComponentFactory Arganmehr(this HtmlHelper helper)
        {
            return new ComponentFactory(helper);
        }

    }
}
