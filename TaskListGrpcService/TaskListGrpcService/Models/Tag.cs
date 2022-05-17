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
        public int TagId { get; set; }

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
    }
}
