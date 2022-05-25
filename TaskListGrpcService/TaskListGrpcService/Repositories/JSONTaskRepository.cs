using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TaskListGrpcServer.Models;

namespace TaskListGrpcServer.Repositories
{
    public class JSONTaskRepository : IRepository<TaskElement>
    {
        private readonly string _fileName = "tasks.json";

        private List<TaskElement> _tasks = new();

        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public List<TaskElement> GetAll()
        {
            Deserialize();
            return _tasks;
        }

        public TaskElement GetById(int id)
        {
            Deserialize();
            return _tasks.FirstOrDefault(obj => obj.UniqueId == id)!;
        }

        public async void Insert(TaskElement obj)
        {
            Deserialize();

            if (_tasks.Count == 0)
            {
                obj.UniqueId = 1;
                _tasks.Add(obj);
            }
            else
            {
                for (int i = 1; i <= _tasks.Count + 1; i++)
                {
                    if (_tasks.FindIndex(ibj => obj.UniqueId == i) == -1)
                    {
                        obj.UniqueId = i;
                        _tasks.Add(obj);
                        break;
                    }
                }
            }

            await SerializeAsync();
        }

        public async void RemoveAll()
        {
            Deserialize();
            _tasks.Clear();
            await SerializeAsync();
        }

        public async void RemoveAt(int id)
        {
            Deserialize();
            _tasks.Remove(_tasks.Find(obj => obj.UniqueId == id)!);
            await SerializeAsync();
        }

        public async void Update(TaskElement executorUpdate)
        {
            Deserialize();
            var index = _tasks.FindIndex(obj => obj.UniqueId == executorUpdate.UniqueId);
            if (index != -1)
                _tasks[index] = executorUpdate;
            await SerializeAsync();
        }

        private async void Deserialize()
        {
            if (_tasks != null)
                return;
            if (!File.Exists(_fileName))
            {
                _tasks = new List<TaskElement>();
            }
            try
            {
                await using FileStream streamMessage = File.Create(_fileName);
                await JsonSerializer.SerializeAsync<List<TaskElement>>(streamMessage, _tasks!, new JsonSerializerOptions { WriteIndented = true });
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _tasks = new List<TaskElement>();
            }
        }

        private async Task SerializeAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                await using FileStream streamMessage = File.Create(_fileName);
                await JsonSerializer.SerializeAsync<List<TaskElement>>(streamMessage, _tasks, new JsonSerializerOptions { WriteIndented = true });
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}
