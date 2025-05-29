using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.Localization
{

    [DataContract]
    [DebuggerDisplay("{LanguageCulture}")]
    public class Language: BaseEntity
    {
        private ICollection<LocaleStringResource> _localeStringResources;

       
        [DataMember]
        public string Name { get; set; }

    
        [DataMember]
        public string LanguageCulture { get; set; }

     
        [DataMember]
        public string UniqueSeoCode { get; set; }

      
        [DataMember]
        public string FlagImageFileName { get; set; }

        
        [DataMember]
        public bool Rtl { get; set; }


        
        [DataMember]
        public bool Published { get; set; }

       
        [DataMember]
        public int DisplayOrder { get; set; }

        public virtual ICollection<LocaleStringResource> LocaleStringResources
        {
            get { return _localeStringResources ?? (_localeStringResources = new HashSet<LocaleStringResource>()); }
            protected set { _localeStringResources = value; }
        }
    

    }
}
