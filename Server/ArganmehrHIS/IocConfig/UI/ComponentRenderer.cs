﻿using Common.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Common.Helpers;
using Common.Helpers.Extentions;

namespace IocConfig.UI
{
    public abstract class ComponentRenderer<TComponent> : IHtmlString where TComponent : Component
    {

        protected ComponentRenderer()
        {
        }

        protected ComponentRenderer(TComponent component)
        {
            this.Component = component;
        }

        protected internal TComponent Component
        {
            get;
            set;
        }

        protected internal ViewContext ViewContext
        {
            get;
            internal set;
        }

        protected internal ViewDataDictionary ViewData
        {
            get;
            internal set;
        }

        public virtual void VerifyState()
        {
            if (this.Component.NameIsRequired && !this.Component.Id.HasValue())
            {
                throw Error.InvalidOperation("A component must have a unique 'Name'. Please provide a name.");
            }
        }

        protected void WriteHtml(HtmlTextWriter writer)
        {
            this.VerifyState();
            this.Component.Id = SanitizeId(this.Component.Id);

            this.WriteHtmlCore(writer);
        }

        protected virtual void WriteHtmlCore(HtmlTextWriter writer)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            using (HtmlTextWriter htmlTextWriter = new HtmlTextWriter(this.ViewContext.Writer))
            {
                this.WriteHtml(htmlTextWriter);
            }
        }

        public string ToHtmlString()
        {
            string str;
            using (var stringWriter = new StringWriter())
            {
                using (var htmlWriter = new HtmlTextWriter(stringWriter))
                {
                    this.WriteHtml(htmlWriter);
                    str = stringWriter.ToString();
                }
            }
            return str;
        }


        protected string SanitizeId(string id)
        {
            return id.SanitizeHtmlId();
        }

    }
}
