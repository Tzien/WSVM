using CeriOS.示例.Entitys.Dto.Dictbaseinfo;
using Mapster;

namespace CeriOS.示例.Entitys.Mapper.Dictbaseinfo;

public class Mapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<DictbaseinfoCrInput, DictbaseinfoEntity>()
            .Map(dest => dest.AllowEdit, src => src.AllowEdit != null ? src.AllowEdit : null)
            .Map(dest => dest.BelongSystem, src => src.BelongSystem != null ? src.BelongSystem : null)
            .Map(dest => dest.CreateName, src => src.CreateName != null ? src.CreateName : null)
            .Map(dest => dest.CreateTime, src => src.CreateTime != null ? src.CreateTime : null)
            .Map(dest => dest.DictType, src => src.DictType != null ? src.DictType : null)
            .Map(dest => dest.FieldDecimal, src => src.FieldDecimal != null ? src.FieldDecimal : null)
            .Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
        ;
        config.ForType<DictbaseinfoEntity, DictbaseinfoInfoOutput>()
            .Map(dest => dest.AllowEdit, src => src.AllowEdit != null ? src.AllowEdit : null)
            .Map(dest => dest.BelongSystem, src => src.BelongSystem != null ? src.BelongSystem : null)
            .Map(dest => dest.CreateName, src => src.CreateName != null ? src.CreateName : null)
            .Map(dest => dest.CreateTime, src => src.CreateTime != null ? src.CreateTime : null)
            .Map(dest => dest.DictType, src => src.DictType != null ? src.DictType : null)
            .Map(dest => dest.FieldDecimal, src => src.FieldDecimal != null ? src.FieldDecimal : null)
            .Map(dest => dest.Remark, src => src.Remark != null ? src.Remark : null)
        ;
    }
}
