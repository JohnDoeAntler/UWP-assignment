using CherryProject.Model.Enum;
using System;
using System.Collections.Generic;

namespace CherryProject.Model
{
	public partial class Promotion
	{
		private DateTime startTime = DateTime.UtcNow;
		private DateTime endTime = DateTime.UtcNow;
		private string concurrencyStamp;

		public Guid Id { get; set; }
		public Guid ProductId { get; set; }
		public string Description { get; set; }
		public DateTime StartTime { get => startTime; set => startTime = value; }
		public DateTime EndTime { get => endTime; set => endTime = value; }
		public double Discount { get; set; }
		public string ImageUrl { get; set; }
		public GeneralStatusEnum Status { get; set; }
		public string ConcurrencyStamp { get => concurrencyStamp ?? (concurrencyStamp = Guid.NewGuid().ToString()); set => concurrencyStamp = value; }

		public virtual Product Product { get; set; }
	}
}
