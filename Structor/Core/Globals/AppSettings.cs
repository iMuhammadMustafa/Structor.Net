namespace Structor.Core.Globals;

public static class AppSettings
{
    public static IConfiguration Configuration = default!;
    public static readonly string SqlLiteConnection = "ConnectionStrings:SqlLiteDatabase";
    public static readonly string SqlLiteInMemoryConnection = "ConnectionStrings:SqlLiteInMemoryDatabase";

    public static readonly string OAUTH_URL = "/api/oauth/login/provider";

}
