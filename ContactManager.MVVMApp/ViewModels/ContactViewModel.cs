using CommunityToolkit.Mvvm.Input;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Windows.Input;
using ContactManager.MVVMApp.Views;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;

namespace ContactManager.MVVMApp.ViewModels
{
    using TModel = Models.Contact;
    public partial class ContactViewModel : ViewModelBase
    {
        #region fields
        private TModel model = new();
        #endregion fields

        #region properties
        public Action? CloseAction { get; set; }
        public TModel Model
        {
            get => model;
            set => model = value ?? new();
        }
        public string FirstName
        {
            get => Model.FirstName;
            set => Model.FirstName = value;
        }
        public string LastName
        {
            get => Model.LastName;
            set => Model.LastName = value;
        }
        public string Company
        {
            get => Model.Company;
            set => Model.Company = value;
        }
        public string Email
        {
            get => Model.Email;
            set => Model.Email = value;
        }
        public string PhoneNumber
        {
            get => Model.PhoneNumber;
            set => Model.PhoneNumber = value;
        }
        public string Address
        {
            get => Model.Address;
            set => Model.Address = value;
        }
        public string Note
        {
            get => Model.Note;
            set => Model.Note = value;
        }
        #endregion properties

        #region commands
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        #endregion commands
        public ContactViewModel()
        {
            CancelCommand = new RelayCommand(() => Close());
            SaveCommand = new RelayCommand(() => Save());
        }
        private void Close()
        {
            CloseAction?.Invoke();
        }
        private async void Save()
        {
            bool canClose = false;
            using var httpClient = new HttpClient { BaseAddress = new Uri(API_BASE_URL) };

            try
            {
                if (Model.Id == 0)
                {
                    var response = httpClient.PostAsync($"Contacts", new StringContent(JsonSerializer.Serialize(Model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        canClose = true;
                    }
                    else
                    {
                        var messageDialog = new MessageDialog("Fehler", "Beim Speichern ist ein Fehler aufgetreten!", MessageType.Error);
                        var mainWindow = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

                        await messageDialog.ShowDialog(mainWindow!);
                        Console.WriteLine($"Fehler beim Abrufen der Contacts. Status: {response.StatusCode}");
                    }
                }
                else
                {
                    var response = httpClient.PutAsync($"Contacts/{Model.Id}", new StringContent(JsonSerializer.Serialize(Model), Encoding.UTF8, "application/json")).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        canClose = true;
                    }
                    else
                    {
                        Console.WriteLine($"Fehler beim Abrufen der Contacts. Status: {response.StatusCode}");
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            if (canClose)
            {
                CloseAction?.Invoke();
            }
        }
    }
}
