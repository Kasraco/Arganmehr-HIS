using IocConfig.Filters;
using IocConfig.Localization;
using IocConfig.SEO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IocConfig.Controllers
{

    [CustomerLastActivity] 
    [LanguageSeoCode]
    //[RequireHttpsByConfigAttribute(SslRequirement.Retain)]
    [CanonicalHostName]
    public abstract partial class PublicControllerBase : SmartController
    {
    }
}
