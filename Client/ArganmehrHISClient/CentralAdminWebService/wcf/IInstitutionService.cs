using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CentralAdminWebService.wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IInstitutionService" in both code and config file together.
    [ServiceContract]
    public interface IInstitutionService
    {
        [OperationContract]
        int DoWork();
    }
}
