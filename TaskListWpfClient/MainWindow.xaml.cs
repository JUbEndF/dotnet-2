using System.Windows;
using TaskListWpfClient.ViewModels;

namespace TaskListWpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(string address)
        {
            InitializeComponent();
            DataContext = new MainViewModel(address);
        }
    }
}
