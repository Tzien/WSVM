using CeriOS.示例.Entitys.Dto.ControlBindDemo;
using JNPF.Common.Models;
using JNPF.Common.Security;
using Mapster;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace CeriOS.示例.Entitys.Mapper.ControlBindDemo;

public class Mapper : IRegister
{
    private static string? ToJson<T>(IList<T> list)
    {
        if (list == null || list.Count == 0) return null;
        var json = JsonSerializer.Serialize(list);
        return json?.Replace("\r\n", "").Replace(" ", "");
    }
    private static List<string> ParseStringList(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return new List<string>();
        try { return JsonSerializer.Deserialize<List<string>>(s) ?? new List<string>(); } catch { return new List<string>(); }
    }
    private static List<FileControlsModel> ParseFiles(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return new List<FileControlsModel>();
        try { return JsonSerializer.Deserialize<List<FileControlsModel>>(s) ?? new List<FileControlsModel>(); } catch { return new List<FileControlsModel>(); }
    }
	public void Register(TypeAdapterConfig config)
	{
		config.ForType<ControlBindDemoCrInput, ControlBindDemoEntity>()
			.Map(dest => dest.cascade_text, src => ToJson(src.cascade_text))
			.Map(dest => dest.date_selected, src => src.date_selected != null ? src.date_selected : null)
			.Map(dest => dest.datetime_selected, src => src.datetime_selected != null ? src.datetime_selected : null)
			.Map(dest => dest.file_url, src => ToJson(src.file_url))
			.Map(dest => dest.images, src => ToJson(src.images))
			.Map(dest => dest.color_value, src => src.color_value != null ? src.color_value : null)
			.Map(dest => dest.rating_value, src => src.rating_value != null ? src.rating_value : null)
			.Map(dest => dest.slider_int_value, src => src.slider_int_value != null ? src.slider_int_value : null)
			.Map(dest => dest.rich_text_content, src => src.rich_text_content != null ? src.rich_text_content : null)
		;
		config.ForType<ControlBindDemoUpInput, ControlBindDemoEntity>()
			.Map(dest => dest.id, src => src.id)
			.Map(dest => dest.cascade_text, src => ToJson(src.cascade_text))
			.Map(dest => dest.date_selected, src => src.date_selected != null ? src.date_selected : null)
			.Map(dest => dest.datetime_selected, src => src.datetime_selected != null ? src.datetime_selected : null)
			.Map(dest => dest.file_url, src => ToJson(src.file_url))
			.Map(dest => dest.images, src => ToJson(src.images))
			.Map(dest => dest.color_value, src => src.color_value != null ? src.color_value : null)
			.Map(dest => dest.rating_value, src => src.rating_value != null ? src.rating_value : null)
			.Map(dest => dest.slider_int_value, src => src.slider_int_value != null ? src.slider_int_value : null)
			.Map(dest => dest.rich_text_content, src => src.rich_text_content != null ? src.rich_text_content : null)
		;
		config.ForType<ControlBindDemoEntity, ControlBindDemoInfoOutput>()
			.Map(dest => dest.cascade_text, src => ParseStringList(src.cascade_text))
			.Map(dest => dest.date_selected, src => src.date_selected != null ? src.date_selected : null)
			.Map(dest => dest.datetime_selected, src => src.datetime_selected != null ? src.datetime_selected : null)
			.Map(dest => dest.file_url, src => ParseFiles(src.file_url))
			.Map(dest => dest.images, src => ParseFiles(src.images))
			.Map(dest => dest.color_value, src => src.color_value != null ? src.color_value : null)
			.Map(dest => dest.rating_value, src => src.rating_value != null ? src.rating_value : null)
			.Map(dest => dest.slider_int_value, src => src.slider_int_value != null ? src.slider_int_value : null)
			.Map(dest => dest.rich_text_content, src => src.rich_text_content != null ? src.rich_text_content : null)
		;

		// 列表输出：格式化日期，保证字符串字段不出现类型名
		config.ForType<ControlBindDemoEntity, ControlBindDemoListOutput>()
			.Map(dest => dest.cascade_text, src => src.cascade_text)
			.Map(dest => dest.date_selected, src => src.date_selected.HasValue ? src.date_selected.Value.ToString("yyyy-MM-dd") : null)
			.Map(dest => dest.datetime_selected, src => src.datetime_selected)
			.Map(dest => dest.file_url, src => src.file_url)
			.Map(dest => dest.images, src => src.images)
			.Map(dest => dest.color_value, src => src.color_value ?? string.Empty)
			.Map(dest => dest.rich_text_content, src => src.rich_text_content)
		;
	}
}
