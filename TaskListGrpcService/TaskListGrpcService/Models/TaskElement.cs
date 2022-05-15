using System.Collections.Generic;

namespace TaskListGrpcServer.Models
{
    public enum Status
    {
        NEW,
        ASSIGNED,
        DISCUSSION,
        COMPLETED,
        CLOSED,
    }
    [System.Serializable]
    public class TaskElement
    {
        public int UniqueId { get; set; }

        public string NameTask { get; set; }

        public string DescriptionTask { get; set; }

        public Executor? ExecutorTask { get; set; }

        public Status CurrentStatus { get; set; }

        public List<Tag> ListTags { get; set; }

        public TaskElement(string name, string description, Executor? executor, Status status, List<Tag> tags)
        {
            NameTask = name;
            DescriptionTask = description;
            ExecutorTask = executor;
            CurrentStatus = status;
            ListTags = tags;
        }

    }
}
