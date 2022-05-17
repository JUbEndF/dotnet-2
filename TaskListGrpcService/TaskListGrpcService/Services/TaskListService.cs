using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using TaskListGrpcServer.Models;
using TaskListGrpcServer.Protos;
using TaskListGrpcServer.Repositories;

namespace TaskListGrpcServer.Services
{
    public class TaskListService : TaskList.TaskListBase
    {
        private readonly ILogger<TaskListService> _logger;

        private readonly ConcurrentDictionary<int, Employee> _players = new();

        private JSONEmployeeRepository _employeeRepository;

        private JSONTagRepository _jsonTagRepository;

        private JSONTaskRepository _jsonTaskRepository;

        public TaskListService(ILogger<TaskListService> logger)
        {
            _logger = logger;
            _employeeRepository = new JSONEmployeeRepository();
            _jsonTagRepository = new JSONTagRepository();
            _jsonTaskRepository = new JSONTaskRepository();
        }

        Employee? Login(LoginRequest loginRequest, IServerStreamWriter<Replies> responseStream)
        {
            var employees = _employeeRepository.GetAll();
            var user = employees.Find(obj => obj.Login == loginRequest.Login);
            _players.TryAdd(user!.Id, user);
            if (user!.LoginCheck(loginRequest.Password))
                return user;
            return null;
        }

        public override async Task Work(IAsyncStreamReader<Request> requestStream,
            IServerStreamWriter<Replies> responseStream,
            ServerCallContext context)
        {
            if (!await requestStream.MoveNext())
                return;

            if (requestStream.Current.RequestCase != Request.RequestOneofCase.Login)
                return;



            var user = Login(requestStream.Current.Login, responseStream);

            try
            {
                var loginReply = new ExaminationReply { Success = user is not null };
                await responseStream.WriteAsync(new Replies { ExaminationReply = loginReply });
                if (user is null)
                    return;
            }
            finally
            {
                if (user is not null)
                    _players.TryRemove(user.Id, out _);
            }
        }
    }
}
