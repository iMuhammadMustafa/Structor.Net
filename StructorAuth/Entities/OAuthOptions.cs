namespace Structor.Auth.Entities;

public class OAuthOptions
{
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string RedirectUrl { get; set; } = default!;

    public string AuthorizationEndpoint { get; set; } = default!;
    public string TokenEndpoint { get; set; } = default!;
    public string UserInformationEndpoint { get; set; } = default!;
    public string Scope { get; set; } = default!;
    public string CallbackUrl { get; set; } = default!;

    internal string DataProtector {  get; set; } = default!;
    internal string DataProtectionSecret { get; set; } = default!;


    //public string CallbackBase { get; set; } = default!;
    //public string CallbackPath { get; set; } = default!;
    //internal string CallbackUrl
    //{ 
    //    get
    //    {
    //        return CallbackBase + CallbackPath;
    //    }
    //    set
    //    {
    //        CallbackUrl = value;
    //    }
    //}

}
