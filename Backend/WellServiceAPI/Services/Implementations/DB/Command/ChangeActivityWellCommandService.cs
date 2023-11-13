﻿using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Command;

namespace WellServiceAPI.Services.Implementations.DB.Command
{
    public class ChangeActivityWellCommandService : CommandServiceBase<ChangeActiveWell>
    {
        public ChangeActivityWellCommandService(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task ExecuteAsync(ChangeActiveWell command)
        {
            if (command == null) return;

            Well? foundWell = await _wellDBContext.Wells.FindAsync(command.Id);

            if (foundWell == null) return;

            foundWell.Active = command.Active;

            await _wellDBContext.SaveChangesAsync();
        }
    }
}
