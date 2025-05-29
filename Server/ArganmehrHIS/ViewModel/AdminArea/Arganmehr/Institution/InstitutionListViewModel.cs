using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.AdminArea.Arganmehr.Institution
{


    public class InstitutionListViewModel
    {
        public IEnumerable<InstitutionModel> Institutions { get; set; }
        public InstituionSearchRequest SearchRequest { get; set; }
    }
}
