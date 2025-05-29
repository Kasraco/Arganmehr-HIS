using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainClasses.Entities.Users;

namespace DomainClasses.Entities.Common
{
    /// <summary>
    /// Represents Rating Record regard by section type for Rating System
    /// </summary>
    public class UserRating
    {
        #region Properties


        /// <summary>
        /// gets or sets Id of Rating Record
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// gets or sets value of rate
        /// </summary>
        public virtual double RatingValue { get; set; }
        /// <summary>
        /// gets or sets Section's Id 
        /// </summary>
        public virtual long SectionId { get; set; }
        /// <summary>
        /// gets or sets Section Type
        /// </summary>
        public virtual RatingSectionType SectionType { get; set; }
        /// <summary>
        /// gets or sets user that rate one section
        /// </summary>
        public virtual User Rater { get; set; }
        /// <summary>
        /// gets or sets Rater Id that rate one section
        /// </summary>
        public virtual long RaterId { get; set; }

        #endregion
    }
}
