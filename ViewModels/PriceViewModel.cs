using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;
using Elkollen.Models;
using Elkollen.Services;

namespace Elkollen.ViewModels
{
    public partial class PricesViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private bool _isInitializing = true;
        private bool _isLoadingData = false;

        [ObservableProperty]
        private ObservableCollection<QuantityPriceModel> quantityPrices = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool hasError;

        [ObservableProperty]
        private DateTime startTime;

        [ObservableProperty]
        private DateTime stopTime;

        [ObservableProperty]
        private decimal totalQuantity;

        [ObservableProperty]
        private decimal totalCost;
        public string FormattedTotalQuantity => $"Summa: {TotalQuantity:F3} kWh";
        public string FormattedTotalCost => $"Kostnad: {(TotalCost / 10):F2} kr";

        public PricesViewModel(IAuthService authService)
        {
            _authService = authService;

            // Initialize date range
            _isInitializing = true;
            StartTime = DateTime.Today.AddDays(-2);
            StopTime = DateTime.Today.AddDays(-1);
            _isInitializing = false;

            // Load prices when the view model is created
            LoadQuantitiesPricesCommand.Execute(null);
        }

        // Helper method to normalize dates
        private DateTime GetNormalizedDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Local);
        }
        // Helper method to calculate totals
        private void CalculateTotals()
        {
            TotalQuantity = QuantityPrices.Sum(qp => qp.Quantity);
            TotalCost = QuantityPrices.Sum(qp => qp.TotalCost * 10);
            OnPropertyChanged(nameof(FormattedTotalQuantity));
            OnPropertyChanged(nameof(FormattedTotalCost));
        }

        [RelayCommand]
        private async Task LoadQuantitiesPrices()
        {
            // Prevent multiple concurrent calls
            if (_isLoadingData)
                return;

            _isLoadingData = true;
            HasError = false;
            ErrorMessage = string.Empty;
            IsLoading = true;

            try
            {
                Debug.WriteLine($"Loading data for StartTime: {StartTime:yyyy-MM-dd}, StopTime: {StopTime:yyyy-MM-dd}");
                var client = HttpClientFactory.CreateHttpClient();

                // Add auth token if the user is authenticated
                string token = await _authService.GetTokenAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }

                // Ensure dates are normalized to midnight
                DateTime normalizedStartTime = GetNormalizedDate(StartTime);
                DateTime normalizedStopTime = normalizedStartTime.AddDays(1);

                // Build the query string with start and stop time parameters (use ISO format)
                string startTimeParam = normalizedStartTime.ToString("o");
                string stopTimeParam = normalizedStopTime.ToString("o");

                string requestPricesUri = $"/api/Prices?startTime={Uri.EscapeDataString(startTimeParam)}&stopTime={Uri.EscapeDataString(stopTimeParam)}";
                string requestQuantitiesUri = $"/api/Metering?startTime={Uri.EscapeDataString(startTimeParam)}&stopTime={Uri.EscapeDataString(stopTimeParam)}";

                var pricesResponse = await client.GetAsync(requestPricesUri);
                var quantitiesResponse = await client.GetAsync(requestQuantitiesUri);

                if (pricesResponse.IsSuccessStatusCode && quantitiesResponse.IsSuccessStatusCode)
                {
                    var priceContent = await pricesResponse.Content.ReadAsStringAsync();
                    var quantityContent = await quantitiesResponse.Content.ReadAsStringAsync();

                    var fetchedPrices = JsonSerializer.Deserialize<List<PriceModel>>(priceContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    var fetchedQuantities = JsonSerializer.Deserialize<List<QuantityModel>>(quantityContent,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    Debug.WriteLine($"Prices count: {fetchedPrices?.Count ?? 0}, Quantities count: {fetchedQuantities?.Count ?? 0}");

                    // Check if the fetched prices and quantities are not null and have the same number of elements
                    if (fetchedPrices == null || fetchedQuantities == null || fetchedPrices.Count != fetchedQuantities.Count)
                    {
                        HasError = true;
                        ErrorMessage = $"Error: Invalid response - Prices: {fetchedPrices?.Count ?? 0}, Quantities: {fetchedQuantities?.Count ?? 0}";
                        return;
                    }

                    QuantityPrices.Clear();

                    foreach (var (price, quantity) in fetchedPrices.Zip(fetchedQuantities, (price, quantity) => (price, quantity)))
                    {
                        QuantityPrices.Add(new QuantityPriceModel
                        {
                            StartTime = quantity.FormattedStartTime,
                            EndTime = quantity.FormattedEndTime,
                            Quantity = quantity.amount,
                            Price = price.price
                        });
                    }
                    // Calculate totals
                    CalculateTotals();
                }
                else
                {
                    HasError = true;
                    ErrorMessage = $"Error: Fetch failed - Prices: {pricesResponse.StatusCode}, Quantities: {quantitiesResponse.StatusCode}";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error: {ex.Message}";
                Debug.WriteLine($"Exception in LoadQuantitiesPrices: {ex}");
            }
            finally
            {
                IsLoading = false;
                _isLoadingData = false;
            }
        }

        // Simplified property changed handlers
        partial void OnStartTimeChanged(DateTime value)
        {
            if (!_isInitializing)
            {
                Debug.WriteLine($"StartTime changed to {value:yyyy-MM-dd}");
                LoadQuantitiesPricesCommand.Execute(null);
            }
        }

        partial void OnStopTimeChanged(DateTime value)
        {
            if (!_isInitializing)
            {
                Debug.WriteLine($"StopTime changed to {value:yyyy-MM-dd}");
                LoadQuantitiesPricesCommand.Execute(null);
            }
        }
    }
}