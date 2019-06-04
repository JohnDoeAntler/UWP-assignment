using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CherryProject.Service
{
	public class StorageManager
	{
		private readonly ApplicationDataContainer localSettings;

		private StorageManager()
		{
			localSettings = ApplicationData.Current.LocalSettings;
		}

		public ApplicationDataContainer ApplicationDataContainer => localSettings;

		private static StorageManager storageManager;

		public static ApplicationDataContainer GetApplicationDataContainer()
		{
			if (storageManager == null)
			{
				storageManager = new StorageManager();
			}

			return storageManager.ApplicationDataContainer;
		}
	}
}
