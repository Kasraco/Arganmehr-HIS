﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace IocConfig.UI
{
    public abstract class Component : IUiComponent
    {

        protected Component()
        {
            this.HtmlAttributes = new RouteValueDictionary();
        }

        public string Id
        {
            get
            {
                return !this.HtmlAttributes.ContainsKey("id")
                    ? this.Name
                    : (this.HtmlAttributes["id"] as string);
            }
            set
            {
                this.HtmlAttributes["id"] = value;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public IDictionary<string, object> HtmlAttributes
        {
            get;
            private set;
        }



        public virtual bool NameIsRequired
        {
            get
            {
                return false;
            }
        }
    }
}
