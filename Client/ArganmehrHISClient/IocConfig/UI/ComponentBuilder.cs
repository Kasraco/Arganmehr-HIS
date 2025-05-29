using Common.UI;
using Common.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Common.Helpers.Extentions;

namespace IocConfig.UI
{
    public abstract class ComponentBuilder<TComponent, TBuilder> : IHtmlString, IHideObjectMembers
        where TComponent : Component
        where TBuilder : ComponentBuilder<TComponent, TBuilder>
    {
        private ComponentRenderer<TComponent> _renderer;

        protected ComponentBuilder(TComponent component, HtmlHelper htmlHelper)
        {
       

            this.Component = component;
            this.HtmlHelper = htmlHelper;
        }

        protected internal HtmlHelper HtmlHelper
        {
            get;
            private set;
        }

        protected internal TComponent Component
        {
            get;
            private set;
        }

        public TComponent ToComponent()
        {
            return this.Component;
        }

        protected ComponentRenderer<TComponent> Renderer
        {
            get
            {
                if (_renderer == null)
                {
                    _renderer = ProjectObjectFactory.Container.GetInstance<ComponentRenderer<TComponent>>();
                    EnrichRenderer(_renderer);
                }
                return _renderer;
            }
            private set
            {
                _renderer = value;
                if (_renderer != null)
                {
                    EnrichRenderer(_renderer);
                }
            }
        }

        private void EnrichRenderer(ComponentRenderer<TComponent> renderer)
        {
            renderer.Component = this.Component;
            renderer.ViewContext = this.HtmlHelper.ViewContext;
            renderer.ViewData = this.HtmlHelper.ViewData;
        }

        public TBuilder WithRenderer<T>()
            where T : ComponentRenderer<TComponent>
        {
            return this.WithRenderer(typeof(T));
        }

        public TBuilder WithRenderer<T>(ComponentRenderer<TComponent> instance)
            where T : ComponentRenderer<TComponent>
        {
            
            return this.WithRenderer(typeof(T));
        }

        public TBuilder WithRenderer(Type rendererType)
        {
            
            var renderer = Activator.CreateInstance(rendererType) as ComponentRenderer<TComponent>;
            if (renderer != null)
            {
                this.Renderer = renderer;
            }
            return this as TBuilder;
        }

        public virtual TBuilder Name(string name)
        {
            this.Component.Name = name;
            return this as TBuilder;
        }

        public virtual TBuilder HtmlAttributes(object attributes)
        {
            return this.HtmlAttributes(CommonHelper.ObjectToDictionary(attributes));
        }

        public virtual TBuilder HtmlAttributes(IDictionary<string, object> attributes)
        {
            this.Component.HtmlAttributes.Merge(attributes);
            return this as TBuilder;
        }

        public string ToHtmlString()
        {
            return this.Renderer.ToHtmlString();
        }

        public override string ToString()
        {
            return this.ToHtmlString();
        }

        public virtual void Render()
        {
            this.Renderer.Render();
        }

        public static implicit operator TComponent(ComponentBuilder<TComponent, TBuilder> builder)
        {
            return builder.ToComponent();
        }

    }
}
