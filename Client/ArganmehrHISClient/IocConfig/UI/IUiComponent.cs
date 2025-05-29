using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IocConfig.UI
{

    public interface IHtmlAttributesContainer
    {

        IDictionary<string, object> HtmlAttributes
        {
            get;
        }

    }
    public interface IUiComponent : IHtmlAttributesContainer
    {

        string Id
        {
            get;
        }

        string Name
        {
            get;
        }

        bool NameIsRequired
        {
            get;
        }

    }
}
