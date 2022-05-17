using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using TaskListGrpcServer.Models;

namespace TaskListGrpcServer.Repositories
{
    public class JSONEmployeeRepository : IRepository<Employee>
    {
        private readonly string _fileName = "employee.json";

        private List<Employee> _employee = new();

        public List<Employee> GetAll()
        {
            Deserialize();
            return _employee;
        }

        public Employee GetById(int id)
        {
            Deserialize();
            return _employee.FirstOrDefault(obj => obj.Id == id)!;
        }

        public void Insert(Employee obj)
        {
            Deserialize();

            if (_employee.Count == 0)
            {
                obj.Id = 1;
                _employee.Add(obj);
            }
            else
            {
                for (int i = 1; i <= _employee.Count + 1; i++)
                {
                    if (_employee.FindIndex(ibj => obj.Id == i) == -1)
                    {
                        obj.Id = i;
                        _employee.Add(obj);
                        break;
                    }
                }
            }

            Serialize();
        }

        public void RemoveAll()
        {
            Deserialize();
            _employee.Clear();
            Serialize();
        }

        public void RemoveAt(int id)
        {
            Deserialize();
            _employee.Remove(_employee.Find(obj => obj.Id == id)!);
            Serialize();
        }

        public void Update(Employee executorUpdate)
        {
            Deserialize();
            var index = _employee.FindIndex(obj => obj.Id == executorUpdate.Id);
            if (index != -1)
                _employee[index] = executorUpdate;
            Serialize();
        }

        private void Deserialize()
        {
            if (!File.Exists(_fileName))
            {
                _employee = new List<Employee>();
            }
            try
            {
                using FileStream? fileStream = File.OpenRead(_fileName);
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<Employee>));
                    _employee = (List<Employee>)serializer.ReadObject(fileStream)!;
                }
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _employee = new List<Employee>();
            }
        }

        private void Serialize()
        {
            using FileStream? fileStream = new(_fileName, FileMode.Create);
            DataContractJsonSerializer formatter = new(typeof(List<Employee>));
            formatter.WriteObject(fileStream, _employee);
        }
    }
}
