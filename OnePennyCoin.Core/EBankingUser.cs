using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using OnePennyCoin.Core.Model;

namespace OnePennyCoin.Core
{
    public class EBankingUser : IdentityUser
    {
        public string AuthToken { get; set; }
        public string SubscriptionKey { get; set; }
        
        public virtual IList<Flow> Flows { get; set; }
    }
}
