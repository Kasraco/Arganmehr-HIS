using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class DeleteConfirmationModel : EntityModelBase
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string ButtonSelector { get; set; }
        public string EntityType { get; set; }
    }
}
