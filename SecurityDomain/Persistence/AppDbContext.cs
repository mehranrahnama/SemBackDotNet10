
using Microsoft.EntityFrameworkCore;
using SemBackDotNet10.SecurityDomain.Model.Entity;
using SecurityUser = SemBackDotNet10.SecurityDomain.Model.Entity.User;

namespace SemBackDotNet10.SecurityDomain.Persistence
{



    public sealed class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AuthProviderEntity> AuthProviders => Set<AuthProviderEntity>();
        public DbSet<SecurityUser> Users => Set<SecurityUser>();
        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<RevokedToken> RevokedTokens => Set<RevokedToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuthProviderEntity>(static entity =>
            {
                entity.ToTable("tbl_auth_provider", "dbo");

                entity.HasKey(static x => x.Id);

                entity.Property(static x => x.Id)
                    .HasColumnName("id").HasConversion<int>();

                entity.Property(static x => x.TenantId)
                    .HasColumnName("tenantid").HasConversion<int>();

                entity.Property(static x => x.Type)
                    .HasColumnName("type")
                    .HasMaxLength(100);

                entity.Property(static x => x.LdapUrl)
                    .HasColumnName("ldap_url");

                entity.Property(static x => x.LdapBaseDn)
                    .HasColumnName("ldap_base_dn");

                entity.Property(static x => x.LdapManagerDn)
                    .HasColumnName("ldap_manager_dn");

                entity.Property(static x => x.LdapManagerPassword)
                    .HasColumnName("ldap_manager_password");

                entity.Property(static x => x.SearchFilter)
                    .HasColumnName("search_filter");

                entity.Property(static x => x.IsSahaBased)
                    .HasColumnName("is_saha_based");

                entity.Property(static x => x.IssuerUri).HasColumnName("issueruri");
                entity.Property(static x => x.TokenUri).HasColumnName("tokenuri");
                entity.Property(static x => x.AuthorizationUri).HasColumnName("authorizationuri");
                entity.Property(static x => x.JwkSetUri).HasColumnName("jwkseturi");
                entity.Property(static x => x.UserInfoUri).HasColumnName("userinfouri");
                entity.Property(static x => x.ClientId).HasColumnName("clientid");
                entity.Property(static x => x.ClientSecret).HasColumnName("clientsecret");
                entity.Property(static x => x.RedirectUri).HasColumnName("redirecturi");
                entity.Property(static x => x.AuthorizationGrantType).HasColumnName("authorizationgranttype");
                entity.Property(static x => x.ClientAuthenticationMethod).HasColumnName("clientauthenticationmethod");
                entity.Property(static x => x.Scope).HasColumnName("scope");
                entity.Property(static x => x.RegistrationId).HasColumnName("registration_id");
            });

            modelBuilder.Entity<SecurityUser>(entity =>
            {
                entity.ToTable("tbluser", "dbo");
                entity.HasKey(x => x.UserId);
                entity.Property(x => x.UserId).HasColumnName("userid").HasConversion<int>();
                entity.Property(x => x.UserName).HasColumnName("username");
                entity.Property(x => x.TenantId).HasColumnName("tenantid").HasConversion<int?>();
                entity.Property(x => x.Retired).HasColumnName("retired");
                entity.Property(x => x.LoginDisabled).HasColumnName("LoginDisabled");
                // The deployed Java databases do not have the optional User.Type column.
                entity.Ignore(x => x.Type);
                entity.HasMany(x => x.Accounts).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.Property(x => x.Id).HasConversion<int>();
                entity.Property(x => x.UserId).HasConversion<int?>();
                entity.Property(x => x.ProviderId).HasConversion<int?>();
                entity.Property(x => x.AccountType).HasConversion<short?>();
                entity.Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
                entity.Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
                entity.HasOne(x => x.Provider).WithMany().HasForeignKey(x => x.ProviderId);
            });

            modelBuilder.Entity<LegacyRefreshSession>(entity =>
            {
                entity.Property(x => x.Id).HasConversion<int>().ValueGeneratedOnAdd();
                entity.Property(x => x.AccountId).HasConversion<int>();
                entity.Property(x => x.ExpiresAt).HasConversion(
                    x => x.UtcDateTime,
                    x => new DateTimeOffset(DateTime.SpecifyKind(x, DateTimeKind.Utc)));
            });

            modelBuilder.Entity<BlackListToken>(entity =>
            {
                entity.Property(x => x.Id).HasConversion<int>();
                entity.Property(x => x.AccountId).HasConversion<int>();
                entity.HasOne(x => x.Account).WithMany().HasForeignKey(x => x.AccountId)
                    .OnDelete(DeleteBehavior.NoAction).HasConstraintName("fk_account_blacklist");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(x => x.Id).HasConversion<int>();
                entity.Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
                entity.Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.Property(x => x.Id).HasConversion<int>();
                entity.Property(x => x.UserId).HasConversion<int>();
                entity.Property(x => x.RoleId).HasConversion<int>();
                entity.Property(x => x.CreatedById).HasColumnName("createdBy").HasConversion<int?>();
                entity.Property(x => x.ModifiedById).HasColumnName("modifiedBy").HasConversion<int?>();
                entity.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId);
                entity.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            });
        }
    }
}
 
