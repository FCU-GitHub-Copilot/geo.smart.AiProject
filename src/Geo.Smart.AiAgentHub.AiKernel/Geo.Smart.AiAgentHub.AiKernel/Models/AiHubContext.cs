using Geo.Smart.AiAgentHub.AiKernel.Models.Configuration;
using Geo.Smart.CommonCore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Geo.Smart.AiAgentHub.AiKernel.Models;

/// <summary>
/// Main DbContext
/// </summary>
public partial class AiHubContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 建構式
    /// </summary>
    public AiHubContext(DbContextOptions<AiHubContext> options,
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
        InitializePartial();
    }

    /// <summary>
    /// 建構式
    /// </summary>
    public AiHubContext(DbContextOptions<AiHubContext> options)
        : base(options)
    {
        InitializePartial();
    }

    public DbSet<ChatCompletionLog> ChatCompletionLogs { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ChatCompletionLogConfiguration());
        modelBuilder.ApplyConfiguration(new ChatMessageConfiguration());
        modelBuilder.ApplyConfiguration(new ChatRoomConfiguration());
    }

    partial void InitializePartial();

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
}