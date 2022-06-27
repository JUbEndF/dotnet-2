using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows;
using System.Windows.Controls;
using TaskListGrpcServer.Protos;
using TaskListWpfClient.Models;
using System.Linq;

namespace TaskListWpfClient.ViewModels
{
    public class TaskViewModel
    {
        private readonly MainViewModel _mainViewModel;
        public TaskProto ModelTask = new();
        public string Name { get; set; }
        public string Description { get; set; }
        public ObservableCollection<EmployeeProto> Employees { get; set; } = new();
        public EmployeeProto TasksSelectEmployee { get; set; } = new();
        public ObservableCollection<CheckedTag> TasksSelectTags { get; set; } = new();
        public ComboBoxItem SelectStatus { get; set; } = new() { DataContext = "0" };

        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public TaskViewModel(MainViewModel mainViewModel, ObservableCollection<EmployeeProto> employees, ObservableCollection<TagProto> allTag)
        {
            _mainViewModel = mainViewModel;
            Name = String.Empty;
            Description = String.Empty;
            OkCommand = ReactiveCommand.Create(Add);
            CancelCommand = ReactiveCommand.Create(Cancel);
            foreach (var tag in allTag)
            {
                TasksSelectTags.Add(new(tag));
            }
            foreach (var employee in employees)
            {
                Employees.Add(employee.Clone());
            }
        }

        public void TegFromTask(ObservableCollection<TagProto> allTag, TaskProto taskProto)
        {
            TasksSelectTags.Clear();
            if (taskProto.Tags.ListTag != null && allTag.Count != 0)
            {
                foreach (var tag in allTag)
                {
                    if (taskProto.Tags.ListTag.ToList().FindIndex(obj => obj.Id == tag.Id) != -1)
                        TasksSelectTags.Add(new(tag) { Selected = true});
                    else TasksSelectTags.Add(new(tag) { Selected = false });
                }
            }
            else
            {
                foreach (var tag in allTag)
                {
                    TasksSelectTags.Add(new(tag));
                }
            }
        }

        public TaskViewModel(MainViewModel mainViewModel, 
            ObservableCollection<EmployeeProto> employees, 
            ObservableCollection<TagProto> allTag, TaskProto taskProto)
        {
            _mainViewModel = mainViewModel;
            ModelTask = taskProto;
            OkCommand = ReactiveCommand.Create(Add);
            CancelCommand = ReactiveCommand.Create(Cancel);
            Name = taskProto.NameTask;
            Description = taskProto.TaskDescription;
            TegFromTask(allTag, taskProto);
            foreach (var employee in employees)
            {
                Employees.Add(employee.Clone());
            }
            switch (taskProto.CurrentState)
            {
                case Status.New:
                    SelectStatus.DataContext = "0";
                    break;
                case Status.Assigned:
                    SelectStatus.DataContext = "1";
                    break;
                case Status.Discussion:
                    SelectStatus.DataContext = "2";
                    break;
                case Status.Completed:
                    SelectStatus.DataContext = "3";
                    break;
                case Status.Closed:
                    SelectStatus.DataContext = "4";
                    break;
            }
        }

        private TaskProto CreateTask()
        {
            TaskProto? taskProto = new()
            {
                NameTask = Name,
                TaskDescription = Description,
                CurrentState = int.Parse((string)SelectStatus.DataContext) switch
                {
                    0 => Status.New,
                    1 => Status.Assigned,
                    2 => Status.Discussion,
                    3 => Status.Completed,
                    4 => Status.Closed,
                    _ => ModelTask.CurrentState,
                }
            };
            if (TasksSelectEmployee.Id == 0)
            {
                if (ModelTask.Executor == null)
                {
                    taskProto.Executor = new();
                    taskProto.CurrentState = Status.New;
                }
                else
                {
                    taskProto.Executor = ModelTask.Executor;
                    if (ModelTask.Executor.Id != TasksSelectEmployee.Id)
                        taskProto.CurrentState = Status.Assigned;
                }
            }
            else
            {
                taskProto.Executor = TasksSelectEmployee;
                if (ModelTask.Executor == null || ModelTask.Executor.Id != TasksSelectEmployee.Id)
                    taskProto.CurrentState = Status.Assigned;
            }
            taskProto.Id = ModelTask.Id;
            var tags = new TagsProto();
            foreach (var tag in TasksSelectTags)
                if (tag.Selected == true)
                    tags.ListTag.Add(new TagProto() { Name = tag.Tag.Name, Id = tag.Tag.Id });
            taskProto.Tags = tags;
            return taskProto;
        }

        private void Add()
        {
            if (Name == String.Empty ||
                Description == String.Empty)
            {
                MessageBox.Show("The task title and description string cannot be empty");
                return;
            }
            if (ModelTask.Id == 0)
            {
                var result = _mainViewModel.AddTask(CreateTask());
            }
            else _mainViewModel.UpdateTask(CreateTask());
            _mainViewModel.UpdateDatabase();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    window.Show();
                }
                if (window.GetType() == typeof(TaskWindow))
                {
                    window.Hide();
                }
            }
        }

        private void Cancel()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    window.Show();
                }
            }
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(TaskWindow))
                {
                    window.Close();
                }
            }
        }
    }
}
