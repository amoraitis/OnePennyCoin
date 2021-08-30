using System;
using System.ComponentModel.DataAnnotations;

namespace OnePennyCoin.Core.Model
{
    public class Flow
    {
        public string UserId { get; set; }
        public virtual EBankingUser User { get; set; }
        
        [Key]
        public Guid FlowId { get; set; }
        
        public string CardId { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
        public DateTime LastRefreshDate { get; set; }
        public bool IsEnabled { get; set; }
    }
}
