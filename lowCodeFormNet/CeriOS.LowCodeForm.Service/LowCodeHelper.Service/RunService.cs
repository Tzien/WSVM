using CERIOS.Common.Const;
using Newtonsoft.Json;


namespace CeriOS.LowCodeForm.Service.LowCodeHelper.Service
{
    public class RunService
    {
        /// <summary>
        /// 无限递归 给控件绑定默认值 (绕过 布局控件).
        /// </summary>
            //string defaultUserId, string defaultDepId, List<string> defaultPosIds, List<string> defaultRoleIds, List<string> defaultGroupIds, List<UserRelationEntity> userRelationList
        public void FieldBindDefaultValue(ref List<Dictionary<string, object>> dicFieldsModelList, string defaultUserId)
        {
            foreach (var item in dicFieldsModelList)
            {
                var obj = JsonConvert.DeserializeObject<Dictionary<string, object>>(item["__config__"].ToString());

                if (obj.ContainsKey("ceriKey") && (obj["ceriKey"].Equals(CeriKeyConst.USERSELECT) || obj["ceriKey"].Equals(CeriKeyConst.DEPSELECT) || obj["ceriKey"].Equals(CeriKeyConst.POSSELECT) || obj["ceriKey"].Equals(CeriKeyConst.ROLESELECT) || obj["ceriKey"].Equals(CeriKeyConst.GROUPSELECT) || obj["ceriKey"].Equals(CeriKeyConst.USERSSELECT)) && obj["defaultCurrent"].Equals(true))
                {
                    switch (obj["ceriKey"])
                    {
                        case CeriKeyConst.USERSSELECT:
                        case CeriKeyConst.USERSELECT:
                            if (item.ContainsKey("selectType") && item["selectType"].Equals("custom"))
                            {
                                var ableDepIds = JsonConvert.DeserializeObject<List<string>>(item["ableDepIds"].ToString());
                                if (ableDepIds == null) ableDepIds = new List<string>();
                                var ablePosIds = JsonConvert.DeserializeObject<List<string>>(item["ablePosIds"].ToString());
                                if (ablePosIds == null) ablePosIds = new List<string>();
                                var ableUserIds = JsonConvert.DeserializeObject<List<string>>(item["ableUserIds"].ToString());
                                if (ableUserIds == null) ableUserIds = new List<string>();
                                var ableRoleIds = JsonConvert.DeserializeObject<List<string>>(item["ableRoleIds"].ToString());
                                if (ableRoleIds == null) ableRoleIds = new List<string>();
                                var ableGroupIds = JsonConvert.DeserializeObject<List<string>>(item["ableGroupIds"].ToString());
                                if (ableGroupIds == null) ableGroupIds = new List<string>();
                                //var userIdList = userRelationList.Where(x => ableUserIds.Contains(x.UserId) || ableDepIds.Contains(x.ObjectId)
                                //    || ablePosIds.Contains(x.ObjectId) || ableRoleIds.Contains(x.ObjectId) || ableGroupIds.Contains(x.ObjectId)).Select(x => x.UserId).ToList();
                                //if (!userIdList.Contains(defaultUserId))
                                //{
                                //    obj["defaultValue"] = null;
                                //    break;
                                //}
                            }
                            if (item.ContainsKey("multiple") && item["multiple"].Equals(true))
                            {
                                if (obj["ceriKey"].Equals(CeriKeyConst.USERSELECT))
                                {
                                    obj["defaultValue"] = new List<string>() { defaultUserId };
                                }
                                else
                                {
                                    obj["defaultValue"] = new List<string>() { string.Format("{0}--user", defaultUserId) };
                                }
                            }
                            else
                            {
                                obj["defaultValue"] = defaultUserId;
                            }
                            break;
                            //case CeriKeyConst.DEPSELECT:
                            //    if (item.ContainsKey("selectType") && item["selectType"].Equals("custom"))
                            //    {
                            //        var defValue = item["ableDepIds"].ToObject<List<string>>();
                            //        if (!defValue.Contains(defaultDepId))
                            //        {
                            //            obj["defaultValue"] = null;
                            //            break;
                            //        }
                            //    }
                            //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true)) obj["defaultValue"] = new List<string>() { defaultDepId };
                            //    else obj["defaultValue"] = defaultDepId;
                            //    break;
                            //case CeriKeyConst.POSSELECT:
                            //    var defaultPosId = defaultPosIds.FirstOrDefault();
                            //    if (item.ContainsKey("selectType") && item["selectType"].Equals("custom"))
                            //    {
                            //        var defValue = item["ablePosIds"].ToObject<List<string>>();
                            //        if (!defValue.Contains(defaultPosId))
                            //        {
                            //            obj["defaultValue"] = null;
                            //            break;
                            //        }
                            //    }
                            //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true))
                            //    {
                            //        obj["defaultValue"] = defaultPosIds;
                            //    }
                            //    else
                            //    {
                            //        if (defaultPosIds.Contains(_userManager.User.PositionId))
                            //        {
                            //            obj["defaultValue"] = _userManager.User.PositionId;
                            //        }
                            //        else
                            //        {
                            //            obj["defaultValue"] = defaultPosId;
                            //        }
                            //    }
                            //    break;
                            //case CeriKeyConst.ROLESELECT:
                            //    var defaultRoleId = defaultRoleIds.FirstOrDefault();
                            //    if (item.ContainsKey("selectType") && item["selectType"].Equals("custom"))
                            //    {
                            //        var defValue = item["ableRoleIds"].ToObject<List<string>>();
                            //        if (!defValue.Contains(defaultRoleId))
                            //        {
                            //            obj["defaultValue"] = null;
                            //            break;
                            //        }
                            //    }
                            //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true)) obj["defaultValue"] = defaultRoleIds;
                            //    else obj["defaultValue"] = defaultRoleId;
                            //    break;
                            //case CeriKeyConst.GROUPSELECT:
                            //    var defaultGroupId = defaultGroupIds.FirstOrDefault();
                            //    if (item.ContainsKey("selectType") && item["selectType"].Equals("custom"))
                            //    {
                            //        var defValue = item["ableGroupIds"].ToObject<List<string>>();
                            //        if (!defValue.Contains(defaultGroupId))
                            //        {
                            //            obj["defaultValue"] = null;
                            //            break;
                            //        }
                            //    }
                            //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true)) obj["defaultValue"] = defaultGroupIds;
                            //    else obj["defaultValue"] = defaultGroupId;
                            //    break;
                    }
                }

                // 子表控件
                if (obj.ContainsKey("ceriKey") && obj["ceriKey"].Equals(CeriKeyConst.TABLE))
                {
                    //var cList = obj["children"].ToObject<List<Dictionary<string, object>>>();
                    var cList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(obj["children"].ToString());

                    foreach (var child in cList)
                    {
                        var cObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(child["__config__"].ToString());
                        //var cObj = child["__config__"].ToObject<Dictionary<string, object>>();
                        if (cObj.ContainsKey("ceriKey") && (cObj["ceriKey"].Equals(CeriKeyConst.USERSELECT) || cObj["ceriKey"].Equals(CeriKeyConst.DEPSELECT) || cObj["ceriKey"].Equals(CeriKeyConst.POSSELECT) || cObj["ceriKey"].Equals(CeriKeyConst.ROLESELECT) || cObj["ceriKey"].Equals(CeriKeyConst.GROUPSELECT) || cObj["ceriKey"].Equals(CeriKeyConst.USERSSELECT)) && cObj["defaultCurrent"].Equals(true))
                        {
                            switch (cObj["ceriKey"])
                            {
                                case CeriKeyConst.USERSSELECT:
                                case CeriKeyConst.USERSELECT:
                                    if (item.ContainsKey("multiple") && item["multiple"].Equals(true))
                                    {
                                        if (cObj["ceriKey"].Equals(CeriKeyConst.USERSELECT))
                                        {
                                            cObj["defaultValue"] = new List<string>() { defaultUserId };
                                        }
                                        else
                                        {
                                            cObj["defaultValue"] = new List<string>() { string.Format("{0}--user", defaultUserId) };
                                        }
                                    }
                                    else
                                    {
                                        cObj["defaultValue"] = defaultUserId;
                                    }
                                    break;
                                //case CeriKeyConst.DEPSELECT:
                                //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true)) cObj["defaultValue"] = new List<string>() { defaultDepId };
                                //    else cObj["defaultValue"] = defaultDepId;
                                //    break;
                                //case CeriKeyConst.POSSELECT:
                                //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true))
                                //    {
                                //        cObj["defaultValue"] = defaultPosIds;
                                //    }
                                //    else
                                //    {
                                //        if (defaultPosIds.Contains(_userManager.User.PositionId))
                                //        {
                                //            cObj["defaultValue"] = _userManager.User.PositionId;
                                //        }
                                //        else
                                //        {
                                //            cObj["defaultValue"] = defaultPosIds.FirstOrDefault();
                                //        }
                                //    }
                                //    break;
                                //case CeriKeyConst.ROLESELECT:
                                //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true)) cObj["defaultValue"] = defaultRoleIds;
                                //    else cObj["defaultValue"] = defaultRoleIds.FirstOrDefault();
                                //    break;
                                //case CeriKeyConst.GROUPSELECT:
                                //    if (item.ContainsKey("multiple") && item["multiple"].Equals(true)) cObj["defaultValue"] = defaultGroupIds;
                                //    else cObj["defaultValue"] = defaultGroupIds.FirstOrDefault();
                                //    break;
                            }
                        }

                        child["__config__"] = cObj;
                    }

                    obj["children"] = cList;
                }

                // 递归布局控件
                if (obj.ContainsKey("children"))
                {
                    var fmList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(obj["children"].ToString());
                    FieldBindDefaultValue(ref fmList, defaultUserId);
                    obj["children"] = fmList;
                }

                item["__config__"] = obj;
            }
        }
    }

}
