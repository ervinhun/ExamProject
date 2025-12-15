using Api.Dto.Auth.Request;
using api.Services;
using DataAccess;
using Test.Util;

namespace Test;

public class SetupTests(
    MyDbContext ctx,
    ISeeder seeder,
    //ITestOutputHelper outputHelper,
    IMyAuthenticationService authService)
{
    [Fact]
    public async Task SeederDoesNotThrowException()
    {
        await seeder.Seed();
    }
}