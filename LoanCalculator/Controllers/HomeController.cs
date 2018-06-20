
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using LoanCalculator.Models;
using Microsoft.AspNet.Identity;



namespace LoanCalculator.Controllers
{

    public class HomeController : Controller
    {
      


        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            
            if (User.Identity.IsAuthenticated)
            {
                
                var loggedUser = User.Identity.GetUserId();
                
                var query = db.Users.Where(x => x.Id == loggedUser);

                var name = query.FirstOrDefault().FirstName;
                
                ViewBag.firstName = name;

                return View();
            }
            return View();
        }


    }
}