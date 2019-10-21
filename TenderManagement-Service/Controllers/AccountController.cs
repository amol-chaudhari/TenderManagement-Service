using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Http;
using TenderManagement_Service.DAL.DTO;
using TenderManagement_Service.DAL.Entities;
using TenderManagement_Service.DAL.Repository;

namespace TenderManagement_Service.Controllers
{
    [RoutePrefix("api/account")]
    public class AccountController : BaseController
    {
        private IAccountRepository _accountRepository;

        public UserManager<ApplicationUserModel> UserManager { get; private set; }

        public AccountController()
            : this(new UserManager<ApplicationUserModel>(new UserStore<ApplicationUserModel>(new ApplicationDbContext())))
        {
            _accountRepository = new AccountRepository(new TMSDBContext());
        }

        public AccountController(UserManager<ApplicationUserModel> userManager)
        {
            UserManager = userManager;
        }

        internal AccountModel LoggedInUser
        {
            get
            {
                var claimsList = LoggedInUserClaims;
                var email = claimsList.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));
                return _accountRepository.GetUserByEmail(email?.Value);
            }
        }

        [HttpGet]
        [Route("userclaims")]
        public HttpResponseMessage GetUserClaims()
        {
            var claimsList = LoggedInUserClaims;
            var email = claimsList.FirstOrDefault(c => c.Type.Equals(ClaimTypes.Email));
            var result = _accountRepository.GetUserByEmail(email?.Value);
            if (result == null)
                return null;
            result.LoggedOn = claimsList.FirstOrDefault(c => c.Type.Equals("LoggedOn")).Value;
            return ToJson(result);
        }

        [Route("register")]
        [HttpPost]
        public HttpResponseMessage Register([FromBody]AccountModel model)
        {
            if (model == null) return ToJson(null);
            model.Email = model.UserName;//email is username
            var user = new ApplicationUserModel() { UserName = model.UserName, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName };
            var result = UserManager.Create(user, model.Password);
            if (result.Succeeded)//means user creation was successful
            {
                var subscriberModel = new SubscriberModel
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    IsNational = model.IsNational,
                    SubscriptionId = model.SubscriptionDuration,
                    IsActive = false //set to deactive by default
                };
                var subscriberRepository = new SubscriberRepository();
                subscriberModel = subscriberRepository.CreateSubscriber(subscriberModel);
                if (subscriberModel?.SubscriberId <= 0) //some issue creating subscriber
                    model = null;
            }
            else
                model = null;
            return ToJson(model);
        }
    }
}