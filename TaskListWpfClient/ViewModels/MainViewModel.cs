using Grpc.Net.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using TaskListGrpcServer.Protos;
using TaskListGrpcServer.Models;
using System;
using System.Windows.Threading;

namespace TaskListWpfClient.ViewModels
{
    public sealed class MainViewModel
    {
        public Status SelectStatus { get; set; } = new();
        public EmployeeProto TasksSearchSelectEmployee { get; set; } = new();
        public ObservableCollection<TagProto> TasksSearchSelectTags { get; set; } = new();

        public TaskProto SelectTask { get; set; } = new();
        public EmployeeProto SelectEmployee { get; set; } = new();
        public TagProto SelectTag { get; set; } = new();

        public ObservableCollection<TaskProto> Tasks { get; set; } = new();
        public ObservableCollection<TaskProto> TasksRelevant { get; set; } = new();
        public ObservableCollection<EmployeeProto> Employees { get; set; } = new();

        public ObservableCollection<TagProto> Tags { get; set; } = new();

        private static readonly TaskList.TaskListClient Client = new(GrpcChannel.ForAddress(Properties.Settings.Default.Host));

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> DeteleCommand { get; }
        public ReactiveCommand<Unit, Unit> CreateTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> ComboBoxEmployeeSelectionChanged { get; set; }

        public MainViewModel()
        {
            UpdateCommand = ReactiveCommand.Create(UpdateDatabase);
            DeteleCommand = ReactiveCommand.Create(DeleteTask);
            CreateTaskCommand = ReactiveCommand.Create(CreateTask);
            ComboBoxEmployeeSelectionChanged = ReactiveCommand.Create(EmployeeSelectionChanged);

            UpdateDatabase();
        }

        public void EmployeeSelectionChanged()
        {
            TasksRelevant = (ObservableCollection<TaskProto>)Tasks.Where(obj => obj.Executor.Surname == SelectEmployee.Surname);
        }

        public void CreateTask()
        {

        }

        private void DeleteTask()
        {
            //TaskListClient.DeleteTask(SelectTask);
            //UpdateDatabase();
        }

        private void UpdateDatabase()
        {
            UpdateDatabaseTask();
            UpdateDatabaseEmployee();
            UpdateDatabaseTag();
        }

        private void UpdateDatabaseTask()
        {
            App.Current.MainWindow.Dispatcher.Invoke(new Action(
            delegate ()
            {
                Tasks.Clear();
                foreach (var task in Client.GetAllTask(new NullRequest { }).Taskslist.List)
                    Tasks.Add(task);
            }));
            TasksRelevant = Tasks;
        }

        private void UpdateDatabaseEmployee()
        {
            Employees.Clear();
            foreach (var employee in Client.GetAllEmployee(new NullRequest { }).Employeeslist.ReplyListEmployee)
                Employees.Add(employee.Clone());
        }

        private void UpdateDatabaseTag()
        {
            var allTag = Client.GetAllTag(new NullRequest { });
            Tags.Clear();
            foreach (var tag in allTag.Tagslist.ListTag)
                Tags.Add(tag);
        }   
    }
}
