using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using TaskListGrpcServer.Models;

namespace TaskListGrpcService.Repositories
{
    public class JSONExecutorRepository
    {
        private readonly string _fileName = "executors.json";

        private List<Executor>? _executors;

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public List<Executor> GetAll()
        {
            Deserialize();
            return _executors!;
        }

        public Executor GetById(int id)
        {
            Deserialize();
            return _executors!.FirstOrDefault(obj => obj.Id == id)!;
        }

        public void Insert(Executor obj)
        {
            Deserialize();

            if (_executors!.Count == 0)
            {
                obj.Id = 1;
                _executors.Add(obj);
            }
            else
            {
                for (int i = 1; i <= _executors.Count + 1; i++)
                {
                    if (_executors.FindIndex(ibj => obj.Id == i) == -1)
                    {
                        obj.Id = i;
                        _executors.Add(obj);
                        break;
                    }
                }
            }

            Serialize();
        }

        public void RemoveAll()
        {
            Deserialize();
            _executors?.Clear();
            Serialize();
        }

        public void RemoveAt(int id)
        {
            Deserialize();
            _executors!.Remove(_executors.Find(obj => obj.Id == id)!);
            Serialize();
        }

        public void Update(Executor executorUpdate)
        {
            Deserialize();
            var index = _executors!.FindIndex(obj => obj.Id == executorUpdate.Id);
            if (index != -1)
                _executors[index] = executorUpdate;
            Serialize();
        }

        private void Deserialize()
        {
            if (_executors != null)
                return;
            if (!File.Exists(_fileName))
            {
                _executors = new List<Executor>();
            }
            try
            {
                using FileStream? fileStream = File.OpenRead(_fileName);
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<Executor>));
                    _executors = (List<Executor>)serializer.ReadObject(fileStream)!;
                }
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _executors = new List<Executor>();
            }
        }

        private void Serialize()
        {
            using FileStream? fileStream = new(_fileName, FileMode.Create);
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(List<Executor>));
            formatter.WriteObject(fileStream, _executors);
        }
    }
}
