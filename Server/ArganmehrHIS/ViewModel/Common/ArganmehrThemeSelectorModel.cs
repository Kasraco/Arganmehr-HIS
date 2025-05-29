using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Common
{
  

    public partial class ArganmehrThemeSelectorModel : ModelBase
    {
        public ArganmehrThemeSelectorModel()
        {
            AvailableThemes = new List<ArganmehrThemeModel>();
        }

        public IList<ArganmehrThemeModel> AvailableThemes { get; set; }

        public ArganmehrThemeModel CurrentTheme { get; set; }
    }


    public partial class ArganmehrThemeModel : ModelBase
    {
        public string Name { get; set; }
        public string Title { get; set; }
    }
}
