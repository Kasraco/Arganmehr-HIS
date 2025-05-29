using System.ComponentModel.DataAnnotations.Schema;

namespace DomainClasses.Entities.Common
{
    [ComplexType]
    public class Statistic 
    {
        /// <summary>
        /// gets or sets  viewed count by rss
        /// </summary>
        public virtual long ViewCountByRss { get; set; }
        /// <summary>
        /// gets or sets viewed count 
        /// </summary>
        public virtual int ViewCount { get; set; }
        /// <summary>
        /// Gets or sets the total number of approved comments
        /// <remarks>The same as if we run Item.Comments.Where(n => n.IsApproved).Count()
        /// We use this property for performance optimization (no SQL command executed)
        /// </remarks>
        /// </summary>
        public virtual int ApprovedCommentsCount { get; set; }
        /// <summary>
        /// Gets or sets the total number of nonapproved comments
        /// <remarks>The same as if we run Item.Comments.Where(n => !n.IsApproved).Count()
        /// We use this property for performance optimization (no SQL command executed)
        /// </remarks>
        /// </summary>
        public virtual int NonApprovedCommentsCount { get; set; }
    }
}
