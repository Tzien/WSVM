﻿﻿
using CeriOS.示例.Entitys.Dto.Allprops2;
using Mapster;
using System.Collections.Generic;
using System.Text.Json;

namespace CeriOS.示例.Entitys.Mapper.Allprops2;

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
		config.ForType<Allprops2CrInput, Allprops2Entity>()
			.Map(dest => dest.DanHang, src => src.DanHang != null ? src.DanHang : null)
			.Map(dest => dest.DanXuan, src => src.DanXuan != null ? src.DanXuan : null)
			.Map(dest => dest.DuoHang, src => src.DuoHang != null ? src.DuoHang : null)
			.Map(dest => dest.DuoXuan, src => ToJson(src.DuoXuan))
			.Map(dest => dest.FWB, src => src.FWB != null ? src.FWB : null)
			.Map(dest => dest.HK, src => src.HK != null ? src.HK : null)
			.Map(dest => dest.JiLian, src => ToJson(src.JiLian))
			.Map(dest => dest.KaiGuan, src => src.KaiGuan)
			.Map(dest => dest.PF, src => src.PF != null ? src.PF : null)
			.Map(dest => dest.RiQi, src => src.RiQi != null ? (System.DateTime?)System.DateTimeOffset.FromUnixTimeMilliseconds(src.RiQi.Value).LocalDateTime : null)
			.Map(dest => dest.ShiJian, src => src.ShiJian != null ? src.ShiJian : null)
			.Map(dest => dest.ShuZi, src => src.ShuZi != null ? src.ShuZi : null)
			.Map(dest => dest.XiaLa, src => src.XiaLa != null ? src.XiaLa : null)
			.Map(dest => dest.YanSe, src => src.YanSe != null ? src.YanSe : null)
		;
		config.ForType<Allprops2Entity, Allprops2InfoOutput>()
			.Map(dest => dest.DanHang, src => src.DanHang != null ? src.DanHang : null)
			.Map(dest => dest.DanXuan, src => src.DanXuan != null ? src.DanXuan : null)
			.Map(dest => dest.DuoHang, src => src.DuoHang != null ? src.DuoHang : null)
			.Map(dest => dest.DuoXuan, src => ParseStringList(src.DuoXuan))
			.Map(dest => dest.FWB, src => src.FWB != null ? src.FWB : null)
			.Map(dest => dest.HK, src => src.HK != null ? src.HK : null)
			.Map(dest => dest.JiLian, src => ParseStringList(src.JiLian))
			.Map(dest => dest.KaiGuan, src => src.KaiGuan != null ? src.KaiGuan : null)
			.Map(dest => dest.PF, src => src.PF != null ? src.PF : null)
			.Map(dest => dest.RiQi, src => src.RiQi != null ? src.RiQi : null)
			.Map(dest => dest.ShiJian, src => src.ShiJian != null ? src.ShiJian : null)
			.Map(dest => dest.ShuZi, src => src.ShuZi != null ? src.ShuZi : null)
			.Map(dest => dest.XiaLa, src => src.XiaLa != null ? src.XiaLa : null)
			.Map(dest => dest.YanSe, src => src.YanSe != null ? src.YanSe : null)
		;
		config.ForType<Allprops2ListOutput, Allprops2InlineEditorOutput>()
			.Map(dest => dest.DanHang, src => src.DanHang != null ? src.DanHang : null)
			.Map(dest => dest.DanXuan, src => src.DanXuan != null ? src.DanXuan : null)
			.Map(dest => dest.DuoHang, src => src.DuoHang != null ? src.DuoHang : null)
			.Map(dest => dest.DuoXuan, src => ParseStringList(src.DuoXuan))
			.Map(dest => dest.HK, src => src.HK != null ? src.HK : null)
			.Map(dest => dest.JiLian, src => ParseStringList(src.JiLian))
			.Map(dest => dest.KaiGuan, src => src.KaiGuan)
			.Map(dest => dest.PF, src => src.PF != null ? src.PF : null)
			.Map(dest => dest.RiQi, src => src.RiQi != null ? (long?)new System.DateTimeOffset(src.RiQi.Value).ToUnixTimeMilliseconds() : null)
			.Map(dest => dest.ShiJian, src => src.ShiJian != null ? src.ShiJian : null)
			.Map(dest => dest.ShuZi, src => src.ShuZi != null ? src.ShuZi : null)
			.Map(dest => dest.XiaLa, src => src.XiaLa != null ? src.XiaLa : null)
		;
		config.ForType<Allprops2Entity, Allprops2ListOutput>()
			.Map(dest => dest.DanHang, src => src.DanHang != null ? src.DanHang : null)
			.Map(dest => dest.DanXuan, src => src.DanXuan != null ? src.DanXuan : null)
			.Map(dest => dest.DuoHang, src => src.DuoHang != null ? src.DuoHang : null)
			.Map(dest => dest.DuoXuan, src => src.DuoXuan != null ? src.DuoXuan : null)
			.Map(dest => dest.HK, src => src.HK != null ? src.HK : null)
			.Map(dest => dest.id, src => src.id)
			.Map(dest => dest.JiLian, src => src.JiLian != null ? src.JiLian : null)
			.Map(dest => dest.KaiGuan, src => src.KaiGuan != null ? src.KaiGuan.ToString() : null)
			.Map(dest => dest.PF, src => src.PF != null ? src.PF : null)
			.Map(dest => dest.RiQi, src => src.RiQi != null ? string.Format("{0:yyyy-MM-dd HH:mm:ss}", src.RiQi) : null)
			.Map(dest => dest.ShiJian, src => src.ShiJian != null ? src.ShiJian : null)
			.Map(dest => dest.ShuZi, src => src.ShuZi != null ? src.ShuZi : null)
			.Map(dest => dest.XiaLa, src => src.XiaLa != null ? src.XiaLa : null)
		;
	}
}
