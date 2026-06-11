import type { App } from 'vue'

import { Button } from './Button'

import {
  Input,
  InputNumber,
  Layout,
  Form,
  Switch,
  Dropdown,
  Menu,
  Select,
  Table,
  Checkbox,
  Tabs,
  Collapse,
  Card,
  Tooltip,
  Row,
  Col,
  Popconfirm,
  Divider,
  Alert,
  AutoComplete,
  Cascader,
  Rate,
  Slider,
  Avatar,
  Tag,
  Space,
  Steps,
  Popover,
  Radio,
  Progress,
  Image,
  Upload
} from 'ant-design-vue'

import { BasicHelp, BasicCaption } from '@/components/Basic'
import { CeriSelect } from '@/components/CeriOS/Select'
import { CeriSwitch } from '@/components/CeriOS/Switch';
import { CeriInput, CeriTextarea, CeriI18nInput } from '@/components/CeriOS/Input'
import { CeriRadio } from '@/components/CeriOS/Radio'
import { CeriTreeSelect } from '@/components/CeriOS/TreeSelect'
import { CeriCheckbox, CeriCheckboxSingle } from '@/components/CeriOS/Checkbox'
import { CeriInputNumber } from '@/components/CeriOS/InputNumber'
import { CeriEmpty } from '@/components/CeriOS/Empty'
import { CeriDatePicker, CeriDateRange, CeriTimePicker, CeriTimeRange } from '@/components/CeriOS/DatePicker';
import { CeriIconPicker } from '@/components/CeriOS/IconPicker';
import { CeriColorPicker } from './CeriOS'
import { CeriCalculate } from '@/components/CeriOS/Calculate';
import { CeriTextTag } from '@/components/CeriOS/TextTag';
import { CeriIframe } from '@/components/CeriOS/Iframe';
import { CeriSign } from '@/components/CeriOS/Sign';
import { CeriSignature } from '@/components/CeriOS/Signature';
import { CeriDivider } from '@/components/CeriOS/Divider';
import { CeriOpenData } from '@/components/CeriOS/OpenData';
import { CeriAreaSelect } from '@/components/CeriOS/AreaSelect';
import { CeriPopupSelect, CeriPopupTableSelect } from '@/components/CeriOS/PopupSelect';
import { CeriPopupAttr } from '@/components/CeriOS/PopupAttr';
import { CeriQrcode } from '@/components/CeriOS/Qrcode';

const CeriGroupTitle = BasicCaption;
CeriGroupTitle.name = 'CeriGroupTitle';

export function registerGlobComp(app: App) {
  app
    .use(CeriPopupAttr)
    .use(CeriPopupSelect)
    .use(CeriPopupTableSelect)
    .use(Input)
    .use(InputNumber)
    .use(Button)
    .use(Layout)
    .use(Form)
    .use(Switch)
    .use(Dropdown)
    .use(Menu)
    .use(CeriCalculate)
    .use(Select)
    .use(Table)
    .use(Checkbox)
    .use(Tabs)
    .use(Card)
    .use(Collapse)
    .use(Tooltip)
    .use(Row)
    .use(Col)
    .use(Popconfirm)
    .use(Popover)
    .use(Divider)
    .use(Slider)
    .use(Rate)
    .use(Alert)
    .use(AutoComplete)
    .use(Cascader)
    .use(Avatar)
    .use(Tag)
    .use(CeriTextTag)
    .use(CeriSwitch)
    .use(Space)
    .use(Steps)
    .use(Radio)
    .use(Progress)
    .use(Image)
    .use(Upload)
    .use(BasicHelp)
    .use(CeriInput)
    .use(CeriI18nInput)
    .use(CeriTextarea)
    .use(CeriSelect)
    .use(CeriRadio)
    .use(CeriTreeSelect)
    .use(CeriInputNumber)
    .use(CeriCheckbox)
    .use(CeriCheckboxSingle)
    .use(CeriEmpty)
    .use(CeriGroupTitle)
    .use(CeriIconPicker)
    .use(CeriDatePicker)
    .use(CeriDateRange)
    .use(CeriTimePicker)
    .use(CeriTimeRange)
    .use(CeriColorPicker)
    .use(CeriIframe)
    .use(CeriSign)
    .use(CeriSignature)
    .use(CeriDivider)
    .use(CeriOpenData)
    .use(CeriAreaSelect)
    .use(CeriQrcode)
}
