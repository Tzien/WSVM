﻿﻿
using JNPF.Common.Models;
using CeriOS.示例.Entitys.Dto.Testallfunction;
using Mapster;
using System.Collections.Generic;
using System.Text.Json;

namespace CeriOS.示例.Entitys.Mapper.Testallfunction;

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
		config.ForType<TestallfunctionCrInput, TestallfunctionEntity>()
			.Map(dest => dest.FileURL, src => ToJson(src.FileURL))
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.PictureURL, src => ToJson(src.PictureURL))
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
		;
		config.ForType<TestallfunctionEntity, TestallfunctionInfoOutput>()
			.Map(dest => dest.FileURL, src => ParseList<FileControlsModel>(src.FileURL))
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.PictureURL, src => ParseList<FileControlsModel>(src.PictureURL))
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
		;
		config.ForType<TestallfunctionEntity, TestallfunctionListOutput>()
			.Map(dest => dest.FileURL, src => ParseList<FileControlsModel>(src.FileURL))
			.Map(dest => dest.id, src => src.id)
			.Map(dest => dest.Name, src => src.Name != null ? src.Name : null)
			.Map(dest => dest.PictureURL, src => ParseList<FileControlsModel>(src.PictureURL))
			.Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
			.Map(dest => dest.Sort, src => src.Sort != null ? src.Sort : null)
		;
	}
}
