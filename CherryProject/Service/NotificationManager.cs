using CherryProject.Model;
using CherryProject.Model.Enum;
using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Notification = CherryProject.Model.Notification;

namespace CherryProject.Service
{
	public class NotificationManager
	{
		public static async void CreateNotification(Guid Receipient, string header, string content, NotificationTypeEnum type, Guid id)
		{
			using (var context = new Context())
			{
				await context.Notification.AddAsync(new Notification() {
					Id = Guid.NewGuid(),
					RecipientId = Receipient,
					SenderId = SignInManager.CurrentUser.Id,
					IsReceived = false,
					Header = header,
					Content = content,
					Type = type,
					ObjectId = id
				});

				await context.SaveChangesAsync();
			}
		}

		public static void SendNotification(Notification notification)
		{
			ToastContent toast = new ToastContent()
			{
				Launch = "SMLC Notifications Visualizer",

				Visual = new ToastVisual()
				{
					BindingGeneric = new ToastBindingGeneric()
					{
						AppLogoOverride = new ToastGenericAppLogo()
						{
							Source = ValidOrDefault(notification.Sender.IconUrl),
							HintCrop = ToastGenericAppLogoCrop.Circle
						},

						Children =
						{
							new AdaptiveText()
							{
								Text = notification.Header,
								HintMaxLines = 1
							},

							new AdaptiveText()
							{
								Text = notification.Content
							},
						}
					}
				},

				Actions = new ToastActionsCustom()
				{
					Buttons =
					{
						new ToastButton("View", new QueryString(){
							{ "Type", notification.Type.ToString() },
							{ "ObjectId", notification.ObjectId.ToString() }
						}.ToString())
						{
						},

						new ToastButton("Cancel", "")
					}
				}
			};

			ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(toast.GetXml()));
		}

		private static string ValidOrDefault(string uri)
		{
			if (Uri.TryCreate(uri, UriKind.Absolute, out Uri iconUrl) && iconUrl != null && (iconUrl.Scheme == Uri.UriSchemeHttp || iconUrl.Scheme == Uri.UriSchemeHttps))
			{
				return uri;
			}

			return "ms-appx:///Assets/Account.jpg";
		}
	}
}
