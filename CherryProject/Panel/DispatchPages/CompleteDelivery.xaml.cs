using CherryProject.Dialog;
using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.DispatchPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class CompleteDelivery : Page
	{
		private Dic dic;

		public CompleteDelivery()
		{
			this.InitializeComponent();
		}

		private async void SelectDic_Click(object sender, RoutedEventArgs e)
		{
			DicDialog dialog = new DicDialog(x => x.Status == DicStatusEnum.Dispatching.ToString());

			ContentDialogResult button;

			bool isCompleted = false;
			bool isDispatching = false;

			using (var context = new Context())
			{
				do
				{
					// if dialog displays more than 1 times
					if (dialog.Dic != null)
					{
						// if the order is not endorsed
						if (!isCompleted)
						{
							ContentDialog error = new ContentDialog
							{
								Title = "Alert",
								Content = "The DIC has not been completed, please wait till storemen completely assemble it.",
								CloseButtonText = "OK",
								Width = 400
							};

							await error.EnqueueAndShowIfAsync();
						}
						else if (!isDispatching)
						{
							ContentDialog error = new ContentDialog
							{
								Title = "Alert",
								Content = "You are not permitted to complete a non-dispatching DIC delivery.",
								CloseButtonText = "OK",
								Width = 400
							};

							await error.EnqueueAndShowIfAsync();
						}
					}

					button = await dialog.EnqueueAndShowIfAsync();

				} while (button == ContentDialogResult.Primary 
					&& !((isDispatching = dialog.Dic.Status == DicStatusEnum.Dispatching.ToString()) 
					&& (isCompleted = context.DidSpare.Count(x => dialog.Dic.Did.Any(y => y.Id == x.DidId)) == dialog.Dic.Did.Sum(x => x.Quantity))));
			}

			if (button == ContentDialogResult.Primary)
			{
				DicGUID.Text = (dic = dialog.Dic).Id.ToString();
				SelectedDic.Text = $"Selected DIC: {dic.Id}";
				SelectedDic.Visibility = Visibility.Visible;
				Submit.IsEnabled = true;
			}
		}

		private async void Submit_Click(object sender, RoutedEventArgs e)
		{
			if (await new ConfirmationDialog().EnqueueAndShowIfAsync() == ContentDialogResult.Primary)
			{
				if (dic == null)
				{
					await new MistakeDialog().EnqueueAndShowIfAsync();
				}
				else
				{
					try
					{
						using (var context = new Context())
						{
							var dic = await context.Dic.FirstOrDefaultAsync(x => x.Id == this.dic.Id);
							dic = await dic.ModifyAsync(x => x.Status = DicStatusEnum.Dispatched.ToString());

							await context.SaveChangesAsync();
						}

						NotificationManager.CreateNotification(dic.Order.DealerId, "An Order Delivery Has Been Completed", $"{SignInManager.CurrentUser.FirstName} {SignInManager.CurrentUser.LastName} has completed one of your order delivery.", NotificationTypeEnum.Dic, dic.OrderId);

						await new SuccessDialog().EnqueueAndShowIfAsync();
					}
					catch (Exception)
					{
						await new ErrorDialog().EnqueueAndShowIfAsync();
					}
				}
			}
		}
	}
}
