using Grpc.Net.Client;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using TaskListGrpcServer.Protos;
using TaskListGrpcServer.Models;
using System;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;

namespace TaskListWpfClient.ViewModels
{
    public sealed class MainViewModel
    {
        public ComboBoxItem SelectStatus { get; set; } = new() { DataContext = "5"};
        public ObservableCollection<EmployeeProto> EmployeesSearch { get; set; } = new();
        public EmployeeProto TasksSearchSelectEmployee { get; set; } = new();
        public ObservableCollection<Pair<TagProto, bool>> TasksSearchSelectTags { get; set; } = new();
        public string NameTaskSearch { get; set; } = string.Empty;
        public string DescriptionTaskSearch { get; set; } = string.Empty;

        public TaskProto SelectTask { get; set; } = new();
        public EmployeeProto SelectEmployee { get; set; } = new();
        public TagProto SelectTag { get; set; } = new();

        public ObservableCollection<TaskProto> Tasks { get; set; } = new();
        public ObservableCollection<TaskProto> TasksRelevant { get; set; } = new();
        public ObservableCollection<EmployeeProto> Employees { get; set; } = new();

        public ObservableCollection<TagProto> Tags { get; set; } = new();

        private static readonly TaskList.TaskListClient Client = new(GrpcChannel.ForAddress(Properties.Settings.Default.Host));

        public ReactiveCommand<Unit, Unit> UpdateCommand { get; }
        public ReactiveCommand<Unit, Unit> ChangeCommandTask { get; }
        public ReactiveCommand<Unit, Unit> DeteleCommandTask { get; }
        public ReactiveCommand<Unit, Unit> CreateCommandTask { get; }
        public ReactiveCommand<Unit, Unit> DeteleCommandEmployee { get; }
        public ReactiveCommand<Unit, Unit> CreateCommandEmployee { get; }
        public ReactiveCommand<Unit, Unit> ChangeCommandEmployee { get; }
        public ReactiveCommand<Unit, Unit> DeteleCommandTag { get; }
        public ReactiveCommand<Unit, Unit> CreateCommandTag { get; }
        public ReactiveCommand<Unit, Unit> ChangeCommandTag { get; }
        public ReactiveCommand<Unit, Unit> SearchTaskCommand { get; }
        public ReactiveCommand<Unit, Unit> ResetSearchCommand { get; }
        public ReactiveCommand<Unit, Unit> IsSelectedCommand { get; }


        public MainViewModel()
        {
            UpdateCommand = ReactiveCommand.Create(UpdateDatabase);
            DeteleCommandTask = ReactiveCommand.Create(DeleteTask);
            CreateCommandTask = ReactiveCommand.Create(CreateTask);
            DeteleCommandEmployee = ReactiveCommand.Create(DeleteEmployee);
            DeteleCommandTag = ReactiveCommand.Create(DeleteTag);
            CreateCommandEmployee = ReactiveCommand.Create(CreateEmployee);
            CreateCommandTag = ReactiveCommand.Create(CreateTag);
            ChangeCommandEmployee = ReactiveCommand.Create(ChangeEmployee);
            ChangeCommandTask = ReactiveCommand.Create(ChangeTask);
            ChangeCommandTag = ReactiveCommand.Create(ChangeTag);
            SearchTaskCommand = ReactiveCommand.Create(SearchTask);
            ResetSearchCommand = ReactiveCommand.Create(ResetSearch);
            IsSelectedCommand = ReactiveCommand.Create(IsSelected);
            UpdateDatabase();
        }

        public void IsSelected()
        {
            
        }

        public void SearchTask()
        {
            UpdateDatabase();
            var search = NameСheck(Tasks);
            search = StatusСheck(search);
            search = DescriptionСheck(search);
            search = EmployeeSelectionChanged(search, TasksSearchSelectEmployee);
            search = TagsСheck(search);
            TasksRelevant.Clear();
            foreach (var item in search)
                TasksRelevant.Add(item);
        }

        public ObservableCollection<TaskProto> TagsСheck(ObservableCollection<TaskProto> list)
        {
            var tags = new ObservableCollection<TagProto>();
            foreach (var item in TasksSearchSelectTags)
                if (item.)tags.Add(item);
            if(TasksSearchSelectTags.Count != 0)
                return new ObservableCollection<TaskProto>(list.Where(obj => obj.Tags.ListTag.Intersect().ToList().Count != 0));
            return list;
        }

        public ObservableCollection<TaskProto> NameСheck(ObservableCollection<TaskProto> list)
        {
            if(NameTaskSearch != String.Empty)
                return new ObservableCollection<TaskProto>(list.Where(obj => obj.NameTask.Contains(NameTaskSearch) == true));
            return list;
        }

        public ObservableCollection<TaskProto> DescriptionСheck(ObservableCollection<TaskProto> list)
        {
            if (NameTaskSearch != String.Empty)
                return new ObservableCollection<TaskProto>(list.Where(obj => obj.TaskDescription.Contains(NameTaskSearch) == true));
            return list;
        }

        public ObservableCollection<TaskProto> StatusСheck(ObservableCollection<TaskProto> list)
        {
            switch (int.Parse((string)SelectStatus.DataContext))
            {
                case 0:
                    return new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.New));
                case 1:
                    return new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Assigned));
                case 2:
                    return new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Discussion));
                case 3:
                    return new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Completed));
                case 4:
                    return new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Closed));
                default:
                    return list;
            }
        }

        public void ResetSearch()
        {
            NameTaskSearch = String.Empty;
            DescriptionTaskSearch = String.Empty;
            SelectStatus.DataContext = "5";
            TasksRelevant.Clear();
            foreach (var item in Tasks)
                TasksRelevant.Add(item);
        }



        public ObservableCollection<TaskProto> EmployeeSelectionChanged(ObservableCollection<TaskProto> tasks, EmployeeProto SearchSelectEmployee)
        {
            if(SearchSelectEmployee != null)
                return new ObservableCollection<TaskProto>(tasks.Where(obj => obj.Executor.Surname == SearchSelectEmployee.Surname));
            return tasks;
        }

        public void ChangeEmployee()
        {

        }

        public void ChangeTask()
        {

        }

        public void ChangeTag()
        {

        }

        public void CreateTask()
        {

        }

        public void CreateEmployee()
        {

        }

        public void CreateTag()
        {

        }

        private void DeleteTask()
        {
            Client.DeleteTask(SelectTask);
            UpdateDatabase();
        }

        private void DeleteTag()
        {
            Client.DeleteTag(SelectTag);
            UpdateDatabase();
        }

        private void DeleteEmployee()
        {
            Client.DeleteEmployee(SelectEmployee);
            UpdateDatabase();
        }

        private void UpdateDatabase()
        {
            UpdateDatabaseTask();
            UpdateDatabaseEmployee();
            UpdateDatabaseTag();
        }

        private void UpdateDatabaseTask()
        {
            Tasks.Clear();
            var allTasks = Client.GetAllTask(new NullRequest());
            if(allTasks.Success == true)
                foreach (var task in allTasks.Taskslist.List)
                    Tasks.Add(task);
            TasksRelevant.Clear();
            foreach (var item in Tasks)
                TasksRelevant.Add(item);
        }

        private void UpdateDatabaseEmployee()
        {
            Employees.Clear();
            EmployeesSearch.Clear();
            var allEmployee = Client.GetAllEmployee(new NullRequest { });
            if (allEmployee.Success == true)
                foreach (var employee in allEmployee.Employeeslist.ReplyListEmployee)
                {
                    Employees.Add(employee.Clone());
                    EmployeesSearch.Add(employee.Clone());
                }
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
