
namespace TenderManagement_Service.DAL.DTO
{
    public class AccountModel
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string LoggedOn { get; set; }
        public string FunctionCode { get; set; }
        public bool IsActive { get; set; }
        public bool IsNational { get; set; }
        public int SubscriptionDuration { get; set; }
    }
}
