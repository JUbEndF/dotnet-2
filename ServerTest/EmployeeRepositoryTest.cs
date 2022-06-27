using System;
using TaskListGrpcServer.Repositories;
using TaskListGrpcServer.Models;
using Xunit;
using System.Threading.Tasks;

namespace ServerTest
{
    public class EmployeeRepositoryTest
    {
        [Fact]
        public async Task AddEmployeeTestAsync()
        {
            JSONEmployeeRepository testRepository = new JSONEmployeeRepository();
            Employee create = new Employee() { Name = "Test", Surname = "Repository" };
            Employee getCreate = await testRepository.Insert(create);
            Employee getById = await testRepository.GetByIdAsync(getCreate.Id);
            Assert.True(getCreate.Name == getById.Name && getCreate.Surname == getById.Surname);
            testRepository.RemoveAtAsync(create.Id);
        }

        [Fact]
        public async Task DeleteEmployeeTestAsync()
        {
            JSONEmployeeRepository testRepository = new JSONEmployeeRepository();
            Employee create = new Employee() { Name = "Test", Surname = "Repository" };
            Employee getCreate = await testRepository.Insert(create);
            testRepository.RemoveAtAsync(getCreate.Id);
            Assert.False(await testRepository.CheckEmployeeAsync(getCreate.Id));
        }

        [Fact]
        public async Task ChangeEmployeeTestAsync()
        {
            JSONEmployeeRepository testRepository = new JSONEmployeeRepository();
            Employee create = new Employee() { Name = "Test", Surname = "Repository" };
            Employee getCreate = await testRepository.Insert(create);
            string change = "newstring";
            getCreate.Surname = change;
            Assert.True(await testRepository.UpdateAsync(getCreate));
        }
    }
}
