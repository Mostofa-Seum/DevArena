using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DevArena.Entities
{
    [Table("contests")]
    public class Contests
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }


        public int host_id { get; set; }

        [Required]
        public string title { get; set; } = null!;

        public DateTime start_time { get; set; }

        public DateTime end_time { get; set; }

        public bool is_active { get; set; } = true;
        [ForeignKey("host_id ")]
        public virtual Host Hosts { get; set; } = null!;
    }
}