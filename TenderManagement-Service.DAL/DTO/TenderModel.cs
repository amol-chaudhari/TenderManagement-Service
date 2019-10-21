using System.Collections.Generic;

namespace TenderManagement_Service.DAL.DTO
{
    public class TenderModel
    {
        public long Id { get; set; }
        public string TenderId { get; set; }
        public string BuyerName { get; set; }
        public string Description { get; set; }
        public string ClosingDate { get; set; }
        public string PublishedDate { get; set; }
        public string InspectionDate { get; set; }
        public bool IsNational { get; set; }
        public int CategoryId { get; set; }
        public string FileName { get; set; }
        public bool IsDistributed { get; set; }
        public string CategoryName { get; set; }
        public string FileContent { get; set; }

        public virtual ICollection<ProvinceModel> Provinces { get; set; }
    }
}