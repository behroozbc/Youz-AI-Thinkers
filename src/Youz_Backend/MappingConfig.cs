using Mapster;
using NetTopologySuite.Geometries;
using Youz_Backend.Dtos;

public class MappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Point, PointDto>()
            .Map(dto => dto.Latitude, src => src.Y)
            .Map(dto => dto.Longitude, src => src.X);
        config.NewConfig<PointDto, Point>().MapWith(dto => new Point(new(dto.Longitude, dto.Latitude)));
        
    }
}