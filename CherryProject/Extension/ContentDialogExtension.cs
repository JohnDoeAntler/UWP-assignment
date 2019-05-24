using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Xaml.Controls;

namespace CherryProject.Extension
{
	public class Null { private Null() {} }

	public static class ContentDialogExtensions
	{

		private static TaskCompletionSource<Null> PreviousDialogCompletion;

		public static async Task<ContentDialogResult> EnqueueAndShowIfAsync(this ContentDialog contentDialog, Func<bool> predicate = null)
		{
			TaskCompletionSource<Null> currentDialogCompletion = new TaskCompletionSource<Null>();
			TaskCompletionSource<Null> previousDialogCompletion = null;

			// No locking needed since we are always on the UI thread.
			if (!CoreApplication.MainView.CoreWindow.Dispatcher.HasThreadAccess)
			{
				throw new NotSupportedException("Can only show dialog from UI thread.");
			}

			previousDialogCompletion = ContentDialogExtensions.PreviousDialogCompletion;
			ContentDialogExtensions.PreviousDialogCompletion = currentDialogCompletion;

			if (previousDialogCompletion != null)
			{
				await previousDialogCompletion.Task;
			}

			var whichButtonWasPressed = ContentDialogResult.None;

			if (predicate == null || predicate())
			{
				whichButtonWasPressed = await contentDialog.ShowAsync();
			}

			currentDialogCompletion.SetResult(null);
			return whichButtonWasPressed;
		}
	}
}
