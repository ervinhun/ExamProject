using api.Etc;

namespace Api.Configuration;

public class SeederForTest : ISeeder
{
    public Task Seed()
    {
        throw new NotImplementedException();
        
        // Add a few players, a few admins, add money to the wallets, etc.
    }
}