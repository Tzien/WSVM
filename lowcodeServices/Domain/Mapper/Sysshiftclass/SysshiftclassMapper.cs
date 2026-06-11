using CeriOS.示例.Entitys.Dto.Sysshiftclass;
using Mapster;
 
namespace CeriOS.示例.Entitys.Mapper.Sysshiftclass;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<SysshiftclassCrInput, SysshiftclassEntity>()
			.Map(dest => dest.ModuleName, src => src.ModuleName != null ? src.ModuleName : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.IsDeleted, src => src.IsDeleted)
		;
		config.ForType<SysshiftclassEntity, SysshiftclassInfoOutput>()
			.Map(dest => dest.ModuleName, src => src.ModuleName != null ? src.ModuleName : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.IsDeleted, src => src.IsDeleted != null ? src.IsDeleted : null)
		;
	}
}
