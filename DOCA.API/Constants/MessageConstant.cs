namespace DOCA.API.Constants;

public class MessageConstant
{
    public static class User
    {
        public const string UserNotFound = "Username không tồn tại";
        public const string LoginFail = "Đăng nhập không thành công";
        public const string UserNameExisted = "Tên đăng nhập đã tồn tại";
        public const string PhoneNumberExisted = "Số điện thoại đã tồn tại";
        public const string RegisterFail = "Tạo tài khoản thất bại";
        public const string MemberNotFound = "Thành viên không tồn tại";
        public const string UpdateFail = "Update không thành công";
        public const string RoleNotFound = "Role không tồn tại";
        public const string MemberAddressNotFound = "Địa chỉ thành viên không tồn tại";
    }

    
    public static class Sms
    {
        public const string SendSmsFailed = "Gửi tin nhắn không thành công";
        public const string OtpAlreadySent = "Mã OTP đã được gửi";
        public const string OtpNotFound = "Mã OTP không tồn tại";
        public const string OtpIncorrect = "Mã OTP không chính xác";
    }
    
    public static class Product
    {
        public const string ProductIsHidden = "Sản phẩm không tồn tại";
        public const string ProductIdNotNull = "Id sản phẩm không được để trống";
        public const string ProductNotFound = "Sản phẩm không tồn tại";
        public const string CreateProductFail = "Tạo sản phẩm không thành công với tên";
        public const string UpdateProductFail = "Cập nhật sản phẩm không thành công";
        public const string ProductOutOfStock = "Sản phẩm đã hết hàng";
    }
    
    public static class Category
    {
        public const string  CategoryIdNotNull = "Id danh mục không được để trống";
        public const string CategoryNotFound = "Danh mục không tồn tại";
        public const string UpdateProductCategoryFail = "Cập nhật danh mục của sản phẩm không thành công";
        public const string UpdateCategoryFail = "Cập nhật danh mục không thành công";
        public const string CreateCategoryFail = "Tạo danh mục không thành công";
        public const string UpdateProductCategoryFailed  = "Cập nhật danh mục sản phẩm không thành công";
        public const string InsertProductCategoryFailed = "Thêm danh mục sản phẩm không thành công";
        public const string DeleteProductCategoryFailed = "Xóa danh mục sản phẩm không thành công";
    }
    
    public static class AnimalCategory
    {
        public const string AnimalCategoryIdNotNull = "Id danh mục không được để trống";
        public const string AnimalCategoryNotFound = "Danh mục không tồn tại";
        public const string UpdateProductCategoryFail = "Cập nhật danh mục của con vật không thành công";
        public const string UpdateCategoryFail = "Cập nhật danh mục không thành công";
        public const string CreateCategoryFail = "Tạo danh mục không thành công";
        public const string UpdateProductCategoryFailed  = "Cập nhật danh mục con vật không thành công";
        public const string InsertProductCategoryFailed = "Thêm danh mục con vật không thành công";
        public const string DeleteProductCategoryFailed = "Xóa danh mục con vật không thành công";
    }
    
    public static class Animal
    {
        public const string AnimalNotFound = "Con vật không tồn tại";
        public const string AnimalIdNotNull = "Id con vật không được để trống";
        public const string CreateAnimalFail = "Tạo con vật không thành công với tên";
        public const string UpdateAnimalFail = "Cập nhật con vật không thành công";
    }
    
    public static class ProductImage
    {
        public const string ProductImageIdNotNull = "Id ảnh sản phẩm không được để trống";
        public const string ProductImageNotFound = "Ảnh sản phẩm không tồn tại";
        public const string DeleteProductImageFail = "Xóa ảnh sản phẩm không thành công";
        public const string MainImageExist = "Ảnh chính đã tồn tại";
        public const string UploadImageFail = "Upload ảnh không thành công";
        public const string AddProductImageFail = "Thêm ảnh sản phẩm không thành công";
        public const string WrongMainImageQuantity = "Bắt buộc phải có một ảnh chính";
    }
    
    public static class AnimalImage
    {
        public const string AnimalImageIdNotNull = "Id ảnh con vật không được để trống";
        public const string AnimalImageNotFound = "Ảnh con vật không tồn tại";
        public const string DeleteAnimalImageFail = "Xóa ảnh con vật không thành công";
        public const string MainImageExist = "Ảnh chính đã tồn tại";
        public const string UploadImageFail = "Upload ảnh không thành công";
        public const string AddAnimalImageFail = "Thêm ảnh con vật không thành công";
        public const string WrongMainImageQuantity = "Bắt buộc phải có một ảnh chính";
    }
    
    
    public static class BlogCategory
    {
        public const string BlogCategoryIdNotNull = "Id danh mục không được để trống";
        public const string BlogCategoryNotFound = "Danh mục không tồn tại";
        public const string UpdateBlogCategoryFail = "Cập nhật danh mục của blog không thành công";
        public const string CreateBlogCategoryFail = "Tạo danh mục không thành công";
        public const string UpdateBlogCategoryFailed  = "Cập nhật danh mục blog không thành công";
        public const string InsertBlogCategoryFailed = "Thêm danh mục blog không thành công";
        public const string DeleteBlogCategoryFailed = "Xóa danh mục blog không thành công";
    }
    
    public static class Blog
    {
        public const string BlogIdNotNull = "Id blog không được để trống";
        public const string BlogNotFound = "Blog không tồn tại";
        public const string CreateBlogFail = "Tạo blog không thành công với tên";
        public const string UpdateBlogFail = "Cập nhật blog không thành công";
    }
     public static class Order
    {
        public const string CreateOrderFail = "Tạo đơn hàng không thành công";
        public const string OrderIdNotNull = "Id đơn hàng không được để trống";
        public const string OrderNotFound = "Đơn hàng không tồn tại";
        public const string OrderStatusNotFound = "Trạng thái đơn hàng không tồn tại";
        public const string UpdateOrderStatusFail = "Cập nhật trạng thái đơn hàng không thành công";
    }
    
    public static class Cart
    {
        public const string AddToCartFail = "Thêm sản phẩm vào giỏ hàng không thành công";
        public const string RemoveFromCartFail = "Xóa sản phẩm khỏi giỏ hàng không thành công";
        public const string QuantityMustBeGreaterThanZero = "Số lượng sản phẩm phải lớn hơn 0";
        public const string UpdateQuantityFail = "Cập nhật số lượng sản phẩm không thành công";
        public const string CartNotFound = "Giỏ hàng không tồn tại";
        public const string CartItemIsNull = "Không còn sản phẩm trong giỏ hàng";
    }
    
    public static class Payment
    {
        public const string YourOrderIsPaid = "Đơn hàng của bạn đã được thanh toán";
        public const string YourOrderIsCancelled = "Đơn hàng của bạn đã bị hủy";
        public const string YourOrderIsCompleted = "Đơn hàng của bạn đã hoàn thành";
        public const string CannotFindPaymentLinkInformation = "Không thể tìm thấy thông tin link thanh toán";
        public const string YourOrderIsNotPaid = "Đơn hàng chưa được thanh toán";
        public const string CannotUpdateStatusPaymentAndOrder = "Không thể cập nhật trạng thái thanh toán và đơn hàng";
        public const string UpdateStatusPaymentAndOrderFail = "Cập nhật trạng thái thanh toán và đơn hàng không thành công";
        public const string PaymentNotFound = "Thanh toán không tồn tại";
        public const string OrderCodeNotNull = "OrderCode không được để trống";
        public const string CreatePaymentFail = "Tạo thanh toán không thành công";
        public const string FailToCreatePaymentLink = "Tạo link thanh toán không thành công";
        public const string PayOsStatusNotTrue = "Trạng thái thanh toán của PayOs không hợp lệ";
        public const string CheckOutFail = "Thanh toán không thành công";
        public const string YourOrderIsRefuseReceive = "Đơn hàng của bạn đã từ chối nhận";
        public const string YourOrderIsStillPreparing = "Đơn hàng của bạn đang chuẩn bị";
        public const string YourOrderIsShipping = "Đơn hàng của bạn đang giao hàng";
    }

}