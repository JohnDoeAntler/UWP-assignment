using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CherryProject.Extension
{
	public static class ControlExtension
	{
		public static string GetText(this RichEditBox element)
		{
			element.Document.GetText(Windows.UI.Text.TextGetOptions.None, out string str);
			return new String(str.SkipLast(1).ToArray());
		}

		public static void SetText(this RichEditBox element, string value)
		{
			element.Document.SetText(Windows.UI.Text.TextSetOptions.None, value);
		}
	}
}
