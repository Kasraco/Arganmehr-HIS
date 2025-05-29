using ServiceLayer.Context;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers.Extentions;
namespace IocConfig.Localization
{
    public class SmartResourceDisplayName : System.ComponentModel.DisplayNameAttribute, IModelAttribute
    {
        private readonly string _callerPropertyName;

        public SmartResourceDisplayName(string resourceKey, [CallerMemberName] string propertyName = null)
            : base(resourceKey)
        {
            ResourceKey = resourceKey;
            _callerPropertyName = propertyName;
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                string value = null;
                var langId = ProjectObjectFactory.Container.GetInstance<IWorkContext>().WorkingLanguage.Id;
                value = ProjectObjectFactory.Container.GetInstance<ILocalizationService>().GetResource(ResourceKey, langId, true, "" /* ResourceKey */, true);

                if (value.IsEmpty() && _callerPropertyName.HasValue())
                {
                    value = _callerPropertyName.SplitPascalCase();
                }

                return value;
            }
        }

        public string Name
        {
            get { return "SmartResourceDisplayName"; }
        }
    }
}
