using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Common
{
    public partial class LanguageModel : EntityModelBase
    {
        public string Name { get; set; }
        public string ISOCode { get; set; }
        public string SeoCode { get; set; }
        public string NativeName { get; set; }
        public string FlagImageFileName { get; set; }

    }
}
