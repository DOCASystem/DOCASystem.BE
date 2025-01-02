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
    
    public class Category
    {
        public const string CategoryEndPoint = ApiEndpoint + "/categories";
        public const string CategoryById = CategoryEndPoint + "/{id}";
        public const string UpdateProductCategory = CategoryById + "/product-category";
        public const string ProductByCategoryId = CategoryById + "/product";
    }
    
    public class AnimalCategory
    {
        public const string CategoryEndPoint = ApiEndpoint + "/animalCategories";
        public const string CategoryById = CategoryEndPoint + "/{id}";
        public const string UpdateAnimalCategory = CategoryById + "/animal-category";
        public const string AnimalByAnimalCategoryId = CategoryById + "/animal";
    }
    
    public class Animal
    {
        public const string AnimalEndpoint = ApiEndpoint + "/animals";
        public const string AnimalById = AnimalEndpoint + "/{id}";
        public const string AnimalImage = AnimalById + "/animal-image";
    }
    
    public class BlogCategory
    {
        public const string CategoryEndPoint = ApiEndpoint + "/blogCategories";
        public const string CategoryById = CategoryEndPoint + "/{id}";
        public const string UpdateAnimalCategory = CategoryById + "/blog-category";
        public const string BlogByBlogCategoryId = CategoryById + "/blog";
    }
    
    public class Blog
    {
        public const string BlogEndpoint = ApiEndpoint + "/blogs";
        public const string BlogById = BlogEndpoint + "/{id}";
    }
    
    public class Order
    {
        public const string OrderEndpoint = ApiEndpoint + "/orders";
        public const string OrderById = OrderEndpoint + "/{id}";
        public const string UpdateOrderStatus = OrderById + "/status";
        public const string UpdateSupportOrderStatusSuccess = UpdateOrderStatus + "/success";
        public const string UpdateSupportOrderStatusRefuse = UpdateOrderStatus + "/refuse";
        public const string UpdateSupportOrderStatusShipping = UpdateOrderStatus + "/shipping";
        public const string OrderItems = OrderById + "/order-items";
    }
    
    public class Cart
    {
        public const string CartEndPoint = ApiEndpoint + "/carts";
    }
    
    public class Payment
    {
        public const string PaymentEndPoint = ApiEndpoint + "/payments";
        public const string PaymentCheckOut = PaymentEndPoint + "/checkout";
    }

}