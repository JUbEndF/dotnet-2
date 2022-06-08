using TaskListGrpcServer.Protos;

namespace TaskListGrpcServer.Models
{
    /// <summary>
    /// Class describing performers and storing their data
    /// </summary>
    [System.Serializable]
    public class Employee
    {
        /// <summary>
        /// id employee
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name employee
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Surname employee
        /// </summary>
        public string Surname { get; set; }

        /// <summary>
        /// Constructor with user data
        /// </summary>
        public Employee(string name, string surname)
        {
            Name = name;
            Surname = surname;
        }

        public Employee()
        {
            Id = -1;
            Name = string.Empty;
            Surname = string.Empty;
        }

        public EmployeeProto ToProtoType()
        {
            return new EmployeeProto { Name = Name, Surname = Surname, Id = Id };
        }
    }
}
