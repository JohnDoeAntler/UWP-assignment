using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace CherryProject.Extension
{
	public static class MD5Hasher
	{
		public static string GetMD5hash(this string value)
		{
			using (MD5 md5 = MD5.Create())
			{
				byte[] hashBytes = md5.ComputeHash(Encoding.ASCII.GetBytes(value));

				StringBuilder sb = new StringBuilder();

				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("X2"));
				}

				return sb.ToString();
			}
		}
	}
}
