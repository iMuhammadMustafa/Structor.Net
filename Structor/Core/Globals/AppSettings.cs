namespace Structor.Core.Globals;

public class AppSettings
{
    public static IConfiguration _configuration;
    public static string SqlLiteConnection = "ConnectionStrings:SqlLiteDatabase";
    public static string SqlLiteInMemoryConnection = "ConnectionStrings:SqlLiteInMemoryDatabase";
}
