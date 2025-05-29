
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Localization
{
    public delegate LocalizedString Localizer(string key, params object[] args);
}
