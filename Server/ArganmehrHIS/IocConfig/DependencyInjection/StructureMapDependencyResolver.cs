using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using IocConfig; 
namespace IocConfig.DependencyInjection
{
    public class StructureMapDependencyResolver : IHttpControllerActivator
    {
        public StructureMapDependencyResolver(HttpConfiguration configuration) { }

        public IHttpController Create(HttpRequestMessage request
            , HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = ProjectObjectFactory.Container.GetInstance(controllerType) as IHttpController;
            return controller;
        }
    }
    
}
