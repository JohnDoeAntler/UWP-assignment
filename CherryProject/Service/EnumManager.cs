using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Service
{
	public static class EnumManager
	{
		public static ObservableCollection<T> GetEnumList<T>() where T : Enum
		{
			return new ObservableCollection<T>(Enum.GetValues(typeof(T)).Cast<T>());
		}
	}
}
