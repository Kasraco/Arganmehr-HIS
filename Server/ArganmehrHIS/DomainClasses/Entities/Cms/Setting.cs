using System.Diagnostics;
using System.Runtime.Serialization;
namespace DomainClasses.Entities.Cms
{

    [DataContract]
    [DebuggerDisplay("{Name}: {Value}")]
    public partial class Setting : BaseEntity
    {
        #region Properties
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual string Value { get; set; }
        public virtual int PortalId { get; set; }
        /// <summary>
        /// gets or sets body of blog post's comment
        /// </summary>
        public virtual byte[] RowVersion { get; set; }
        #endregion

    }
}
