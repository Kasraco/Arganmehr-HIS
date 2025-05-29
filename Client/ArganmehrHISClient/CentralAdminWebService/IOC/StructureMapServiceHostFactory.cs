
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Web;
using IocConfig;
//using StructureMap;

using DataLayer.Context;
namespace CentralAdminWebService.IOC
{
    public class StructureMapServiceHostFactory : ServiceHostFactory
    {
        public StructureMapServiceHostFactory()
        {
            var c = ProjectObjectFactory.Container;
            //ObjectFactory.Initialize(
            //    scan =>
            //    {
            //        scan.For<IUnitOfWork>()
            //         .Use<ApplicationDbContext>();
            //    });

        }

        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
        {
            return new StructureMapServiceHost(serviceType, baseAddresses);
        }
    }
}