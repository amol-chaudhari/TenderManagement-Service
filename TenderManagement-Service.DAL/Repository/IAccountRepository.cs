using System.Collections.Generic;
using TenderManagement_Service.DAL.DTO;

namespace TenderManagement_Service.DAL.Repository
{
    public interface IAccountRepository
    {
        AccountModel GetUserByEmail(string email);
    }
}
