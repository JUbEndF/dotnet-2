using ReactiveUI;
using System;
using System.Reactive;
using System.Windows;
using TaskListGrpcServer.Protos;

namespace TaskListWpfClient.ViewModels
{
    public class EmployeeViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public EmployeeProto Employee { get; set; } = new();
        private readonly MainViewModel _mainViewModel;
        public ReactiveCommand<Unit, Unit> OkCommand { get; }
        public ReactiveCommand<Unit, Unit> CancelCommand { get; }

        public EmployeeViewModel(MainViewModel mainViewModel, EmployeeProto employeeProto)
        {
            _mainViewModel = mainViewModel;
            Employee = employeeProto;
            Name = employeeProto.Name;
            Surname = employeeProto.Surname;
            OkCommand = ReactiveCommand.Create(Add);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        public EmployeeViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            OkCommand = ReactiveCommand.Create(Add);
            CancelCommand = ReactiveCommand.Create(Cancel);
        }

        private void Add()
        {
            if (Name == String.Empty)
            {
                MessageBox.Show("The name string cannot be empty");
                return;
            }
            Employee.Name = Name;
            Employee.Surname = Surname;
            if (Employee.Id == 0)
                _mainViewModel.AddEmployee(Employee);
            else _mainViewModel.UpdateEmployee(Employee);
            _mainViewModel.UpdateDatabase();
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    window.Show();
                }
            }
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(EmployeeWindow))
                {
                    window.Close();
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
                if (window.GetType() == typeof(EmployeeWindow))
                {
                    window.Close();
                }
            }
        }
    }
}
