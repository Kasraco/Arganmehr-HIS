using Common.Caching;
using DomainClasses.Entities.Directory;
using ServiceLayer.Context;
using ServiceLayer.Contracts.AM.Directory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers.Extentions;
using DataLayer.Context;
namespace ServiceLayer.EFServiecs.AM.Directory
{
    public partial class CountryService : ICountryService
    {
        #region Constants
        private const string COUNTRIES_ALL_KEY = "Arganmher.country.all-{0}";
        private const string COUNTRIES_BILLING_KEY = "Arganmher.country.billing-{0}";
        private const string COUNTRIES_SHIPPING_KEY = "Arganmher.country.shipping-{0}";
        private const string COUNTRIES_PATTERN_KEY = "Arganmher.country.";
        #endregion

        #region Fields

        private readonly IDbSet<Country> _countryRepository;        
        private readonly ICacheManagerKRB _cacheManager;
        private readonly ICAContext _caContext;
        private readonly IUnitOfWork _uow;

        #endregion

        #region Ctor

        public CountryService(IUnitOfWork uow,ICacheManagerKRB cacheManager,
            ICAContext caContext)
        {
            _uow = uow;
            _cacheManager = cacheManager;
            _countryRepository = _uow.Set<Country>();
            _caContext = caContext;
            
        }


        #endregion

        #region Methods

        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void DeleteCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Remove(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAllCountries(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = _countryRepository.AsQueryable();

                if (!showHidden)
                    query = query.Where(c => c.Published);

                query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);

             
                    //var currentStoreId = _caContext.CurrentCA.Id;
                    //query = from c in query
                    //        join sc in _storeMappingRepository.Table
                    //        on new { c1 = c.Id, c2 = "Country" } equals new { c1 = sc.EntityId, c2 = sc.EntityName } into c_sm
                    //        from sc in c_sm.DefaultIfEmpty()
                    //        where !c.LimitedToStores || currentStoreId == sc.StoreId
                    //        select c;

                    //query = from c in query
                    //        group c by c.Id into cGroup
                    //        orderby cGroup.Key
                    //        select cGroup.FirstOrDefault();

                    //query = query.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name);
                

                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow billing
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAllCountriesForBilling(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_BILLING_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var allCountries = GetAllCountries(showHidden);

                var countries = allCountries.Where(x => x.AllowsBilling).ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets all countries that allow shipping
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAllCountriesForShipping(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_SHIPPING_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var allCountries = GetAllCountries(showHidden);

                var countries = allCountries.Where(x => x.AllowsShipping).ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryById(int countryId)
        {
            if (countryId == 0)
                return null;

            return _countryRepository.Find(countryId);
        }

        /// <summary>
        /// Gets a country by two or three letter ISO code
        /// </summary>
        /// <param name="letterIsoCode">Country two or three letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByTwoOrThreeLetterIsoCode(string letterIsoCode)
        {
            if (letterIsoCode.HasValue())
            {
                if (letterIsoCode.Length == 2)
                    return GetCountryByTwoLetterIsoCode(letterIsoCode);
                else if (letterIsoCode.Length == 3)
                    return GetCountryByThreeLetterIsoCode(letterIsoCode);
            }
            return null;
        }

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode)
        {
            if (twoLetterIsoCode.IsEmpty())
                return null;

            var query = from c in _countryRepository.ToList()
                        where c.TwoLetterIsoCode == twoLetterIsoCode
                        select c;

            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByThreeLetterIsoCode(string threeLetterIsoCode)
        {
            if (threeLetterIsoCode.IsEmpty())
                return null;

            var query = from c in _countryRepository.ToList()
                        where c.ThreeLetterIsoCode == threeLetterIsoCode
                        select c;

            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void InsertCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Add(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void UpdateCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _uow.SaveChanges();

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

        }

        #endregion
    }
}
