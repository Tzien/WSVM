const columnList = [
  {
    "label": "级联选择",
    "labelI18nCode": "",
    "prop": "cascade_text",
    "fixed": "none",
    "align": "left",
    "ceriKey": "cascader",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "cascade_text",
    "fullName": "级联选择",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "cascader",
      "label": "级联选择",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriCascader",
      "tagIcon": "icon-ym icon-ym-generator-cascader",
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
      "tableName": "control_bind_demo",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "dataType": "static",
      "propsUrl": "",
      "propsName": "",
      "useCache": true,
      "templateJson": [],
      "dictionaryType": "",
      "formId": "formItemc42ac9",
      "renderKey": 1778485994727
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}",
      "blur": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "options": [
      {
        "fullName": "A",
        "id": "a",
        "children": [
          {
            "fullName": "A1",
            "id": "a1"
          }
        ]
      },
      {
        "fullName": "B",
        "id": "b",
        "children": [
          {
            "fullName": "B1",
            "id": "b1"
          }
        ]
      }
    ],
    "props": {
      "value": "id",
      "label": "fullName",
      "children": "children"
    },
    "placeholder": "请选择",
    "disabled": false,
    "clearable": true,
    "filterable": false,
    "multiple": false,
    "__vModel__": "cascade_text"
  },
  {
    "label": "日期选择",
    "labelI18nCode": "",
    "prop": "date_selected",
    "fixed": "none",
    "align": "left",
    "ceriKey": "datePicker",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "date_selected",
    "fullName": "日期选择",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "datePicker",
      "label": "日期选择",
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
      "tableName": "control_bind_demo",
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
      "formId": "formItem17685b",
      "renderKey": 1778485983543
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
    "__vModel__": "date_selected"
  },
  {
    "label": "时间选择",
    "labelI18nCode": "",
    "prop": "datetime_selected",
    "fixed": "none",
    "align": "left",
    "ceriKey": "timePicker",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "datetime_selected",
    "fullName": "时间选择",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "timePicker",
      "label": "时间选择",
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
      "tableName": "control_bind_demo",
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
      "formId": "formItemade3d1",
      "renderKey": 1778486039927
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
    "__vModel__": "datetime_selected"
  },
  {
    "label": "文件上传",
    "labelI18nCode": "",
    "prop": "file_url",
    "fixed": "none",
    "align": "left",
    "ceriKey": "uploadFile",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "file_url",
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
      "tableName": "control_bind_demo",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItema77141",
      "renderKey": 1779331579954
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
    "__vModel__": "file_url"
  },
  {
    "label": "图片上传",
    "labelI18nCode": "",
    "prop": "images",
    "fixed": "none",
    "align": "left",
    "ceriKey": "uploadImg",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "images",
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
      "tableName": "control_bind_demo",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItemc069e1",
      "renderKey": 1779331580514
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
    "__vModel__": "images"
  },
  {
    "label": "评分",
    "labelI18nCode": "",
    "prop": "rating_value",
    "fixed": "none",
    "align": "left",
    "ceriKey": "rate",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "rating_value",
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
      "tableName": "control_bind_demo",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItem1af757",
      "renderKey": 1779331581490
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "count": 5,
    "allowHalf": false,
    "disabled": false,
    "__vModel__": "rating_value"
  },
  {
    "label": "滑块",
    "labelI18nCode": "",
    "prop": "slider_int_value",
    "fixed": "none",
    "align": "left",
    "ceriKey": "slider",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "slider_int_value",
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
      "tableName": "control_bind_demo",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItemf32576",
      "renderKey": 1779331581945
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
    "__vModel__": "slider_int_value"
  }
]
export default columnList