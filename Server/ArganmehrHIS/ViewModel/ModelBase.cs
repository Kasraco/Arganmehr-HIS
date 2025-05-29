using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ViewModel
{
    public sealed class CustomPropertiesDictionary : Dictionary<string, object>
    {
    }

    public abstract partial class ModelBase
    {
        protected ModelBase()
        {
            this.CustomProperties = new CustomPropertiesDictionary();
        }

        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Use this property to store any custom value for your models. 
        /// </summary>
        [IgnoreMap]
        public CustomPropertiesDictionary CustomProperties { get; set; }
    }


    public abstract partial class EntityModelBase : ModelBase
    {
        public virtual int Id { get; set; }
    }


    public abstract partial class TabbableModel : EntityModelBase
    {
        [IgnoreMap]
        public virtual string[] LoadedTabs { get; set; }
    }
}
