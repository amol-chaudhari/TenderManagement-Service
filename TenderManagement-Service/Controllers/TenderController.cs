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
    [RoutePrefix("api/tenders")]
    public class TenderController : BaseController
    {
        private ITenderRepository _tenderRepository;

        public TenderController()
        {
            _tenderRepository = new TenderRepository(new TMSDBContext());
        }
        public TenderController(ITenderRepository tenderRepository)
        {
            _tenderRepository = tenderRepository;
        }

        [HttpGet]
        [Route("files")]
        public HttpResponseMessage GetTenderFiles()
        {
            return ToJson(_tenderRepository.GetTenderFiles());
        }

        [HttpGet]
        [Route("locations")]
        public HttpResponseMessage GetTenderLocations()
        {
            return ToJson(_tenderRepository.GetTenderLocations());
        }

        [HttpGet]
        [Route("provinces")]
        public HttpResponseMessage GetTenderProvinces()
        {
            return ToJson(_tenderRepository.GetTenderProvinces());
        }

        [HttpGet]
        [Route("categories")]
        public HttpResponseMessage GetTenderCategories()
        {
            return ToJson(_tenderRepository.GetTenderCategories());
        }

        [HttpPost]
        [Route("data")]
        public HttpResponseMessage GetTenderData([FromBody]TenderFileModel model)
        {
            return ToJson(_tenderRepository.GetTenderData(model));
        }

        [HttpPut]
        [Route("{id}")]
        public HttpResponseMessage UpdateTenderData(long id, [FromBody]TenderModel model)
        {
            return ToJson(_tenderRepository.UpdateTenderData(id, model));
        }

        [HttpPost]
        [Route("")]
        public HttpResponseMessage InsertTenderData([FromBody]TenderModel model)
        {
            return ToJson(_tenderRepository.InsertTenderData(model));
        }
    }
}