using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Extension
{
	public static class SpareExtension
	{
		public static bool FirstToTailMatch(this string str1, string str2)
		{
			int length = str1.Length > str2.Length ? str2.Length : str1.Length;

			for (int i = 0; i < length; i++)
			{
				if (str1[i] != str2[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}
