# Auto-Refresh Feature Documentation

## Overview
The Dashboard now automatically refreshes symbol statistics every second, simulating real-time market data updates.

## Features

### Auto-Refresh Mechanism
- **Refresh Interval**: 1 second
- **What Updates**: Current prices, price changes, percentage changes, and last updated timestamp
- **Visual Indicator**: Pulsing "● Auto-Refreshing" indicator in green
- **Resource Management**: Proper disposal of timer when window closes

## Implementation Details

### Technical Architecture

#### 1. DispatcherTimer
```csharp
_refreshTimer = new DispatcherTimer
{
    Interval = TimeSpan.FromSeconds(1)
};
_refreshTimer.Tick += RefreshTimer_Tick;
_refreshTimer.Start();
```

- Uses WPF's `DispatcherTimer` for UI thread-safe updates
- Fires every 1 second
- Automatically handles UI synchronization

#### 2. Price Simulation
```csharp
var priceVariation = (decimal)(_random.NextDouble() * 0.15 - 0.05); // -5% to +10%
var newCurrentPrice = basePrice * (1 + priceVariation);
```

- Base price stored from initial order average
- Each refresh applies new random variation
- Variation range: -5% to +10%
- Creates realistic price movements

#### 3. INotifyPropertyChanged Implementation
```csharp
public class SymbolStatistics : INotifyPropertyChanged
{
    private decimal _currentPrice;

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
}
```

- All properties notify UI of changes
- Formatted properties also update automatically
- Ensures real-time UI updates

#### 4. IDisposable Pattern
```csharp
public void Dispose()
{
    if (_refreshTimer != null)
    {
        _refreshTimer.Stop();
        _refreshTimer.Tick -= RefreshTimer_Tick;
    }
}
```

- Properly stops timer when window closes
- Prevents memory leaks
- Unsubscribes from events

### Data Flow

1. **Initial Load** (DashboardViewModel constructor):
   - Load orders from Orders.json
   - Calculate base prices (average of order prices)
   - Store base prices in dictionary
   - Generate initial current prices with variation
   - Display in UI

2. **Every Second** (Timer Tick):
   - For each symbol in SymbolStats collection:
     - Get stored base price
     - Apply new random variation
     - Calculate new current price
     - Update PriceChange and PercentageChange
     - Update LastUpdated timestamp
   - UI automatically refreshes due to INotifyPropertyChanged

3. **Window Close**:
   - Dispose method called
   - Timer stopped
   - Events unsubscribed
   - Resources cleaned up

## Visual Indicators

### Auto-Refresh Indicator
```xaml
<TextBlock Text="● Auto-Refreshing" FontSize="12" Foreground="#81C784">
    <TextBlock.Triggers>
        <EventTrigger RoutedEvent="Loaded">
            <BeginStoryboard>
                <Storyboard RepeatBehavior="Forever">
                    <DoubleAnimation Storyboard.TargetProperty="Opacity" 
                                   From="1.0" To="0.3" Duration="0:0:1" 
                                   AutoReverse="True"/>
                </Storyboard>
            </BeginStoryboard>
        </EventTrigger>
    </TextBlock.Triggers>
</TextBlock>
```

- Pulsing green indicator
- Fades from 100% to 30% opacity
- 1-second animation cycle (matches refresh rate)
- Repeats forever

### Last Updated Timestamp
```xaml
<TextBlock Text="{Binding SymbolStats[0].LastUpdated, StringFormat='HH:mm:ss'}"/>
```

- Displays time in HH:mm:ss format
- Updates every second with refresh
- Shows exact time of last price update

### Color-Coded Values
- **Green**: Positive price changes (gains)
- **Red**: Negative price changes (losses)
- **Colors update dynamically** as prices change

## Performance Considerations

### Optimizations
1. **In-place Updates**: Updates existing SymbolStatistics objects rather than recreating
2. **Cached Base Prices**: Stores base prices in dictionary to avoid recalculation
3. **UI Thread**: Uses DispatcherTimer for automatic UI thread marshaling
4. **Efficient Bindings**: Uses two-way data binding with proper change notifications

### Resource Usage
- **Memory**: Minimal (only stores base prices dictionary)
- **CPU**: Low (simple arithmetic calculations every second)
- **UI Thread**: Efficient (batch property changes)

## Configuration

### Change Refresh Interval
Modify the interval in `DashboardViewModel.cs`:

```csharp
_refreshTimer = new DispatcherTimer
{
    Interval = TimeSpan.FromSeconds(1)  // Change this value
};
```

Examples:
- `TimeSpan.FromMilliseconds(500)` - Refresh every 0.5 seconds
- `TimeSpan.FromSeconds(5)` - Refresh every 5 seconds
- `TimeSpan.FromMinutes(1)` - Refresh every minute

### Change Price Variation Range
Modify in `RefreshSymbolStatistics()` method:

```csharp
// Current: -5% to +10%
var priceVariation = (decimal)(_random.NextDouble() * 0.15 - 0.05);

// Smaller range: -2% to +5%
var priceVariation = (decimal)(_random.NextDouble() * 0.07 - 0.02);

// Larger range: -10% to +20%
var priceVariation = (decimal)(_random.NextDouble() * 0.30 - 0.10);
```

## Usage

### Viewing Auto-Refresh
1. Open Dashboard from MainWindow
2. Notice the pulsing "● Auto-Refreshing" indicator
3. Watch prices change every second
4. Observe "Last Updated" timestamp updating
5. See color changes as prices move up/down

### Example Behavior
```
Time      Symbol  Price    Change    Change %
14:30:00  AAPL   $185.25  +$2.50    +1.37% (Green)
14:30:01  AAPL   $184.10  +$1.35    +0.74% (Green)
14:30:02  AAPL   $182.90  -$0.85    -0.46% (Red)
14:30:03  AAPL   $186.45  +$3.70    +2.03% (Green)
```

## Troubleshooting

### Prices Not Updating
- **Check**: Timer is started in constructor
- **Verify**: Window is not closed
- **Ensure**: Orders.json has valid data

### UI Freezing
- **Cause**: Likely not using DispatcherTimer
- **Solution**: Ensure using `System.Windows.Threading.DispatcherTimer`
- **Check**: Not using `System.Timers.Timer` (wrong thread)

### Memory Leak
- **Cause**: Timer not disposed
- **Solution**: Ensure `Dispose()` is called on window close
- **Verify**: `Closed` event is subscribed in DashboardWindow.xaml.cs

### Colors Not Updating
- **Cause**: SymbolStatistics not implementing INotifyPropertyChanged
- **Solution**: Verify all properties raise PropertyChanged events
- **Check**: Converters are properly registered in Window.Resources

## Code Changes Summary

### Modified Files

1. **DashboardViewModel.cs**
   - Added `IDisposable` interface
   - Added `DispatcherTimer` field
   - Added `_symbolBasePrices` dictionary
   - Added `RefreshTimer_Tick` event handler
   - Added `RefreshSymbolStatistics()` method
   - Added `Dispose()` method
   - Modified constructor to start timer

2. **SymbolStatistics.cs**
   - Implemented `INotifyPropertyChanged`
   - Converted all properties to notify properties
   - Added backing fields for all properties
   - Added proper OnPropertyChanged calls

3. **DashboardWindow.xaml.cs**
   - Added ViewModel field
   - Added `Closed` event handler
   - Added `Dispose()` call in close handler

4. **DashboardWindow.xaml**
   - Added "Auto-Refreshing" indicator
   - Added pulsing animation to indicator
   - Updated layout for new indicator

## Benefits

### For Users
- **Real-time Updates**: See price changes without manual refresh
- **Better UX**: Automatic updates create dynamic experience
- **Visual Feedback**: Pulsing indicator shows system is working
- **Accurate Data**: Timestamp shows exact update time

### For Development
- **MVVM Compliant**: Proper separation of concerns
- **Testable**: Timer can be mocked for unit tests
- **Maintainable**: Clean code with proper disposal
- **Extensible**: Easy to connect to real market data API

## Future Enhancements

1. **Configurable Interval**: Allow user to set refresh rate
2. **Pause/Resume**: Add button to pause auto-refresh
3. **Real API Integration**: Replace simulation with real market data
4. **Performance Mode**: Reduce refresh rate when window not focused
5. **Alert Thresholds**: Notify user when price changes exceed threshold
6. **Historical Chart**: Track and display price history over time
7. **Selective Refresh**: Only refresh visible symbols
8. **Batch Updates**: Group multiple symbol updates for efficiency

## Testing

### Manual Testing Steps
1. Open Dashboard
2. Verify "Auto-Refreshing" indicator is pulsing
3. Watch at least 5 refresh cycles (5 seconds)
4. Verify prices are changing
5. Verify "Last Updated" timestamp updates
6. Verify colors change (green/red) appropriately
7. Close window
8. Verify no errors in debug output

### Performance Testing
1. Open Dashboard with many symbols (10+)
2. Monitor CPU usage (should be <1%)
3. Monitor memory (should be stable)
4. Leave open for 5 minutes
5. Verify smooth operation
6. Close and verify cleanup

### Integration Testing
1. Create orders in Trading Window
2. Open Dashboard
3. Verify symbols appear
4. Verify auto-refresh works
5. Create more orders
6. Close and reopen Dashboard
7. Verify new symbols included
8. Verify refresh still working

## API for Real Market Data

### Example Integration (Future)
```csharp
// Replace simulation with real API call
private async Task RefreshSymbolStatistics()
{
    foreach (var stats in SymbolStats)
    {
        // Call real market data API
        var marketData = await _marketDataService.GetQuote(stats.Symbol);

        stats.CurrentPrice = marketData.LastPrice;
        stats.PriceChange = marketData.LastPrice - stats.OpeningPrice;
        stats.PercentageChange = (stats.PriceChange / stats.OpeningPrice) * 100;
        stats.LastUpdated = marketData.Timestamp;
    }
}
```

Popular APIs:
- Alpha Vantage
- IEX Cloud
- Finnhub
- Yahoo Finance
- Polygon.io
