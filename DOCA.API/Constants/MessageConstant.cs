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

}