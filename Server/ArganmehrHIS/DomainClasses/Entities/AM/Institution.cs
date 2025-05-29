using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.AM
{
    public class Institution:BaseEntity
    {
        public string InstitutionCode { get; set; }
        public string InstitutionTitle { get; set; }

        public string CountryDivitionCode { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string MobileNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebSiteAddress { get; set; }
        public string WebServiceAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreateDate { get; set; }
    }
}
