const columnList = [
  {
    "label": "滑块",
    "labelI18nCode": "",
    "prop": "Code",
    "fixed": "none",
    "align": "left",
    "ceriKey": "slider",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "Code",
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
      "tableName": "i18ntag",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "formId": "formItemf10846",
      "renderKey": 1781056825283
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
    "__vModel__": "Code"
  },
  {
    "label": "日期选择",
    "labelI18nCode": "",
    "prop": "CreateTime",
    "fixed": "none",
    "align": "left",
    "ceriKey": "datePicker",
    "sortable": false,
    "resizable": true,
    "width": null,
    "id": "CreateTime",
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
      "defaultCurrent": false,
      "required": false,
      "layout": "colFormItem",
      "span": 24,
      "dragDisabled": false,
      "visibility": [
        "pc",
        "app"
      ],
      "tableName": "i18ntag",
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
      "formId": "formItem955cd2",
      "renderKey": 1781056841947
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
  }
]
export default columnList