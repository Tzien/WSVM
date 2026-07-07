﻿
using CeriOS.示例.Entitys.Dto.Allprops;
using Mapster;
using System.Collections.Generic;
using System.Text.Json;

namespace CeriOS.示例.Entitys.Mapper.Allprops;

public class Mapper : IRegister
{
        private static string? ToJson<T>(IList<T> list)
        {
                if (list == null || list.Count == 0) return null;
                return JsonSerializer.Serialize(list).Replace("\r\n", "").Replace(" ", "");
        }

        private static List<string> ParseStringList(string? value)
        {
                if (string.IsNullOrWhiteSpace(value)) return new List<string>();
                try
                {
                        return JsonSerializer.Deserialize<List<string>>(value) ?? new List<string>();
                }
                catch
                {
                        return new List<string>();
                }
        }

        private static List<List<string>> ParseNestedStringList(string? value)
        {
                if (string.IsNullOrWhiteSpace(value)) return new List<List<string>>();
                try
                {
                        return JsonSerializer.Deserialize<List<List<string>>>(value) ?? new List<List<string>>();
                }
                catch
                {
                        return new List<List<string>>();
                }
        }

        private static List<T> ParseList<T>(string? value)
        {
                if (string.IsNullOrWhiteSpace(value)) return new List<T>();
                try
                {
                        return JsonSerializer.Deserialize<List<T>>(value) ?? new List<T>();
                }
                catch
                {
                        return new List<T>();
                }
        }

	public void Register(TypeAdapterConfig config)
	{
		config.ForType<AllpropsCrInput, AllpropsEntity>()
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
			.Map(dest => dest.Text, src => ToJson(src.Text))
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
			.Map(dest => dest.Text, src => ParseStringList(src.Text))
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
		config.ForType<AllpropsEntity, AllpropsListOutput>()
			.Map(dest => dest.id, src => src.id)
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort.ToString() : null)
			.Map(dest => dest.Text, src => src.Text != null ? src.Text : null)
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.CreateTime, src => string.Format("{0:yyyy-MM-dd HH:mm:ss}", src.CreateTime))
			.Map(dest => dest.IsDeleted, src => src.IsDeleted != null ? src.IsDeleted : null)
			.Map(dest => dest.Enabled, src => src.Enabled != null ? src.Enabled.ToString() : null)
			.Map(dest => dest.LastLoginTime, src => src.LastLoginTime != null ? src.LastLoginTime : null)
			.Map(dest => dest.Enmu, src => src.Enmu != null ? src.Enmu.ToString() : null)
			.Map(dest => dest.HK, src => src.HK != null ? src.HK.ToString() : null)
			.Map(dest => dest.PF, src => src.PF != null ? src.PF.ToString() : null)
		;
	}
}
