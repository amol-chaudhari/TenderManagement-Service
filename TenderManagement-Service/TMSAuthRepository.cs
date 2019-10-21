using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Security.Claims;
using TenderManagement_Service.DAL.Entities;
using TenderManagement_Service.DAL.Repository;

namespace TenderManagement_Service
{
    public class TMSAuthRepository : IDisposable
    {
        private IAccountRepository _accountRepository;

        private readonly ApplicationUserManager _userManager;

        public TMSAuthRepository(ApplicationUserManager userManager)
        {
            _accountRepository = new AccountRepository(new TMSDBContext());
            _userManager = userManager;
        }


        public void FillCustomClaims(ClaimsIdentity identity, string email)
        {
            var user = _accountRepository.GetUserByEmail(email);
            if (user != null)
            {
                identity.AddClaim(new Claim("UserId", user.Id.ToString()));
                identity.AddClaim(new Claim("Function", user.FunctionCode));
                identity.AddClaim(new Claim("Activated", user.IsActive.ToString()));
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}