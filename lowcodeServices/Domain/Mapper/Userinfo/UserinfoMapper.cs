using CeriOS.示例.Entitys.Dto.Userinfo;
using Mapster;
 
namespace CeriOS.示例.Entitys.Mapper.Userinfo;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<UserinfoCrInput, UserinfoEntity>()
			.Map(dest => dest.Enabled, src => src.Enabled != null ? src.Enabled : null)
			.Map(dest => dest.RealName, src => src.RealName != null ? src.RealName : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Sex, src => src.Sex != null ? src.Sex : null)
		;
		config.ForType<UserinfoEntity, UserinfoInfoOutput>()
			.Map(dest => dest.Enabled, src => src.Enabled != null ? src.Enabled : null)
			.Map(dest => dest.RealName, src => src.RealName != null ? src.RealName : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Sex, src => src.Sex != null ? src.Sex : null)
		;
	}
}
