using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskListGrpcServer.Protos;

namespace TaskListWpfClient.ViewModels
{
    public class MainViewModel
    {
        public ObservableCollection<ListTask> Tasks { get; set; } = new();

        public MainViewModel()
        {

        }

    }
}
