using System.ComponentModel.DataAnnotations;

namespace ForumDigitalParadise.Data.Models
{
    public class UserAccount
    {
        [Key]
        public int Id { get; set; }

        public string PrimaryUserId { get; set; }

        public string AccountUserId { get; set; }

        public ApplicationUser PrimaryUser { get; set; }
        public ApplicationUser AccountUser { get; set; }
    }
}
