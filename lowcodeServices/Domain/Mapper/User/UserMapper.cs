using CeriOS.示例.Entitys.Dto.User;
using Mapster;
 
namespace CeriOS.示例.Entitys.Mapper.User;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<UserCrInput, UserEntity>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Address, src => src.Address != null ? src.Address : null)
		;
		config.ForType<UserEntity, UserInfoOutput>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Address, src => src.Address != null ? src.Address : null)
		;
	}
}
