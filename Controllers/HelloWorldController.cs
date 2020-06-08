using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace test_mvc_webapp.Controllers
{
    public class HelloWorldController : Controller
    {
        public ActionResult Landing(string name)
        {
            return Content($"Hello {name}");
        }
    }
}