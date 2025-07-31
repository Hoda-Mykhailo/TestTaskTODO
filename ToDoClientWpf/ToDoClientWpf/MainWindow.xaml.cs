using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Grpc.Net.Client;
using Grpc.Core;
using Google.Protobuf.WellKnownTypes;
using ToDoProto;

namespace ToDoClientWpf
{
    public partial class MainWindow : Window
    {
        private readonly ToDoService.ToDoServiceClient client;
        public ObservableCollection<ToDoItemViewModel> Items { get; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            var channel = GrpcChannel.ForAddress("http://localhost:50051");
            client = new ToDoService.ToDoServiceClient(channel);

            _ = SubscribeAsync(); // запускаємо стрім без очікування
        }

        private async Task SubscribeAsync()
        {
            var call = client.Subscribe(new ToDoProto.Empty());
            try
            {
                while (await call.ResponseStream.MoveNext())
                {
                    var proto = call.ResponseStream.Current;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var existing = Items.FirstOrDefault(x => x.Id == proto.Id);
                        if (existing != null)
                        {
                            existing.Description = proto.Description;
                            existing.Completed = proto.Completed;
                        }
                        else
                        {
                            Items.Add(new ToDoItemViewModel
                            {
                                Id = proto.Id,
                                Description = proto.Description,
                                Completed = proto.Completed
                            });
                        }
                    });
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                // клієнт закрив стрім
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var desc = txtNew.Text.Trim();
            if (!string.IsNullOrEmpty(desc))
            {
                await client.AddItemAsync(new AddRequest { Description = desc });
                txtNew.Clear();
            }
        }

        private async void dgItems_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgItems.CurrentItem is ToDoItemViewModel vm)
            {
                await client.UpdateStatusAsync(new UpdateRequest { Id = vm.Id });
            }
        }
    }
}