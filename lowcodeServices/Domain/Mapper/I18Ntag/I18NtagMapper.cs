using CeriOS.示例.Entitys.Dto.I18Ntag;
using Mapster;
 
namespace CeriOS.示例.Entitys.Mapper.I18Ntag;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<I18NtagCrInput, I18NtagEntity>()
			.Map(dest => dest.TagName, src => src.TagName != null ? src.TagName : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
			.Map(dest => dest.CreateTime, src => src.CreateTime != null ? src.CreateTime : null)
		;
		config.ForType<I18NtagEntity, I18NtagInfoOutput>()
			.Map(dest => dest.TagName, src => src.TagName != null ? src.TagName : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
			.Map(dest => dest.CreateTime, src => src.CreateTime != null ? src.CreateTime : null)
		;
	}
}
