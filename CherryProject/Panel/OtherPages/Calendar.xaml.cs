using CherryProject.Extension;
using CherryProject.Model;
using CherryProject.Model.Enum;
using CherryProject.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace CherryProject.Panel.OtherPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Calendar : Page
    {
		private readonly ObservableCollection<Notification> notifications;

        public Calendar()
        {
            this.InitializeComponent();

			notifications = new ObservableCollection<Notification>();

			CalendarView.SelectedDatesChanged += Calendar_SelectedDatesChanged;
		}

		public ObservableCollection<Notification> Notifications => notifications;
		
		private void Calendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
		{
			using (var context = new Context())
			{
				IQueryable<Notification> notifications;

				switch (SignInManager.CurrentUser.Role)
				{

					case RoleEnum.AreaManager:
						{
							notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == SignInManager.CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != SignInManager.CurrentUser.Id && x.SenderId == x.RecipientId && x.Type != NotificationTypeEnum.Dic));

							break;
						}

					case RoleEnum.DispatchClerk:
						{
							notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == SignInManager.CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != SignInManager.CurrentUser.Id && x.SenderId == x.RecipientId && x.Type != NotificationTypeEnum.Order));

							break;
						}

					case RoleEnum.Administrator:
						{
							notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == SignInManager.CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != SignInManager.CurrentUser.Id && x.SenderId == x.RecipientId));

							break;
						}

					// dealer
					// sales manager
					// storemen
					default:
						{
							notifications = context.Notification.Include(x => x.Sender).Where(x => (x.RecipientId == SignInManager.CurrentUser.Id && x.SenderId != x.RecipientId || x.RecipientId != SignInManager.CurrentUser.Id && x.SenderId == x.RecipientId && x.Type != NotificationTypeEnum.Order && x.Type != NotificationTypeEnum.Dic));

							break;
						}
				}

				this.notifications.UpdateObservableCollection(notifications.Where(x => x.Timestamp.Date == sender.SelectedDates.FirstOrDefault().Date));
			}
		}
	}
}
