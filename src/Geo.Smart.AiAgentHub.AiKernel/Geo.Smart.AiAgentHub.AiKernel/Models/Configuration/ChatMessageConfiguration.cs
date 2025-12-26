using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Geo.Smart.AiAgentHub.AiKernel.Models.Configuration;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.ToTable("ChatMessage", "dbo");
        builder.HasKey(x => x.MessageId).HasName("PK_ChatMessage").IsClustered();

        builder.Property(x => x.MessageId).HasColumnName(@"MessageId").HasColumnType("uniqueidentifier").IsRequired().ValueGeneratedNever();
        builder.Property(x => x.RoomId).HasColumnName(@"RoomId").HasColumnType("uniqueidentifier").IsRequired();
        builder.Property(x => x.Role).HasColumnName(@"Role").HasColumnType("nvarchar(20)").IsRequired().HasMaxLength(20);
        builder.Property(x => x.Content).HasColumnName(@"Content").HasColumnType("nvarchar(max)").IsRequired();
        builder.Property(x => x.SentAt).HasColumnName(@"SentAt").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.LlmServiceId).HasColumnName(@"LlmServiceId").HasColumnType("nvarchar(100)").IsRequired().HasMaxLength(100);
        builder.Property(x => x.LogId).HasColumnName(@"LogId").HasColumnType("varchar(50)").IsRequired(false).IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.Tokens).HasColumnName(@"Tokens").HasColumnType("bigint").IsRequired(false);
        builder.Property(x => x.CreatedDate).HasColumnName(@"CreatedDate").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.CreatedBy).HasColumnName(@"CreatedBy").HasColumnType("nvarchar(128)").IsRequired().HasMaxLength(128);
        builder.Property(x => x.UpdatedDate).HasColumnName(@"UpdatedDate").HasColumnType("datetime").IsRequired();
        builder.Property(x => x.UpdatedBy).HasColumnName(@"UpdatedBy").HasColumnType("nvarchar(128)").IsRequired().HasMaxLength(128);
        builder.Property(x => x.IsEnabled).HasColumnName(@"IsEnabled").HasColumnType("bit").IsRequired();
        builder.Property(x => x.ToolSelected).HasColumnName(@"ToolSelected").HasColumnType("nvarchar(max)").IsRequired(false);

        // Foreign keys
        builder.HasOne(a => a.ChatRoom).WithMany(b => b.ChatMessages).HasForeignKey(c => c.RoomId).OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK_ChatMessage_ChatRoom");
    }
}