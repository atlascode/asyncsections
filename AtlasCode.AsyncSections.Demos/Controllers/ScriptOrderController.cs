using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AtlasCode.AsyncSections;
using System.Threading.Tasks;

namespace Async.Controllers
{
	public class ScriptOrderController : AsyncSectionController
    {
		public void IndexAsync()
		{
			// Do Something Async
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(5000);
				ViewBag.Message = "5 second task complete";
				AsyncSection("Task1");
				AsyncManager.OutstandingOperations.Decrement();
			});

			// Do Something Async
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(2000);
				ViewBag.Message2 = "2 second task complete";
				AsyncSection("Task2");
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
