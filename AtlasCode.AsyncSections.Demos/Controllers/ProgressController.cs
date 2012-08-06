using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtlasCode.AsyncSections;
using System.Threading.Tasks;

namespace Async.Controllers
{
	public class ProgressController : AsyncSectionController
    {
		public void IndexAsync()
		{
			// Do Something Async
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					System.Threading.Thread.Sleep(1000);

					ViewBag.Message = "Iteration " + i;
					AsyncSection("Console");
				}

				ViewBag.Message = "Complete";
				AsyncSection("Console");

				AsyncManager.OutstandingOperations.Decrement();
			});

			// Send down the view so far
			AsyncView();
		}

		public ActionResult IndexCompleted()
		{
			return null;
		}
    }
}
