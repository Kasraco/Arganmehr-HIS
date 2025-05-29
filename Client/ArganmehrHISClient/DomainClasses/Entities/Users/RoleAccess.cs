using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.Users
{
    public class RoleAccess
    {
        public int Id { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public long RoleId { get; set; }

        public virtual Role Role { get; set; }


      
    }
}
