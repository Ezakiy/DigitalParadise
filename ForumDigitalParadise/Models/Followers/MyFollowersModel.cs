namespace ForumDigitalParadise.Models.Followers
{
    public class MyFollowersModel
    {
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string? Bio {  get; set; }    
        public string? BannerImageUrl { get; set; }
        public string IsFollowingStatus { get; set; }
        public DateTime? FollowingDate { get; set;}
        public List<FollowerViewModel> Followers { get; set; }
    }
}
