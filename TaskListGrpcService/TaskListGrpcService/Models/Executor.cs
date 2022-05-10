namespace TaskListGrpcService.Models
{
    public class Executor
    {
        public string Name { get; set; }
        public string Surname { get; set; }

        private string _password;
        public string Login { get; set; }

        public Executor(string name, string surname, string password, string login)
        {
            Name = name;
            Surname = surname;
            _password = password;
            Login = login;
        }

        public bool LoginCheck(string password) => _password == password;
    }
}
