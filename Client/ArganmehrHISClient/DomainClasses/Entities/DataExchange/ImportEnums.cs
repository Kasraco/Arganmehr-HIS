using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.DataExchange
{
   
        public enum ImportEntityType
        {
            Institution = 0,
            NewsLetterSubscription
        }

        public enum ImportFileType
        {
            CSV = 0,
            XLSX
        }

        [Flags]
        public enum ImportModeFlags
        {
            Insert = 1,
            Update = 2
        }
    
}
