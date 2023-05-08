namespace Structor.Core.Globals;

public static class AppSettings
{
    public static IConfiguration Configuration = default!;
    public static string SqlLiteConnection = "ConnectionStrings:SqlLiteDatabase";
    public static string SqlLiteInMemoryConnection = "ConnectionStrings:SqlLiteInMemoryDatabase";

}
