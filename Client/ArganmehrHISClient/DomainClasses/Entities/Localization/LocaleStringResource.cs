using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.Localization
{
       [DebuggerDisplay("{ResourceName} - {ResourceValue}")]
    public class LocaleStringResource : BaseEntity
    {

        public int LanguageId { get; set; }


        public string ResourceName { get; set; }


        public string ResourceValue { get; set; }

        public bool? IsFromPlugin { get; set; }

        public bool? IsTouched { get; set; }


        public virtual Language Language { get; set; }

    }
}
