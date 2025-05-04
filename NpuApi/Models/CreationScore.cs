using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NpuApi.Models
{
    public class CreationScore
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        [Required]
        public Guid CreationId { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Creativity { get; set; }
        
        [Required]
        [Range(1, 5)]
        public int Uniqueness { get; set; }
        
        public virtual Creation? Creation { get; set; }
        public virtual User? User { get; set; }
    }
}