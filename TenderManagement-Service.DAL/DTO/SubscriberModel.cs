
using System.Collections.Generic;

namespace TenderManagement_Service.DAL.DTO
{
    public class SubscriberModel
    {
        public long SubscriberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public string ContactNumber { get; set; }
        public string MobileNumber { get; set; }
        public bool IsActive { get; set; }
        public int SubscriptionId { get; set; }
        public bool IsNational { get; set; }

        
        public virtual ICollection<ProvinceModel> Provinces { get; set; }
        public virtual ICollection<SubscriberAccomodity> SubscriberAccomodities { get; set; }
        public virtual ICollection<SubscriberRecipient> SubscriberRecipients { get; set; }
    }

    public class SubscriberAccomodity
    {
        public long CommodityId { get; set; }
        public int CategoryId { get; set; }
        public long SubscriberId { get; set; }
    }

    public class SubscriberRecipient
    {
        public long RecipientId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public long SubscriberId { get; set; }
    }
}