using Microsoft.AspNetCore.Identity;

namespace Teletext.Models
{
    public class TeletextUser : IdentityUser
    {

        public virtual ICollection<Favourites> Favourites { get; set; }

        public TeletextUser()
        {
            Favourites = new List<Favourites>();
        }
    }
}
