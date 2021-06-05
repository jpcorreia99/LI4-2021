namespace PenedaVes.Models
{
    public class FollowedCamera
    {
        public int Id { get; set; }
        public int CameraId { get; set; }
        public string UserId {get; set; }

        public virtual Camera Camera { get; set; }
        public virtual ApplicationUser User {get; set; }
    }
}