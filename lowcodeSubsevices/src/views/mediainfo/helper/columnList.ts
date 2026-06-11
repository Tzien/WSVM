const columnList = [
  {
    "label": "ID",
    "labelI18nCode": "",
    "prop": "MediaInfoId",
    "fixed": "none",
    "align": "left",
    "ceriKey": "input",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "MediaInfoId",
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
      "tableName": "mediainfo",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItem9228f7",
      "renderKey": 1773716546488
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
    "__vModel__": "MediaInfoId"
  },
  {
    "label": "名称",
    "labelI18nCode": "",
    "prop": "Name",
    "fixed": "none",
    "align": "left",
    "ceriKey": "input",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Name",
    "fullName": "名称",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "input",
      "label": "名称",
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
      "tableName": "mediainfo",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItemfe6a93",
      "renderKey": 1773716587409
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
      "tableName": "mediainfo",
      "noShow": false,
      "regList": [],
      "trigger": [
        "blur",
        "change"
      ],
      "formId": "formItemc79390",
      "renderKey": 1773716548136
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
    "label": "开关",
    "labelI18nCode": "",
    "prop": "IsDeleted",
    "fixed": "none",
    "align": "left",
    "ceriKey": "switch",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "IsDeleted",
    "fullName": "开关",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "switch",
      "label": "开关",
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
      "tableName": "mediainfo",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItem2fbbe0",
      "renderKey": 1773716547768
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "disabled": false,
    "activeTxt": "开",
    "inactiveTxt": "关",
    "activeValue": 1,
    "inactiveValue": 0,
    "__vModel__": "IsDeleted"
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
      "tableName": "mediainfo",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItem0ffa9d",
      "renderKey": 1773716547336
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