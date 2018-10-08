namespace IRunesWebApp.Models
{
    using System.Collections.Generic;

    public class User : BaseModel<string>
    {
        public User()
        {
            this.Albums = new HashSet<AlbumUser>();
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public virtual ICollection<AlbumUser> Albums { get; set; }
    }
}
