namespace Geo.Smart.AiAgentHub.DataAccess;

/// <summary>
/// GdbContext 的 partial class，擴充自定義功能
/// </summary>
public partial class GdbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string,
    ApplicationUserClaim, IdentityUserRole<string>, ApplicationUserLogin, ApplicationRoleClaim,
    ApplicationUserToken>
{
    /// <summary>
    /// 複寫 SaveChanges
    /// https://dev.to/rickystam/ef-core-how-to-implement-basic-auditing-on-your-entities-1mbm
    /// </summary>
    /// <returns> </returns>
    public override int SaveChanges()
    {
        SetAuditableValues();
        return base.SaveChanges();
    }

    /// <summary>
    /// 複寫 SaveChanges， 非同步
    /// </summary>
    /// <param name="cancellationToken"> </param>
    /// <returns> </returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetAuditableValues();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// 設定 Auditable 物件的 IsEnabled、Created、Updated 的預設值
    /// </summary>
    private void SetAuditableValues()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is AuditableEntity
                && (e.State == EntityState.Added || e.State == EntityState.Modified)
            );
        foreach (var entry in entries)
        {
            if (entry.Entity is not AuditableEntity entity)
            {
                continue;
            }
            var now = DateTime.Now;
            var loginUser = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? "unknown";
            if (entry.State == EntityState.Added)
            {
                entity.CreatedDate = now;
                entity.CreatedBy = string.IsNullOrWhiteSpace(entity.CreatedBy) ? loginUser : entity.CreatedBy;
                entity.IsEnabled = true;
            }
            else
            {
                Entry(entity).Property(p => p.CreatedDate).IsModified = false;
                Entry(entity).Property(p => p.CreatedBy).IsModified = false;
            }
            entity.UpdatedDate = now;
            entity.UpdatedBy = string.IsNullOrWhiteSpace(entity.UpdatedBy) ? loginUser : entity.UpdatedBy;
        }
    }

    /// <summary>
    /// 當有 [純量值函式] 時，需要擴充 OnModelCreating 來設定 DbFunction
    /// </summary>
    /// <param name="modelBuilder"></param>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
#pragma warning disable CS8604 // 可能有 Null 參考引數。

        //建立 [純量值函式]
        modelBuilder.HasDbFunction(
            typeof(GdbContext).GetMethod(nameof(FnUserFullName),
            [typeof(string)])
        )
        .HasName("FnUserFullName");
#pragma warning restore CS8604 // 可能有 Null 參考引數。
    }

    /// <summary>
    /// 取得使用者完整姓名
    /// </summary>
    /// <param name="loginId"></param>
    /// <returns></returns>
    public string FnUserFullName(string loginId)
    {
        if (string.IsNullOrWhiteSpace(loginId))
        {
            return string.Empty;
        }

        var user = ApplicationUsers
            .AsNoTracking()
            .FirstOrDefault(u => u.Id == loginId);

        return user?.FullName ?? loginId;
    }
}