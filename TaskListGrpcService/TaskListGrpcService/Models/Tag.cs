namespace TaskListGrpcServer.Models
{
    [System.Serializable]
    public class Tag
    {        
        public int TagId { get; set; }
        
        public string TagName { get; set; }

        public int Color { get; set; }

        public Tag(string tagName, int color)
        {
            TagName = tagName;
            Color = color;
        }
    }
}
