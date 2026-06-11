using CeriOS.示例.Entitys.Dto.Mediainfo;
using Mapster;
 
namespace CeriOS.示例.Entitys.Mapper.Mediainfo;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<MediainfoCrInput, MediainfoEntity>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.IsDeleted, src => src.IsDeleted)
		;
		config.ForType<MediainfoEntity, MediainfoInfoOutput>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.IsDeleted, src => src.IsDeleted != null ? src.IsDeleted : null)
		;
	}
}
