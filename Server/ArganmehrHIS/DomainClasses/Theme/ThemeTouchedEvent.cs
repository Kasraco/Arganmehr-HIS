using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Theme
{
    public class ThemeTouchedEvent
    {
        public ThemeTouchedEvent(string themeName)
        {
            this.ThemeName = themeName;
        }

        public string ThemeName { get; set; }
    }
}
