using Northwind.Core.Entities;
using Northwind.Core.Interfaces;
using Northwind.Domain.Entities;
using Northwind.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NorthwindContext _context;

        public UnitOfWork(NorthwindContext context)
        {
            _context = context;
            CategoryRepository = new Repository<Category>(_context);
            ProductRepository = new Repository<Product>(_context);
            AccountRepository = new Repository<User>(_context);
        }

        public IRepository<Category> CategoryRepository { get; }
        public IRepository<Product> ProductRepository { get; }
        public IRepository<User> AccountRepository { get; }

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
