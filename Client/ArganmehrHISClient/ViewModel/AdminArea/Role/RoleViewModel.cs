using System.Collections.Generic;
using System.ComponentModel;
using System.Web.Mvc;

namespace ViewModel.AdminArea.Role
{
    public class RoleViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual bool IsActive { get; set; }
        public bool IsSystemRole { get; set; }
        public bool IsDefaultForRegister { get; set; }
    }


    public class CheckListBoxGroup
    {
        public string GroupText { get; set; }
        public List<CheckListBoxItem> Items { get; set; }

    }

    public class CheckListBoxItem
    {
        public string Value { get; set; }
        public string Text { get; set; }
        public bool Selected { get; set; }
    }
}
