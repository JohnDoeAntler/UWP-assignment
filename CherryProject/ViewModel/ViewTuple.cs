using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.ViewModel
{
	public class ViewTuple
	{
		public string Header { get; set; }

		public string Content { get; set; }

		public ViewTuple(
			object header,
			object content)
		{
			this.Header = header.ToString();
			this.Content = content.ToString();
		}
	}
}
