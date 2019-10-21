using System.Collections.Generic;
using TenderManagement_Service.DAL.DTO;

namespace TenderManagement_Service.DAL.Repository
{
    public interface ISubscriberRepository
    {
        IList<SubscriberModel> GetSubscribers();
        SubscriberModel GetSubscriberData(long id);
        SubscriberModel UpdateSubscriberData(long id, SubscriberModel model);
        IList<TenderModel> GetTendersData(AccountModel model);
        IList<SubscriptionTypeModel> GetSubscriptionTypes();
        SubscriberModel CreateSubscriber(SubscriberModel model);
    }
}
