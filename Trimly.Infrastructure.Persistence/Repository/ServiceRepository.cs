using Microsoft.EntityFrameworkCore;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Domain.Models;
using Trimly.Infrastructure.Persistence.Context;

namespace Trimly.Infrastructure.Persistence.Repository
{
    public class ServiceRepository : GenericRepository<Services>, IServiceRepository
    {
        public ServiceRepository(TrimlyContext context) : base(context) { }

        private string GenerateDiscountCode(double discountPercentage)
        {
            string code = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();

            code = $"{code}-{discountPercentage}%"; 

            return code;
        }

        public async Task ApplyDiscountCodeAsync(Services services, Guid registeredCompaniesId, string discountCode, CancellationToken cancellationToken)
        {
            var discountPercentageString = discountCode.Split('-').Last().Replace("%", "");
            var discountPercentage = double.Parse(discountPercentageString) / 100.0;

            decimal discountAmount = services.Price * (decimal)discountPercentage;
            services.Price = services.Price - discountAmount;

            services.ConfirmationCode = discountCode;

            await _context.SaveChangesAsync(cancellationToken);

        }

        public async Task<IEnumerable<Services>> GetServicesByCompanyIdAsync(Guid companyId,
            CancellationToken cancellationToken) => 
        await _context.Set<Services>()
            .AsNoTracking()
            .Where(x => x.RegisteredCompanyId == companyId)
            .ToListAsync(cancellationToken);
        
        public async Task<IEnumerable<Services>> GetServicesByDurationInMinutesAsync(Guid registeredCompaniesId, int durationInMinutes, CancellationToken cancellationToken) =>
            await _context.Set<Services>()
            .AsNoTracking()
            .Where(d => d.DurationInMinutes == durationInMinutes)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Services>> GetServicesByNameAsync(Guid registeredCompaniesId, string name,  CancellationToken cancellationToken) => 
            await _context.Set<Services>()
            .AsNoTracking()
            .Where(s => s.RegisteredCompanyId == registeredCompaniesId && s.Name == name)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Services>> GetServicesByPriceAsync(Guid registeredCompaniesId, decimal price,  CancellationToken cancellationToken) => 
            await _context.Set<Services>()
            .AsNoTracking()
            .Where(s => s.RegisteredCompanyId == registeredCompaniesId && s.Price == price)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Services>> GetServicesWithDurationLessThan30MinutesAsync(Guid registeredCompaniesId, CancellationToken cancellationToken) => 
            await _context.Set<Services>()
            .AsNoTracking()
            .Where(s => s.RegisteredCompanyId == registeredCompaniesId && s.DurationInMinutes < 30)
            .ToListAsync(cancellationToken);
        
    }
}

