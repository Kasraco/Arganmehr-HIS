﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainClasses.Entities.SEO
{
    public enum CanonicalHostNameRule
    {
        /// <summary>
        /// Doesn't matter (as requested)
        /// </summary>
        NoRule,
        /// <summary>
        /// The www prefix is required (www.myshop.com is default host)
        /// </summary>
        RequireWww,
        /// <summary>
        /// The www prefix should be omitted (myshop.com is default host)
        /// </summary>
        OmitWww
    }
}
