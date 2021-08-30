using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnePennyCoin.Core;
using OnePennyCoin.Core.Model;

namespace OnePennyCoin.Data
{
    public class ApplicationDbContext : IdentityDbContext<EBankingUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public List<Flow> Flows { get; set; }
        public List<EBankingUser> EBankingUsers { get; set; }
    }
}
