using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TaskListGrpcServer.Models;

namespace TaskListGrpcServer.Repositories
{
    public class JSONTagRepository : IRepository<Tag>
    {
        private readonly string _fileName = "tags.json";

        private List<Tag> _tags = new();

        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public async Task<List<Tag>> GetAllAsync()
        {
            await DeserializeAsync();
            return _tags;
        }

        public async Task<Tag> GetByIdAsync(int id)
        {
            await DeserializeAsync();
            return _tags.FirstOrDefault(obj => obj.TagId == id)!;
        }

        public async void Insert(Tag obj)
        {
            await DeserializeAsync();

            if (_tags.Count == 0)
            {
                obj.TagId = 1;
                _tags.Add(obj);
            }
            else
            {
                for (int i = 1; i <= _tags.Count + 1; i++)
                {
                    if (_tags.FindIndex(ibj => obj.TagId == i) == -1)
                    {
                        obj.TagId = i;
                        _tags.Add(obj);
                        break;
                    }
                }
            }

            await SerializeAsync();
        }

        public async void RemoveAllAsync()
        {
            await DeserializeAsync();
            _tags.Clear();
            await SerializeAsync();
        }

        public async void RemoveAtAsync(int id)
        {
            await DeserializeAsync();
            _tags.Remove(_tags.Find(obj => obj.TagId == id)!);
            await SerializeAsync();
        }

        public async Task<bool> UpdateAsync(Tag executorUpdate)
        {
            await DeserializeAsync();
            var index = _tags.FindIndex(obj => obj.TagId == executorUpdate.TagId);
            if (index != -1)
                _tags[index] = executorUpdate;
            else return false;
            await SerializeAsync();
            return true;
        }

        private async Task DeserializeAsync()
        {
            if (_tags != null)
                return;
            await _semaphoreSlim.WaitAsync();
            if (!File.Exists(_fileName))
            {
                _tags = new List<Tag>();
            }
            try
            {
                await using FileStream streamMessage = File.Create(_fileName);
                await JsonSerializer.SerializeAsync<List<Tag>>(streamMessage, _tags!, new JsonSerializerOptions { WriteIndented = true });
                _semaphoreSlim.Release();
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _tags = new List<Tag>();
                _semaphoreSlim.Release();
            }
        }

        private async Task SerializeAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await using FileStream streamMessage = File.Create(_fileName);
                await JsonSerializer.SerializeAsync<List<Tag>>(streamMessage, _tags, new JsonSerializerOptions { WriteIndented = true });
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
