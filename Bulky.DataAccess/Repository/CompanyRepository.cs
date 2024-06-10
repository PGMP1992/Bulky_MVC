using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;


namespace Bulky.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company> , ICompanyRepository
    {
        private ApplicationDBContext _db;

        public CompanyRepository((ApplicationDBContext db) : base(db)
        {
            _db = db;    
        }

        public void Add(Company entity)
        {
            throw new NotImplementedException();
        }

        public Company Get(Expression<Func<Company, bool>> filter, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Company> GetAll(string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public void Remove(Company entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<Company> entity)
        {
            throw new NotImplementedException();
        }

        public void Update(ICompanyRepository obj)
        {
            throw new NotImplementedException();
        }
    }
}
