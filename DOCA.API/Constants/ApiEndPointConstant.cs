namespace DOCA.API.Constants;

public class ApiEndPointConstant
{
    static ApiEndPointConstant() {}
    
    public const string RootEndPoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndPoint + ApiVersion;
    
    
    public class Auth
    {
        public const string SendOtp = ApiEndpoint + "/otp";
        public const string Signup = ApiEndpoint + "/signup";
        public const string Login = ApiEndpoint + "/login";
        public const string ForgetPassword = ApiEndpoint + "/forget-password";
    }
    
    public class Member
    {
        public const string MemberEndpoint = ApiEndpoint + "/members";
        public const string MemberById = MemberEndpoint + "/{id}";
        public const string MemberInformation = MemberEndpoint + "/information";
        public const string MemberWarrantyRequest = MemberEndpoint + "/warranty-requests";
    }
    
    public class Product
    {
        public const string ProductEndpoint = ApiEndpoint + "/products";
        public const string ProductById = ProductEndpoint + "/{id}";
        public const string ProductImage = ProductById + "/product-image";
    }
}