using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using StackExchange.Profiling;
using System.Reflection;
using System.IO;
using System.Web.WebPages;
using AtlasCode.AsyncSections;

namespace Async.Controllers
{
	public class MultipleTaskController : AsyncSectionController
	{
		public void IndexAsync()
		{
			// Do Something Async
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(2000); // Simulate something that takes a while to load data

				ViewBag.Message = "Data loaded from message task 1 after 2 seconds"; // Imagine we got this string from some database or service
				AsyncSection("Message1");

				AsyncManager.OutstandingOperations.Decrement();
			});

			// Do Something Async
			AsyncManager.OutstandingOperations.Increment();
			Task.Factory.StartNew(() =>
			{
				System.Threading.Thread.Sleep(1000); // Simulate something that takes a while to load data

				ViewBag.Message2 = "Data loaded from message task 2 after 1 second"; // Imagine we got this string from some database or service
				AsyncSection("Message2");

				AsyncManager.OutstandingOperations.Decrement();
			});

			// Send down the view so far
			AsyncView();
		}

		public ActionResult IndexCompleted()
		{
			return null;
		}

		public ActionResult Test()
		{
			return PartialView();
		}
	}
}
