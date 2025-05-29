using System;
using System.Collections.Generic;
using Web;

namespace ServiceLayer.Security
{
    public  class PermissionRecord
    {
        public string RoleName { get; set; }
        public IEnumerable<ControllerDescription> Permissions { get; set; }
    }
}
