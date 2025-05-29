using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DomainClasses.Entities.AM
{
    public class CountryDivition:BaseEntity
    {
        public string Code { get; set; }
        public string ProvinceTitle { get; set; }
        public string CityTitle { get; set; }
        public string RegionTitle { get; set; }
        public string Town { get; set; }
        public string Rural { get; set; }
        public string Abadi { get; set; }
    }
}
        