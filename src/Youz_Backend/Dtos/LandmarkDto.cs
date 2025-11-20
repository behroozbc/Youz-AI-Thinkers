using NetTopologySuite.Geometries;
namespace Youz_Backend.Dtos;
public class LandmarkDto
{
    public required string Name { get; set; }
    public required PointDto Location { get; set; }
    public required string Description { get; set; }
}