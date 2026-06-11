using CERIOS.App.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.ViewEngine
{
    public class AnonymousTypeWrapper : DynamicObject
    {
        /// <summary>
        /// 匿名模型
        /// </summary>
        private readonly object model;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="model"></param>
        public AnonymousTypeWrapper(object model)
        {
            this.model = model;
        }

        /// <summary>
        /// 获取成员信息
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var propertyInfo = model.GetType().GetProperty(binder.Name);

            if (propertyInfo == null)
            {
                result = null;
                return false;
            }

            result = propertyInfo.GetValue(model, null);

            if (result == null)
            {
                return true;
            }

            var type = result.GetType();

            if (result.IsAnonymous())
            {
                result = new AnonymousTypeWrapper(result);
            }

            var isEnumerable = typeof(IEnumerable).IsAssignableFrom(type);
            if (isEnumerable && result is not string)
            {
                var actType = type.IsArray ? type.GetElementType() : type.GenericTypeArguments[0];

                // 修复集合的泛型类型为匿名类型时类型转换
                var genericType = actType.IsAnonymous()
                    ? typeof(List<AnonymousTypeWrapper>)
                    : typeof(List<>).MakeGenericType(actType);

                // 创建集合实例
                var list = Activator.CreateInstance(genericType);
                var addMethod = list.GetType().GetMethod("Add");

                var data = result as IEnumerable;
                foreach (var item in data)
                {
                    addMethod.Invoke(list, new object[] {
                    item.IsAnonymous() ? new AnonymousTypeWrapper(item) : item
                });
                }

                result = list;
            }

            return true;
        }

    }
}
