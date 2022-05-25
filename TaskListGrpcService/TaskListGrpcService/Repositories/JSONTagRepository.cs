using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading;
using TaskListGrpcServer.Models;

namespace TaskListGrpcServer.Repositories
{
    public class JSONTagRepository : IRepository<Tag>
    {
        private readonly string _fileName = "tags.json";

        private List<Tag> _tags = new();

        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        public List<Tag> GetAll()
        {
            Deserialize();
            return _tags;
        }

        public Tag GetById(int id)
        {
            Deserialize();
            return _tags.FirstOrDefault(obj => obj.TagId == id)!;
        }

        public void Insert(Tag obj)
        {
            Deserialize();

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

            Serialize();
        }

        public void RemoveAll()
        {
            Deserialize();
            _tags.Clear();
            Serialize();
        }

        public void RemoveAt(int id)
        {
            Deserialize();
            _tags.Remove(_tags.Find(obj => obj.TagId == id)!);
            Serialize();
        }

        public void Update(Tag executorUpdate)
        {
            Deserialize();
            var index = _tags.FindIndex(obj => obj.TagId == executorUpdate.TagId);
            if (index != -1)
                _tags[index] = executorUpdate;
            Serialize();
        }

        private void Deserialize()
        {
            if (!File.Exists(_fileName))
            {
                _tags = new List<Tag>();
            }
            try
            {
                using FileStream? fileStream = File.OpenRead(_fileName);
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<Tag>));
                    _tags = (List<Tag>)serializer.ReadObject(fileStream)!;
                }
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _tags = new List<Tag>();
            }
        }

        private void Serialize()
        {
            using FileStream? fileStream = new(_fileName, FileMode.Create);
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(List<Tag>));
            formatter.WriteObject(fileStream, _tags);
        }
    }
}
