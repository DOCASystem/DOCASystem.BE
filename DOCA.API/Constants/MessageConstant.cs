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
    }

    public static class Product
    {
        public const string ProductIsHidden = "Sản phẩm không tồn tại";
        public const string ProductIdNotNull = "Id sản phẩm không được để trống";
        public const string ProductNotFound = "Sản phẩm không tồn tại";
        public const string CreateProductFail = "Tạo sản phẩm không thành công với tên";
        public const string UpdateProductFail = "Cập nhật sản phẩm không thành công";
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
        public const string  AnimalCategoryIdNotNull = "Id danh mục không được để trống";
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
    
    
    

}