﻿﻿
using CeriOS.示例.Entitys.Dto.I18Ntag;
using Mapster;
using System.Collections.Generic;
using System.Text.Json;

namespace CeriOS.示例.Entitys.Mapper.I18Ntag;

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
		config.ForType<I18NtagCrInput, I18NtagEntity>()
			.Map(dest => dest.TagName, src => src.TagName != null ? src.TagName : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
		;
		config.ForType<I18NtagEntity, I18NtagInfoOutput>()
			.Map(dest => dest.TagName, src => src.TagName != null ? src.TagName : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
		;
		config.ForType<I18NtagListOutput, I18NtagInlineEditorOutput>()
			.Map(dest => dest.TagName, src => src.TagName != null ? src.TagName : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
		;
		config.ForType<I18NtagEntity, I18NtagListOutput>()
			.Map(dest => dest.id, src => src.id)
			.Map(dest => dest.TagName, src => src.TagName != null ? src.TagName : null)
			.Map(dest => dest.Code, src => src.Code != null ? src.Code : null)
		;
	}
}
