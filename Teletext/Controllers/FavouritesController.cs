using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Teletext.Models;
using Teletext.Services;

namespace Teletext.Controllers
{
    [Authorize]
    public class FavouritesController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<TeletextUser> _userManager;
        private readonly ITeletextRepository _repo;

        public FavouritesController(ILogger<HomeController> logger, UserManager<TeletextUser> userManager, ITeletextRepository repo)
        {
            _logger = logger;
            _userManager = userManager;
            _repo = repo;
        }

        public async Task<IActionResult> Index()
        {
            var aspnetUser = _userManager.GetUserAsync(HttpContext.User).Result;
            var dto = _repo.Channels.GetDTOChannels(aspnetUser).Result;
            return View(dto);
        }

        public async Task<IActionResult> AddToFavourites(long programId)
        {
            var aspnetUser = await _userManager.GetUserAsync(HttpContext.User);

            var fav = new Favourites
            {
                UserId = aspnetUser.Id,
                ProgramId = programId
            };

            await _repo.Favourites.Add(fav);

            return RedirectToAction("Index", "Favourites");
        }

        public async Task<IActionResult> RemoveFromFavourites(long favouriteId)
        {

            await _repo.Favourites.Delete(favouriteId);

            return RedirectToAction("Index", "Favourites");
        }
    }
}
