
namespace TenderManagement_Service.DAL.DTO
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentId { get; set; }
    }
}
