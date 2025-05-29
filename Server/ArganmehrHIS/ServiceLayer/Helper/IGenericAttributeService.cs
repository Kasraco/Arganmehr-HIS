using Common.Collections;
using DomainClasses;
using DomainClasses.Entities.Users;
using DomainClasses.Helper;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Helper
{
    public partial interface IGenericAttributeService
    {
        /// <summary>
        /// Deletes an attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        void DeleteAttribute(GenericAttribute attribute);

        /// <summary>
        /// Gets an attribute
        /// </summary>
        /// <param name="attributeId">Attribute identifier</param>
        /// <returns>An attribute</returns>
        GenericAttribute GetAttributeById(int attributeId);

        /// <summary>
        /// Inserts an attribute
        /// </summary>
        /// <param name="attribute">attribute</param>
        void InsertAttribute(GenericAttribute attribute);

        /// <summary>
        /// Updates the attribute
        /// </summary>
        /// <param name="attribute">Attribute</param>
        void UpdateAttribute(GenericAttribute attribute);

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="keyGroup">Key group</param>
        /// <returns>Generic attributes</returns>
        IList<GenericAttribute> GetAttributesForEntity(long entityId, string keyGroup);

        /// <summary>
        /// Get attributes
        /// </summary>
        /// <param name="entityIds">Entity identifiers</param>
        /// <param name="keyGroup">Key group</param>
        /// <returns>Generic attributes</returns>
        Multimap<long, GenericAttribute> GetAttributesForEntity(long[] entityIds, string keyGroup);

        /// <summary>
        /// Get queryable attributes
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="keyGroup">The key group</param>
        /// <returns>Queryable attributes</returns>
        IQueryable<GenericAttribute> GetAttributes(string key, string keyGroup);

        /// <summary>
        /// Save attribute value
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="storeId">Store identifier; pass 0 if this attribute will be available for all stores</param>
        void SaveAttribute<TPropType>(User entity, string key, TPropType value, int storeId = 0);

        /// <summary>
        /// Save attribute value
        /// </summary>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="entityId">Entity identifier</param>
        /// <param name="key">The key</param>
        /// <param name="keyGroup">The key group</param>
        /// <typeparam name="TPropType">Property type</typeparam>
        /// <param name="storeId">Store identifier; pass 0 if this attribute will be available for all stores</param>
        void SaveAttribute<TPropType>(long entityId, string key, string keyGroup, TPropType value, int storeId = 0);
    }
}
