using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using TenderManagement_Service.DAL.DTO;
using TenderManagement_Service.DAL.Entities;

namespace TenderManagement_Service.Provider
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            try
            {
                var data = await context.Request.ReadFormAsync();
                var userStore = new UserStore<ApplicationUserModel>(new ApplicationDbContext());
                var manager = new UserManager<ApplicationUserModel>(userStore);
                var user = await manager.FindAsync(context.UserName, context.Password);
                if (user != null)
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
                    identity.AddClaim(new Claim("Username", user.UserName));
                    identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
                    identity.AddClaim(new Claim("FirstName", user.FirstName));
                    identity.AddClaim(new Claim("LastName", user.LastName));
                    identity.AddClaim(new Claim("Hash", context.Password));
                    identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));

                    //change it to accomodate above usermanager, need to combine it together
                    var authManager = new ApplicationUserManager();
                    authManager.Repository = new TMSAuthRepository(authManager);
                    authManager.Repository.FillCustomClaims(identity, user.Email);
                    context.Validated(identity);
                }
                else return;
            }
            catch (Exception ex)
            {

            }
        }
    }
}