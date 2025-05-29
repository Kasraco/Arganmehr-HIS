using DataLayer.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace CentralAdminWebService.wcf
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "InstitutionService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select InstitutionService.svc or InstitutionService.svc.cs at the Solution Explorer and start debugging.
    public class InstitutionService : IInstitutionService
    {

        private IUnitOfWork _uow { get; set; }

        public InstitutionService(IUnitOfWork uow)
        {
            _uow = uow;

        }
        public int DoWork()
        {
            var i = 100;
            var a = i * 20;
            return a;
        }
    }
}
