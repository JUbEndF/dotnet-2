using Grpc.Net.Client;
using System.Collections.ObjectModel;
using TaskListGrpcServer.Protos;

namespace TaskListWpfClient.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<TaskProto> Tasks { get; set; } = new();

        public ObservableCollection<EmployeeProto> Employees { get; set; } = new();

        public TaskList.TaskListClient TaskListClient = new(GrpcChannel.ForAddress(Properties.Settings.Default.Host));

        public MainViewModel()
        {
            var allTask = TaskListClient.GetAllTask(new NullRequest());
            foreach (var task in allTask.List)
                Tasks.Add(task);
            var allEmploees = TaskListClient.GetAllEmployee(new NullRequest());
            foreach (var employee in allEmploees.ReplyListEmployee)
                Employees.Add(employee);
        }
    }
}
