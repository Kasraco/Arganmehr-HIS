using System.ComponentModel.DataAnnotations.Schema;

namespace DomainClasses.Entities.Common
{
    /// <summary>
    /// Represents Social Snippent Of Sections For Show Summary 
    /// </summary>
    [ComplexType]
    public class SocialSnippet
    {
        /// <summary>
        /// gets or sets icon name with size 200*200 px for snippet 
        /// </summary>
        public virtual string SnippetIconName { get; set; }
        /// <summary>
        /// gets or sets title for snippet
        /// </summary>
        public virtual string SnippetTitle { get; set; }
        /// <summary>
        /// gets or sets description for snippet
        /// </summary>
        public virtual string SnippetDescription { get; set; }
        /// <summary>
        /// gets or sets content type of snippet (image/book/article/video)
        /// https://developers.google.com/+/web/snippet/
        /// </summary>
        public virtual string SnippetContentType { get; set; }
        /// <summary>
        /// gets or sets absolute url of item
        /// </summary>
        public virtual string SnippetUrl { get; set; }
    }
}
