using TaskListGrpcServer.Protos;

namespace TaskListWpfClient.Models
{
    public class CheckedTag
    {
        private bool _selected;
        public TagProto Tag { get; set; }

        public CheckedTag(TagProto tag)
        {
            Tag = tag;
        }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
            }
        }
    }
}