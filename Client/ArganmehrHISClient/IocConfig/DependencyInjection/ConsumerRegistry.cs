using Common.Infrastructure;
using DomainClasses.Event;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IocConfig.DependencyInjection
{
    public class ConsumerRegistry: Registry
    {
        public ConsumerRegistry()
        {
            Scan(scan =>
            {
                    scan.TheCallingAssembly();
                    scan.WithDefaultConventions();
                    scan.AddAllTypesOf(typeof(IConsumer<>));
                });
            
        }

       
    }
}
