//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TenderManagement_Service.DAL.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class SubscriberRecipient
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long RecipientId { get; set; }
        public long SubscriberId { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
    }
}
