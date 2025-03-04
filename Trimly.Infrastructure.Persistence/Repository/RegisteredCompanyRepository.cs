using Microsoft.EntityFrameworkCore;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;
using Trimly.Infrastructure.Persistence.Context;

namespace Trimly.Infrastructure.Persistence.Repository
{
    public class RegisteredCompanyRepository: GenericRepository<RegisteredCompanies>, IRegisteredCompanyRepository
    {
        public RegisteredCompanyRepository(TrimlyContext context) : base(context) { }


        public async Task<RegisteredCompanies> FilterByNameAsync(string nameCompany, CancellationToken cancellationToken)
        {
            return await _context.Set<RegisteredCompanies>()      
            .AsNoTracking()
            .Where(n => n.Name.ToLower().Contains(nameCompany.ToLower()))
            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<RegisteredCompanies>> FilterByStatusAsync(Status status, CancellationToken cancellationToken) => 
            await _context.Set<RegisteredCompanies>()
            .Where(s => s.Status == status)
            .ToListAsync(cancellationToken);


        public async Task<IEnumerable<RegisteredCompanies>> GetRecentAsync(CancellationToken cancellationToken) => 
            await _context.Set<RegisteredCompanies>()
            .AsNoTracking()
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync(cancellationToken);


        public async Task<IEnumerable<RegisteredCompanies>> OrderByIdAscAsync(CancellationToken cancellationToken) => 
            await _context.Set<RegisteredCompanies>()
            .AsNoTracking()
            .OrderBy(i => i.RegisteredCompaniesId)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<RegisteredCompanies>> OrderByIdDescAsync(CancellationToken cancellationToken) => 
            await _context.Set<RegisteredCompanies>()
            .AsNoTracking()
            .OrderByDescending(d => d.RegisteredCompaniesId)
            .ToListAsync(cancellationToken);
        

        public async Task<IEnumerable<RegisteredCompanies>> OrderByNameAsync(CancellationToken cancellationToken) => 
            await _context.Set<RegisteredCompanies>()
            .AsNoTracking()
            .OrderBy(n => n.Name)
            .ToListAsync(cancellationToken);
       
    }
}
