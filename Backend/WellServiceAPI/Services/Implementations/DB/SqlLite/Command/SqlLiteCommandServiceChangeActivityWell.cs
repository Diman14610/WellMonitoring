using WellServiceAPI.Data;
using WellServiceAPI.Models;
using WellServiceAPI.Services.Abstractions.DB;
using WellServiceAPI.Shared.Actions.Command;

namespace WellServiceAPI.Services.Implementations.DB.SqlLite.Command
{
    public class SqlLiteCommandServiceChangeActivityWell : CommandServiceBase<ChangeActiveWell>
    {
        public SqlLiteCommandServiceChangeActivityWell(WellDBContext wellDBContext) : base(wellDBContext)
        {
        }

        public override async Task ExecuteAsync(ChangeActiveWell command)
        {
            if (command == null) return;

            Well? foundWell = await _wellDBContext.Wells.FindAsync(command.Id).ConfigureAwait(false);

            if (foundWell == null) return;

            foundWell.Active = command.Active;

            await _wellDBContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
