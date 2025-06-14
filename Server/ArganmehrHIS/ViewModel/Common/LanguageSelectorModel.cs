﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Common
{
    public partial class LanguageSelectorModel : ModelBase
    {
        public LanguageSelectorModel()
        {
            AvailableLanguages = new List<LanguageModel>();
            ReturnUrls = new Dictionary<string, string>();
        }

        public IList<LanguageModel> AvailableLanguages { get; set; }

        public IDictionary<string, string> ReturnUrls { get; private set; }

        public int CurrentLanguageId { get; set; }

        public bool UseImages { get; set; }
    }
}
