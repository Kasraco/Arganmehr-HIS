using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Install
{
    public class CentralAdminModel
    {
        [DisplayName("نام مرکز مدیریت")]
        public string Name { get; set; }

        [DisplayName("عنوان مرکز مدیریت")]
        public string Title { get; set; }

        [DisplayName("آدرس وب سرویس")]
        public string WebServiceURL { get; set; }

        [DisplayName("لوگوی مرکز مدیریت")]
        public byte[] Logo { get; set; }

        [DisplayName("مدیر")]
        public int UserId { get; set; }

        [DisplayName("نام مدیر سیستم")]
        public string ManagerName { get; set; }

         [DisplayName("کلمه عبور")]
        public string Password { get; set; }

    }
}
