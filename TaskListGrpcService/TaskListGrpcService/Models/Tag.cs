namespace TaskListGrpcService.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        
        public string TagName { get; set; }

        public int Color { get; set; }

        public Tag(int tagId, string tagName, int color)
        {
            TagId = tagId;
            TagName = tagName;
            Color = color;
        }
    }
}
