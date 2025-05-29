using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ViewModel.AdminArea.Arganmehr
{
    

    public class LanguageModel : EntityModelBase
    {
        public LanguageModel()
        {
            FlagFileNames = new List<string>();
        }

       // [AllowHtml]

       [Required]
        public string LanguageName { get; set; }

        [AllowHtml]
        public string LanguageCulture { get; set; }
        public List<SelectListItem> AvailableCultures { get; set; }

        [AllowHtml]
        public string UniqueSeoCode { get; set; }
        public List<SelectListItem> AvailableTwoLetterLanguageCodes { get; set; }

        //flags
        [AllowHtml]
        public string FlagImageFileName { get; set; }
        public IList<string> FlagFileNames { get; set; }
        public List<SelectListItem> AvailableFlags { get; set; }

        public bool Rtl { get; set; }

        public bool Published { get; set; }

        public int DisplayOrder { get; set; }

        //Store mapping
        public bool LimitedToInstitution { get; set; }
        public int[] SelectedInstitutionIds { get; set; }
    }
}
