using FastMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using TenderManagement_Service.DAL.DTO;
using TenderManagement_Service.DAL.Entities;
using System.Linq;
using System.Data.Entity;

namespace TenderManagement_Service.DAL.Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly TMSDBContext _context;

        public AccountRepository()
        {
            _context = new TMSDBContext();
        }

        public AccountRepository(TMSDBContext context)
        {
            _context = context;
        }

        public AccountModel GetUserByEmail(string email)
        {
            TypeAdapterConfig<Subscriber, AccountModel>
            .NewConfig()
            .MapFrom(dest => dest.Id, src => src.SubscriberId)
            .MapFrom(dest => dest.FunctionCode, src => src.Function.Code);

            var entity = _context.Subscribers.FirstOrDefault(o => o.Email.ToLower().Equals(email.ToLower()));
            return TypeAdapter.Adapt<Subscriber, AccountModel>(entity);
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
