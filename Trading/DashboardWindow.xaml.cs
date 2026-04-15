using System;
using System.Windows;
using Trading.ViewModels;

namespace Trading
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {
        private DashboardViewModel _viewModel;

        public DashboardWindow()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel();
            DataContext = _viewModel;
            Closed += DashboardWindow_Closed;
        }

        private void DashboardWindow_Closed(object sender, EventArgs e)
        {
            _viewModel?.Dispose();
        }

        private void OpenTradingWindow_Click(object sender, RoutedEventArgs e)
        {
            var tradingWindow = new TradingWindow();
            tradingWindow.Show();
        }
    }
}
