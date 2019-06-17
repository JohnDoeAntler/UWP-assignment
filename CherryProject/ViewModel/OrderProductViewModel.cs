using CherryProject.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CherryProject.ViewModel
{
	public class OrderProductViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public OrderProductViewModel(OrderProduct item)
		{
			OrderProduct = item;
		}

		public OrderProduct OrderProduct { get; }

		public uint Quantity
		{
			get => OrderProduct.Quantity;
			set {
				OrderProduct.Quantity = value;
				OnPropertyChanged();
			}
		}

		public uint ReadOnlyQuantity => OrderProduct.Quantity;

		public string ProductName => OrderProduct.Product.Name;

		public double Price {
			get {
				return Quantity * OrderProduct.Product.PriceHistory.Where(x => x.Timestamp < OrderProduct.LastTimeModified).OrderByDescending(x => x.Timestamp).FirstOrDefault().Price;
			}
		}

		public double Weight => Quantity * OrderProduct.Product.Weight;

		public void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			// Raise the PropertyChanged event, passing the name of the property whose value has changed.
			this.PropertyChanged(this, new PropertyChangedEventArgs("Price"));
			this.PropertyChanged(this, new PropertyChangedEventArgs("Weight"));
		}
	}
}
