const searchList = [
  {
    "label": "评分",
    "labelI18nCode": "",
    "prop": "rating_value",
    "ceriKey": "rate",
    "searchType": 3,
    "searchMultiple": false,
    "isKeyword": false,
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
    "label": "级联选择",
    "labelI18nCode": "",
    "prop": "cascade_text",
    "ceriKey": "cascader",
    "searchType": 1,
    "searchMultiple": false,
    "isKeyword": false,
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
  }
]
export default searchList