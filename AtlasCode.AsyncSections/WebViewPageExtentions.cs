using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Reflection;

namespace System.Web.Mvc
{
	public static class WebViewPageAsyncExtentions
	{
		public static HelperResult RenderBodyAsync(this WebViewPage page)
		{
			var result = page.RenderBody();
			result.WriteTo(page.ViewContext.Writer);
			page.Flush();

			return null; // We manually write the body content to the current text writer so we can flush it, so we dont want to return it again or we would get 2 bodies
		}

		internal static void Flush(this WebViewPage page, bool sendPadding = false)
		{
			page.Flush(page.ViewContext.Writer, sendPadding);
		}

		internal static void Flush(this WebViewPage page, TextWriter writer, bool sendPadding = false)
		{
			var sb = ((StringWriter)writer).GetStringBuilder();
			if (page.Context.Response.IsClientConnected)
			{
				page.Context.Response.Write(sb);

				try
				{
					page.Context.Response.Flush();
				}
				catch
				{
				}

				// Hack to fix IE missing closing tags
				if (sendPadding)
				{
					try
					{
						page.Context.Response.Write(Environment.NewLine); 
						page.Context.Response.Flush();
					}
					catch
					{
					}
				}
			}
			sb.Length = 0;
		}
	}
}