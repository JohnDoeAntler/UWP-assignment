using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Panel;
using CherryProject.Service.SignStatus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

							await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
							{
								for (; ; )
								{
									// 1 min refresh
									await Task.Delay(60000);

									if (CurrentUser != null)
									{
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
