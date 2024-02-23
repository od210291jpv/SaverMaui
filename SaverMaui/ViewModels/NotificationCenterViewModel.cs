using Microsoft.AspNetCore.SignalR.Client;

using SaverMaui.Services.Helpers;
using SaverMaui.SignalRModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.Dispatching;

namespace SaverMaui.ViewModels
{
    public class NotificationCenterViewModel : BaseViewModel
    {
        private static NotificationCenterViewModel Instance;

        HubConnection hubConnection;

        public string Message { get; set; }

        public ObservableCollection<Notification> Messages { get; set; }

        private bool isBusy;

        public bool IsBusy
        {
            get => isBusy;
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;
                    OnPropertyChanged("IsBusy");
                }
            }
        }

        private bool isConnected;

        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected != value)
                {
                    isConnected = value;
                    OnPropertyChanged("IsConnected");
                }
            }
        }

        public Command NotifyClientsCommand { get => notifyClientsCommand; }

        private Command notifyClientsCommand;

        public NotificationCenterViewModel()
        {
            IsConnected = false;
            IsBusy = false;

            this.hubConnection = new HubConnectionBuilder()
                .WithUrl(UriHelper.SendNotificationUrl)
                .Build();

            this.Messages = new ObservableCollection<Notification>();

            this.notifyClientsCommand = new Command(async () => await SendMessage($"[{DateTime.Now}] Trying to establish connection..."), () => true);

            hubConnection.Closed += async (error) =>
            {
                await SendMessage("Connection Closed");
                IsConnected = false;
                await Task.Delay(5000);
                await Connect();
            };

            hubConnection.On<string>("SendNotificationsAsync", async (message) =>
            {
                await Task.Run(() => SendLocalMessage(message));
            });

            Task.Run(() =>
            {
                Application.Current?.Dispatcher.Dispatch(async () =>
                await hubConnection.StartAsync());
            });

            //this.notifyClientsCommand.Execute(this.isConnected);

            Instance = this;
        }

        public static NotificationCenterViewModel GetInstance() 
        {
            if (Instance is null) 
            {
                Instance = new NotificationCenterViewModel();
            }

            return Instance;
        }

        public async Task Connect()
        {
            if (IsConnected)
                return;
            try
            {
                await hubConnection.StartAsync();
                SendLocalMessage("Connected");

                IsConnected = true;
            }
            catch (Exception ex)
            {
                SendLocalMessage($"Error on connection: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            if (!IsConnected)
                return;

            await hubConnection.StopAsync();
            IsConnected = false;
            SendLocalMessage("Disconnected");
        }

        public async Task SendMessage(string message = "")
        {
            if (!isConnected) 
            {
                await Connect();
            }

            try
            {
                IsBusy = true;
                await hubConnection.InvokeAsync("SendNotificationsAsync", message);
            }
            catch (Exception ex)
            {
                SendLocalMessage($"Error sending: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void SendLocalMessage(string message)
        {
            try 
            {
                Messages.Insert(0, new Notification
                {
                    Message = message,
                });
            }
            catch { }
        }

        public new event PropertyChangedEventHandler PropertyChanged;
        public new void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
