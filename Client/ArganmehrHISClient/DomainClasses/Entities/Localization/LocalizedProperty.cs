using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.Localization
{
    public class LocalizedProperty : BaseEntity
    {

        [DataMember]
        [Index("IX_LocalizedProperty_Compound", 1)]
        public int EntityId { get; set; }

        [DataMember]
        [Index("IX_LocalizedProperty_Compound", 4)]
        public int LanguageId { get; set; }


        [DataMember]
        [Index("IX_LocalizedProperty_Compound", 3)]
        [StringLength(450)]
        public string LocaleKeyGroup { get; set; }

        [DataMember]
        [Index("IX_LocalizedProperty_Compound", 2)]
        [StringLength(450)]
        public string LocaleKey { get; set; }


        [DataMember]
        public string LocaleValue { get; set; }


        [DataMember]
        public virtual Language Language { get; set; }
    }
}
