using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("announcements")]
    public class Announcement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public int HostId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey("HostId")]
        public virtual Host Host { get; set; }
    }
}
