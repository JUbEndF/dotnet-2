using System.ComponentModel;
using TaskListGrpcServer.Protos;

namespace TaskListWpfClient.Models
{
    public class CheckedTag : INotifyPropertyChanged
    {
        private bool selected;
        private TagProto tag;

        public bool Selected
        {
            get { return selected; }
            set
            {
                selected = value;
            }
        }
        public TagProto Tag
        {
            get { return tag; }
            set
            {
                tag = value;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}