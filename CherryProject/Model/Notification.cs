using CherryProject.Model.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.Model
{
	public partial class Notification
	{
		public Guid Id { get; set; }
		public Guid RecipientId { get; set; }
		public Guid SenderId { get; set; }
		public bool IsReceived { get; set; }
		public string Header { get; set; }
		public string Content { get; set; }
		public NotificationTypeEnum Type { get; set; }
		public Guid ObjectId { get; set; }
		public DateTime Timestamp { get; set; }

		public virtual User Recipient { get; set; }
		public virtual User Sender { get; set; }
	}
}
