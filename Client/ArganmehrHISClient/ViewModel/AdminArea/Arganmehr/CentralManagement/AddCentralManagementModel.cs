using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AdminArea.Arganmehr.CentralManagement
{
    public class AddCentralManagementModel
    {
        [DisplayName("نام مرکز مدیریت")]
        public string Name { get; set; }
        public string Title { get; set; }
        public byte[] Logo { get; set; }
        public string CreateDate { get; set; }
        public string EstablishmentDate { get; set; }
        public int User { get; set; }
    }
}
