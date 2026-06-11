using CeriOS.示例.Entitys.Dto.Sysoraganize;
using Mapster;
 
namespace CeriOS.示例.Entitys.Mapper.Sysoraganize;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<SysoraganizeCrInput, SysoraganizeEntity>()
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Status, src => src.Status)
		;
		config.ForType<SysoraganizeEntity, SysoraganizeInfoOutput>()
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Status, src => src.Status != null ? src.Status : null)
		;
	}
}
