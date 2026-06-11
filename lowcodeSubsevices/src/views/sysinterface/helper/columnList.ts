const columnList = [
  {
    "label": "ID",
    "labelI18nCode": "",
    "prop": "Id",
    "fixed": "none",
    "align": "left",
    "ceriKey": "input",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Id",
    "fullName": "ID",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "input",
      "label": "ID",
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
      "tableName": "sysinterface",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItemff1229",
      "renderKey": 1773717494344
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
    "__vModel__": "Id"
  },
  {
    "label": "系统ID",
    "labelI18nCode": "",
    "prop": "SysInfoID",
    "fixed": "none",
    "align": "left",
    "ceriKey": "input",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "SysInfoID",
    "fullName": "系统ID",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "input",
      "label": "系统ID",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriInput",
      "tagIcon": "icon-ym icon-ym-generator-input",
      "tableAlign": "left",
      "tableFixed": "none",
      "className": [],
      "required": true,
      "layout": "colFormItem",
      "span": 24,
      "dragDisabled": false,
      "visibility": [
        "pc",
        "app"
      ],
      "tableName": "sysinterface",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItemede165",
      "renderKey": 1773717494728
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
    "__vModel__": "SysInfoID"
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
      "tableName": "sysinterface",
      "noShow": false,
      "regList": [],
      "trigger": [
        "blur",
        "change"
      ],
      "formId": "formItem069abb",
      "renderKey": 1773717497792
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
    "prop": "IsActive",
    "fixed": "none",
    "align": "left",
    "ceriKey": "switch",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "IsActive",
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
      "defaultValue": 0,
      "required": false,
      "layout": "colFormItem",
      "span": 24,
      "dragDisabled": false,
      "visibility": [
        "pc",
        "app"
      ],
      "tableName": "sysinterface",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItem571363",
      "renderKey": 1773717498624
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "disabled": false,
    "activeTxt": "开",
    "inactiveTxt": "关",
    "activeValue": 1,
    "inactiveValue": 0,
    "__vModel__": "IsActive"
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
      "tableName": "sysinterface",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItem21d76d",
      "renderKey": 1773717499048
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
  }
]
export default columnList