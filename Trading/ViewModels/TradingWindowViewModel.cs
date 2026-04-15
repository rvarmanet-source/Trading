using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Trading.Commands;
using Trading.Models;

namespace Trading.ViewModels
{
    public class TradingWindowViewModel : INotifyPropertyChanged
    {
        private const string OrdersFilePath = "Orders.json";

        private string _symbol;
        private string _quantity;
        private string _price;
        private bool _isBuy = true;

        public ObservableCollection<Order> OpenOrders { get; set; } = new();
        public ObservableCollection<Order> TradeHistory { get; set; } = new();

        public ICommand SubmitOrderCommand { get; }

        public TradingWindowViewModel()
        {
            SubmitOrderCommand = new RelayCommand(async _ => await SubmitOrderAsync(), CanSubmitOrder);
            LoadOrders();
        }

        public string Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                OnPropertyChanged();
            }
        }

        public string Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public string Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public bool IsBuy
        {
            get => _isBuy;
            set
            {
                _isBuy = value;
                OnPropertyChanged();
            }
        }

        private bool CanSubmitOrder(object parameter)
        {
            return !string.IsNullOrWhiteSpace(Symbol) &&
                   int.TryParse(Quantity, out int qty) && qty > 0 &&
                   decimal.TryParse(Price, out decimal prc) && prc > 0;
        }

        private async Task SubmitOrderAsync()
        {
            if (!int.TryParse(Quantity, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Invalid quantity.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(Price, out decimal price) || price <= 0)
            {
                MessageBox.Show("Invalid price.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var side = IsBuy ? "Buy" : "Sell";

            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                Symbol = Symbol.Trim(),
                Quantity = quantity,
                Price = price,
                Side = side,
                Status = "Open",
                Timestamp = DateTime.Now
            };

            OpenOrders.Add(order);
            await SaveOrdersAsync();

            // Clear input fields
            Symbol = string.Empty;
            Quantity = string.Empty;
            Price = string.Empty;
            IsBuy = true;

            // Simulate order fill after a short delay
            await Task.Delay(1500);
            order.Status = "Filled";
            OpenOrders.Remove(order);
            TradeHistory.Add(order);
            await SaveOrdersAsync();
        }

        private async Task SaveOrdersAsync()
        {
            try
            {
                var ordersData = new OrdersData
                {
                    OpenOrders = OpenOrders.ToList(),
                    TradeHistory = TradeHistory.ToList()
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = JsonSerializer.Serialize(ordersData, options);
                await File.WriteAllTextAsync(OrdersFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving orders: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrders()
        {
            try
            {
                if (File.Exists(OrdersFilePath))
                {
                    var json = File.ReadAllText(OrdersFilePath);
                    var ordersData = JsonSerializer.Deserialize<OrdersData>(json);

                    if (ordersData != null)
                    {
                        foreach (var order in ordersData.OpenOrders)
                        {
                            OpenOrders.Add(order);
                        }

                        foreach (var order in ordersData.TradeHistory)
                        {
                            TradeHistory.Add(order);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading orders: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
