using CeriOS.示例.Entitys.Dto.Sysinterface;
using Mapster;
 
namespace CeriOS.示例.Entitys.Mapper.Sysinterface;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<SysinterfaceCrInput, SysinterfaceEntity>()
			.Map(dest => dest.SysInfoID, src => src.SysInfoID != null ? src.SysInfoID : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.IsActive, src => src.IsActive)
		;
		config.ForType<SysinterfaceEntity, SysinterfaceInfoOutput>()
			.Map(dest => dest.SysInfoID, src => src.SysInfoID != null ? src.SysInfoID : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.IsActive, src => src.IsActive != null ? src.IsActive : null)
		;
	}
}
