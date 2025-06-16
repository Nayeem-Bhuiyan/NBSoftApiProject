using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartApp.Application.DTOs.Common;
using SmartApp.Domain.Entities;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Domain.Entities.MasterData;


namespace SmartApp.Persistence.DBContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        #region MasterData
        public DbSet<Country> Country { get; set; }
        #endregion

        #region ViewModel_Data_Retrive
        public DbSet<DropdownDto> VmDropdownInfo { get; set; } = null;
        #endregion

        // Auto change tracker for audit fields
        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries<BaseAuditEntity>();
            var now = DateTime.Now;

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.createdDate = now;
                        entry.Entity.isDeleted = false;
                        // Optionally set createdBy here if you have user context
                        break;
                    case EntityState.Modified:
                        entry.Entity.updatedDate = now;
                        // Optionally set updatedBy here if you have user context
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.isDeleted = true;
                        entry.Entity.deletedDate = now;
                        // Optionally set deletedBy here if you have user context
                        break;
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Rename_Identity_TableName
            modelBuilder.Entity<ApplicationUser>(entity => { entity.ToTable(name: "UserInfo"); });
            modelBuilder.Entity<ApplicationRole>(entity => { entity.ToTable(name: "Roles"); });
            modelBuilder.Entity<IdentityUserRole<string>>(entity => { entity.ToTable("UserRoles"); });
            modelBuilder.Entity<IdentityUserClaim<string>>(entity => { entity.ToTable("UserClaims"); });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => { entity.ToTable("UserLogins"); });
            modelBuilder.Entity<IdentityUserToken<string>>(entity => { entity.ToTable("UserTokens"); });
            modelBuilder.Entity<IdentityRoleClaim<string>>(entity => { entity.ToTable("RoleClaims"); });
            #endregion

            modelBuilder.Entity<DropdownDto>().HasNoKey().ToView("view_name_that_doesnt_exist");

            #region SEED_DATA

            #region MasterPanel

            #region Country
            modelBuilder.Entity<Country>().HasData(new List<Country>
            {
                new Country // 1
                {
                    Id=1,
                    Name = "Bangladesh",
                    Continent="Asia",
                    Code="",
                    PhoneCode="+880",
                    Currency = "Taka"
                }
            });
            #endregion

            #endregion
            #endregion

        }
    }
}