const columnList = [
  {
    "label": "姓名",
    "labelI18nCode": "",
    "prop": "Name",
    "fixed": "none",
    "align": "left",
    "ceriKey": "input",
    "sortable": false,
    "resizable": true,
    "width": null,
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
    "label": "排序",
    "labelI18nCode": "",
    "prop": "Sort",
    "fixed": "none",
    "align": "left",
    "ceriKey": "inputNumber",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Sort",
    "fullName": "排序",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "inputNumber",
      "label": "排序",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriInputNumber",
      "tagIcon": "icon-ym icon-ym-generator-number",
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
      "trigger": [
        "blur",
        "change"
      ],
      "formId": "formItem375c09",
      "renderKey": 1781246934286
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}",
      "blur": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "placeholder": "请输入",
    "controls": false,
    "addonBefore": "",
    "addonAfter": "",
    "thousands": false,
    "isAmountChinese": false,
    "step": 1,
    "disabled": false,
    "__vModel__": "Sort"
  },
  {
    "label": "是否启用",
    "labelI18nCode": "",
    "prop": "Enabled",
    "fixed": "none",
    "align": "left",
    "ceriKey": "switch",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Enabled",
    "fullName": "是否启用",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "switch",
      "label": "是否启用",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriSwitch",
      "tagIcon": "icon-ym icon-ym-generator-switch",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "defaultValue": 1,
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
      "formId": "formItem78887f",
      "renderKey": 1781246944460
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "disabled": false,
    "activeTxt": "开",
    "inactiveTxt": "关",
    "activeValue": 1,
    "inactiveValue": 0,
    "__vModel__": "Enabled"
  },
  {
    "label": "备注",
    "labelI18nCode": "",
    "prop": "Remark",
    "fixed": "none",
    "align": "left",
    "ceriKey": "textarea",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Remark",
    "fullName": "备注",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "textarea",
      "label": "备注",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriTextarea",
      "tagIcon": "icon-ym icon-ym-generator-textarea",
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
      "formId": "formItem1ab2df",
      "renderKey": 1781246926716
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}",
      "blur": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "placeholder": "请输入",
    "autoSize": {
      "minRows": 4,
      "maxRows": 4
    },
    "clearable": true,
    "maxlength": null,
    "showCount": false,
    "readonly": false,
    "disabled": false,
    "__vModel__": "Remark"
  },
  {
    "label": "是否删除",
    "labelI18nCode": "",
    "prop": "IsDeleted",
    "fixed": "none",
    "align": "left",
    "ceriKey": "radio",
    "sortable": false,
    "resizable": true,
    "width": null,
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
    "label": "距离（米）",
    "labelI18nCode": "",
    "prop": "Text",
    "fixed": "none",
    "align": "left",
    "ceriKey": "checkbox",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Text",
    "fullName": "距离（米）",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "checkbox",
      "label": "距离（米）",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriCheckbox",
      "tagIcon": "icon-ym icon-ym-generator-checkbox",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "defaultValue": [],
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
      "formId": "formItem29d0c8",
      "renderKey": 1781247018308
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "options": [
      {
        "fullName": "超过50",
        "id": "1"
      },
      {
        "fullName": "小于200",
        "id": "2"
      }
    ],
    "props": {
      "label": "fullName",
      "value": "id"
    },
    "direction": "horizontal",
    "disabled": false,
    "__vModel__": "Text"
  },
  {
    "label": "运动类型",
    "labelI18nCode": "",
    "prop": "Enmu",
    "fixed": "none",
    "align": "left",
    "ceriKey": "select",
    "sortable": false,
    "resizable": true,
    "width": null,
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
    "label": "创建时间",
    "labelI18nCode": "",
    "prop": "CreateTime",
    "fixed": "none",
    "align": "left",
    "ceriKey": "datePicker",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "CreateTime",
    "fullName": "创建时间",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "datePicker",
      "label": "创建时间",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriDatePicker",
      "tagIcon": "icon-ym icon-ym-generator-date",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "defaultValue": null,
      "defaultCurrent": true,
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
      "startTimeRule": false,
      "startTimeType": 1,
      "startTimeTarget": 1,
      "startTimeValue": null,
      "startRelationField": "",
      "endTimeRule": false,
      "endTimeType": 1,
      "endTimeTarget": 1,
      "endTimeValue": null,
      "endRelationField": "",
      "formId": "formItem2e7583",
      "renderKey": 1781247178932
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}",
      "blur": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "placeholder": "请选择",
    "format": "yyyy-MM-dd",
    "startTime": null,
    "endTime": null,
    "disabled": false,
    "clearable": true,
    "__vModel__": "CreateTime"
  },
  {
    "label": "最后登录时间",
    "labelI18nCode": "",
    "prop": "LastLoginTime",
    "fixed": "none",
    "align": "left",
    "ceriKey": "timePicker",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "LastLoginTime",
    "fullName": "最后登录时间",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "timePicker",
      "label": "最后登录时间",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriTimePicker",
      "tagIcon": "icon-ym icon-ym-generator-time",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "defaultValue": "",
      "defaultCurrent": true,
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
      "startTimeRule": false,
      "startTimeType": 1,
      "startTimeTarget": 1,
      "startTimeValue": null,
      "startRelationField": "",
      "endTimeRule": false,
      "endTimeType": 1,
      "endTimeTarget": 1,
      "endTimeValue": null,
      "endRelationField": "",
      "formId": "formItem42c558",
      "renderKey": 1781247201324
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}",
      "blur": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "placeholder": "请选择",
    "format": "HH:mm:ss",
    "startTime": null,
    "endTime": null,
    "disabled": false,
    "clearable": true,
    "__vModel__": "LastLoginTime"
  },
  {
    "label": "评分",
    "labelI18nCode": "",
    "prop": "PF",
    "fixed": "none",
    "align": "left",
    "ceriKey": "rate",
    "sortable": false,
    "resizable": true,
    "width": null,
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
  },
  {
    "label": "滑块",
    "labelI18nCode": "",
    "prop": "HK",
    "fixed": "none",
    "align": "left",
    "ceriKey": "slider",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "HK",
    "fullName": "滑块",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "slider",
      "label": "滑块",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriSlider",
      "tagIcon": "icon-ym icon-ym-generator-slider",
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
      "formId": "formItem3a4e20",
      "renderKey": 1781247450444
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "disabled": false,
    "min": 0,
    "max": 100,
    "step": 1,
    "__vModel__": "HK"
  }
]
export default columnList