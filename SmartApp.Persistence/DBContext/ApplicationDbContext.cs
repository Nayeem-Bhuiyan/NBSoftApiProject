using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartApp.Application.DTOs.Common;
using SmartApp.Domain.Entities;
using SmartApp.Domain.Entities.Auth;
using SmartApp.Domain.Entities.Logging;
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

        #region Auth
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        #endregion

        #region Logging
        public DbSet<ApplicationLog> ApplicationLogs { get; set; }
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

            #region Role_Permission_Mapping
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.ToTable("RolePermissions");
                entity.HasKey(rp => rp.Id);

                entity.HasIndex(rp => new { rp.RoleId, rp.PermissionId }).IsUnique();

                entity.HasOne(rp => rp.Role)
                      .WithMany()
                      .HasForeignKey(rp => rp.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                      .WithMany(p => p.RolePermissions)
                      .HasForeignKey(rp => rp.PermissionId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.ToTable("Permissions");
                entity.HasKey(p => p.Id);
                entity.HasIndex(p => new { p.Controller, p.Action, p.HttpMethod }).IsUnique();
            });
            #endregion

            #region Logging
            modelBuilder.Entity<ApplicationLog>(entity =>
            {
                entity.ToTable("ApplicationLogs");
                entity.HasKey(l => l.Id);
                entity.Property(l => l.Level).HasMaxLength(50);
                entity.Property(l => l.CorrelationId).HasMaxLength(100);
                entity.Property(l => l.UserId).HasMaxLength(450);
                entity.Property(l => l.UserName).HasMaxLength(256);
                entity.Property(l => l.ClientIp).HasMaxLength(50);
                entity.Property(l => l.RequestPath).HasMaxLength(500);
                entity.Property(l => l.HttpMethod).HasMaxLength(10);
                entity.Property(l => l.MachineName).HasMaxLength(100);
                entity.Property(l => l.Environment).HasMaxLength(50);

                // ← index for common query patterns in fintech log analysis
                entity.HasIndex(l => l.TimeStamp);
                entity.HasIndex(l => l.Level);
                entity.HasIndex(l => l.CorrelationId);
                entity.HasIndex(l => l.UserId);
            });
            #endregion

            modelBuilder.Entity<DropdownDto>().HasNoKey().ToView("view_name_that_doesnt_exist");

        }
    }
}