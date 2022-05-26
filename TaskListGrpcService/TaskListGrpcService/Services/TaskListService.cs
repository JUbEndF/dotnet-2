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

        private readonly ConcurrentDictionary<int, Employee> _users = new();

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

        public override async Task Work(IAsyncStreamReader<Request> requestStream,
            IServerStreamWriter<Replies> responseStream,
            ServerCallContext context)
        {
            if (!await requestStream.MoveNext())
                return;

            if (requestStream.Current.RequestCase != Request.RequestOneofCase.Login && requestStream.Current.RequestCase != Request.RequestOneofCase.CreateUser)
                return;



            var user = requestStream.Current.RequestCase == Request.RequestOneofCase.Login ?
                Login(requestStream.Current.Login, responseStream) :
                Registration(requestStream.Current.CreateUser, responseStream);

            try
            {
                var loginReply = new ExaminationReply { Success = user is not null };
                await responseStream.WriteAsync(new Replies { ExaminationReply = loginReply });
                if (user is null)
                    return;


                while (await requestStream.MoveNext())
                {
                    switch (requestStream.Current.RequestCase)
                    {
                        case Request.RequestOneofCase.None:
                            throw new ApplicationException();
                        case Request.RequestOneofCase.ListDataRequest:
                            await ListDataReplies(requestStream.Current.ListDataRequest, responseStream);
                            break;
                        case Request.RequestOneofCase.TaskRequest:
                            await RequestTask(requestStream.Current.TaskRequest, responseStream);
                            break;
                        case Request.RequestOneofCase.TaskDataRequest:
                            await DataTaskReplies(requestStream.Current.TaskDataRequest, responseStream);
                            break;
                        case Request.RequestOneofCase.TaskIdDelete:
                            DeleteTask(requestStream.Current.TaskIdDelete, responseStream);
                            break;
                        case Request.RequestOneofCase.TagIdDelete:
                            DeleteTag(requestStream.Current.TagIdDelete, responseStream);
                            break;
                        case Request.RequestOneofCase.EmployeeIdDelete:
                            DeleteEmployee(requestStream.Current.EmployeeIdDelete, responseStream);
                            break;
                        case Request.RequestOneofCase.TegRequest:
                            await RequestTag(requestStream.Current.TegRequest, responseStream);
                            break;
                        case Request.RequestOneofCase.ReadExistingTags:
                            await ListTagReplies(requestStream.Current.ReadExistingTags, responseStream);
                            break;
                        default: throw new ApplicationException();
                    }
                }
            }
            finally
            {
                if (user is not null)
                    _users.TryRemove(user.Id, out _);
            }
        }

        Employee? Login(LoginRequest loginRequest, IServerStreamWriter<Replies> responseStream)
        {
            var employees = _employeeRepository.GetAll();
            var user = employees.Find(obj => obj.Login == loginRequest.Login);
            if (user is null)
                return null;
            _users.TryAdd(user!.Id, user);
            if (user!.LoginCheck(loginRequest.Password))
                return user;
            return null;
        }

        Employee? Registration(NewUsers newUserRequest, IServerStreamWriter<Replies> responseStream)
        {
            var employee = new Employee(newUserRequest.Name, newUserRequest.Surname,
                newUserRequest.Password, newUserRequest.Login);
            _employeeRepository.Insert(employee);
            var employees = _employeeRepository.GetAll();
            var user = employees.Find(obj => obj.Login == employee.Login);
            return user;
        }

        async void DeleteTask(int taskIdDelete, IServerStreamWriter<Replies> responseStream)
        {
            try
            {
                _jsonTaskRepository.RemoveAt(taskIdDelete);
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
            finally
            {
                var message = new ExaminationReply { Success = true };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
        }

        async void DeleteEmployee(int employeeIdDelete, IServerStreamWriter<Replies> responseStream)
        {
            try
            {
                _employeeRepository.RemoveAt(employeeIdDelete);
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
            finally
            {
                var message = new ExaminationReply { Success = true };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
        }

        async Task ListDataReplies(ListTaskRequest listTaskRequest, IServerStreamWriter<Replies> responseStream)
        {
            var listRepliesTask = new ListTaskReply();
            try
            {
                foreach (var item in _jsonTaskRepository.GetAll())
                {
                    listRepliesTask.List.Add(new ListTaskReply.Types.ListTask
                    {
                        Id = item.UniqueId,
                        Executor = item.ExecutorTask.Surname,
                        NameTask = item.NameTask
                    });
                }
            }
            finally
            {
                await responseStream.WriteAsync(new Replies { List = listRepliesTask });
            }
        }

        async Task RequestTask(TaskProto requestTask, IServerStreamWriter<Replies> responseStream)
        {
            try
            {
                var executor = _employeeRepository.GetById(requestTask.Executor.Id);
                var task = new TaskElement(
                    requestTask.NameTask,
                    requestTask.TaskDescription,
                    executor,
                    requestTask.CurrentState,
                    TaskElement.ProtoToTags(requestTask.Tags)
                )
                {
                    UniqueId = requestTask.Id
                };
                if (_jsonTaskRepository.GetAll().FindIndex(obj => obj.UniqueId == task.UniqueId) != -1)
                    _jsonTaskRepository.Update(task);
                else
                    _jsonTaskRepository.Insert(task);
            }
            finally
            {
                var examinationReply = new ExaminationReply { Success = true };
                await responseStream.WriteAsync(new Replies { ExaminationReply = examinationReply });
            }
        }

        async Task DataTaskReplies(TaskListGrpcServer.Protos.TaskDataRequest taskDataRequest, IServerStreamWriter<Replies> responseStream)
        {
            var searchTask = _jsonTaskRepository.GetById(taskDataRequest.RequestTaskId);
            var executorProto = searchTask.ExecutorTask != null ? searchTask.ExecutorTask.ToProtoType() :
                new Employee().ToProtoType();
            var repliesTask = new TaskProto
            {
                Id = searchTask.UniqueId,
                CurrentState = searchTask.CurrentStatus,
                Executor = executorProto,
                NameTask = searchTask.NameTask,
                Tags = searchTask.TagsToProto(),
                TaskDescription = searchTask.DescriptionTask,
            };
            await responseStream.WriteAsync(new Replies { ReplyTask = repliesTask });
        }

        async Task RequestTag(TagProto requestTag, IServerStreamWriter<Replies> responseStream)
        {
            try
            {
                var tag = new Tag(
                    requestTag.Name,
                    requestTag.Color
                )
                {
                    TagId = requestTag.Id
                };
                if (_jsonTagRepository.GetAll().FindIndex(obj => obj.TagId == tag.TagId) != -1)
                    _jsonTagRepository.Update(tag);
                else
                    _jsonTagRepository.Insert(tag);
            }
            finally
            {
                var examinationReply = new ExaminationReply { Success = true };
                await responseStream.WriteAsync(new Replies { ExaminationReply = examinationReply });
            }
        }

        async Task ListTagReplies(RequestAllTags requestAllTags, IServerStreamWriter<Replies> responseStream)
        {
            var listTagsReplies = new TagsProto();
            try
            {
                foreach (var item in _jsonTagRepository.GetAll())
                {
                    listTagsReplies.ListTag.Add(new TagProto
                    {
                        Color = item.Color,
                        Name = item.TagName,
                        Id = item.TagId
                    });
                }
            }
            finally
            {
                await responseStream.WriteAsync(new Replies { ReplyListTags = listTagsReplies });
            }
        }

        async void DeleteTag(int tagIdDelete, IServerStreamWriter<Replies> responseStream)
        {
            try
            {
                _jsonTagRepository.RemoveAt(tagIdDelete);
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
            finally
            {
                var message = new ExaminationReply { Success = true };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
        }
    }
}
