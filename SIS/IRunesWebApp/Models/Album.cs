namespace IRunesWebApp.Models
{
    using System.Linq;
    using System.Collections.Generic;

    public class Album : BaseModel<string>
    {
        public Album()
        {
            this.Tracks = new HashSet<Track>();
            this.Owners = new HashSet<AlbumUser>();
        }

        public string Name { get; set; }

        public string Cover { get; set; }

        public decimal Price => this.Tracks.Sum(t => t.Price) * (100.00m - 0.13m);

        public virtual ICollection<Track> Tracks { get; set; }

        public virtual ICollection<AlbumUser> Owners { get; set; }
    }
}
