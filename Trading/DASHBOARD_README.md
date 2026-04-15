# Dashboard Feature Documentation

## Overview
The Dashboard displays symbol performance statistics based on orders stored in `Orders.json`. It shows price changes and percentage increases for each traded symbol.

## Features

### Symbol Statistics Display
- **Symbol Name**: Stock/asset symbol from your orders
- **Opening Price**: Average price from your historical orders
- **Current Price**: Simulated current price (with random variation for demo)
- **Change**: Absolute price change ($)
- **Change %**: Percentage change
- **Total Orders**: Number of orders placed for this symbol
- **Total Quantity**: Total shares/units traded

### Visual Indicators
- **Green text**: Positive price change (gains)
- **Red text**: Negative price change (losses)
- **Color-coded percentage**: Easy visualization of performance

### Portfolio Summary
- Total number of symbols traded
- Active positions count
- Last updated timestamp

## How It Works

### Data Flow
1. **DashboardViewModel** reads `Orders.json`
2. Groups orders by symbol
3. Calculates statistics:
   - Average order price (used as "opening price")
   - Simulated current price (random +/- variation)
   - Price change and percentage
   - Order counts and quantities
4. Displays in DataGrid with color-coded values

### Price Simulation
Since we don't have real-time market data, the dashboard:
- Uses average order price as baseline
- Applies random variation (-5% to +10%)
- Shows realistic price movements

## Navigation

### From MainWindow:
- Click **"Open Dashboard"** button → Opens Dashboard
- Click **"Open Trading Window"** button → Opens Trading Window

### From Dashboard:
- Click **"Trading"** button in sidebar → Opens Trading Window

## Components

### Models
- **SymbolStatistics.cs**: Data model for symbol performance
  - Symbol, prices, changes, counts
  - Formatted properties for display

### ViewModels
- **DashboardViewModel.cs**: MVVM ViewModel
  - Loads and processes order data
  - Calculates statistics
  - Provides ObservableCollection for binding

### Views
- **DashboardWindow.xaml**: UI layout
  - Modern dark theme
  - Sidebar navigation
  - Summary cards
  - Performance grid with color coding

### Converters
- **GreaterThanZeroConverter.cs**: Detects positive values
- **LessThanZeroConverter.cs**: Detects negative values
- Used for conditional color formatting

## Usage Instructions

1. **Create Orders First**
   - Open Trading Window
   - Submit orders with symbols (e.g., AAPL, MSFT, GOOGL)
   - Orders are saved to Orders.json

2. **View Dashboard**
   - Click "Open Dashboard" from MainWindow
   - Dashboard loads and displays symbol statistics

3. **Interpret Data**
   - Green % = Symbol gaining value
   - Red % = Symbol losing value
   - Higher % = Larger price movement

## Technical Details

### MVVM Architecture
- **Model**: SymbolStatistics, Order, OrdersData
- **View**: DashboardWindow.xaml
- **ViewModel**: DashboardViewModel
- **Commands**: RelayCommand
- **Converters**: Color coding converters

### Data Binding
```xaml
<!-- Collection binding -->
<DataGrid ItemsSource="{Binding SymbolStats}"/>

<!-- Property binding with formatting -->
<TextBlock Text="{Binding PercentageChangeFormatted}"/>

<!-- Conditional styling with converters -->
<DataTrigger Binding="{Binding PercentageChange, Converter={StaticResource GreaterThanZeroConverter}}" Value="True">
    <Setter Property="Foreground" Value="#81C784"/>
</DataTrigger>
```

### Statistics Calculation
```csharp
// Average price from orders
var averagePrice = orders.Average(o => o.Price);

// Simulated current price
var priceVariation = (decimal)(_random.NextDouble() * 0.15 - 0.05);
var currentPrice = averagePrice * (1 + priceVariation);

// Calculate change
var priceChange = currentPrice - averagePrice;
var percentageChange = (priceChange / averagePrice) * 100;
```

## Future Enhancements

1. **Real-time Data Integration**
   - Connect to market data API
   - Live price updates
   - WebSocket streaming

2. **Charts & Graphs**
   - Price history charts
   - Performance trends
   - Volume analysis

3. **Advanced Analytics**
   - Profit/Loss calculations
   - Risk metrics
   - Portfolio diversification

4. **Filtering & Sorting**
   - Filter by gain/loss
   - Sort by performance
   - Search symbols

5. **Refresh Functionality**
   - Auto-refresh timer
   - Manual refresh button
   - Configurable intervals

## Color Scheme

- **Background**: #1A2233 (Dark blue-gray)
- **Cards**: #232B3E (Slightly lighter)
- **Accent**: #4FC3F7 (Cyan blue)
- **Positive**: #81C784 (Green)
- **Negative**: #E57373 (Red)
- **Warning**: #FFD54F (Amber)
- **Active**: #1976D2 (Blue)

## Example Data

### Sample Orders.json
```json
{
  "OpenOrders": [],
  "TradeHistory": [
    {
      "OrderId": "guid-1",
      "Symbol": "AAPL",
      "Quantity": 100,
      "Price": 150.00,
      "Side": "Buy",
      "Status": "Filled",
      "Timestamp": "2024-01-15T10:30:00"
    },
    {
      "OrderId": "guid-2",
      "Symbol": "AAPL",
      "Quantity": 50,
      "Price": 152.00,
      "Side": "Buy",
      "Status": "Filled",
      "Timestamp": "2024-01-15T14:00:00"
    }
  ]
}
```

### Resulting Dashboard Display
- Symbol: AAPL
- Opening Price: $151.00 (average of 150 and 152)
- Current Price: $155.55 (simulated)
- Change: +$4.55
- Change %: +3.01% (in green)
- Total Orders: 2
- Total Quantity: 150

## Troubleshooting

### No Data Displayed
- Ensure Orders.json exists
- Verify orders have valid Symbol values
- Check that orders have been saved from Trading Window

### Color Coding Not Working
- Verify converters are registered in Window.Resources
- Check converter namespace imports
- Ensure DataTriggers are properly configured

### Navigation Issues
- Verify event handlers are connected
- Check button Click attributes
- Ensure code-behind methods exist
