
using System.Runtime.Serialization;
using System.Web.Mvc;
using Common.Controller;
using ServiceLayer.Mailers;
using StructureMap.Configuration.DSL;
using System.Runtime.Serialization.Formatters.Binary;

namespace IocConfig
{
    public class ServiceLayerRegistery : Registry
    {
        public ServiceLayerRegistery()
        {

            Scan(scanner =>
            {
                scanner.WithDefaultConventions();
                scanner.AssemblyContainingType<UserMailer>();
            });
        }
    }
}
