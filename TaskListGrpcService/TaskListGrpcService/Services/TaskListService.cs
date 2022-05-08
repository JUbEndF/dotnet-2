using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskListGrpcServer.Protos;

namespace TaskListGrpcService
{
    public class TaskListService : TaskList.TaskListBase
    {
        private readonly ILogger<TaskListService> _logger;
        public TaskListService(ILogger<TaskListService> logger)
        {
            _logger = logger;
        }

        public override Task<Replies> Work(Request request, ServerCallContext context)
        {
            return Task.FromResult(new Replies (){}) ;
        }
    }
}
