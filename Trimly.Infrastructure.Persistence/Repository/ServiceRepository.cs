﻿using Microsoft.EntityFrameworkCore;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Domain.Enum;
using Trimly.Core.Domain.Models;
using Trimly.Infrastructure.Persistence.Context;

namespace Trimly.Infrastructure.Persistence.Repository
{
    public class ServiceRepository : GenericRepository<Services>, IServiceRepository
    {
        public ServiceRepository(TrimlyContext context) : base(context) { }
        

        public async Task<IEnumerable<Services>> GetServicesByCompanyIdAsync(Guid companyId,
            CancellationToken cancellationToken) => 
        await _context.Set<Services>()
            .AsNoTracking()
            .Where(x => x.RegisteredCompanyId == companyId)
            .ToListAsync(cancellationToken);

        public async Task<IEnumerable<Services>> GetCompletedServicesByMonthAsync(Guid registeredCompaniesId,
            int year, int month, CancellationToken cancellationToken) =>
            await _context.Set<Services>()
                .AsNoTracking()
                .Where(x => x.RegisteredCompanyId == registeredCompaniesId && 
                            x.ServiceStatus == ServiceStatus.Completed &&
                            x.CompletedAt.HasValue && 
                            x.CompletedAt.Value.Year == year && 
                            x.CompletedAt.Value.Month == month)
                .ToListAsync(cancellationToken);
        
        public async Task<IEnumerable<Services>> GetServicesByDurationInMinutesAsync(Guid registeredCompaniesId, int durationInMinutes, CancellationToken cancellationToken) =>
            await _context.Set<Services>()
            .AsNoTracking()
            .Where(d =>  d.RegisteredCompanyId == registeredCompaniesId  && d.DurationInMinutes == durationInMinutes)
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

