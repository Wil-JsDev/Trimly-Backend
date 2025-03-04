using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trimly.Core.Application.Interfaces.Repository;
using Trimly.Core.Domain.Models;
using Trimly.Infrastructure.Persistence.Context;

namespace Trimly.Infrastructure.Persistence.Repository
{
    public class ReviewRepository : GenericRepository<Reviews>, IReviewsRepository
    {
        public ReviewRepository(TrimlyContext context) : base(context) { }

        public async Task<double> GetAverageRatingAsync(Guid registeredCompanyId, CancellationToken cancellationToken)
        {
            return Math.Round((await _context.Set<Reviews>()
                    .Where(c => c.RegisteredCompanyId == registeredCompanyId)
                    .Select(r => (double?)r.Rating)
                    .AverageAsync(cancellationToken)) ?? 0.0, 1);

        }
    }
}
