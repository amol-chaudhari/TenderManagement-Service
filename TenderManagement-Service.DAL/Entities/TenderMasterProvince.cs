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
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class TenderMasterProvince
    {
        [System.ComponentModel.DataAnnotations.Key]
        public long TenderProvinceId { get; set; }
        public long TenderId { get; set; }
        public int ProvinceId { get; set; }
        public bool IsDeleted { get; set; }

        [ForeignKey("TenderId")]
        public virtual TenderMaster TenderMaster { get; set; }
    }
}
