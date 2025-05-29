using Common.Caching;
using Common.Collections;
using DataLayer.Context;
using DomainClasses;
using DomainClasses.Helper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Helpers.Extentions;
using DataLayer;
using DomainClasses.Entities.Users;
namespace ServiceLayer.Helper
{
    public partial class GenericAttributeService : IGenericAttributeService
    {
        #region Constants

        private const string GENERICATTRIBUTE_KEY = "Arganmehr.genericattribute.{0}-{1}";
        private const string GENERICATTRIBUTE_PATTERN_KEY = "Arganmehr.genericattribute.";
        #endregion

        #region Fields

        private readonly IDbSet<GenericAttribute> _genericAttributeRepository;
        private readonly IUnitOfWork _uow;
        private readonly ICacheManagerKRB _cacheManager;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="genericAttributeRepository">Generic attribute repository</param>
        /// <param name="eventPublisher">Event published</param>
        /// <param name="orderRepository">Order repository</param>
        public GenericAttributeService(ICacheManagerKRB cacheManager,IUnitOfWork uow )
        {
            this._cacheManager = cacheManager;
            this._uow = uow;
            this._genericAttributeRepository = _uow.Set<GenericAttribute>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public virtual void DeleteAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            long entityId = attribute.EntityId;
            string keyGroup = attribute.KeyGroup;

            _genericAttributeRepository.Remove(attribute);
            _uow.SaveChanges();
            //cache
            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);


        }

        /// <summary>
        /// Gets an attribute
        /// </summary>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>An attribute</returns>
        public virtual GenericAttribute GetAttributeById(int attributeId)
        {
            if (attributeId == 0)
                return null;

            var attribute = _genericAttributeRepository.Find(attributeId);
            return attribute;
        }

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        public virtual void InsertAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");

            _genericAttributeRepository.Add(attribute);
            _uow.SaveChanges();
            //cache
            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            //event notifications
       
        }

        /// <summary>
        /// Updates the attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        public virtual void UpdateAttribute(GenericAttribute attribute)
        {
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            var at = _genericAttributeRepository.Find(attribute.Id);
            if (at != null)
            {
                at.EntityId=attribute.EntityId;
                at.Key = attribute.Key;
                at.KeyGroup = attribute.KeyGroup;
                at.StoreId = attribute.StoreId;
                at.Value = attribute.Value;                
            }
            _uow.SaveChanges();

            //cache
            _cacheManager.RemoveByPattern(GENERICATTRIBUTE_PATTERN_KEY);

            //event notifications
            
        }

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="keyGroup">Key group</param>
        /// <returns>Get attributes</returns>
        public virtual IList<GenericAttribute> GetAttributesForEntity(long entityId, string keyGroup)
        {
            string key = string.Format(GENERICATTRIBUTE_KEY, entityId, keyGroup);
            return _cacheManager.Get(key, () =>
            {
                var query = from ga in _genericAttributeRepository.AsNoTracking()
                            where ga.EntityId == entityId &&
                            ga.KeyGroup == keyGroup
                            select ga;
                var attributes = query.ToList();
                return attributes;
            });
        }

        public virtual Multimap<long, GenericAttribute> GetAttributesForEntity(long[] entityIds, string keyGroup)
        {


            var query = _genericAttributeRepository.AsQueryable().AsNoTracking()
                .Where(x => entityIds.Contains(x.EntityId) && x.KeyGroup == keyGroup);

            var map = query
                .ToList()
                .ToMultimap(x => x.EntityId, x => x);

            return map;
        }

        /// <summary>
        /// Get queryable attributes
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="keyGroup">The key group</param>
        /// <returns>Queryable attributes</returns>
        public virtual IQueryable<GenericAttribute> GetAttributes(string key, string keyGroup)
        {
            var query =
                from ga in _genericAttributeRepository.AsQueryable()
                where ga.Key == key && ga.KeyGroup == keyGroup
                select ga;

            return query;
        }

        /// <summary>
        /// Save attribute value
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier; pass 0 if this attribute will be available for all stores</param>
        public virtual void SaveAttribute<TPropType>(User entity, string key, TPropType value, int storeId = 0)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            SaveAttribute(entity.Id, key, entity.GetUnproxiedEntityType().Name, value, storeId);
        }

        public virtual void SaveAttribute<TPropType>(long entityId, string key, string keyGroup, TPropType value, int storeId = 0)
        {
           

            var props = GetAttributesForEntity(entityId, keyGroup)
                 .Where(x => x.StoreId == storeId)
                 .ToList();

            var prop = props.FirstOrDefault(ga =>
                ga.Key.Equals(key, StringComparison.InvariantCultureIgnoreCase)); // should be culture invariant

            string valueStr = value.Convert<string>();

            if (prop != null)
            {
                if (string.IsNullOrWhiteSpace(valueStr))
                {
                    //delete
                    DeleteAttribute(prop);
                }
                else
                {
                    //update
                    prop.Value = valueStr;
                    UpdateAttribute(prop);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(valueStr))
                {
                    //insert
                    prop = new GenericAttribute
                    {
                        EntityId = entityId,
                        Key = key,
                        KeyGroup = keyGroup,
                        Value = valueStr,
                        StoreId = storeId
                    };
                    InsertAttribute(prop);
                }
            }
        }

        #endregion
    }
}
