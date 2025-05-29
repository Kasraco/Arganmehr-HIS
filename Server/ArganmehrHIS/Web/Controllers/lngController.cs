using AutoMapper;
using ServiceLayer;
using ServiceLayer.Contracts.AM.Directory;
using ServiceLayer.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Web.Controllers
{
    public class lngController : ApiController
    {

          private readonly ILanguageService _languageService;
        private readonly ICountryService _countryService;
        private readonly ICommonServices _services;

        private readonly IMappingEngine _mappingEngine;
        public lngController(ILanguageService languageService, IMappingEngine mappingEngine,
            ICountryService countryService,
            ICommonServices services)
        {
            this._languageService = languageService;
            this._countryService = countryService;
            this._services = services;
            _mappingEngine = mappingEngine;

        }


        public string Get(int id)
        {
            return string.Format("You entered: {0}", id);
        }

    }
}
