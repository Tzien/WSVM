import type { Component } from 'vue'
import type { ComponentType } from './types/index'

const componentMap = new Map<ComponentType, Component>()

/**
 * Component list, register here to setting it in the form
 */
import { StrengthMeter } from '@/components/StrengthMeter'
import { CountdownInput } from '@/components/CountDown'

// ceri 组件
import {
  CeriInput,
  CeriSelect,
  CeriTextarea,
  CeriInputNumber,
  CeriSwitch,
  CeriRadio,
  CeriCheckbox,
  CeriCascader,
  CeriDatePicker,
  CeriDateRange,
  CeriDepSelect,
  CeriPosSelect,
  CeriRoleSelect,
  CeriTreeSelect,
  CeriPopupTableSelect,
  CeriUserSelect,
  CeriUsersSelect,
  CeriAutoComplete,
  CeriTimePicker,
  CeriUploadFile,
  CeriUploadImg,
  CeriColorPicker,
  CeriEditor,
  CeriRate,
  CeriSlider,
  CeriLink,
  CeriButton,
  CeriAlert,
  CeriText,
  CeriCalculate,
  CeriQrcode,
  CeriBarcode,
  CeriIframe,
  CeriSign,
  CeriSignature,
  CeriGroupTitle,
  CeriDivider,
  CeriOpenData,
  CeriAreaSelect,
  CeriRelationForm,
  CeriRelationFormAttr,
  CeriPopupSelect,
  CeriPopupAttr
} from '@/components/CeriOS'

// componentMap.set('StrengthMeter', StrengthMeter)
// componentMap.set('InputCountDown', CountdownInput)

// componentMap.set('InputGroup', CeriInputGroup)
// componentMap.set('InputSearch', CeriInputSearch)
// componentMap.set('MonthPicker', CeriMonthPicker)
// componentMap.set('WeekPicker', CeriWeekPicker)

componentMap.set('Alert', CeriAlert)
componentMap.set('AreaSelect', CeriAreaSelect)
componentMap.set('AutoComplete', CeriAutoComplete)
componentMap.set('Button', CeriButton)
// componentMap.set('Cron', CeriCron)
componentMap.set('Cascader', CeriCascader)
componentMap.set('ColorPicker', CeriColorPicker)
componentMap.set('Checkbox', CeriCheckbox)
// componentMap.set('CeriCheckboxSingle', CeriCheckboxSingle)
componentMap.set('DatePicker', CeriDatePicker)
// componentMap.set('DateRange', CeriDateRange)
componentMap.set('TimePicker', CeriTimePicker)
// componentMap.set('TimeRange', CeriTimeRange)
componentMap.set('Divider', CeriDivider)
componentMap.set('Editor', CeriEditor)
componentMap.set('GroupTitle', CeriGroupTitle)
componentMap.set('Input', CeriInput)
// componentMap.set('InputPassword', CeriInputPassword)
componentMap.set('Textarea', CeriTextarea)
componentMap.set('InputNumber', CeriInputNumber)
// componentMap.set('IconPicker', CeriIconPicker)
componentMap.set('Link', CeriLink)
// componentMap.set('OrganizeSelect', CeriOrganizeSelect)
componentMap.set('DepSelect', CeriDepSelect)
componentMap.set('PosSelect', CeriPosSelect)
// componentMap.set('GroupSelect', CeriGroupSelect)
componentMap.set('RoleSelect', CeriRoleSelect)
componentMap.set('UserSelect', CeriUserSelect)
componentMap.set('UsersSelect', CeriUsersSelect)
componentMap.set('Qrcode', CeriQrcode)
componentMap.set('Barcode', CeriBarcode)
componentMap.set('Radio', CeriRadio)
componentMap.set('Rate', CeriRate)
componentMap.set('Select', CeriSelect)
componentMap.set('Slider', CeriSlider)
componentMap.set('Signature', CeriSignature)
componentMap.set('Switch', CeriSwitch)
componentMap.set('Text', CeriText)
componentMap.set('TreeSelect', CeriTreeSelect)
componentMap.set('UploadFile', CeriUploadFile)
componentMap.set('UploadImg', CeriUploadImg)
// componentMap.set('UploadImgSingle', CeriUploadImgSingle)
// componentMap.set('BillRule', CeriInput)
componentMap.set('ModifyUser', CeriInput)
componentMap.set('ModifyTime', CeriInput)
componentMap.set('CreateUser', CeriOpenData)
componentMap.set('CreateTime', CeriOpenData)
componentMap.set('CurrOrganize', CeriOpenData)
componentMap.set('CurrPosition', CeriOpenData)
componentMap.set('RelationForm', CeriRelationForm)
componentMap.set('RelationFormAttr', CeriRelationFormAttr)
componentMap.set('PopupSelect', CeriPopupSelect)
componentMap.set('PopupTableSelect', CeriPopupTableSelect)
componentMap.set('PopupAttr', CeriPopupAttr)
// componentMap.set('NumberRange', CeriNumberRange)
componentMap.set('Calculate', CeriCalculate)
// componentMap.set('InputTable', CeriInputTable)
// componentMap.set('Location', CeriLocation)
componentMap.set('Iframe', CeriIframe)
componentMap.set('Sign', CeriSign);

export function add(compName: ComponentType, component: Component) {
  componentMap.set(compName, component)
}

export function del(compName: ComponentType) {
  componentMap.delete(compName)
}

export { componentMap }
