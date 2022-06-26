using System.Collections.ObjectModel;
using System.Windows;
using TaskListGrpcServer.Protos;
using TaskListWpfClient.ViewModels;

namespace TaskListWpfClient
{
    /// <summary>
    /// Логика взаимодействия для TaskWindow.xaml
    /// </summary>
    public partial class TaskWindow : Window
    {
        public TaskWindow(MainViewModel mainViewModel,
            ObservableCollection<EmployeeProto> employees,
            ObservableCollection<TagProto> allTag, TaskProto taskProto)
        {
            InitializeComponent();
            DataContext = new TaskViewModel(mainViewModel, employees, allTag, taskProto);
        }

        public TaskWindow(MainViewModel mainViewModel,
            ObservableCollection<EmployeeProto> employees,
            ObservableCollection<TagProto> allTag)
        {
            InitializeComponent();
            DataContext = new TaskViewModel(mainViewModel, employees, allTag);
        }
    }
}
