using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;

namespace TenderManagement_Service.Controllers
{
    public class BaseController : ApiController
    {
        protected HttpResponseMessage ToJson(dynamic obj)
        {
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
            return response;
        }

        protected IList<Claim> LoggedInUserClaims
        {
            get
            {
                var user = HttpContext.Current.GetOwinContext().Authentication.User;
                var claims = user.Claims;
                return claims.ToList();
            }
        }
    }
}