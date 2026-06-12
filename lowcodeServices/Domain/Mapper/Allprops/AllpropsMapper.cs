using CeriOS.示例.Entitys.Dto.Allprops;
using JNPF.Common.Security;
using Mapster;
using System.Collections.Generic;

namespace CeriOS.示例.Entitys.Mapper.Allprops;

public class Mapper : IRegister
{
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<AllpropsCrInput, AllpropsEntity>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Text, src => src.Text != null && src.Text.Count > 0 ? src.Text.ToJsonString().Replace("\r\n", "").Replace(" ", "") : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.CreateTime, src => src.CreateTime != null ? src.CreateTime : null)
			.Map(dest => dest.IsDeleted, src => src.IsDeleted != null ? src.IsDeleted : null)
			.Map(dest => dest.Enabled, src => src.Enabled)
			.Map(dest => dest.LastLoginTime, src => src.LastLoginTime != null ? src.LastLoginTime : null)
			.Map(dest => dest.Enmu, src => src.Enmu != null ? src.Enmu : null)
			.Map(dest => dest.HK, src => src.HK != null ? src.HK : null)
			.Map(dest => dest.Color, src => src.Color != null ? src.Color : null)
			.Map(dest => dest.FWB, src => src.FWB != null ? src.FWB : null)
			.Map(dest => dest.PF, src => src.PF != null ? src.PF : null)
		;
		config.ForType<AllpropsEntity, AllpropsInfoOutput>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Text, src => src.Text != null ? src.Text.ToObject<List<string>>() : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.CreateTime, src => src.CreateTime != null ? src.CreateTime : null)
			.Map(dest => dest.IsDeleted, src => src.IsDeleted != null ? src.IsDeleted : null)
			.Map(dest => dest.Enabled, src => src.Enabled != null ? src.Enabled : null)
			.Map(dest => dest.LastLoginTime, src => src.LastLoginTime != null ? src.LastLoginTime : null)
			.Map(dest => dest.Enmu, src => src.Enmu != null ? src.Enmu : null)
			.Map(dest => dest.HK, src => src.HK != null ? src.HK : null)
			.Map(dest => dest.Color, src => src.Color != null ? src.Color : null)
			.Map(dest => dest.FWB, src => src.FWB != null ? src.FWB : null)
			.Map(dest => dest.PF, src => src.PF != null ? src.PF : null)
		;
	}
}
