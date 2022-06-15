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
        /// The color to display the background of the tag
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// Constructor with tag data
        /// </summary>
        public Tag(string tagName, int color)
        {
            TagName = tagName;
            Color = color;
        }

        public Tag(int id, string tagName, int color)
        {
            Id = id;
            TagName = tagName;
            Color = color;
        }

        public Tag(TagProto tagProto)
        {
            Id = tagProto.Id;
            TagName = tagProto.Name;
            Color = tagProto.Color;
        }

        public TagProto TagToProto()
        {
            return new TagProto { Color = Color, Id = Id, Name = TagName };
        }

        public bool Equals(Tag tag) => TagName == tag.TagName;
    }
}
