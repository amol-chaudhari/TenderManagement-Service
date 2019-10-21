using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TenderManagement_Service.DAL.DTO;
using TenderManagement_Service.DAL.Entities;
using TenderManagement_Service.DAL.Repository;

namespace TenderManagement_Service.Controllers
{
    [RoutePrefix("api/subscribers")]
    public class SubscriberController : BaseController
    {
        private ISubscriberRepository _subscriberRepository;

        public SubscriberController()
        {
            _subscriberRepository = new SubscriberRepository(new TMSDBContext());
        }
        public SubscriberController(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        [HttpGet]
        [Route("list")]
        public HttpResponseMessage GetSubscribers()
        {
            return ToJson(_subscriberRepository.GetSubscribers());
        }

        [HttpGet]
        [Route("")]
        public HttpResponseMessage GetSubscriberData()
        {
            var accountController = new AccountController();
            return ToJson(_subscriberRepository.GetSubscriberData(accountController.LoggedInUser.Id));
        }
        
        [HttpGet]
        [Route("tenders")]
        public HttpResponseMessage GetTendersData()
        {
            var accountController = new AccountController();
            return ToJson(_subscriberRepository.GetTendersData(accountController.LoggedInUser));
        }

        [HttpPut]
        [Route("{id}")]
        public HttpResponseMessage UpdateSubscriberData(long id, [FromBody]SubscriberModel model)
        {
            return ToJson(_subscriberRepository.UpdateSubscriberData(id, model));
        }

        [HttpGet]
        [Route("subscriptions")]
        public HttpResponseMessage GetSubscriptionTypes()
        {
            return ToJson(_subscriberRepository.GetSubscriptionTypes());
        }
    }
}