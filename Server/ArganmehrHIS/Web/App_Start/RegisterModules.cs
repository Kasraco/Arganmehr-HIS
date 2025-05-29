
using System.Web;
using Web;

[assembly: PreApplicationStartMethod(typeof(RegisterModules), "Start")]
namespace Web
{
    public class RegisterModules
    {
        public static void Start()
        {
            //    DynamicModuleUtility.RegisterModule(typeof(DosAttackModule));
            //    DynamicModuleUtility.RegisterModule(typeof(AntiXssModule));
            //    DynamicModuleUtility.RegisterModule(typeof(Common.HttpCompress.HttpModule));
        }
    }
}