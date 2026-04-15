# Trading Application - MVVM Architecture

## Project Structure

```
Trading/
├── Commands/
│   └── RelayCommand.cs          # ICommand implementation for MVVM
├── Converters/
│   └── InverseBooleanConverter.cs  # Converter for Buy/Sell radio buttons
├── Models/
│   ├── Order.cs                 # Order data model
│   └── OrdersData.cs            # Data container for JSON persistence
├── ViewModels/
│   └── TradingWindowViewModel.cs   # ViewModel for TradingWindow
├── Views/
│   ├── MainWindow.xaml          # Landing page
│   ├── MainWindow.xaml.cs
│   ├── TradingWindow.xaml       # Main trading interface
│   └── TradingWindow.xaml.cs
└── App.xaml
```

## MVVM Pattern Implementation

### Model Layer
**Location**: `Models/`

- **Order.cs**: Represents a trading order with properties:
  - OrderId (unique identifier)
  - Symbol (stock symbol)
  - Quantity
  - Price
  - Side (Buy/Sell)
  - Status (Open/Filled)
  - Timestamp

- **OrdersData.cs**: Container for serialization
  - OpenOrders collection
  - TradeHistory collection

### ViewModel Layer
**Location**: `ViewModels/`

- **TradingWindowViewModel.cs**: Business logic for trading operations
  - Properties:
    - `Symbol`, `Quantity`, `Price` - Bound to input fields
    - `IsBuy` - Bound to Buy/Sell radio buttons
    - `OpenOrders` - ObservableCollection for data grid
    - `TradeHistory` - ObservableCollection for data grid

  - Commands:
    - `SubmitOrderCommand` - ICommand for order submission

  - Methods:
    - `SubmitOrderAsync()` - Creates and processes orders
    - `SaveOrdersAsync()` - Persists orders to JSON
    - `LoadOrders()` - Loads orders from JSON on startup

  - Implements: `INotifyPropertyChanged` for two-way binding

### View Layer
**Location**: Root directory

- **TradingWindow.xaml**: XAML with data bindings
  - TextBoxes bound to ViewModel properties with `UpdateSourceTrigger=PropertyChanged`
  - RadioButtons bound to `IsBuy` property
  - Button bound to `SubmitOrderCommand`
  - DataGrids bound to ObservableCollections

- **TradingWindow.xaml.cs**: Minimal code-behind
  - Sets DataContext to ViewModel instance
  - No business logic (follows MVVM principle)

### Command Layer
**Location**: `Commands/`

- **RelayCommand.cs**: Generic ICommand implementation
  - Executes delegate actions
  - Supports CanExecute for button enabling/disabling
  - Integrates with WPF CommandManager for UI updates

### Converter Layer
**Location**: `Converters/`

- **InverseBooleanConverter.cs**: IValueConverter
  - Inverts boolean for Sell radio button
  - Ensures only one radio button is selected at a time

## Key MVVM Benefits Achieved

1. **Separation of Concerns**
   - UI logic separated from business logic
   - Models independent of Views
   - ViewModels mediate between Views and Models

2. **Testability**
   - ViewModels can be unit tested without UI
   - Business logic testable independently

3. **Maintainability**
   - Changes to UI don't affect business logic
   - Easy to modify data binding without code changes

4. **Reusability**
   - ViewModels and Models can be reused
   - Commands can be shared across views

## Data Binding Examples

### Two-Way Binding
```xaml
<TextBox Text="{Binding Symbol, UpdateSourceTrigger=PropertyChanged}"/>
```

### Command Binding
```xaml
<Button Command="{Binding SubmitOrderCommand}"/>
```

### Collection Binding
```xaml
<DataGrid ItemsSource="{Binding OpenOrders}"/>
```

### Converter Binding
```xaml
<RadioButton IsChecked="{Binding IsBuy, Converter={StaticResource InverseBooleanConverter}}"/>
```

## Data Persistence

- Orders are automatically saved to `Orders.json` in the application directory
- JSON format includes both OpenOrders and TradeHistory
- Orders are loaded on application startup
- Uses System.Text.Json for serialization

## Usage

1. Start the application → MainWindow appears
2. Click "Open Trading Window" → TradingWindow opens
3. Enter order details (Symbol, Quantity, Price)
4. Select Buy or Sell
5. Click "Submit Order"
6. Order appears in Open Orders grid
7. After 1.5 seconds, moves to Trade History
8. All data persisted to Orders.json

## Technologies

- .NET 9
- WPF (Windows Presentation Foundation)
- MVVM Pattern
- Data Binding
- ICommand
- INotifyPropertyChanged
- System.Text.Json

## Future Enhancements

- Add validation attributes to Model
- Implement async RelayCommand for better async/await support
- Add dependency injection for ViewModels
- Implement navigation service
- Add unit tests for ViewModels
- Implement IDataErrorInfo for validation
