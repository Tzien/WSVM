using CeriOS.LowCodeForm.Model.ViewModel.HelperModel;

namespace CeriOS.LowCodeForm.Model.Helper
{
    public static class TreeHelper
    {
        /// <summary>
        /// 建造树结构.
        /// </summary>
        /// <param name="allNodes">所有的节点.</param>
        /// <param name="parentId">节点.</param>
        /// <returns></returns>
        public static List<T> ToTree<T>(this List<T> allNodes, string parentId = "0")
            where T : TreeModel, new()
        {
            List<T> resData = new List<T>();

            // 查找出父类对象
            List<T> rootNodes = allNodes.FindAll(x => x.parentId == parentId || string.IsNullOrEmpty(x.parentId));

            // 移除父类对象
            allNodes.RemoveAll(x => x.parentId == parentId || string.IsNullOrEmpty(x.parentId));
            resData = rootNodes;
            resData.ForEach(aRootNode =>
            {
                aRootNode.hasChildren = HaveChildren(allNodes, aRootNode.id);
                if (aRootNode.hasChildren)
                {
                    aRootNode.children = _GetChildren(allNodes, aRootNode);
                    aRootNode.num = aRootNode.children.Count();
                }
                else
                {
                    aRootNode.isLeaf = !aRootNode.hasChildren;
                    aRootNode.children = null;
                }
            });
            return resData;
        }

        /// <summary>
        /// 获取所有子节点.
        /// </summary>
        /// <typeparam name="T">树模型（TreeModel或继承它的模型.</typeparam>
        /// <param name="nodes">所有节点列表.</param>
        /// <param name="parentNode">父节点Id.</param>
        /// <returns></returns>
        private static List<object> _GetChildren<T>(List<T> nodes, T parentNode)
            where T : TreeModel, new()
        {
            Type type = typeof(T);
            var properties = type.GetProperties().ToList();
            List<object> resData = new List<object>();

            // 查找出父类对象
            var children = nodes.FindAll(x => x.parentId == parentNode.id);

            // 移除父类对象
            nodes.RemoveAll(x => x.parentId == parentNode.id);
            children.ForEach(aChildren =>
            {
                T newNode = new T();
                resData.Add(newNode);

                // 赋值属性
                foreach (var aProperty in properties.Where(x => x.CanWrite))
                {
                    var value = aProperty.GetValue(aChildren, null);
                    aProperty.SetValue(newNode, value);
                }

                newNode.hasChildren = HaveChildren(nodes, aChildren.id);
                if (newNode.hasChildren)
                {
                    newNode.children = _GetChildren(nodes, newNode);
                }
                else
                {
                    newNode.isLeaf = !newNode.hasChildren;
                    newNode.children = null;
                }
            });
            return resData;
        }

        /// <summary>
        /// 判断当前节点是否有子节点.
        /// </summary>
        /// <typeparam name="T">树模型.</typeparam>
        /// <param name="nodes">所有节点.</param>
        /// <param name="nodeId">当前节点Id.</param>
        /// <returns></returns>
        private static bool HaveChildren<T>(List<T> nodes, string nodeId)
            where T : TreeModel, new()
        {
            return nodes.Exists(x => x.parentId == nodeId);
        }
    }


}
