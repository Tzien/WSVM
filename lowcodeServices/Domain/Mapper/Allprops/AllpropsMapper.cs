using CeriOS.示例.Entitys.Dto.Allprops;
using JNPF.Common.Security;
using Mapster;
using System.Collections.Generic;
using System.Text.Json;

namespace CeriOS.示例.Entitys.Mapper.Allprops;

public class Mapper : IRegister
{
	private static string SerializeList<T>(ICollection<T> value)
	{
		return value != null && value.Count > 0 ? JsonSerializer.Serialize(value) : null;
	}

	private static List<T> DeserializeList<T>(string value)
	{
		if (string.IsNullOrWhiteSpace(value)) return null;
		try
		{
			return JsonSerializer.Deserialize<List<T>>(value);
		}
		catch
		{
			return null;
		}
	}

	public void Register(TypeAdapterConfig config)
	{
		config.ForType<AllpropsCrInput, AllpropsEntity>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Text, src => SerializeList(src.Text))
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
			.Map(dest => dest.Text, src => DeserializeList<string>(src.Text))
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
