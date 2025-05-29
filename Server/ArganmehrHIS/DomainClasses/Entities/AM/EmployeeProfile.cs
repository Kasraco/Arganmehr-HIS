using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainClasses.Entities.Users;
namespace DomainClasses.Entities.AM
{
   public class EmployeeProfile:BaseEntity
    {


        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NationalCode { get; set; }
        public string BirthDate { get; set; }
        public string TelephoneNumber { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }

        public virtual User User { get; set; }


    }
}
