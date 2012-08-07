using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using AtlasCode.AsyncSections;

namespace Async.Controllers
{
    public class ComparisonController : AsyncSectionController
    {
        //
        // GET: /Comparison/

        public ActionResult Index()
        {
            return View();
        }

		public void AsyncAsync()
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

		public ActionResult AsyncCompleted()
		{
			return null;
		}

		public void NoAsyncAsync()
		{
			// Do Something Async
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(5000);
				AsyncManager.Parameters.Add("message1", "5 second task complete");			
				AsyncManager.OutstandingOperations.Decrement();
			});

			// Do Something Async
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(2000);
				AsyncManager.Parameters.Add("message2", "2 second task complete");	
				AsyncManager.OutstandingOperations.Decrement();
			});
		}

		public ActionResult NoAsyncCompleted(string message1, string message2)
		{
			ViewBag.Message = message1;
			ViewBag.Message2 = message2;
			return View();
		}

    }
}
