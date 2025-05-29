using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IocConfig.UI
{
    [Serializable]
    public class SelectedTabInfo
    {
        public string TabId { get; set; }
        public string Path { get; set; }
    }

    public enum TabPull
    {
        Left,
        Right
    }

    public class Tab : NavigationItemWithContent
    {

        public Tab()
        {
            // [...]
        }

        public string Name
        {
            get;
            set;
        }

        public TabPull Pull
        {
            get;
            set;
        }

    }
}
