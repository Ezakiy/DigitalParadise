namespace ForumDigitalParadise.Models.ApplicationUser
{
    public class SettingsModel
    {
        public int DisplayId { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string? UserRating { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? BannerImageUrl { get; set; }
        public bool IsActive { get; set; }
        public string? UserDescription { get; set; }
        public bool IsAdmin { get; set; }
        public DateTime MemberSince { get; set; }
    }
}
