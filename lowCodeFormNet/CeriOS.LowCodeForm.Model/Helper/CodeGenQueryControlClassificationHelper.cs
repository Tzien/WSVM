using CERIOS.Common.Const;

namespace CeriOS.LowCodeForm.Model.Helper;

/// <summary>
/// 代码生成查询控件归类帮助类.
/// </summary>
public class CodeGenQueryControlClassificationHelper
{
    /// <summary>
    /// 列表查询控件.
    /// </summary>
    /// <param name="type">1-Web设计,2-App设计,3-流程表单,4-Web表单,5-App表单.</param>
    /// <returns></returns>
    public static Dictionary<string, List<string>> ListQueryControl(int type)
    {
        Dictionary<string, List<string>> listQueryControl = new Dictionary<string, List<string>>();
        switch (type)
        {
            case 4:
                {
                    var useInputList = new List<string>();
                    useInputList.Add(CeriKeyConst.COMINPUT);
                    useInputList.Add(CeriKeyConst.LOCATION);
                    useInputList.Add(CeriKeyConst.TEXTAREA);
                    useInputList.Add(CeriKeyConst.CERITEXT);
                    useInputList.Add(CeriKeyConst.BILLRULE);
                    listQueryControl["inputList"] = useInputList;

                    var useDateList = new List<string>();
                    useDateList.Add(CeriKeyConst.CREATETIME);
                    useDateList.Add(CeriKeyConst.MODIFYTIME);
                    listQueryControl["dateList"] = useDateList;

                    var useSelectList = new List<string>();
                    useSelectList.Add(CeriKeyConst.SELECT);
                    useSelectList.Add(CeriKeyConst.RADIO);
                    useSelectList.Add("checkbox");
                    listQueryControl["selectList"] = useSelectList;

                    var timePickerList = new List<string>();
                    timePickerList.Add(CeriKeyConst.TIME);
                    listQueryControl["timePickerList"] = timePickerList;

                    var numRangeList = new List<string>();
                    numRangeList.Add(CeriKeyConst.NUMINPUT);
                    numRangeList.Add(CeriKeyConst.CALCULATE);
                    numRangeList.Add(CeriKeyConst.RATE);
                    numRangeList.Add(CeriKeyConst.SLIDER);
                    listQueryControl["numRangeList"] = numRangeList;

                    var datePickerList = new List<string>();
                    datePickerList.Add(CeriKeyConst.DATE);
                    listQueryControl["datePickerList"] = datePickerList;

                    var userSelectList = new List<string>();
                    userSelectList.Add(CeriKeyConst.CREATEUSER);
                    userSelectList.Add(CeriKeyConst.MODIFYUSER);
                    userSelectList.Add(CeriKeyConst.USERSELECT);
                    listQueryControl["userSelectList"] = userSelectList;

                    var usersSelectList = new List<string>();
                    usersSelectList.Add(CeriKeyConst.USERSSELECT);
                    listQueryControl["usersSelectList"] = usersSelectList;

                    var comSelectList = new List<string>();
                    comSelectList.Add(CeriKeyConst.COMSELECT);
                    comSelectList.Add(CeriKeyConst.CURRORGANIZE);
                    listQueryControl["comSelectList"] = comSelectList;

                    var depSelectList = new List<string>();
                    depSelectList.Add(CeriKeyConst.CURRDEPT);
                    depSelectList.Add(CeriKeyConst.DEPSELECT);
                    listQueryControl["depSelectList"] = depSelectList;

                    var posSelectList = new List<string>();
                    posSelectList.Add(CeriKeyConst.CURRPOSITION);
                    posSelectList.Add(CeriKeyConst.POSSELECT);
                    listQueryControl["posSelectList"] = posSelectList;

                    var useCascaderList = new List<string>();
                    useCascaderList.Add(CeriKeyConst.CASCADER);
                    listQueryControl["useCascaderList"] = useCascaderList;

                    var jNPFAddressList = new List<string>();
                    jNPFAddressList.Add(CeriKeyConst.ADDRESS);
                    listQueryControl["JNPFAddressList"] = jNPFAddressList;

                    var treeSelectList = new List<string>();
                    treeSelectList.Add(CeriKeyConst.TREESELECT);
                    listQueryControl["treeSelectList"] = treeSelectList;

                    var autoCompleteList = new List<string>();
                    autoCompleteList.Add(CeriKeyConst.AUTOCOMPLETE);
                    listQueryControl["autoCompleteList"] = autoCompleteList;
                }

                break;
            case 5:
                {
                    var inputList = new List<string>();
                    inputList.Add(CeriKeyConst.COMINPUT);
                    inputList.Add(CeriKeyConst.LOCATION);
                    inputList.Add(CeriKeyConst.TEXTAREA);
                    inputList.Add(CeriKeyConst.CERITEXT);
                    inputList.Add(CeriKeyConst.BILLRULE);
                    inputList.Add(CeriKeyConst.CALCULATE);
                    listQueryControl["input"] = inputList;

                    var numRangeList = new List<string>();
                    numRangeList.Add(CeriKeyConst.NUMINPUT);
                    numRangeList.Add(CeriKeyConst.RATE);
                    numRangeList.Add(CeriKeyConst.SLIDER);
                    listQueryControl["numRange"] = numRangeList;

                    var switchList = new List<string>();
                    switchList.Add(CeriKeyConst.SWITCH);
                    listQueryControl["switch"] = switchList;

                    var selectList = new List<string>();
                    selectList.Add(CeriKeyConst.RADIO);
                    selectList.Add(CeriKeyConst.CHECKBOX);
                    selectList.Add(CeriKeyConst.SELECT);
                    listQueryControl["select"] = selectList;

                    var cascaderList = new List<string>();
                    cascaderList.Add(CeriKeyConst.CASCADER);
                    listQueryControl["cascader"] = cascaderList;

                    var timeList = new List<string>();
                    timeList.Add(CeriKeyConst.TIME);
                    listQueryControl["timePicker"] = timeList;

                    var dateList = new List<string>();
                    dateList.Add(CeriKeyConst.DATE);
                    dateList.Add(CeriKeyConst.CREATETIME);
                    dateList.Add(CeriKeyConst.MODIFYTIME);
                    listQueryControl["datePicker"] = dateList;

                    var comSelectList = new List<string>();
                    comSelectList.Add(CeriKeyConst.COMSELECT);
                    comSelectList.Add(CeriKeyConst.CURRORGANIZE);
                    listQueryControl["organizeSelect"] = comSelectList;

                    var depSelectList = new List<string>();
                    depSelectList.Add(CeriKeyConst.DEPSELECT);
                    depSelectList.Add(CeriKeyConst.CURRDEPT);
                    listQueryControl["depSelect"] = depSelectList;

                    var posSelectList = new List<string>();
                    posSelectList.Add(CeriKeyConst.POSSELECT);
                    posSelectList.Add(CeriKeyConst.CURRPOSITION);
                    listQueryControl["posSelect"] = posSelectList;

                    var userSelectList = new List<string>();
                    userSelectList.Add(CeriKeyConst.USERSELECT);
                    userSelectList.Add(CeriKeyConst.CREATEUSER);
                    userSelectList.Add(CeriKeyConst.MODIFYUSER);
                    listQueryControl["userSelect"] = userSelectList;

                    var usersSelectList = new List<string>();
                    usersSelectList.Add(CeriKeyConst.USERSSELECT);
                    listQueryControl["usersSelect"] = usersSelectList;

                    var treeSelectList = new List<string>();
                    treeSelectList.Add(CeriKeyConst.TREESELECT);
                    listQueryControl["treeSelect"] = treeSelectList;

                    var addressList = new List<string>();
                    addressList.Add(CeriKeyConst.ADDRESS);
                    listQueryControl["areaSelect"] = addressList;

                    listQueryControl["autoComplete"] = new List<string> { CeriKeyConst.AUTOCOMPLETE };

                    listQueryControl["groupSelect"] = new List<string>() { CeriKeyConst.GROUPSELECT };

                    listQueryControl["roleSelect"] = new List<string>() { CeriKeyConst.ROLESELECT };
                }

                break;
        }

        return listQueryControl;
    }
}