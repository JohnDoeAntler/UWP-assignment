using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Panel;
using CherryProject.Service.SignStatus;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace CherryProject.Service
{
	namespace SignStatus{
		public enum Status
		{
			Success,
			UsernameFailure,
			PasswordFailure,
			Disabled,
			DatabaseFailure
		}
	}

	public class SignInManager
	{
		public static User CurrentUser { get; private set; }

		public static async Task<Status> SignInAsync(string username, string password)
		{
			try
			{
				var result = await UserManager.FindUserAsync(x => x.UserName == username);

				// if user in not null.
				if (result != null)
				{
					// if the password is matched
					if (result.PasswordHash.Equals(password.GetMD5hash()))
					{
						// if the account is not disabled.
						if (result.Status != GeneralStatusEnum.Disabled)
						{
							CurrentUser = result;

							await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
							{
								while (true)
								{
									if (CurrentUser != null)
									{
										using (var context = new Context())
										{
											IQueryable<Notification> notifications;

											switch (CurrentUser.Role)
											{
												// dealer
												// sales manager
												// storemen
												default:
												{
													notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != CurrentUser.Id && x.SenderId == x.RecipientId && x.Type != NotificationTypeEnum.Order && x.Type != NotificationTypeEnum.Dic) && x.IsReceived == false);

													break;
												}

												case RoleEnum.AreaManager:
												{
													notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != CurrentUser.Id && x.SenderId == x.RecipientId && x.Type != NotificationTypeEnum.Dic) && x.IsReceived == false);

													break;
												}

												case RoleEnum.DispatchClerk:
												{
													notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != CurrentUser.Id && x.SenderId == x.RecipientId && x.Type != NotificationTypeEnum.Order) && x.IsReceived == false);

													break;
												}

												case RoleEnum.Administrator:
												{
													notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != CurrentUser.Id && x.SenderId == x.RecipientId) && x.IsReceived == false);

													break;
												}
											}

											foreach (var notification in notifications)
											{
												if (StorageManager.GetApplicationDataContainer().Values[CurrentUser.Id.ToString() + notification.Id] == null)
												{
													NotificationManager.SendNotification(notification);
												}

												if (notification.SenderId != notification.RecipientId || (notification.SenderId == notification.RecipientId && (notification.Type == NotificationTypeEnum.Order || notification.Type == NotificationTypeEnum.Dic || DateTime.UtcNow - notification.Timestamp > new TimeSpan(1, 0, 0, 0))))
												{
													notification.IsReceived = true;
												}
												else if (notification.SenderId == notification.RecipientId)
												{
													StorageManager.GetApplicationDataContainer().Values[CurrentUser.Id.ToString() + notification.Id] = true;
												}
											}

											await context.SaveChangesAsync();
										}

										if (!await ValidateSecurityStamp())
										{
											SignOut();

											Frame navigationFrame = Window.Current.Content as Frame;
											navigationFrame.Navigate(typeof(LoginPage), null, new DrillInNavigationTransitionInfo());

											break;
										}
									}
									else
									{
										break;
									}

									// 1 min refresh
									await Task.Delay(60000);
								}
							});

						return Status.Success;
						}else return Status.Disabled;
					}else return Status.PasswordFailure;
				}else return Status.UsernameFailure;
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				throw e;
			}
		}

		public static void SignOut() => CurrentUser = null;

		public static async Task<bool> ValidateSecurityStamp() => (await UserManager.FindUserAsync(x => x.Id == CurrentUser.Id)).SecurityStamp == (CurrentUser?.SecurityStamp ?? string.Empty);
	}
}
