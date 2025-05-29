using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IocConfig
{
 
    public class ArganmehrDependingSettingData
    {
        public ArganmehrDependingSettingData()
        {
            OverrideSettingKeys = new List<string>();
        }

        public int ActiveArganmehrScopeConfiguration { get; set; }
        public List<string> OverrideSettingKeys { get; set; }
        public string RootSettingClass { get; set; }
    }
}
