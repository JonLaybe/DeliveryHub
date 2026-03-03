using System.ComponentModel.DataAnnotations;

namespace DiscountService.Core.Entities
{
    public abstract class BaseEntity
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime ModifiedOn { get; set; }


        public Guid? CreatedBy { get; set; }


        public Guid? ModifiedBy { get; set; }
    }
}
