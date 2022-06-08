using Grpc.Net.Client;
using System.Collections.ObjectModel;
using TaskListGrpcServer.Protos;

namespace TaskListWpfClient.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<ListTaskElement> Tasks { get; set; } = new();

        private TaskList.TaskListClient _client = new(GrpcChannel.ForAddress("https://localhost:5001"));

        public MainViewModel()
        {

        }

    }
}
