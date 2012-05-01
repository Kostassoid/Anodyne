using System.Web.Mvc;

namespace Kostassoid.BlogNote.Web.Controllers
{
    using Contracts;
    using Models;

    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        //
        // GET: /Home/

        [HttpGet]
        public ActionResult Index()
        {
            return View(new UserModel());
        }

        [HttpPost]
        public ActionResult Index(UserModel user)
        {
            if (ModelState.IsValid)
            {
                var userId = _userService.EnsureUserExists(user.Name, user.Email);
                Session["userId"] = userId;
                Session["userName"] = user.Name;
                return RedirectToAction("Blog");
            }

            return View(user);
        }

        [HttpGet]
        public ActionResult Blog()
        {
            if (Session["userId"] == null)
                return RedirectToAction("Index");

            return View();
        }



    }
}
