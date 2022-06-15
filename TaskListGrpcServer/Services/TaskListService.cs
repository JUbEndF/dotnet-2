using Grpc.Core;
using System.Threading.Tasks;
using TaskListGrpcServer.Models;
using TaskListGrpcServer.Protos;
using TaskListGrpcServer.Repositories;

namespace TaskListGrpcServer.Services
{
    public class TaskListService : TaskList.TaskListBase
    {
        private readonly JSONEmployeeRepository _jsonEmployeeRepository;

        private readonly JSONTagRepository _jsonTagRepository;

        private readonly JSONTaskRepository _jsonTaskRepository;

        public TaskListService()
        {
            _jsonEmployeeRepository = new JSONEmployeeRepository();
            _jsonTagRepository = new JSONTagRepository();
            _jsonTaskRepository = new JSONTaskRepository();
        }

        public override Task<EmployeeProtoReply> AddEmployee(EmployeeProto request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(new EmployeeProtoReply
                {
                    Employee = _jsonEmployeeRepository.Insert(new(request))!.Result.ToProtoType(),
                    Error = new ExaminationReply { Success = true }
                });
            }
            catch
            {
                return Task.FromResult(new EmployeeProtoReply
                {
                    Error = new ExaminationReply
                    {
                        Success = false
                    }
                });
            }
        }

        public override Task<TagProtoReply> AddTag(TagProto request, ServerCallContext context)
        {
            try
            {
                return Task.FromResult(new TagProtoReply
                {
                    Tag = _jsonTagRepository.Insert(new(request))!.Result.TagToProto(),
                    Error = new ExaminationReply { Success = true }
                });
            }
            catch
            {
                return Task.FromResult(new TagProtoReply
                {
                    Error = new ExaminationReply
                    {
                        Success = false
                    }
                });
            }
        }

        public override Task<TaskProtoReply> AddTask(TaskProto request, ServerCallContext context)
        {
            try
            {

                return Task.FromResult(new TaskProtoReply
                {
                    Task = _jsonTaskRepository.Insert(new(request))!.Result.ToProto(),
                    Error = new ExaminationReply { Success = true }
                });
            }
            catch
            {
                return Task.FromResult(new TaskProtoReply
                {
                    Error = new ExaminationReply
                    {
                        Success = false
                    }
                });
            }
        }

        public override Task<ExaminationReply> DeleteEmployee(EmployeeProto request, ServerCallContext context)
        {
            try
            {
                _jsonEmployeeRepository.RemoveAtAsync(request.Id);
            }
            catch
            {
                return Task.FromResult(new ExaminationReply
                {
                    Success = false
                }
                );
            }
            return Task.FromResult(new ExaminationReply
            {
                Success = true
            }
            );
        }

        public override Task<ExaminationReply> DeleteTag(TagProto request, ServerCallContext context)
        {
            try
            {
                _jsonTagRepository.RemoveAtAsync(request.Id);
            }
            catch
            {
                return Task.FromResult(new ExaminationReply
                {
                    Success = false
                }
                );
            }
            return Task.FromResult(new ExaminationReply
            {
                Success = true
            }
            );
        }

        public override Task<ExaminationReply> DeleteTask(TaskProto request, ServerCallContext context)
        {
            try
            {
                _jsonTaskRepository.RemoveAtAsync(request.Id);
            }
            catch
            {
                return Task.FromResult(new ExaminationReply
                {
                    Success = false
                }
                );
            }
            return Task.FromResult(new ExaminationReply
            {
                Success = true
            }
            );
        }

        public override Task<ListEmployeeReply> GetAllEmployee(NullRequest request, ServerCallContext context)
        {
            var employeeListReply = new ListEmployeeReply();
            foreach (var employee in _jsonEmployeeRepository.GetAllAsync().Result)
            {
                employeeListReply.ReplyListEmployee.Add(employee.ToProtoType());
            }
            return Task.FromResult(employeeListReply);
        }

        public override Task<TagsProto> GetAllTag(NullRequest request, ServerCallContext context)
        {
            var tagListReply = new TagsProto();
            foreach (var tag in _jsonTagRepository.GetAllAsync().Result)
            {
                tagListReply.ListTag.Add(tag.TagToProto());
            }
            return Task.FromResult(tagListReply);
        }

        public override Task<ListTaskReply> GetAllTask(NullRequest request, ServerCallContext context)
        {
            var tagListReply = new ListTaskReply();
            foreach (var task in _jsonTaskRepository.GetAllAsync().Result)
            {
                tagListReply.List.Add(task.ToProto());
            }
            return Task.FromResult(tagListReply);
        }

        public override async Task<ExaminationReply> UpdateEmployee(EmployeeProto request, ServerCallContext context)
        {
            try
            {
                await _jsonEmployeeRepository.UpdateAsync(new(request));
            }
            catch
            {
                return new ExaminationReply
                {
                    Success = false
                };
            }
            return new ExaminationReply
            {
                Success = true
            };
        }

        public override async Task<ExaminationReply> UpdateTag(TagProto request, ServerCallContext context)
        {
            try
            {
                await _jsonTagRepository.UpdateAsync(new(request));
            }
            catch
            {
                return new ExaminationReply
                {
                    Success = false
                };
            }
            return new ExaminationReply
            {
                Success = true
            };
        }

        public override async Task<ExaminationReply> UpdateTask(TaskProto request, ServerCallContext context)
        {
            try
            {
                await _jsonTaskRepository.UpdateAsync(new TaskElement(request));
            }
            catch
            {
                return new ExaminationReply
                {
                    Success = false
                };
            }
            return new ExaminationReply
            {
                Success = true
            };
        }
    }
}
