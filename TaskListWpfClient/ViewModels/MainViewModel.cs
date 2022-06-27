using Grpc.Net.Client;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Controls;
using TaskListGrpcServer.Protos;
using TaskListWpfClient.Models;

namespace TaskListWpfClient.ViewModels
{
    public class MainViewModel
    {
        public ComboBoxItem SelectStatus { get; set; } = new() { DataContext = "5" };
        public ObservableCollection<EmployeeProto> EmployeesSearch { get; set; } = new();
        public EmployeeProto TasksSearchSelectEmployee { get; set; } = new();
        public ObservableCollection<CheckedTag> TasksSearchSelectTags { get; set; } = new();
        public string NameTaskSearch { get; set; } = string.Empty;
        public string DescriptionTaskSearch { get; set; } = string.Empty;

        public TaskProto SelectTask { get; set; } = new();
        public EmployeeProto SelectEmployee { get; set; } = new();
        public TagProto SelectTag { get; set; } = new();

        public ObservableCollection<TaskProto> Tasks { get; set; } = new();
        public ObservableCollection<TaskProto> TasksRelevant { get; set; } = new();
        public ObservableCollection<EmployeeProto> Employees { get; set; } = new();

        public ObservableCollection<TagProto> Tags { get; set; } = new();

        private readonly TaskList.TaskListClient Client; 

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

        public MainViewModel(string address)
        {
            Client = new(GrpcChannel.ForAddress(address));
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

            UpdateDatabase();
        }

        public void SearchTask()
        {
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
                if (item.Selected != false) tags.Add(item.Tag);
            if (tags.Count != 0)
                return new ObservableCollection<TaskProto>(list.Where(obj => obj.Tags.ListTag.Intersect(tags).ToList().Count != 0));
            return list;
        }

        public ObservableCollection<TaskProto> NameСheck(ObservableCollection<TaskProto> list)
        {
            if (NameTaskSearch != String.Empty)
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
            return int.Parse((string)SelectStatus.DataContext) switch
            {
                0 => new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.New)),
                1 => new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Assigned)),
                2 => new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Discussion)),
                3 => new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Completed)),
                4 => new ObservableCollection<TaskProto>(list.Where(obj => obj.CurrentState == Status.Closed)),
                _ => list,
            };
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
            if (SearchSelectEmployee != null)
                return new ObservableCollection<TaskProto>(tasks.Where(obj => obj.Executor.Surname == SearchSelectEmployee.Surname));
            return tasks;
        }

        public void ChangeEmployee()
        {
            if (SelectEmployee.Id != 0)
            {
                EmployeeWindow changeEmployee = new(this, SelectEmployee);
                App.Current.MainWindow.Hide();
                changeEmployee.Show();
            }
        }


        public void ChangeTask()
        {
            if (SelectTask.Id != 0)
            {
                TaskWindow createTask = new(this, Employees, Tags, SelectTask);
                App.Current.MainWindow.Hide();
                createTask.Show();
            }
        }

        public void ChangeTag()
        {
            if (SelectTag.Id != 0)
            {
                TagWindow changeTag = new(this, SelectTag);
                App.Current.MainWindow.Hide();
                changeTag.Show();
            }
        }

        public void CreateTask()
        {
            TaskWindow createTask = new(this, Employees, Tags);
            App.Current.MainWindow.Hide();
            createTask.Show();
        }

        public void CreateEmployee()
        {
            EmployeeWindow changeEmployee = new(this);
            App.Current.MainWindow.Hide();
            changeEmployee.Show();
        }

        public void CreateTag()
        {
            TagWindow createTask = new(this);
            App.Current.MainWindow.Hide();
            createTask.Show();
        }

        private void DeleteTask()
        {
            if (SelectTask != null)
            {
                Client.DeleteTask(SelectTask);
                UpdateDatabase();
            }
        }

        private void DeleteTag()
        {
            if (SelectTag != null)
            {
                Client.DeleteTag(SelectTag);
                UpdateDatabase();
            }
        }

        private void DeleteEmployee()
        {
            if (SelectEmployee != null)
            {
                Client.DeleteEmployee(SelectEmployee);
                UpdateDatabase();
            }
        }

        public void UpdateDatabase()
        {
            UpdateDatabaseTask();
            UpdateDatabaseEmployee();
            UpdateDatabaseTag();
        }

        private void UpdateDatabaseTask()
        {
            Tasks.Clear();
            var allTasks = Client.GetAllTask(new NullRequest());
            if (allTasks.Success == true)
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
            TasksSearchSelectTags.Clear();
            foreach (var tag in allTag.Tagslist.ListTag)
            {
                Tags.Add(tag);
                TasksSearchSelectTags.Add(new(tag));
            }
        }

        public void AddTask(TaskProto taskProto)
        {
            Client.AddTask(taskProto);
        }

        public void UpdateTask(TaskProto taskProto)
        {
            Client.UpdateTask(taskProto);
        }

        public void AddTag(TagProto tagProto)
        {
            Client.AddTag(tagProto);
        }

        public void UpdateTag(TagProto tagProto)
        {
            Client.UpdateTag(tagProto);
        }

        public void AddEmployee(EmployeeProto employee)
        {
            Client.AddEmployee(employee);
        }

        public void UpdateEmployee(EmployeeProto employee)
        {
            Client.UpdateEmployee(employee);
        }
    }
}
