﻿using System;
using System.Collections.Generic;
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
	}
}
