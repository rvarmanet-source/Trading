using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Trading
{
    public partial class TradingWindow : Window
    {
        public ObservableCollection<Order> OpenOrders { get; set; } = new();
        public ObservableCollection<Order> TradeHistory { get; set; } = new();

        public TradingWindow()
        {
            InitializeComponent();
            OpenOrdersGrid.ItemsSource = OpenOrders;
            TradeHistoryGrid.ItemsSource = TradeHistory;
            SubmitOrderButton.Click += SubmitOrder_Click;
        }

        private async void SubmitOrder_Click(object sender, RoutedEventArgs e)
        {
            var symbol = SymbolTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(symbol))
            {
                MessageBox.Show("Symbol is required.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Invalid quantity.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Invalid price.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var side = BuyRadioButton.IsChecked == true ? "Buy" : "Sell";

            var order = new Order
            {
                Symbol = symbol,
                Quantity = quantity,
                Price = price,
                Side = side,
                Status = "Open"
            };

            OpenOrders.Add(order);

            // Simulate order fill after a short delay
            await Task.Delay(1500);
            order.Status = "Filled";
            OpenOrders.Remove(order);
            TradeHistory.Add(order);

            // Optionally clear input fields
            SymbolTextBox.Text = "";
            QuantityTextBox.Text = "";
            PriceTextBox.Text = "";
            BuyRadioButton.IsChecked = true;
        }
    }

    public class Order
    {
        public string Symbol { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Side { get; set; }
        public string Status { get; set; }
    }
}
