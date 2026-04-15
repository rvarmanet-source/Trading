using System.Windows;
using Trading.ViewModels;

namespace Trading
{
    /// <summary>
    /// Interaction logic for TradingWindow.xaml
    /// </summary>
    public partial class TradingWindow : Window
    {
        public TradingWindow()
        {
            InitializeComponent();
            DataContext = new TradingWindowViewModel();
        }
    }
}
