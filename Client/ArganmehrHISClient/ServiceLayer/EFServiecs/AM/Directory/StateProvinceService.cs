using Common.Caching;
using DataLayer.Context;
using DomainClasses.Entities.Directory;
using ServiceLayer.Contracts.AM.Directory;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.EFServiecs.AM.Directory
{
    public partial class StateProvinceService : IStateProvinceService
    {
        #region Constants
        private const string STATEPROVINCES_ALL_KEY = "Arganmher.stateprovince.all-{0}";
        private const string STATEPROVINCES_PATTERN_KEY = "Arganmher.stateprovince.";
        #endregion

        #region Fields

        private readonly IDbSet<StateProvince> _stateProvinceRepository;
        private readonly ICacheManagerKRB _cacheManager;
        private readonly IUnitOfWork _uow;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="stateProvinceRepository">State/province repository</param>
        /// <param name="eventPublisher">Event published</param>
        public StateProvinceService(IUnitOfWork uow,ICacheManagerKRB cacheManager)
        {
            _cacheManager = cacheManager;
            _uow = uow;
            _stateProvinceRepository = _uow.Set<StateProvince>();

        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes a state/province
        /// </summary>
        /// <param name="stateProvince">The state/province</param>
        public virtual void DeleteStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Remove(stateProvince);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
        }

        public virtual IQueryable<StateProvince> GetAllStateProvinces(bool showHidden = false)
        {
            var query = _stateProvinceRepository.AsQueryable();

            if (!showHidden)
                query = query.Where(x => x.Published);

            return query;
        }

        /// <summary>
        /// Gets a state/province
        /// </summary>
        /// <param name="stateProvinceId">The state/province identifier</param>
        /// <returns>State/province</returns>
        public virtual StateProvince GetStateProvinceById(int stateProvinceId)
        {
            if (stateProvinceId == 0)
                return null;

            return _stateProvinceRepository.Find(stateProvinceId);
        }

        /// <summary>
        /// Gets a state/province 
        /// </summary>
        /// <param name="abbreviation">The state/province abbreviation</param>
        /// <returns>State/province</returns>
        public virtual StateProvince GetStateProvinceByAbbreviation(string abbreviation)
        {
            var query = from sp in _stateProvinceRepository.AsQueryable()
                        where sp.Abbreviation == abbreviation
                        select sp;
            var stateProvince = query.FirstOrDefault();
            return stateProvince;
        }

        /// <summary>
        /// Gets a state/province collection by country identifier
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>State/province collection</returns>
        public virtual IList<StateProvince> GetStateProvincesByCountryId(int countryId, bool showHidden = false)
        {
            string key = string.Format(STATEPROVINCES_ALL_KEY, countryId);
            return _cacheManager.Get(key, () =>
            {
                var query = from sp in _stateProvinceRepository.AsQueryable()
                            orderby sp.DisplayOrder
                            where sp.CountryId == countryId &&
                            (showHidden || sp.Published)
                            select sp;
                var stateProvinces = query.ToList();
                return stateProvinces;
            });
        }

        /// <summary>
        /// Inserts a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void InsertStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");

            _stateProvinceRepository.Add(stateProvince);

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);

        }

        /// <summary>
        /// Updates a state/province
        /// </summary>
        /// <param name="stateProvince">State/province</param>
        public virtual void UpdateStateProvince(StateProvince stateProvince)
        {
            if (stateProvince == null)
                throw new ArgumentNullException("stateProvince");
         

            _cacheManager.RemoveByPattern(STATEPROVINCES_PATTERN_KEY);
            _uow.SaveChanges();
        }

        #endregion
    }
}
