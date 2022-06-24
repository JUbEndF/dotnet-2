using TaskListGrpcServer.Protos;

namespace TaskListGrpcServer.Models
{
    /// <summary>
    /// Class describing the structure of tags for tasks
    /// </summary>
    [System.Serializable]
    public class Tag
    {
        /// <summary>
        /// Unique Tag id 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name tag
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// Constructor with tag data
        /// </summary>
        public Tag(string tagName)
        {
            TagName = tagName;
        }

        public Tag(int id, string tagName)
        {
            Id = id;
            TagName = tagName;
        }

        public Tag(TagProto tagProto)
        {
            Id = tagProto.Id;
            TagName = tagProto.Name;
        }

        public Tag()
        {
            Id = -1;
            TagName = string.Empty;
        }

        public TagProto TagToProto()
        {
            return new TagProto { Id = Id, Name = TagName };
        }

        public bool Equals(Tag tag) => TagName == tag.TagName;
    }
}
