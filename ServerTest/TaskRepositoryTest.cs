using System;
using TaskListGrpcServer.Repositories;
using TaskListGrpcServer.Models;
using Xunit;
using System.Threading.Tasks;

namespace ServerTest
{
    public class TaskRepositoryTest
    {
        [Fact]
        public async Task AddTaskTestAsync()
        {
            JSONTaskRepository testRepository = new JSONTaskRepository();
            TaskElement create = new TaskElement() { NameTask = "test" , DescriptionTask = "test"};
            TaskElement getCreate = await testRepository.Insert(create);
            TaskElement getById = await testRepository.GetByIdAsync(getCreate.Id);
            Assert.True(getCreate.NameTask == getById.NameTask && getCreate.DescriptionTask == getById.DescriptionTask);
            testRepository.RemoveAtAsync(create.Id);
        }

        [Fact]
        public async Task DeleteTaskTestAsync()
        {
            JSONTaskRepository testRepository = new JSONTaskRepository();
            TaskElement create = new TaskElement() { NameTask = "test", DescriptionTask = "test" };
            TaskElement getCreate = await testRepository.Insert(create);
            testRepository.RemoveAtAsync(getCreate.Id);
            Assert.False(await testRepository.CheckTaskAsync(getCreate.Id));
        }

        [Fact]
        public async Task ChangeTaskTestAsync()
        {
            JSONTaskRepository testRepository = new JSONTaskRepository();
            TaskElement create = new TaskElement() { NameTask = "test", DescriptionTask = "test" };
            TaskElement getCreate = await testRepository.Insert(create);
            string change = "newstring";
            getCreate.NameTask = change;
            Assert.True(await testRepository.UpdateAsync(getCreate));
        }
    }
}
