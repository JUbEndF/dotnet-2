using ProtoBuf;
using TaskListGrpcServer.Protos;

namespace TaskListGrpcServer.Models
{
    /// <summary>
    /// Class describing performers and storing their data
    /// </summary>
    [System.Serializable]
    [ProtoContract]
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
        /// Password employee
        /// </summary>
        private string _password;

        /// <summary>
        /// LOgin employee
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Constructor with user data
        /// </summary>
        public Employee(string name, string surname, string password, string login)
        {
            Name = name;
            Surname = surname;
            _password = password;
            Login = login;
        }

        public Employee()
        {
            Id = -1;
            Name = string.Empty;
            Surname = string.Empty;
            _password = string.Empty;
            Login = string.Empty;
        }

        public EmployeeProto ToProtoType()
        {
            return new EmployeeProto { Login = Login, Name = Name, Surname = Surname };
        }



        public bool LoginCheck(string password) => _password == password;
    }
}
