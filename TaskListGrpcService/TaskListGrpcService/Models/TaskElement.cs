

namespace TaskListGrpcService.Models
{
    public enum Status
    {
        NEW,
        ASSIGNED,
        DISCUSSION,
        COMPLETED,
        CLOSED,
    }

    public class TaskElement
    {
        public int UniqueId { get; set; }

        public string NameTask { get; set; }

        public string DescriptionTask { get; set; }

        public Executor? ExecutorTask { get; set; }

        public Status CurrentStatus { get; set; }

        public TaskElement(int id, string name, string description, Executor? executor, Status status)
        {
            UniqueId = id;
            NameTask = name;
            DescriptionTask = description;
            ExecutorTask = executor;
            CurrentStatus = status;
        }

    }
}
