using Microsoft.AspNetCore.SignalR.Client;

using System.ComponentModel;

namespace SignalRServiceClient
{
    public class SignalRService
    {

        public event PropertyChangedEventHandler PropertyChanged;
        public List<string> Messages { get; set; } = new List<string>();

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null) 
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }

        private readonly HubConnection hubConnection;

        public string Message { get; set; }

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

        private readonly string methodName;

        public SignalRService(string uri, string methodName)
        {
            IsConnected = false;
            IsBusy = false;

            this.methodName = methodName;

            this.hubConnection = new HubConnectionBuilder()
                .WithUrl(uri)
                .Build();

            hubConnection.On<string>(methodName, async (message) =>
            {
                await Task.Run(() => this.Messages.Add(message));
            });

            Task.Run(() =>
            {
                Application.Current?.Dispatcher.Dispatch(async () =>
                await hubConnection.StartAsync());
            });
        }

        public async Task Connect()
        {
            //if (IsConnected == true)
            //    return;
            try
            {
                await hubConnection.StartAsync();
                await SendMessage("Connected");

                IsConnected = true;
            }
            catch (Exception ex)
            {
                await SendMessage($"Error on connection: {ex.Message}");
            }
        }

        public async Task Disconnect()
        {
            if (!IsConnected)
                return;

            await hubConnection.StopAsync();
            IsConnected = false;
            await SendMessage("Disconnected");
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
                await hubConnection.InvokeAsync(this.methodName, message);
            }
            catch (Exception ex)
            {
                
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}