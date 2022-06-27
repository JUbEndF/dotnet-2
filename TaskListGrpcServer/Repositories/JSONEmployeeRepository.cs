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
    public class JSONEmployeeRepository : IRepository<Employee>
    {
        private readonly string _fileName = "employee.json";

        private List<Employee>? _employee;

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

        public async Task<List<Employee>> GetAllAsync()
        {
            await DeserializeAsync();
            return _employee!;
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            await DeserializeAsync();
            return _employee!.FirstOrDefault(obj => obj.Id == id)!;
        }

        public async Task<Employee> Insert(Employee obj)
        {
            await DeserializeAsync();

            if (_employee!.Count == 0)
            {
                obj.Id = 1;
                _employee.Add(obj);
            }
            else
            {
                for (int i = 1; i <= _employee.Count + 1; i++)
                {
                    if (_employee.FindIndex(ptr => ptr.Id == i) == -1)
                    {
                        obj.Id = i;
                        _employee.Add(obj);
                        break;
                    }
                }
            }
            await SerializeAsync();
            return _employee.Find(ptr => ptr.Id == obj.Id)!;
        }

        public async void RemoveAllAsync()
        {
            await DeserializeAsync();
            _employee!.Clear();
            await SerializeAsync();
        }

        public async void RemoveAtAsync(int id)
        {
            await DeserializeAsync();
            _employee!.Remove(_employee.Find(obj => obj.Id == id)!);
            await SerializeAsync();
        }

        public async Task<bool> UpdateAsync(Employee executorUpdate)
        {
            await DeserializeAsync();
            var index = _employee!.FindIndex(obj => obj.Id == executorUpdate.Id);
            if (index != -1)
                _employee[index] = executorUpdate;
            else return false;
            await SerializeAsync();
            return true;
        }

        private async Task DeserializeAsync()
        {
            await _semaphoreSlim.WaitAsync();
            if (!File.Exists(_fileName))
            {
                _employee = new List<Employee>();
                _semaphoreSlim.Release();
                return;
            }
            try
            {
                using FileStream? fileStream = File.OpenRead(_fileName);
                {
                    var serializer = new DataContractJsonSerializer(typeof(List<Employee>));
                    _employee = (List<Employee>)serializer.ReadObject(fileStream)!;
                }
                _semaphoreSlim.Release();
            }
            catch
            {
                Console.Write("An error occurred while reading the file\n");
                _employee = new List<Employee>();
                _semaphoreSlim.Release();
            }
        }

        private async Task SerializeAsync()
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                using FileStream? fileStream = new(_fileName, FileMode.Create);
                DataContractJsonSerializer formatter = new(typeof(List<Employee>));
                formatter.WriteObject(fileStream, _employee);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<bool> CheckEmployeeAsync(int id)
        {
            await DeserializeAsync();
            if (_employee!.FindIndex(f => f.Id == id) != -1)
            {
                return true;
            }
            return false;
        }
    }
}
