﻿using Microsoft.EntityFrameworkCore;
using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Query;

namespace WellServiceAPI.Services.Implementations.DB.SqlLite.Query
{
    public class SQLLiteQueryServiceGetAllWellsByActivityParam : QueryServiceBase<GetAllWellsByActivityParam, IEnumerable<Well>>
    {
        public SQLLiteQueryServiceGetAllWellsByActivityParam(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task<IEnumerable<Well>> ExecuteAsync(GetAllWellsByActivityParam query)
        {
            return await _wellDBContext.Wells
                .Where(w => w.Active == query.Active)
                .Include(w => w.Company)
                .Include(w => w.Telemetries)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
