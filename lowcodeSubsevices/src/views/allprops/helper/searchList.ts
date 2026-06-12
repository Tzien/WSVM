const searchList = [
  {
    "label": "姓名",
    "labelI18nCode": "",
    "prop": "Name",
    "ceriKey": "input",
    "searchType": 2,
    "searchMultiple": false,
    "isKeyword": false,
    "id": "Name",
    "fullName": "姓名",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "input",
      "label": "姓名",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriInput",
      "tagIcon": "icon-ym icon-ym-generator-input",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "required": false,
      "layout": "colFormItem",
      "span": 24,
      "dragDisabled": false,
      "visibility": [
        "pc",
        "app"
      ],
      "tableName": "allprops",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItem2780be",
      "renderKey": 1781246909645
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}",
      "blur": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "placeholder": "请输入",
    "useScan": false,
    "useMask": false,
    "maskConfig": {
      "filler": "*",
      "maskType": 1,
      "prefixType": 1,
      "prefixLimit": 0,
      "prefixSpecifyChar": "",
      "suffixType": 1,
      "suffixLimit": 0,
      "suffixSpecifyChar": "",
      "ignoreChar": "",
      "useUnrealMask": false,
      "unrealMaskLength": 1
    },
    "clearable": true,
    "addonBefore": "",
    "addonAfter": "",
    "prefixIcon": "",
    "suffixIcon": "",
    "maxlength": null,
    "showCount": false,
    "showPassword": false,
    "readonly": false,
    "disabled": false,
    "__vModel__": "Name"
  },
  {
    "label": "是否删除",
    "labelI18nCode": "",
    "prop": "IsDeleted",
    "ceriKey": "radio",
    "searchType": 1,
    "searchMultiple": false,
    "isKeyword": false,
    "id": "IsDeleted",
    "fullName": "是否删除",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "radio",
      "label": "是否删除",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriRadio",
      "tagIcon": "icon-ym icon-ym-generator-radio",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "defaultValue": "0",
      "required": false,
      "layout": "colFormItem",
      "span": 24,
      "dragDisabled": false,
      "visibility": [
        "pc",
        "app"
      ],
      "tableName": "allprops",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "dataType": "static",
      "dictionaryType": "",
      "propsUrl": "",
      "propsName": "",
      "useCache": true,
      "templateJson": [],
      "formId": "formItem5b13cd",
      "renderKey": 1781246963396
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "options": [
      {
        "fullName": "是",
        "id": "1"
      },
      {
        "fullName": "否",
        "id": "0"
      }
    ],
    "props": {
      "label": "fullName",
      "value": "id"
    },
    "direction": "horizontal",
    "optionType": "default",
    "buttonStyle": "solid",
    "size": "default",
    "disabled": false,
    "__vModel__": "IsDeleted"
  },
  {
    "label": "运动类型",
    "labelI18nCode": "",
    "prop": "Enmu",
    "ceriKey": "select",
    "searchType": 1,
    "searchMultiple": true,
    "isKeyword": false,
    "id": "Enmu",
    "fullName": "运动类型",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "select",
      "label": "运动类型",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriSelect",
      "tagIcon": "icon-ym icon-ym-generator-select",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "defaultValue": "",
      "required": false,
      "layout": "colFormItem",
      "span": 24,
      "dragDisabled": false,
      "visibility": [
        "pc",
        "app"
      ],
      "tableName": "allprops",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "dataType": "static",
      "dictionaryType": "",
      "propsUrl": "",
      "propsName": "",
      "useCache": true,
      "templateJson": [],
      "formId": "formItemfecac4",
      "renderKey": 1781247070772
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "options": [
      {
        "fullName": "走路",
        "id": "1"
      },
      {
        "fullName": "骑车",
        "id": "2"
      }
    ],
    "props": {
      "label": "fullName",
      "value": "id"
    },
    "placeholder": "请选择",
    "clearable": true,
    "disabled": false,
    "filterable": false,
    "multiple": false,
    "__vModel__": "Enmu"
  },
  {
    "label": "评分",
    "labelI18nCode": "",
    "prop": "PF",
    "ceriKey": "rate",
    "searchType": 3,
    "searchMultiple": false,
    "isKeyword": false,
    "id": "PF",
    "fullName": "评分",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "rate",
      "label": "评分",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriRate",
      "tagIcon": "icon-ym icon-ym-generator-rate",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "defaultValue": 0,
      "required": false,
      "layout": "colFormItem",
      "span": 24,
      "dragDisabled": false,
      "visibility": [
        "pc",
        "app"
      ],
      "tableName": "allprops",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItemf34b6b",
      "renderKey": 1781247397653
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "count": 5,
    "allowHalf": false,
    "disabled": false,
    "__vModel__": "PF"
  }
]
export default searchList