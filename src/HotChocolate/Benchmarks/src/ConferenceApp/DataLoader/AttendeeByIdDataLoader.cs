using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate.ConferencePlanner.Data;
using GreenDonut;
using HotChocolate.Fetching;

namespace HotChocolate.ConferencePlanner.DataLoader
{
    public class AttendeeByIdDataLoader : BatchDataLoader<int, Attendee>
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

        public AttendeeByIdDataLoader(
            IBatchScheduler batchScheduler,
            IDbContextFactory<ApplicationDbContext> dbContextFactory)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory ?? 
                throw new ArgumentNullException(nameof(dbContextFactory));
        }

        protected override async Task<IReadOnlyDictionary<int, Attendee>> LoadBatchAsync(
            IReadOnlyList<int> keys,
            CancellationToken cancellationToken)
        {
            if (keys.Count == 1)
            {
                Counter.One();
            }

            await using ApplicationDbContext dbContext = 
                _dbContextFactory.CreateDbContext();

            return await dbContext.Attendees
                .Where(s => keys.Contains(s.Id))
                .ToDictionaryAsync(t => t.Id, cancellationToken);
        }
    }
}