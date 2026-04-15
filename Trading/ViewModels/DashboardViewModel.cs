using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Trading.Models;

namespace Trading.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged, IDisposable
    {
        private const string OrdersFilePath = "Orders.json";
        private readonly Random _random = new Random();
        private readonly DispatcherTimer _refreshTimer;
        private Dictionary<string, decimal> _symbolBasePrices = new();

        public ObservableCollection<SymbolStatistics> SymbolStats { get; set; } = new();

        public DashboardViewModel()
        {
            LoadSymbolStatistics();

            // Setup auto-refresh timer (every 1 second)
            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshSymbolStatistics();
        }

        private void LoadSymbolStatistics()
        {
            try
            {
                if (!File.Exists(OrdersFilePath))
                {
                    return;
                }

                var json = File.ReadAllText(OrdersFilePath);
                var ordersData = JsonSerializer.Deserialize<OrdersData>(json);

                if (ordersData == null)
                    return;

                // Combine all orders (open and history)
                var allOrders = new List<Order>();
                allOrders.AddRange(ordersData.OpenOrders);
                allOrders.AddRange(ordersData.TradeHistory);

                // Group by symbol
                var symbolGroups = allOrders.GroupBy(o => o.Symbol);

                SymbolStats.Clear();
                _symbolBasePrices.Clear();

                foreach (var group in symbolGroups)
                {
                    var symbol = group.Key;
                    var orders = group.ToList();

                    if (string.IsNullOrWhiteSpace(symbol))
                        continue;

                    // Calculate average price from orders
                    var averagePrice = orders.Average(o => o.Price);
                    _symbolBasePrices[symbol] = averagePrice;

                    // Simulate current price with random variation (-5% to +10%)
                    var priceVariation = (decimal)(_random.NextDouble() * 0.15 - 0.05); // -5% to +10%
                    var currentPrice = averagePrice * (1 + priceVariation);

                    var priceChange = currentPrice - averagePrice;
                    var percentageChange = (priceChange / averagePrice) * 100;

                    var stats = new SymbolStatistics
                    {
                        Symbol = symbol,
                        OpeningPrice = averagePrice,
                        CurrentPrice = currentPrice,
                        PriceChange = priceChange,
                        PercentageChange = percentageChange,
                        TotalOrders = orders.Count,
                        TotalQuantity = orders.Sum(o => o.Quantity),
                        LastUpdated = DateTime.Now
                    };

                    SymbolStats.Add(stats);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading symbol statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshSymbolStatistics()
        {
            try
            {
                // Update each symbol's current price with new random variation
                foreach (var stats in SymbolStats)
                {
                    if (_symbolBasePrices.TryGetValue(stats.Symbol, out var basePrice))
                    {
                        // Simulate price movement with small random variation
                        var priceVariation = (decimal)(_random.NextDouble() * 0.15 - 0.05); // -5% to +10%
                        var newCurrentPrice = basePrice * (1 + priceVariation);

                        stats.CurrentPrice = newCurrentPrice;
                        stats.PriceChange = newCurrentPrice - basePrice;
                        stats.PercentageChange = (stats.PriceChange / basePrice) * 100;
                        stats.LastUpdated = DateTime.Now;

                        // Trigger property change notifications
                        OnPropertyChanged(nameof(SymbolStats));
                    }
                }
            }
            catch (Exception ex)
            {
                // Silently handle refresh errors to avoid disrupting the UI
                System.Diagnostics.Debug.WriteLine($"Refresh error: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_refreshTimer != null)
            {
                _refreshTimer.Stop();
                _refreshTimer.Tick -= RefreshTimer_Tick;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
