using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Dic
	{
		private string concurrencyStamp;
		private DateTime lastTimeModified;

		public string Id { get; set; }
		public string OrderId { get; set; }
		public string Status { get; set; }
		public DateTime LastTimeModified { get => lastTimeModified == DateTime.MinValue ? (lastTimeModified = DateTime.Now) : lastTimeModified; set => lastTimeModified = value; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Order Order { get; set; }
	}
}
