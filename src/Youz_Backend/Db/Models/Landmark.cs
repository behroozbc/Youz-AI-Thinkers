using NetTopologySuite.Geometries;
using Pgvector;
namespace Youz_Backend.DB.Models;

public class Landmark : BaseEntity
{
    public string Name { get; set; }
    public Point Location { get; set; }
    public string Description { get; set; }
    public ICollection<DescriptionChunk> DescriptionChunks { get; set; }
    public ICollection<ImageChunk> ImageChunks { get; set; }
}
public class ImageChunk : BaseEntity
{
    public byte[] Image { get; set; }
    public Vector ImageEmbedding { get; set; }
    public string Caption { get; set; }
    public Guid LandmarkID { get; set; }
    public Landmark Landmark { get; set; }
}
public class DescriptionChunk : BaseEntity
{
    public string Text { get; set; }
    public Vector TextEmbedding { get; set; }
    public Guid LandmarkID { get; set; }
    public Landmark Landmark { get; set; }
}
public class BaseEntity
{
    public Guid ID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}