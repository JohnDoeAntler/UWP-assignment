using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class PanelExtension
	{
		public static string ClassNameToString(this Type name)
		{
			string str = string.Empty;

			str += name.Name[0];

			for (int i = 1; i < name.Name.Length; i++)
			{
				char letter = name.Name[i];
				if (char.IsUpper(letter)){
					str += " ";
				}
				str += letter;
			}

			return str;
		}

		public static bool IsDoubleNumeric(this string str) => double.TryParse(str, out _) || string.IsNullOrEmpty(str);

		public static bool IsIntegerNumeric(this string str) => int.TryParse(str, out _) || string.IsNullOrEmpty(str);

		public static void UpdateObservableCollection<T>(this ObservableCollection<T> orignal, IEnumerable<T> list)
		{
			orignal.Clear();

			foreach (T element in list)
			{
				orignal.Add(element);
			}
		}
	}
}
