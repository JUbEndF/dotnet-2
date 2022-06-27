using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
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
            return _tags!;
        }

        public async Task<Tag> GetByIdAsync(int id)
        {
            await DeserializeAsync();
            return _tags!.FirstOrDefault(obj => obj.Id == id)!;
        }

        public async Task<Tag> Insert(Tag obj)
        {
            await DeserializeAsync();

            if (_tags!.Count == 0)
            {
                obj.Id = 1;
                _tags.Add(obj);
            }
            else
            {
                for (int i = 1; i <= _tags.Count + 1; i++)
                {
                    if (_tags.FindIndex(ptr => ptr.Id == i) == -1)
                    {
                        obj.Id = i;
                        _tags.Add(obj);
                        break;
                    }
                }
            }

            await SerializeAsync();
            return _tags.Find(ptr => ptr.Id == obj.Id)!;
        }

        public async void RemoveAllAsync()
        {
            await DeserializeAsync();
            _tags!.Clear();
            await SerializeAsync();
        }

        public async void RemoveAtAsync(int id)
        {
            await DeserializeAsync();
            _tags!.Remove(_tags.Find(obj => obj.Id == id)!);
            await SerializeAsync();
        }

        public async Task<bool> UpdateAsync(Tag executorUpdate)
        {
            await DeserializeAsync();
            var index = _tags!.FindIndex(obj => obj.Id == executorUpdate.Id);
            if (index != -1)
                _tags[index] = executorUpdate;
            else return false;
            await SerializeAsync();
            return true;
        }

        private async Task DeserializeAsync()
        {
            await _semaphoreSlim.WaitAsync();
            if (!File.Exists(_fileName))
            {
                _tags = new List<Tag>();
                _semaphoreSlim.Release();
                return;
            }
            try
            {
                using FileStream? fileStream = File.OpenRead(_fileName);
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<Tag>));
                    _tags = (List<Tag>)serializer.ReadObject(fileStream)!;
                }
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
                using FileStream? fileStream = new(_fileName, FileMode.Create);
                DataContractJsonSerializer formatter = new(typeof(List<Tag>));
                formatter.WriteObject(fileStream, _tags);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> CheckTagAsync(int id)
        {
            await DeserializeAsync();
            if (_tags!.FindIndex(f => f.Id == id) != -1)
            {
                return true;
            }
            return false;
        }
    }
}
