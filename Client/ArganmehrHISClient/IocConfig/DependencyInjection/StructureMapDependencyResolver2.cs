using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IocConfig.DependencyInjection
{
    public class StructureMapDependencyResolver2 : IDependencyResolver
    {
        //public IDependencyScope BeginScope()
        //{
        //    return this;
        //}

        public object GetService(Type serviceType)
        {
            if (serviceType.IsAbstract || serviceType.IsInterface || !serviceType.IsClass)
                return ProjectObjectFactory.Container.TryGetInstance(serviceType);
            return ProjectObjectFactory.Container.GetInstance(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return ProjectObjectFactory.Container.GetAllInstances(serviceType).Cast<object>();
        }

        //public void Dispose()
        //{ }


    }
}
