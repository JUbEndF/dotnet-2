using ProtoBuf;
using System.Collections.Generic;
using TaskListGrpcServer.Protos;

namespace TaskListGrpcServer.Models
{
    /// <summary>
    /// Class describing the task
    /// </summary>
    [System.Serializable]
    [ProtoContract]
    public class TaskElement
    {
        /// <summary>
        /// Unique task id 
        /// </summary>
        public int UniqueId { get; set; }

        /// <summary>
        /// Task name
        /// </summary>
        public string NameTask { get; set; }

        /// <summary>
        /// String describing the task
        /// </summary>
        public string DescriptionTask { get; set; }

        /// <summary>
        /// The executor of this task
        /// </summary>
        public Employee ExecutorTask { get; set; }

        /// <summary>
        /// The current state of the job
        /// </summary>
        public Status CurrentStatus { get; set; }

        /// <summary>
        ///set of tags associated with a task 
        /// </summary>
        public List<Tag> ListTags { get; set; }

        /// <summary>
        /// Constructor with task data
        /// </summary>
        public TaskElement(string name, string description, Employee executor, Status status, List<Tag> tags)
        {
            UniqueId = -1;
            NameTask = name;
            DescriptionTask = description;
            ExecutorTask = executor;
            CurrentStatus = status;
            ListTags = tags;
        }

        public TaskElement()
        {
            UniqueId = -1;
            NameTask = string.Empty;
            DescriptionTask = string.Empty;
            CurrentStatus = Status.New;
            ExecutorTask = new Employee();
            ListTags = new List<Tag>();
        }

        public TagsProto TagsToProto()
        {
            var tags = new TagsProto();
            foreach (var tag in ListTags)
                tags.ListTag.Add(new TagProto { Color = tag.Color, Id = tag.TagId, Name = tag.TagName });
            return tags;
        }

        public static List<Tag> ProtoToTags(TagsProto tagsProto)
        {
            var tags = new List<Tag>();
            foreach (var tag in tagsProto.ListTag)
                tags.Add(new Tag(tag.Id, tag.Name, tag.Color));
            return tags;
        }
    }
}
