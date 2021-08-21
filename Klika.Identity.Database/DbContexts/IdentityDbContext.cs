using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers;
using Klika.Identity.Model.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Klika.Identity.Database.DbContexts
{
    public class IdentityDbContext : IdentityDbContext<ApplicationUser>
    {
        #region Ctor

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options) { }
        
        #region Trigger overriding for EntityFrameworkCore.Triggers
            
        public override Int32 SaveChanges() {
            return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess: true);
        }
        public override Int32 SaveChanges(Boolean acceptAllChangesOnSuccess) {
            return this.SaveChangesWithTriggers(base.SaveChanges, acceptAllChangesOnSuccess);
        }
        public override Task<Int32> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken)) {
            return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, acceptAllChangesOnSuccess: true, cancellationToken: cancellationToken);
        }
        public override Task<Int32> SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken)) {
            return this.SaveChangesWithTriggersAsync(base.SaveChangesAsync, acceptAllChangesOnSuccess, cancellationToken);
        }
        
        #endregion
        
        #endregion
    }
}
