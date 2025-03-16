using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using ContactManager.MVVMApp.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContactManager.MVVMApp.ViewModels
{
    using TModel = Models.Contact;
    public partial class ContactsViewModel : ViewModelBase
    {
        #region fields
        private string _filter = string.Empty;
        private TModel? selectedItem;
        private readonly List<TModel> _items = [];
        #endregion fields

        #region properties
        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                ApplyFilter(value);
                OnPropertyChanged();
            }
        }
        public TModel? SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<TModel> Items { get; } = [];
        #endregion properties

        public ContactsViewModel()
        {
            _ = LoadCompaniesAsync();
        }

        #region commands
        [RelayCommand]
        public async Task LoadItems()
        {
            await LoadCompaniesAsync();
        }
        [RelayCommand]
        public async Task AddItem()
        {
            var window = new ContactWindow();
            var viewModel = new ContactViewModel { CloseAction = window.Close };

            window.DataContext = viewModel;
            // Aktuelles Hauptfenster als Parent setzen
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (mainWindow != null)
            {
                await window.ShowDialog(mainWindow);
                _ = LoadCompaniesAsync();
            }
        }
        [RelayCommand]
        public async Task EditItem(TModel model)
        {
            var window = new ContactWindow();
            var viewModel = new ContactViewModel { Model = model, CloseAction = window.Close };

            window.DataContext = viewModel;
            // Aktuelles Hauptfenster als Parent setzen
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            if (mainWindow != null)
            {
                await window.ShowDialog(mainWindow);
                _ = LoadCompaniesAsync();
            }
        }
        [RelayCommand]
        public async Task DeleteItem(TModel model)
        {
            var messageDialog = new MessageDialog("Delete", $"Wollen Sie den Kontact '{model}' löschen?", MessageType.Question);
            var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

            // Aktuelles Hauptfenster als Parent setzen
            await messageDialog.ShowDialog(mainWindow!);

            if (messageDialog.Result == MessageResult.Yes)
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };


                var response = await httpClient.DeleteAsync($"contacts/{model.Id}");

                if (response.IsSuccessStatusCode == false)
                {
                    messageDialog = new MessageDialog("Error", "Beim Löschen ist ein Fehler aufgetreten!", MessageType.Error);
                    await messageDialog.ShowDialog(mainWindow!);
                }
                else
                {
                    _ = LoadCompaniesAsync();
                }
            }
        }
        #endregion commands

        private async void ApplyFilter(string filter)
        {
            // UI-Update sicherstellen
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                var selectedItem = SelectedItem;

                Items.Clear();
                foreach (var item in _items)
                {
                    if (item != null && item.ToString()!.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        Items.Add(item);
                    }
                }
                if (selectedItem != null)
                {
                    SelectedItem = Items.FirstOrDefault(e => e.Id == selectedItem.Id);
                }
            });
        }
        private async Task LoadCompaniesAsync()
        {
            try
            {
                using var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };
                var response = await httpClient.GetStringAsync("contacts");
                var items = JsonSerializer.Deserialize<List<TModel>>(response, _jsonSerializerOptions);

                if (items != null)
                {
                    _items.Clear();
                    foreach (var item in items)
                    {
                        _items.Add(item);
                    }
                    ApplyFilter(Filter);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading companies: {ex.Message}");
            }
        }
    }
}
