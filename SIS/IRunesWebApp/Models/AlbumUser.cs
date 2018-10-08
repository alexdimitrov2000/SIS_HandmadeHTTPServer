namespace IRunesWebApp.Models
{
    public class AlbumUser
    {
        public string AlbumId { get; set; }
        public virtual Album Album { get; set; }

        public string UserId { get; set; }
        public virtual User User { get; set; }
    }
}
