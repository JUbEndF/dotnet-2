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
    public class JSONTaskRepository : IRepository<TaskElement>
    {
        private readonly string _fileName = "tasks.json";

        private List<TaskElement>? _tasks;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public async Task<List<TaskElement>> GetAllAsync()
        {
            await Deserialize();
            return _tasks!;
        }

        public async Task<TaskElement> GetByIdAsync(int id)
        {
            await Deserialize();
            return _tasks!.FirstOrDefault(obj => obj.Id == id)!;
        }

        public async Task<TaskElement> Insert(TaskElement obj)
        {
            await Deserialize();

            if (_tasks!.Count == 0)
            {
                obj.Id = 1;
                _tasks.Add(obj);
            }
            else
            {
                for (int i = 1; i <= _tasks.Count + 1; i++)
                {
                    if (_tasks.FindIndex(ptr => ptr.Id == i) == -1)
                    {
                        obj.Id = i;
                        _tasks.Add(obj);
                        break;
                    }
                }
            }

            await SerializeAsync();
            return _tasks.Find(ptr => ptr.Id == obj.Id)!;
        }

        public async void RemoveAllAsync()
        {
            await Deserialize();
            _tasks!.Clear();
            await SerializeAsync();
        }

        public async void RemoveAtAsync(int id)
        {
            await Deserialize();
            _tasks!.Remove(_tasks!.Find(obj => obj.Id == id)!);
            await SerializeAsync();
        }

        public async Task<bool> UpdateAsync(TaskElement executorUpdate)
        {
            await Deserialize();
            var index = _tasks!.FindIndex(obj => obj.Id == executorUpdate.Id);
            if (index != -1)
                _tasks[index] = executorUpdate;
            else return false;
            await SerializeAsync();
            return true;
        }

        public async void RemoveEmployee(Employee employee)
        {
            await Deserialize();
            foreach (var item in _tasks!)
            {
                await UpdateAsync(item.DeleteEmployee(employee));
            }
            await SerializeAsync();
        }

        public async void ChangeEmployee(Employee employee)
        {
            await Deserialize();
            foreach (var item in _tasks!)
            {
                await UpdateAsync(item.ChangeEmployee(employee));
            }
            await SerializeAsync();
        }

        public async void ChangeTag(Tag tag)
        {
            await Deserialize();
            foreach (var item in _tasks!)
            {
                await UpdateAsync(item.ChangeTag(tag));
            }
            await SerializeAsync();
        }

        public async void RemoveTag(Tag tag)
        {
            await Deserialize();
            foreach(var item in _tasks!)
            {
                await UpdateAsync(item.DeleteTag(tag));
            }
            await SerializeAsync();
        }

        private async Task Deserialize()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                if (!File.Exists(_fileName))
                {
                    _tasks = new List<TaskElement>();
                    _semaphoreSlim.Release();
                    return;
                }
                using FileStream? fileStream = File.OpenRead(_fileName);
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<TaskElement>));
                    _tasks = (List<TaskElement>)serializer.ReadObject(fileStream)!;
                }
                _semaphoreSlim.Release();
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _tasks = new List<TaskElement>();
                _semaphoreSlim.Release();
            }
        }

        private async Task SerializeAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                using FileStream? fileStream = new(_fileName, FileMode.Create);
                DataContractJsonSerializer formatter = new(typeof(List<TaskElement>), new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true });
                formatter.WriteObject(fileStream, _tasks);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> CheckTaskAsync(int id)
        {
            await Deserialize();
            if (_tasks!.FindIndex(f => f.Id == id) != -1)
            {
                return true;
            }
            return false;
        }
    }
}
