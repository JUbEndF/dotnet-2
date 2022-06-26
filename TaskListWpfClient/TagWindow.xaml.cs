using System.Windows;
using TaskListGrpcServer.Protos;
using TaskListWpfClient.ViewModels;

namespace TaskListWpfClient
{
    /// <summary>
    /// Логика взаимодействия для TagWindow.xaml
    /// </summary>
    public partial class TagWindow : Window
    {
        public TagWindow(MainViewModel mainViewModel, TagProto tagProto)
        {
            InitializeComponent();
            DataContext = new TagViewModel(mainViewModel, tagProto);
        }

        public TagWindow(MainViewModel mainViewModel)
        {
            InitializeComponent();
            DataContext = new TagViewModel(mainViewModel);
        }
    }
}
