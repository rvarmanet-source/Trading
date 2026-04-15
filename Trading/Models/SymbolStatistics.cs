using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Trading.Models
{
    public class SymbolStatistics : INotifyPropertyChanged
    {
        private string _symbol;
        private decimal _currentPrice;
        private decimal _openingPrice;
        private decimal _priceChange;
        private decimal _percentageChange;
        private int _totalOrders;
        private int _totalQuantity;
        private DateTime _lastUpdated;

        public string Symbol
        {
            get => _symbol;
            set
            {
                _symbol = value;
                OnPropertyChanged();
            }
        }

        public decimal CurrentPrice
        {
            get => _currentPrice;
            set
            {
                _currentPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PercentageChangeFormatted));
                OnPropertyChanged(nameof(PriceChangeFormatted));
            }
        }

        public decimal OpeningPrice
        {
            get => _openingPrice;
            set
            {
                _openingPrice = value;
                OnPropertyChanged();
            }
        }

        public decimal PriceChange
        {
            get => _priceChange;
            set
            {
                _priceChange = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PriceChangeFormatted));
            }
        }

        public decimal PercentageChange
        {
            get => _percentageChange;
            set
            {
                _percentageChange = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(PercentageChangeFormatted));
            }
        }

        public int TotalOrders
        {
            get => _totalOrders;
            set
            {
                _totalOrders = value;
                OnPropertyChanged();
            }
        }

        public int TotalQuantity
        {
            get => _totalQuantity;
            set
            {
                _totalQuantity = value;
                OnPropertyChanged();
            }
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set
            {
                _lastUpdated = value;
                OnPropertyChanged();
            }
        }

        public string PercentageChangeFormatted => $"{PercentageChange:+0.00;-0.00}%";
        public string PriceChangeFormatted => $"{PriceChange:+0.00;-0.00}";

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
