using System.Collections.Generic;

namespace TaskListGrpcServer.Models
{
    /// <summary>
    /// Job execution status enumerations
    /// </summary>
    public enum Status
    {
        NEW,
        ASSIGNED,
        DISCUSSION,
        COMPLETED,
        CLOSED,
    }
    /// <summary>
    /// Class describing the task
    /// </summary>
    [System.Serializable]
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
        public Employee? ExecutorTask { get; set; }

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
        public TaskElement(string name, string description, Employee? executor, Status status, List<Tag> tags)
        {
            NameTask = name;
            DescriptionTask = description;
            ExecutorTask = executor;
            CurrentStatus = status;
            ListTags = tags;
        }
    }
}
