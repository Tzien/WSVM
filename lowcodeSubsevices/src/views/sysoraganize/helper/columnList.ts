const columnList = [
  {
    "label": "组织名称",
    "labelI18nCode": "",
    "prop": "Name",
    "fixed": "none",
    "align": "center",
    "ceriKey": "input",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Name",
    "fullName": "组织名称",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "input",
      "label": "组织名称",
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
      "tableName": "sysoraganize",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItema7bd12",
      "renderKey": 1773302366050
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
    "label": "组织编码",
    "labelI18nCode": "",
    "prop": "Code",
    "fixed": "none",
    "align": "center",
    "ceriKey": "input",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Code",
    "fullName": "组织编码",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "input",
      "label": "组织编码",
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
      "tableName": "sysoraganize",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItemf4e18c",
      "renderKey": 1773302653826,
      "unique": true
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
    "__vModel__": "Code"
  },
  {
    "label": "排序",
    "labelI18nCode": "",
    "prop": "Sort",
    "fixed": "none",
    "align": "center",
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
      "tableName": "sysoraganize",
      "noShow": false,
      "regList": [],
      "trigger": [
        "blur",
        "change"
      ],
      "formId": "formItem835276",
      "renderKey": 1773302367904
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
    "label": "是否激活",
    "labelI18nCode": "",
    "prop": "Status",
    "fixed": "none",
    "align": "center",
    "ceriKey": "switch",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Status",
    "fullName": "是否激活",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "switch",
      "label": "是否激活",
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
      "tableName": "sysoraganize",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItemc3af02",
      "renderKey": 1773302368304
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "disabled": false,
    "activeTxt": "正常",
    "inactiveTxt": "冻结",
    "activeValue": 1,
    "inactiveValue": 0,
    "__vModel__": "Status"
  },
  {
    "label": "备注",
    "labelI18nCode": "",
    "prop": "Remark",
    "fixed": "none",
    "align": "center",
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
      "tableName": "sysoraganize",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItem50491a",
      "renderKey": 1773302367192
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