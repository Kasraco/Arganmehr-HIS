using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IocConfig.Localization
{
    public interface ILocalizedModelLocal
    {
        int LanguageId { get; set; }
    }

    public interface ILocalizedModel
    {
    }

    public interface ILocalizedModel<TLocalizedModel> : ILocalizedModel
    {
        IList<TLocalizedModel> Locales { get; set; }
    }
}
