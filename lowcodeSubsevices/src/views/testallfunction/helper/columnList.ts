const columnList = [
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
      "tableName": "testallfunction",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItem74cddc",
      "renderKey": 1783318221157
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
      "tableName": "testallfunction",
      "noShow": false,
      "regList": [],
      "trigger": "blur",
      "formId": "formItem3820ef",
      "renderKey": 1783318222484
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
    "label": "排序",
    "labelI18nCode": "",
    "prop": "sort",
    "fixed": "none",
    "align": "left",
    "ceriKey": "inputNumber",
    "sortable": true,
    "resizable": true,
    "width": null,
    "id": "sort",
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
      "tableName": "testallfunction",
      "noShow": false,
      "regList": [],
      "trigger": [
        "blur",
        "change"
      ],
      "formId": "formItem0e82c8",
      "renderKey": 1783318226604
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
    "__vModel__": "sort"
  },
  {
    "label": "文件上传",
    "labelI18nCode": "",
    "prop": "FileURL",
    "fixed": "none",
    "align": "left",
    "ceriKey": "uploadFile",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "FileURL",
    "fullName": "文件上传",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "uploadFile",
      "label": "文件上传",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriUploadFile",
      "tagIcon": "icon-ym icon-ym-generator-upload",
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
      "tableName": "testallfunction",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItem1f8d46",
      "renderKey": 1783318256956
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "disabled": false,
    "accept": "",
    "fileSize": 10,
    "sizeUnit": "MB",
    "buttonText": "点击上传",
    "limit": 9,
    "pathType": "defaultPath",
    "sortRule": [],
    "timeFormat": "YYYY",
    "folder": "",
    "tipText": "",
    "__vModel__": "FileURL"
  },
  {
    "label": "图片上传",
    "labelI18nCode": "",
    "prop": "PictureURL",
    "fixed": "none",
    "align": "left",
    "ceriKey": "uploadImg",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "PictureURL",
    "fullName": "图片上传",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "uploadImg",
      "label": "图片上传",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriUploadImg",
      "tagIcon": "icon-ym icon-ym-generator-upload",
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
      "tableName": "testallfunction",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItem5e6a0a",
      "renderKey": 1783318257563
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "disabled": false,
    "fileSize": 10,
    "sizeUnit": "MB",
    "buttonText": "点击上传",
    "limit": 9,
    "pathType": "defaultPath",
    "sortRule": [],
    "timeFormat": "YYYY",
    "folder": "",
    "tipText": "",
    "showType": "card",
    "__vModel__": "PictureURL"
  }
]
export default columnList