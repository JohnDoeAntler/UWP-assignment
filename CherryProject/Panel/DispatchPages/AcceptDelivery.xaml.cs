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
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CherryProject.Panel.DispatchPages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class AcceptDelivery : Page
	{
		private Dic dic;

		public AcceptDelivery()
		{
			this.InitializeComponent();
		}

		private async void SelectDic_Click(object sender, RoutedEventArgs e)
		{
			DicDialog dialog = new DicDialog(x => x.Status == DicStatusEnum.Dispatched.ToString());

			ContentDialogResult button;

			bool isCompleted = false;

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
								Content = "You are not permitted to accept a non-dispatched despatch instruction cover delivery.",
								CloseButtonText = "OK",
								Width = 400
							};

							await error.EnqueueAndShowIfAsync();
						}
					}

					button = await dialog.EnqueueAndShowIfAsync();

				} while (button == ContentDialogResult.Primary && !(isCompleted = dialog.Dic.Status == DicStatusEnum.Dispatched.ToString()));
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
							dic = await dic.ModifyAsync(x => x.Status = DicStatusEnum.Accepted.ToString());

							await context.SaveChangesAsync();
						}

						NotificationManager.CreateNotification(SignInManager.CurrentUser.Id, "An Order Delivery Has Been Accepted", $"{SignInManager.CurrentUser.FirstName} {SignInManager.CurrentUser.LastName} has accepted an order delivery.", NotificationTypeEnum.Dic, dic.OrderId);

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
