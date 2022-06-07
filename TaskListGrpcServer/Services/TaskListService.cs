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
        private readonly ConcurrentDictionary<int, Employee> _users = new();

        private readonly JSONEmployeeRepository _jsonEmployeeRepository;

        private readonly JSONTagRepository _jsonTagRepository;

        private readonly JSONTaskRepository _jsonTaskRepository;

        public TaskListService(ILogger<TaskListService> logger)
        {
            _jsonEmployeeRepository = new JSONEmployeeRepository();
            _jsonTagRepository = new JSONTagRepository();
            _jsonTaskRepository = new JSONTaskRepository();
        }

        public override async Task Work(IAsyncStreamReader<Request> requestStream,
            IServerStreamWriter<Replies> responseStream,
            ServerCallContext context)
        {
            try
            {
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
                        case Request.RequestOneofCase.ReadExistingUsers:
                            await ListEmployeeReplies(requestStream.Current.ReadExistingUsers, responseStream);
                            break;
                        case Request.RequestOneofCase.UserRequest:
                            await RequestEmployee(requestStream.Current.UserRequest, responseStream);
                            break;
                        default: throw new ApplicationException();
                    }
                }
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
        }

        async Task RequestEmployee(EmployeeProto requestEmployee, IServerStreamWriter<Replies> responseStream)
        {
            try
            {
                var emloyee = new Employee(
                    requestEmployee.Name,
                    requestEmployee.Surname
                )
                {
                    Id = requestEmployee.Id
                };
                if (_jsonEmployeeRepository.GetAllAsync().Result.FindIndex(obj => obj.Id == emloyee.Id) != -1)
                    _ = _jsonEmployeeRepository.UpdateAsync(emloyee);
                else
                    _jsonEmployeeRepository.Insert(emloyee);
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
            finally
            {
                var examinationReply = new ExaminationReply { Success = true };
                await responseStream.WriteAsync(new Replies { ExaminationReply = examinationReply });
            }
        }

        async void DeleteTask(int taskIdDelete, IServerStreamWriter<Replies> responseStream)
        {
            try
            {
                _jsonTaskRepository.RemoveAtAsync(taskIdDelete);
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
                _jsonEmployeeRepository.RemoveAtAsync(employeeIdDelete);
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
                foreach (var item in _jsonTaskRepository.GetAllAsync().Result)
                {
                    listRepliesTask.List.Add(new ListTask
                    {
                        Id = item.UniqueId,
                        Executor = item.ExecutorTask.Surname,
                        NameTask = item.NameTask
                    });
                }
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
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
                var executor = _jsonEmployeeRepository.GetByIdAsync(requestTask.Executor.Id).Result;
                executor ??= new Employee(); 
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
                if (_jsonTaskRepository.GetAllAsync().Result.FindIndex(obj => obj.UniqueId == task.UniqueId) != -1)
                    _ = _jsonTaskRepository.UpdateAsync(task);
                else
                    _jsonTaskRepository.Insert(task);
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
            finally
            {
                var examinationReply = new ExaminationReply { Success = true };
                await responseStream.WriteAsync(new Replies { ExaminationReply = examinationReply });
            }
        }

        async Task DataTaskReplies(TaskListGrpcServer.Protos.TaskDataRequest taskDataRequest, IServerStreamWriter<Replies> responseStream)
        {
            var searchTask = _jsonTaskRepository.GetByIdAsync(taskDataRequest.RequestTaskId).Result;
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
                if (_jsonTagRepository.GetAllAsync().Result.FindIndex(obj => obj.TagId == tag.TagId) != -1)
                    _ = _jsonTagRepository.UpdateAsync(tag);
                else
                    _jsonTagRepository.Insert(tag);
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
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
                foreach (var item in _jsonTagRepository.GetAllAsync().Result)
                {
                    listTagsReplies.ListTag.Add(new TagProto
                    {
                        Color = item.Color,
                        Name = item.TagName,
                        Id = item.TagId
                    });
                }
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
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
                _jsonTagRepository.RemoveAtAsync(tagIdDelete);
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

        async Task ListEmployeeReplies(RequestAllEmployee requestAllTags, IServerStreamWriter<Replies> responseStream)
        {
            var listEmployeeReplies = new ListEmployeeReply();
            try
            {
                foreach (var item in _jsonEmployeeRepository.GetAllAsync().Result)
                {
                    listEmployeeReplies.ReplyListEmployee.Add(new ReplyEmployee
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Surname = item.Surname
                    });
                }
            }
            catch (Exception)
            {
                var message = new ExaminationReply { Success = false };
                await responseStream.WriteAsync(new Replies { ExaminationReply = message });
            }
            finally
            {
                await responseStream.WriteAsync(new Replies { ReplyListEmployee = listEmployeeReplies });
            }
        }
    }
}
