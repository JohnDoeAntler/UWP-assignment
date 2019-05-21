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
			return str;
		}
	}
}
