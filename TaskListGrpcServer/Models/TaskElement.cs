using System.Collections.Generic;
using TaskListGrpcServer.Protos;

namespace TaskListGrpcServer.Models
{
    /// <summary>
    /// Class describing the task
    /// </summary>
    [System.Serializable]
    public class TaskElement
    {
        /// <summary>
        /// Unique task id 
        /// </summary>
        public int Id { get; set; }

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
            Id = -1;
            NameTask = name;
            DescriptionTask = description;
            ExecutorTask = executor;
            CurrentStatus = status;
            ListTags = tags;
        }

        public TaskElement(TaskProto taskProto)
        {
            Id = taskProto.Id;
            NameTask = taskProto.NameTask;
            DescriptionTask = taskProto.TaskDescription;
            ListTags = TaskElement.ProtoToTags(taskProto.Tags);
            CurrentStatus = taskProto.CurrentState;
            ExecutorTask = new(taskProto.Executor);
        }

        public TaskElement()
        {
            Id = -1;
            NameTask = string.Empty;
            DescriptionTask = string.Empty;
            CurrentStatus = Status.New;
            ExecutorTask = new Employee();
            ListTags = new List<Tag>();
        }

        public TaskProto ToProto()
        {
            return new TaskProto()
            {
                NameTask = NameTask,
                TaskDescription = DescriptionTask,
                CurrentState = CurrentStatus,
                Executor = ExecutorTask.ToProtoType(),
                Tags = this.TagsToProto(),
                Id = Id
            };
        }

        public TaskElement ChangeEmployee(Employee employee)
        {
            if(ExecutorTask.Id != 0 && employee.Id == ExecutorTask.Id)
            {
                ExecutorTask.Name = employee.Name;
                ExecutorTask.Surname = employee.Surname;
            }
            return this;
        }

        public TaskElement ChangeTag(Tag tag)
        {
            var index = ListTags.FindIndex(obj => obj.Id == tag.Id);
            if (index != -1)
                ListTags[index].TagName = tag.TagName;
            return this;
        }

        public TaskElement DeleteTag(Tag tag)
        {
            var index = ListTags.FindIndex(obj => obj.Id == tag.Id);
            if(index != -1)
                ListTags.RemoveAt(index);
            return this;
        }

        public TaskElement DeleteEmployee(Employee employee)
        {
            if (employee.Id == ExecutorTask.Id)
                ExecutorTask = new();
            return this;
        }

        public TagsProto TagsToProto()
        {
            var tags = new TagsProto();
            foreach (var tag in ListTags)
                tags.ListTag.Add(new TagProto { Id = tag.Id, Name = tag.TagName });
            return tags;
        }

        public static List<Tag> ProtoToTags(TagsProto tagsProto)
        {
            var tags = new List<Tag>();
            foreach (var tag in tagsProto.ListTag)
                tags.Add(new Tag(tag.Id, tag.Name));
            return tags;
        }
    }
}
