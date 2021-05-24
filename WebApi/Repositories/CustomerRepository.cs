using System;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models;

namespace WebApi.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetById(int id);
        ICollection<Customer> ListAll();
        void AddOrUpdate(Customer customer);
        void Remove(int id);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private DatabaseContext _context;

        public CustomerRepository(DatabaseContext context)
        {
            _context = context;
        }

        public ICollection<Customer> ListAll()
        {
            return _context.Customers.ToList();
        }
        public Customer GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.Id == id);
        }

        public void AddOrUpdate(Customer customer)
        {
            if (customer.Id == default)
                _context.Add(customer);
            else
                _context.Update(customer);

            _context.SaveChanges();
        }

        public void Remove(int id)
        {
            var removeEntity = GetById(id);
            if (removeEntity is null) throw new KeyNotFoundException();
            _context.Remove(removeEntity);
            _context.SaveChanges();
        }
    }
}
