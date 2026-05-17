using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DevArena.Models
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public string email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; } = null!;

        [DisplayName("Sign in as Judge")]
        public bool IsJudgeLogin { get; set; }
    }
}