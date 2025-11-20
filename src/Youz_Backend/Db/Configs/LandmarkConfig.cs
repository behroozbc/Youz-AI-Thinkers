using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Youz_Backend.DB.Models;
namespace Youz_Backend.DB.Config;

public class DescriptionChunkConfig : IEntityTypeConfiguration<DescriptionChunk>
{
    public void Configure(EntityTypeBuilder<DescriptionChunk> builder)
    {
        builder.Property(c => c.TextEmbedding).HasColumnType("vector(3072)");
    }
}
public class LandmarkConfig : IEntityTypeConfiguration<Landmark>
{
    public void Configure(EntityTypeBuilder<Landmark> builder)
    {
    }
}
public class ImageChunkConfig : IEntityTypeConfiguration<ImageChunk>
{
    public void Configure(EntityTypeBuilder<ImageChunk> builder)
    {
        builder.Property(c => c.ImageEmbedding).HasColumnType("vector(3072)");
    }
}