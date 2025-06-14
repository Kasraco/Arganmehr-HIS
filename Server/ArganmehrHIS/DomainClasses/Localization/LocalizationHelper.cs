﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers.Extentions;

namespace DomainClasses.Localization
{
    public static class LocalizationHelper
    {
        private readonly static HashSet<string> _cultureCodes =
            new HashSet<string>(
                CultureInfo.GetCultures(CultureTypes.NeutralCultures | CultureTypes.SpecificCultures | CultureTypes.UserCustomCulture)
                .Select(x => x.IetfLanguageTag)
                .Where(x => !string.IsNullOrWhiteSpace(x)));

        public static bool IsValidCultureCode(string cultureCode)
        {
            return _cultureCodes.Contains(cultureCode);
        }

        public static string GetLanguageNativeName(string locale)
        {
            try
            {
                if (locale.HasValue())
                {
                    var info = CultureInfo.GetCultureInfoByIetfLanguageTag(locale);
                    if (info != null)
                        return info.NativeName;
                }
            }
            catch { }

            return null;
        }
    }
}
