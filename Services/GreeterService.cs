using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace GrpcServiceSimple
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly List<Employee> _employee = new List<Employee>();
        private int idCount = 0;
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
            _employee.Add(new Employee()
            {
                EmployeeId = idCount++,
                Firstname = "John",
                Lastname = "Smith",
                Salary = 400
            });
        }
        public override Task<EmployeeList> GetAll(Empty empty, ServerCallContext context)
        {
            EmployeeList pl = new EmployeeList();
            pl.Employee.AddRange(_employee);
            return Task.FromResult(pl);
        }
        public override Task<Employee> Get(EmployeeId employeeId, ServerCallContext context)
        {
            return Task.FromResult((from p in _employee where p.EmployeeId == employeeId.Id select p).FirstOrDefault());
        }
        public override Task<Employee> Insert(Employee employee, ServerCallContext context)
        {
            employee.EmployeeId = idCount++;
            _employee.Add(employee);
            return Task.FromResult(employee);
        }
        public override Task<Employee> Update(Employee employee, ServerCallContext context)
        {
            var employeeToUpdate = (from p in _employee where p.EmployeeId == employee.EmployeeId select p).FirstOrDefault();
            if (employeeToUpdate != null)
            {
                employeeToUpdate.Firstname = employee.Firstname;
                employeeToUpdate.Lastname = employee.Lastname;
                employeeToUpdate.Salary = employee.Salary;
                return Task.FromResult(employee);
            }
            return Task.FromException<Employee>(new EntryPointNotFoundException());
        }
        public override Task<Empty> Delete(EmployeeId employeeId, ServerCallContext context)
        {
            var employeeToDelete = (from p in _employee where p.EmployeeId == employeeId.Id select p).FirstOrDefault();
            if (employeeToDelete == null)
            {
                return Task.FromException<Empty>(new EntryPointNotFoundException());
            }
            _employee.Remove(employeeToDelete);
            return Task.FromResult(new Empty());
        }


    }
}
