// import { Input, DatePicker } from 'ant-design-vue'

import { BasicCaption } from '@/components/Basic'
import { CeriInput, CeriTextarea, CeriI18nInput } from '@/components/CeriOS/Input'
import { CeriAutoComplete } from '@/components/CeriOS/AutoComplete'
import { CeriDivider } from '@/components/CeriOS/Divider'
import { CeriSelect } from '@/components/CeriOS/Select'
import { CeriRadio } from '@/components/CeriOS/Radio'
import { CeriInputNumber } from '@/components/CeriOS/InputNumber'
import { CeriSwitch } from '@/components/CeriOS/Switch'
import { CeriCheckbox } from '@/components/CeriOS/Checkbox'
import { CeriCascader } from '@/components/CeriOS/Cascader'
import { CeriDatePicker, CeriDateRange, CeriTimePicker } from '@/components/CeriOS/DatePicker'
import { CeriUploadFile, CeriUploadImg } from '@/components/CeriOS/Upload'
import { CeriColorPicker } from '@/components/CeriOS/ColorPicker'
import { Tinymce } from '@/components/Tinymce/index'
import { CeriRate } from '@/components/CeriOS/Rate'
import { CeriSlider } from './Slider'
import { CeriLink } from '@/components/CeriOS/Link'
import { CeriButton } from '@/components/CeriOS/Button'
import { CeriAlert } from '@/components/CeriOS/Alert'
import { CeriQrcode } from '@/components/CeriOS/Qrcode'
import { CeriText } from '@/components/CeriOS/Text'
import { CeriBarcode } from '@/components/CeriOS/Barcode'
import { CeriRelationForm } from '@/components/CeriOS/RelationForm'
import { CeriRelationFormAttr } from '@/components/CeriOS/RelationFormAttr'
import { CeriCalculate } from '@/components/CeriOS/Calculate'
import { CeriIframe } from '@/components/CeriOS/Iframe'
import { CeriSign } from '@/components/CeriOS/Sign'
import { CeriSignature } from '@/components/CeriOS/Signature'
import { CeriOpenData } from '@/components/CeriOS/OpenData'

import { CeriAreaSelect } from '@/components/CeriOS/AreaSelect'
import { CeriPopupSelect, CeriPopupTableSelect } from '@/components/CeriOS/PopupSelect'
import { CeriPopupAttr } from '@/components/CeriOS/PopupAttr'
import {
  CeriOrganizeSelect,
  CeriDepSelect,
  CeriPosSelect,
  CeriGroupSelect,
  CeriRoleSelect,
  CeriUserSelect,
  CeriUsersSelect
} from '@/components/CeriOS/Organize'

import { CeriTreeSelect } from '@/components/CeriOS/TreeSelect'
const CeriEditor = Tinymce
CeriEditor.name = 'CeriEditor'

const CeriGroupTitle = BasicCaption
CeriGroupTitle.name = 'CeriGroupTitle'

export {
  CeriPopupAttr,
  CeriAreaSelect,
  CeriText,
  CeriAlert,
  CeriAutoComplete,
  CeriQrcode,
  CeriBarcode,
  CeriI18nInput,
  CeriInput,
  CeriTextarea,
  CeriDivider,
  CeriSelect,
  CeriRadio,
  CeriInputNumber,
  CeriSwitch,
  CeriCheckbox,
  CeriCascader,
  CeriDatePicker,
  CeriDateRange,
  CeriTimePicker,
  CeriUploadFile,
  CeriUploadImg,
  CeriColorPicker,
  CeriEditor,
  CeriRate,
  CeriSlider,
  CeriLink,
  CeriButton,
  CeriRelationForm,
  CeriRelationFormAttr,
  CeriPopupSelect,
  CeriPopupTableSelect,
  CeriDepSelect,
  CeriPosSelect,
  CeriUserSelect,
  CeriUsersSelect,
  CeriRoleSelect,
  CeriTreeSelect,
  CeriCalculate,
  CeriIframe,
  CeriSign,
  CeriSignature,
  CeriGroupTitle,
  CeriOpenData
}
