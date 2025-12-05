using api.Etc;
using DataAccess;

namespace Test;

public class SetupTests(
        MyDbContext ctx,
        ISeeder seeder)
    //ITestOutputHelper outputHelper,
    //IAuthService authService)
{
/*
    [Fact]
    public async Task RegisterReturnsJwtWhichCanVerifyAgain()
    {
        var result = await authService.Register(new RegisterRequestDto
        {
            Email = "tes@email.dk",
            Password = "asædkjlsadjsadjlksad"
        });
        outputHelper.WriteLine(result.Token);
        var token = await authService.VerifyAndDecodeToken(result.Token); //Does not throw is the "assertion" here
        outputHelper.WriteLine(JsonSerializer.Serialize(token));
    }

    [Fact]
    public async Task SeederDoesNotThrowException()
    {
        await seeder.Seed();
    }*/
}