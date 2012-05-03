using System.Web.Mvc;

namespace Kostassoid.BlogNote.Web.Controllers
{
    using System;
    using System.Web.Security;
    using Contracts;
    using Models.Forms;
    using Models.View;
    using Query;

    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserQuery _userQuery;

        public HomeController(IUserService userService, UserQuery userQuery)
        {
            _userService = userService;
            _userQuery = userQuery;
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View(new IndexViewData(_userQuery.GetAll(), new UserForm()));
        }

        [HttpPost]
        public ActionResult Index(UserForm user)
        {
            if (ModelState.IsValid)
            {
                var userId = _userService.EnsureUserExists(user.Name, user.Email);
                FormsAuthentication.SetAuthCookie(userId.ToString(), false);
                return RedirectToAction("Blog");
            }

            return View(new IndexViewData(_userQuery.GetAll(), user));
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize]
        public ActionResult Blog()
        {
            return View(new BlogViewData(_userQuery.GetOne(new Guid(HttpContext.User.Identity.Name))));
        }

    }
}
