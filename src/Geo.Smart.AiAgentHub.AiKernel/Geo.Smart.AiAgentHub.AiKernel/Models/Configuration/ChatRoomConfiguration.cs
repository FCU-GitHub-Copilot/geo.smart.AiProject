using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Geo.Smart.AiAgentHub.AiKernel.Models.Configuration;

public class ChatRoomConfiguration : IEntityTypeConfiguration<ChatRoom>
{
    public void Configure(EntityTypeBuilder<ChatRoom> builder)
    {
        builder.ToTable("ChatRoom", "dbo");
        builder.HasKey(x => x.RoomId).HasName("PK_ChatRoom").IsClustered();

        builder.Property(x => x.RoomId).HasColumnName(@"RoomId").HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
        builder.Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar(100)").IsRequired().HasMaxLength(100);
        builder.Property(x => x.History).HasColumnName(@"History").HasColumnType("text(2147483647)").IsRequired().IsUnicode(false).HasMaxLength(2147483647);
        builder.Property(x => x.CreatedDate).HasColumnName(@"CreatedDate").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName(@"CreatedBy").HasColumnType("nvarchar(128)").IsRequired().HasMaxLength(128);
        builder.Property(x => x.UpdatedDate).HasColumnName(@"UpdatedDate").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.UpdatedBy).HasColumnName(@"UpdatedBy").HasColumnType("nvarchar(128)").IsRequired().HasMaxLength(128);
        builder.Property(x => x.IsEnabled).HasColumnName(@"IsEnabled").HasColumnType("bit").IsRequired();
        builder.Property(x => x.LlmServiceId).HasColumnName(@"LlmServiceId").HasColumnType("nvarchar(100)").IsRequired().HasMaxLength(100);
        builder.Property(x => x.ToolSelected).HasColumnName(@"ToolSelected").HasColumnType("nvarchar(max)").IsRequired();
    }
}