using System.ComponentModel.DataAnnotations.Schema;

namespace DomainClasses.Entities.Common
{
    [ComplexType]
    public class Rating
    {
        public virtual double? TotalRating { get; set; }
        public virtual int? TotalRaters { get; set; }
        public virtual double? AverageRating { get; set; }
    }
}
