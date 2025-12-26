using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Geo.Smart.AiAgentHub.AiKernel.Models.Configuration;

public class ChatCompletionLogConfiguration : IEntityTypeConfiguration<ChatCompletionLog>
{
    public void Configure(EntityTypeBuilder<ChatCompletionLog> builder)
    {
        builder.ToTable("ChatCompletionLog", "dbo");
        builder.HasKey(x => x.LogSeq).HasName("PK_ChatCompletionLog").IsClustered();

        builder.Property(x => x.LogSeq).HasColumnName(@"LogSeq").HasColumnType("int").IsRequired().ValueGeneratedOnAdd().UseIdentityColumn();
        builder.Property(x => x.LogId).HasColumnName(@"LogId").HasColumnType("varchar(50)").IsRequired(false).IsUnicode(false).HasMaxLength(50);
        builder.Property(x => x.Metadata).HasColumnName(@"Metadata").HasColumnType("text(2147483647)").IsRequired(false).IsUnicode(false).HasMaxLength(2147483647);
        builder.Property(x => x.CreatedDate).HasColumnName(@"CreatedDate").HasColumnType("datetimeoffset").IsRequired();
        builder.Property(x => x.PromptToken).HasColumnName(@"PromptToken").HasColumnType("bigint").IsRequired(false);
        builder.Property(x => x.CompletionToken).HasColumnName(@"CompletionToken").HasColumnType("bigint").IsRequired(false);
        builder.Property(x => x.TotalToken).HasColumnName(@"TotalToken").HasColumnType("bigint").IsRequired(false);
    }
}