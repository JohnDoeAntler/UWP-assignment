using CherryProject.Attribute;
using CherryProject.Model.Enum;
using CherryProject.Panel.AccountPages;
using CherryProject.Panel.CategoryPages;
using CherryProject.Panel.DispatchPages;
using CherryProject.Panel.InvoicePages;
using CherryProject.Panel.OrderPages;
using CherryProject.Panel.OtherPages;
using CherryProject.Panel.ProductPages;
using CherryProject.Panel.PromotionPages;
using CherryProject.Panel.SparePages;
using CherryProject.Panel.StatisticPages;
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
		public static IEnumerable<Type> GetPermission(RoleEnum role, bool showHidden = true)
		{
			switch (role)
			{
				case RoleEnum.Dealer:
					return new[]
					{
						// account
						typeof(ViewAccount),
						typeof(ModifyAccount),
						// dispatch
						typeof(AcceptDelivery),
						typeof(ViewDispatachNote),
						typeof(ViewOrderDispatchStatus),
						// invoice
						typeof(PrintInvoice),
						// order
						typeof(CancelOrder),
						typeof(CreateOrder),
						typeof(ModifyOrder),
						typeof(SearchOrders),
						typeof(UnreserveOrder),
						typeof(ViewOrder),
						// other
						typeof(Calendar),
						// product
						typeof(SearchProducts),
						typeof(ViewProduct),
						// promotion
						typeof(ViewPromotions),
					};
					
				case RoleEnum.AreaManager:
					return new[]
					{
						// account
						typeof(CreateAccount),
						typeof(ModifyAccount),
						typeof(SearchAccounts),
						typeof(ViewAccount),
						// dispatch
						typeof(DeliverOrder),
						// order
						typeof(CancelOrder),
						typeof(CreateOrder),
						typeof(EndorseOrder),
						typeof(ModifyOrder),
						typeof(SearchOrders),
						typeof(ViewOrder),
						// other
						typeof(Calendar),
						// product
						typeof(SearchProducts),
						typeof(ViewProduct),
					};
					
				case RoleEnum.SalesManager:
					return new[]
					{
						// account
						typeof(ViewAccount),
						// category
						typeof(AddCategory),
						// other
						typeof(Calendar),
						// product
						typeof(AddProduct),
						typeof(ModifyProduct),
						typeof(SearchProducts),
						typeof(ViewProduct),
						// promotion
						typeof(AddPromotion),
						typeof(ModifyPromotion),
						typeof(ViewPromotions),
						// statistic
						typeof(ViewBestSellingProduct),
						typeof(ViewDangerLevelProduct),
						typeof(ViewReorderLevelProduct),
						typeof(ViewProductSellingVolume),
						typeof(ViewSellingVolume)
					};

				case RoleEnum.SalesOrderOfficer:
					return new[]
					{
						// account
						typeof(SearchAccounts),
						typeof(ViewAccount),
						// dispatch
						typeof(ViewDispatachNote),
						typeof(ViewOrderDispatchStatus),
						// order
						typeof(SearchOrders),
						typeof(ViewOrder),
						// other
						typeof(Calendar),
						// product
						typeof(SearchProducts),
						typeof(ViewProduct),
						// promotion
						typeof(ViewPromotions),
						// statistic
						typeof(ViewBestSellingProduct),
						typeof(ViewDangerLevelProduct),
						typeof(ViewReorderLevelProduct),
						typeof(ViewProductSellingVolume),
						typeof(ViewSellingVolume)
					};

				case RoleEnum.Storemen:
					return new[]
					{
						// account
						typeof(ViewAccount),
						// order
						typeof(SearchOrders),
						typeof(ViewOrder),
						// other
						typeof(Calendar),
						// spare
						typeof(AddSpare),
						typeof(AddSpares),
						typeof(AssembleSpare)
					};

				case RoleEnum.DispatchClerk:
					return new[]
					{
						// account
						typeof(ViewAccount),
						// dispatch
						typeof(CompleteDelivery),
						typeof(ViewDispatachNote),
						typeof(ViewOrderDispatchStatus),
						// other
						typeof(Calendar),
					};

				case RoleEnum.Administrator:
					return GetTypesInNamespace("CherryProject.Panel.", showHidden);

				default:
					return null;
			}
		}

		public static IEnumerable<Type> GetTypesInNamespace(string @namespace, bool showHidden = true) 
			=> Assembly
			.GetExecutingAssembly()
			.GetTypes()
			.Where(x => x.Namespace != null 
				&& x.Namespace.Contains(@namespace)
				&& typeof(Page).IsAssignableFrom(x)
				&& (showHidden ? true : x.GetCustomAttribute<HiddenAttribute>() == null))
			.ToArray();
	}
}
