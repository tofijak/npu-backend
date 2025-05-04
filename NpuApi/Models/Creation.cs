using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NpuApi.Models
{
    public class Creation
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(1000)]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string NicePartName { get; set; } = string.Empty;
        
        [Required]
        public Guid UserId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual ICollection<CreationScore>? CreationScores { get; set; }

        public virtual User? User { get; set; }
    }
}