namespace DOCA.API.Constants;

public class ApiEndPointConstant
{
    static ApiEndPointConstant() {}
    
    public const string RootEndPoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndPoint + ApiVersion;
    
    public class User
    {
        public const string UserEndpoint = ApiEndpoint + "/user";
        public const string Login = UserEndpoint + "/login";
    }
}