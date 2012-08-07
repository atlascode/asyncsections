using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Reflection;
using System.IO;
using System.Web.WebPages;
using System.Globalization;

namespace AtlasCode.AsyncSections
{
	public class AsyncSectionController : AsyncController
	{
		public WebViewPage Page { get; private set; }
		public Dictionary<string, SectionWriter> AsyncSections { get; private set; }
		public IView CurrentView { get; set; }
		private TextWriter finalWriter;

		protected void AsyncView()
		{
			this.AsyncManager.Sync(() =>
			{
				var result = View();

				var viewEngineResult = result.ViewEngineCollection.FindView(ControllerContext, ControllerContext.RouteData.GetRequiredString("action"), result.MasterName);
				result.View = viewEngineResult.View;

				CurrentView = result.View as IView;

				// Instantiate the view class. There are alot of internal/private reflections here
				var _buildManager = typeof(BuildManagerCompiledView).GetProperty("BuildManager", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(CurrentView, null);
				IViewPageActivator _viewPageActivator = typeof(BuildManagerCompiledView).GetField("ViewPageActivator", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(CurrentView) as IViewPageActivator;
				Type compiledType = _buildManager.GetType().GetMethod("System.Web.Mvc.IBuildManager.GetCompiledType", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(_buildManager, new object[] { ((dynamic)CurrentView).ViewPath }) as Type;
				if (compiledType != null)
				{
					Page = _viewPageActivator.Create(ControllerContext, compiledType) as WebViewPage;
				}

				// Create a new textwriter to hold the body that we flush early.
				TextWriter output = new StringWriter(CultureInfo.CurrentCulture);

				ViewContext viewContext = new ViewContext(ControllerContext, CurrentView, this.ViewData, this.TempData, output);
				Page.VirtualPath = ((dynamic)CurrentView).ViewPath;
				Page.ViewContext = viewContext;
				Page.ViewData = viewContext.ViewData;
				Page.InitHelpers();
				
				WebPageRenderingBase startPage = null;
				dynamic razorView = new ExposedObject(CurrentView);
				if (((dynamic)CurrentView).RunViewStartPages)
				{
					startPage = razorView.StartPageLookup(Page, "_ViewStart", ((dynamic)CurrentView).ViewStartFileExtensions);
				}

				// ExecutePageHierarchy
				var pageContext = new WebPageContext(ControllerContext.HttpContext, null, null);
				Page.PushContext(pageContext, output);

				if (startPage != null)
				{
					if (startPage != Page)
					{
						IDictionary<object, object> pageData = null;
						object model = null;
						bool isLayoutPage = false;
						//WebPageContext webPageContext = WebPageContext.CreateNestedPageContext<object>(pageContext, pageData, model, isLayoutPage);
						var method = typeof(WebPageContext).GetMethod("CreateNestedPageContext", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(new Type[] { typeof(object) });
						WebPageContext webPageContext = method.Invoke(null, new object[] { pageContext, pageData, model, isLayoutPage }) as WebPageContext;

						((dynamic)new ExposedObject(webPageContext)).Page = startPage;
						((dynamic)new ExposedObject(startPage)).PageContext = webPageContext;
					}
					startPage.ExecutePageHierarchy();
				}
				else
				{
					Page.ExecutePageHierarchy();
				}

				// Move Async Sections into a new dictionary. This prevents them being treated as standard sections and throwing required exceptions
				Dictionary<string, SectionWriter> sectionWriters = typeof(WebPageBase).GetProperty("SectionWriters", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Page, new object[] { }) as Dictionary<string, SectionWriter>;
				AsyncSections = sectionWriters.Where(x => x.Key.EndsWith("Async")).ToDictionary(x => x.Key, x => x.Value);
				foreach (var section in AsyncSections.Keys)
				{
					sectionWriters.Remove(section);
				}

				// End ExecutePageHierarchy
				Page.PopContext(); // This actually starts the rendering but we flush as part of RenderBodyAsync

				// Keep a reference to the last part of the page after the RenderBodyAsync call, so we can send it after all async sections have been rendered
				finalWriter = Page.ViewContext.Writer;
			});
		}

		protected void AsyncSection(string sectionName)
		{
			this.AsyncManager.Sync(() =>
			{
				// Add a textwriter to the stack for the async sections to render to
				TextWriter output = new StringWriter(CultureInfo.CurrentCulture);
				Page.OutputStack.Push(output);

				// Refresh the view context with any new data that may have been loaded async
				ViewContext viewContext = new ViewContext(ControllerContext, CurrentView, this.ViewData, this.TempData, output);
				Page.ViewContext = viewContext;
				Page.ViewData = viewContext.ViewData;
				Page.InitHelpers();

				if (!sectionName.EndsWith("Async"))
					sectionName += "Async";

				AsyncSections[sectionName].Invoke();

				// Send this async section down the wire
				Page.Flush(true);
				Page.OutputStack.Pop();
			});
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			base.OnActionExecuted(filterContext);

			// Send the last part of the page after the RenderBodyAsync call if there is one
			if (finalWriter != null)
			{
				Page.Flush(finalWriter);
			}
		}
	}
}