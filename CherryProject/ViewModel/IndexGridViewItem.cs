using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CherryProject.ViewModel
{
	public sealed partial class IndexGridViewItem
	{
		public string Title { get; set; }

		public string Tag { get => Title.Replace(" ", ""); }

		public string Description { get; set; }

		public Symbol Icon { get; set; }

		public IEnumerable<Type> Views { get; set; }
	}
}
