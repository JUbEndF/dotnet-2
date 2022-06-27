using System;
using TaskListGrpcServer.Repositories;
using TaskListGrpcServer.Models;
using Xunit;
using System.Threading.Tasks;

namespace ServerTest
{
    public class TagRepositoryTest
    {
        [Fact]
        public async Task AddTagTestAsync()
        {
            JSONTagRepository testRepository = new JSONTagRepository();
            Tag create = new Tag() { TagName = "test"};
            Tag getCreate = await testRepository.Insert(create);
            Tag getById = await testRepository.GetByIdAsync(getCreate.Id);
            Assert.True(getCreate.TagName == getById.TagName);
            testRepository.RemoveAtAsync(create.Id);
        }

        [Fact]
        public async Task DeleteTagTestAsync()
        {
            JSONTagRepository testRepository = new JSONTagRepository();
            Tag create = new Tag() { TagName = "test" };
            Tag getCreate = await testRepository.Insert(create);
            testRepository.RemoveAtAsync(getCreate.Id);
            Assert.False(await testRepository.CheckTagAsync(getCreate.Id));
        }

        [Fact]
        public async Task ChangeTagTestAsync()
        {
            JSONTagRepository testRepository = new JSONTagRepository();
            Tag create = new Tag() { TagName = "test" };
            Tag getCreate = await testRepository.Insert(create);
            string change = "newstring";
            getCreate.TagName = change;
            Assert.True(await testRepository.UpdateAsync(getCreate));
        }
    }
}
