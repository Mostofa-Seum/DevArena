using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevArena.Entities
{
    [Table("contests")]
    public class Contests
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int HostId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [ForeignKey("HostId")]
        public virtual Host Hosts { get; set; } = null!;
    }
}