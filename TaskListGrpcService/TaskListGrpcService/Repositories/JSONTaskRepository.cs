using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using TaskListGrpcServer.Models;

namespace TaskListGrpcServer.Repositories
{
    public class JSONTaskRepository : IRepository<TaskElement>
    {
        private readonly string _fileName = "tasks.json";

        private List<TaskElement> _tasks = new();

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

        public void Insert(TaskElement obj)
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

            Serialize();
        }

        public void RemoveAll()
        {
            Deserialize();
            _tasks.Clear();
            Serialize();
        }

        public void RemoveAt(int id)
        {
            Deserialize();
            _tasks.Remove(_tasks.Find(obj => obj.UniqueId == id)!);
            Serialize();
        }

        public void Update(TaskElement executorUpdate)
        {
            Deserialize();
            var index = _tasks.FindIndex(obj => obj.UniqueId == executorUpdate.UniqueId);
            if (index != -1)
                _tasks[index] = executorUpdate;
            Serialize();
        }

        private void Deserialize()
        {
            if (_tasks != null)
                return;
            if (!File.Exists(_fileName))
            {
                _tasks = new List<TaskElement>();
            }
            try
            {
                using FileStream? fileStream = File.OpenRead(_fileName);
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<TaskElement>));
                    _tasks = (List<TaskElement>)serializer.ReadObject(fileStream)!;
                }
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _tasks = new List<TaskElement>();
            }
        }

        private void Serialize()
        {
            using FileStream? fileStream = new(_fileName, FileMode.Create);
            DataContractJsonSerializer formatter = new(typeof(List<TaskElement>));
            formatter.WriteObject(fileStream, _tasks);
        }
    }
}
