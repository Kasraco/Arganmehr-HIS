using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Common.Helpers;
using Common.Security;
using Common.Security.HiddenField;
using IocConfig;

namespace Web
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        #region Fields
        private readonly IEncryptSettingsProvider _settings;
        #endregion

        #region Ctor
        public StructureMapControllerFactory()
        {
            _settings = new EncryptSettingsProvider();
        }

        #endregion
       
        #region override CreateController
       
        private IRijndaelStringEncrypter GetDecrypter(System.Web.Routing.RequestContext requestContext)
        {
            var decrypter = new RijndaelStringEncrypter(_settings, requestContext.GetActionKey());
            return decrypter;
        }
        public override IController CreateController(System.Web.Routing.RequestContext requestContext, string controllerName)
        {
            var parameters = requestContext.HttpContext.Request.Params;
            var encryptedParamKeys = parameters.AllKeys.Where(x => x.StartsWith(_settings.EncryptionPrefix)).ToList();

            IRijndaelStringEncrypter decrypter = null;

            foreach (var key in encryptedParamKeys)
            {
                if (decrypter == null)
                {
                    decrypter = GetDecrypter(requestContext);
                }

                var oldKey = key.Replace(_settings.EncryptionPrefix, string.Empty);
                var oldValue = decrypter.Decrypt(parameters[key]);
                if (requestContext.RouteData.Values[oldKey] != null)
                {
                    if (requestContext.RouteData.Values[oldKey].ToString() != oldValue)
                        throw new ApplicationException("Form values is modified!");
                }
                requestContext.RouteData.Values[oldKey] = oldValue;
            }

            if (decrypter != null)
            {
                decrypter.Dispose();
            }

            return base.CreateController(requestContext, controllerName);
        }

        #endregion

        #region override GetControllerInstance


        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                //var url = requestContext.HttpContext.Request.RawUrl;
                ////string.Format("Page not found: {0}", url).LogException();

                //requestContext.RouteData.Values["controller"] = MVC.Search.Name;
                //requestContext.RouteData.Values["action"] = MVC.Search.ActionNames.Index;
                //requestContext.RouteData.Values["keyword"] = url.GenerateSlug().Replace("-", " ");
                //requestContext.RouteData.Values["categoryId"] = 0;
                //return SampleObjectFactory.Container.GetInstance(typeof(SearchController)) as Controller;
                throw new InvalidOperationException(string.Format("Page not found: {0}", requestContext.HttpContext.Request.RawUrl));
            }
            var controller = ProjectObjectFactory.Container.GetInstance(controllerType) as Controller;
            if (controller != null)
            {
                controller.TempDataProvider = ProjectObjectFactory.Container.GetInstance<ITempDataProvider>();

            }
            return controller;
        }
        #endregion
    }
}




















