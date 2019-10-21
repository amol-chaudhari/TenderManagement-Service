using System.Collections.Generic;
using TenderManagement_Service.DAL.DTO;

namespace TenderManagement_Service.DAL.Repository
{
    public interface ITenderRepository
    {
        IList<TenderFileModel> GetTenderFiles();
        IList<LocationModel> GetTenderLocations();
        IList<ProvinceModel> GetTenderProvinces();
        IList<CategoryModel> GetTenderCategories();
        TenderModel GetTenderData(TenderFileModel model);
        TenderModel UpdateTenderData(long id, TenderModel model);
        TenderModel InsertTenderData(TenderModel model);
    }
}
