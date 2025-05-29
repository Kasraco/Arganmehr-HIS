using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DomainClasses.Theme
{
    public class ThemeFolderData : ITopologicSortable<string>
    {
        public string FolderName { get; set; }
        public string FullPath { get; set; }
        public string VirtualBasePath { get; set; }
        public XmlDocument Configuration { get; set; }
        public string BaseTheme { get; set; }

        string ITopologicSortable<string>.Key
        {
            get { return FolderName; }
        }

        string[] ITopologicSortable<string>.DependsOn
        {
            get
            {
                if (BaseTheme == null)
                    return null;

                return new[] { BaseTheme };
            }
        }
    }
}
