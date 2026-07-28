const searchList = [
  {
    "label": "单选框组",
    "labelI18nCode": "",
    "prop": "Enmu",
    "ceriKey": "radio",
    "searchType": 1,
    "searchMultiple": false,
    "isKeyword": false,
    "id": "Enmu",
    "fullName": "单选框组",
    "fullNameI18nCode": [
      ""
    ],
    "__config__": {
      "ceriKey": "radio",
      "label": "单选框组",
      "tipLabel": "",
      "showLabel": true,
      "tag": "CeriRadio",
      "tagIcon": "icon-ym icon-ym-generator-radio",
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
      "trigger": "change",
      "dataType": "static",
      "dictionaryType": "",
      "propsUrl": "",
      "propsName": "",
      "useCache": true,
      "templateJson": [],
      "formId": "formItemfe87aa",
      "renderKey": 1784872030657
    },
    "on": {
      "change": "({ data, rowIndex, formData, setFormData, setShowOrHide, setRequired, setDisabled, onlineUtils }) => {\n    // 在此编写代码\n    \n}"
    },
    "style": {
      "width": "100%"
    },
    "options": [
      {
        "fullName": "单选一",
        "id": "1"
      },
      {
        "fullName": "单选二",
        "id": "2"
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
    "__vModel__": "Enmu"
  },
  {
    "label": "级联选择",
    "labelI18nCode": "",
    "prop": "Name",
    "ceriKey": "cascader",
    "searchType": 1,
    "searchMultiple": false,
    "isKeyword": false,
    "id": "Name",
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
      "tableName": "allprops",
      "noShow": false,
      "regList": [],
      "trigger": "change",
      "dataType": "static",
      "propsUrl": "",
      "propsName": "",
      "useCache": true,
      "templateJson": [],
      "dictionaryType": "",
      "formId": "formItemd414f8",
      "renderKey": 1784872032177
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
        "id": "1",
        "fullName": "级联1",
        "children": [
          {
            "fullName": "级联1-1",
            "id": "11"
          }
        ]
      },
      {
        "fullName": "级联2",
        "id": "2",
        "children": [
          {
            "fullName": "级联2-1",
            "id": "21"
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
    "__vModel__": "Name"
  }
]
export default searchList