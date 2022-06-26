using System.Windows;
using TaskListGrpcServer.Protos;
using TaskListWpfClient.ViewModels;

namespace TaskListWpfClient
{
    /// <summary>
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : Window
    {
        public EmployeeWindow(MainViewModel mainViewModel, EmployeeProto employee)
        {
            InitializeComponent();
            DataContext = new EmployeeViewModel(mainViewModel, employee);
        }

        public EmployeeWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new EmployeeViewModel(mainViewModel);
        }
    }
}
