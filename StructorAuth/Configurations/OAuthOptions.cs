using Structor.Auth.Enums;

namespace Structor.Auth.Configurations;

public class OAuthOptions
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUrl { get; set; } = string.Empty;
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint { get; set; } = string.Empty;
    public string UserInformationEndpoint { get; set; } = string.Empty;
    public string Scope { get; set; } = string.Empty;
    public string CallbackUrl { get; set; } = string.Empty;
    internal string DataProtector { get; set; } = string.Empty;
    public string DataProtectionSecret { get; set; } = string.Empty;
    public OAuthProvider OAuthProvider { get; set; }


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
