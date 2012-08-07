using System.Web.Mvc;
using AtlasCode.AsyncSections;

namespace Async.Controllers
{
    public class MinimumController : AsyncSectionController
    {
        public ActionResult Index()
        {
			AsyncView(); // Return the view before the async loading starts
			System.Threading.Thread.Sleep(5000); // Pretend to load data
			AsyncSection("Task1"); // Render the AsyncSection
            return null; // Return null rather than a view because we have already rendered everything
        }
    }
}
