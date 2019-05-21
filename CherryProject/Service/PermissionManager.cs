using CherryProject.Model.Enum;
using CherryProject.Panel.Account;
using CherryProject.Panel.Order;
using CherryProject.Panel.Product;
using CherryProject.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CherryProject.Service
{
	public class PermissionManager
	{
		public static IEnumerable<Type> GetPermission(RoleEnum role)
		{
			switch (role)
			{
				case RoleEnum.Dealer:
					return new[]{
						typeof(ViewAccount),
						typeof(ViewOrder),
						typeof(SearchOrders),
						typeof(CreateOrder),
						typeof(ModifyOrder),
						typeof(CancelOrder),
						typeof(SearchProducts)
					};
					
				case RoleEnum.AreaManager:
					return new []{
						typeof(ViewAccount),
						typeof(SearchAccounts),
						typeof(CreateAccount),
						typeof(ModifyAccount),
						typeof(DisableAccount),
						typeof(SearchOrders),
						typeof(ViewOrder),
						typeof(SearchOrders),
						typeof(CreateOrder),
						typeof(ModifyOrder),
						typeof(EndorseOrder),
						typeof(CancelOrder)
					};
					
				case RoleEnum.SalesManager:
					return new[]{
						typeof(ViewAccount),
					};

				case RoleEnum.SalesOrderOfficer:
					return new[]{
						typeof(ViewAccount),
					};

				case RoleEnum.Storemen:
					return new[]{
						typeof(ViewAccount),
					};

				case RoleEnum.DispatchClerk:
					return new[]{
						typeof(ViewAccount),
					};

				case RoleEnum.Administrator:
					return GetTypesInNamespace("CherryProject.Panel.");

				default:
					return null;
			}
		}

		public static IEnumerable<Type> GetTypesInNamespace(string @namespace) 
			=> Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.Where(x => x.Namespace != null 
				&& x.Namespace.Contains(@namespace)
				&& typeof(Page).IsAssignableFrom(x))
			.ToArray();
	}
}
