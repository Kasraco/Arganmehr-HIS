
using DomainClasses.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.ThemService
{
    public interface IText
    {
        LocalizedString Get(string key, params object[] args);
    }
}
