using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevArena.Entities
{
    [Table("problems")]
    public class Problems
    {
        [Key]
        public int ProblemId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string InputFormat { get; set; }
        public string OutputFormat { get; set; }
        public int JudgeId { get; set; }
        public int ContestId { get; set; }

        [ForeignKey("JudgeId")]
        public virtual Judge Judge { get; set; }

        [ForeignKey("ContestId")]
        public virtual Contests Contest { get; set; }
    }
}
