using FastMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using TenderManagement_Service.DAL.DTO;
using TenderManagement_Service.DAL.Entities;

namespace TenderManagement_Service.DAL.Repository
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly TMSDBContext _context;

        public SubscriberRepository()
        {
            _context = new TMSDBContext();
        }

        public SubscriberRepository(TMSDBContext context)
        {
            _context = context;
        }

        public IList<SubscriberModel> GetSubscribers()
        {
            TypeAdapterConfig<Subscriber, SubscriberModel>
            .NewConfig()
            .IgnoreMember(dest => dest.SubscriberAccomodities)
            .IgnoreMember(dest => dest.SubscriberRecipients);

            var tenders = _context.Subscribers.ToList();
            var result = TypeAdapter.Adapt<List<SubscriberModel>>(tenders);
            return result;
        }

        public SubscriberModel GetSubscriberData(long id)
        {
            var entity = _context.Subscribers.Find(id);
            //filter deleted items
            if (entity.SubscriberAccomodities != null) entity.SubscriberAccomodities = entity.SubscriberAccomodities.Where(d => !d.IsDeleted).ToList();
            return TypeAdapter.Adapt<Subscriber, SubscriberModel>(entity);
        }

        public IList<TenderModel> GetTendersData(AccountModel model)
        {
            //get tender based on subscriber type - national or provinces
            //filter them on categories for which subscriber subscribed for!

            var accomodities = _context.SubscriberAccomodities.Where(o => o.SubscriberId == model.Id && !o.IsDeleted).Select(x => x.CategoryId).Distinct().ToList();

            if (accomodities.Count == 0)//means no commodities set up by subscriber, return nothing
                return new List<TenderModel>();

            TypeAdapterConfig<TenderMaster, TenderModel>
            .NewConfig()
            .MapFrom(dest => dest.CategoryName, src => src.Category.CategoryName);

            //get tenders
                //which are distributed
                //are processed for distribution
                //matches subscriber subscription type - national or provincial
                //closingdate is still in future or today
                //contain category from one of the subscribers categories
            var tenders = _context.TenderMasters.Where(o => o.IsDistributed && o.HasProcessed == 1 
                          && o.IsNational == model.IsNational
                          && o.ClosingDate >= DateTime.Now && accomodities.Contains(o.CategoryId.Value)).ToList();

            //if its provincial, need to filter by customer's selected provinces
            if (!model.IsNational)
            {
                var preferredProvinces = _context.SubscriberProvinces.Where(d => d.SubscriberId == model.Id && !d.IsDeleted).Select(d => d.ProvinceId).Distinct().ToList();

                if(preferredProvinces.Count == 0)//means no preferences set up by subscriber, return nothing
                    return new List<TenderModel>();

                //filter by customer selection only
                tenders = tenders.Where(d => d.Provinces.Any(x => !x.IsDeleted && preferredProvinces.Contains(x.ProvinceId))).ToList();
            }

            var result = TypeAdapter.Adapt<List<TenderModel>>(tenders);
            return result;
        }

        public SubscriberModel UpdateSubscriberData(long id, SubscriberModel model)
        {
            if (id > 0 && model.SubscriberId == id)
            {
                var entity = TypeAdapter.Adapt<SubscriberModel, Subscriber>(model);

                //save provinces
                var existingProvinces = _context.SubscriberProvinces.Where(d => d.SubscriberId== id).ToList();
                if (!entity.IsNational.HasValue || !entity.IsNational.Value)
                {
                    //check for removed ones
                    var newProvinces = entity.Provinces.Select(d => d.ProvinceId).ToList();
                    var removedProvinces = existingProvinces.Where(d => !newProvinces.Contains(d.ProvinceId) && !d.IsDeleted);//only checked the ones which are not removed before

                    foreach (var province in removedProvinces)
                    {
                        province.IsDeleted = true;
                        _context.Entry(province).State = EntityState.Modified;
                    }

                    //add new one and check for existing ones(skip saving)
                    foreach (var province in entity.Provinces)
                    {
                        var existingProvince = _context.SubscriberProvinces.FirstOrDefault(d => d.SubscriberId == id && d.ProvinceId == province.ProvinceId);
                        if (existingProvince == null)
                        {
                            province.SubscriberId = id;
                            _context.SubscriberProvinces.Add(province);
                        }
                        else if (existingProvince.IsDeleted) //save db update operation
                        {
                            existingProvince.IsDeleted = false;
                            _context.Entry(existingProvince).State = EntityState.Modified;
                        }
                    }
                }

                entity.Provinces = null;

                //save accomodities first
                //what happens when subscriber removes existing ones
                var existingAccomodities = _context.SubscriberAccomodities.Where(o => o.SubscriberId == id).ToList();
                var newAccomodities = entity.SubscriberAccomodities.Select(d => d.CategoryId).ToList();
                var removedAccomodities = existingAccomodities.Where(d => !newAccomodities.Contains(d.CategoryId) && !d.IsDeleted);

                //mark deleted commodities
                foreach (var removed in removedAccomodities)
                {
                    removed.IsDeleted = true;
                    _context.Entry(removed).State = EntityState.Modified;
                }

                //add new ones and check if exists already(skip saving)
                foreach (var accomodity in entity.SubscriberAccomodities)
                {
                    var existingAccomodity = _context.SubscriberAccomodities.FirstOrDefault(o => o.SubscriberId == id && o.CategoryId == accomodity.CategoryId);
                    if (existingAccomodity == null)
                    {
                        _context.SubscriberAccomodities.Add(accomodity);
                    }
                    else if(existingAccomodity.IsDeleted) //only if its deleted, save db update operation
                    {
                        existingAccomodity.IsDeleted = false;
                        _context.Entry(existingAccomodity).State = EntityState.Modified;
                    }
                }

                entity.SubscriberAccomodities = null;
                _context.Entry(entity).State = EntityState.Modified;
                _context.Entry(entity).Property(d => d.DateCreated).IsModified = false;
                _context.Entry(entity).Property(d => d.FunctionId).IsModified = false;
                _context.Entry(entity).Property(d => d.IsNational).IsModified = false;

                //save children
                foreach (var recipent in entity.SubscriberRecipients)
                {
                    if (recipent.RecipientId <= 0)
                    {
                        //check if its exists already, then we can just modify it
                        var existing = _context.SubscriberRecipients
                                        .FirstOrDefault(o => o.SubscriberId == entity.SubscriberId
                                        && (o.ContactName.ToLower().Equals(recipent.ContactName.ToLower()) || o.Email.ToLower().Equals(recipent.Email.ToLower())));
                        if (existing != null)
                        {
                            existing.ContactName = recipent.ContactName;
                            existing.Email = recipent.Email;
                            existing.IsDeleted = false;
                            _context.Entry(existing).State = EntityState.Modified;
                        }
                        else _context.SubscriberRecipients.Add(recipent);
                    }
                    else
                        _context.Entry(recipent).State = EntityState.Modified;
                }

                var outcome = _context.SaveChanges();
                return TypeAdapter.Adapt<Subscriber, SubscriberModel>(entity);
            }
            return null;
        }

        public IList<SubscriptionTypeModel> GetSubscriptionTypes()
        {
            var sourceList = _context.SubscriptionTypes.Where(o => o.IsActive).ToList();
            return TypeAdapter.Adapt<List<SubscriptionType>, List<SubscriptionTypeModel>>(sourceList);
        }

        public SubscriberModel CreateSubscriber(SubscriberModel model)
        {
            if (model != null)
            {
                var entity = TypeAdapter.Adapt<SubscriberModel, Subscriber>(model);
                entity.DateCreated = DateTime.Now;
                entity.FunctionId = _context.Functions.FirstOrDefault(d => d.FunctionName.ToLower().Equals("subscriber") && d.IsActive)?.FunctionId;//default to subscriber
                _context.Subscribers.Add(entity);
                var outcome = _context.SaveChanges();
                if (outcome > 0)
                    return TypeAdapter.Adapt<Subscriber, SubscriberModel>(entity);
            }
            return null;
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
