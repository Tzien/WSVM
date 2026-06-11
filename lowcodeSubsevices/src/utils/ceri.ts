import { useUserStore } from '@/store'
import type { UserInfo } from '@/types/store'
// import { useUserStoreWithOut } from '@/store/modules/user';
import router from '@/router';
import { useMessage } from '@/hooks/web/useMessage';
import { isString, isNumber } from '@/utils/is';
import { cloneDeep } from 'lodash-es';

interface OnlineUserInfo {
  userid: string
  loginname: string
  token?: string
}

export function getScriptFunc(str) {
  let func = null
  try {
    func = eval(str)
    if (Object.prototype.toString.call(func) !== '[object Function]') return false
    return func
  } catch (_) {
    return false
  }
}

export function getDateFormat(format) {
  if (!format) return 'YYYY-MM-DD HH:mm:ss';
  const formatObj = {
    yyyy: 'YYYY',
    'yyyy-MM': 'YYYY-MM',
    'yyyy-MM-dd': 'YYYY-MM-DD',
    'yyyy-MM-dd HH:mm': 'YYYY-MM-DD HH:mm',
    'yyyy-MM-dd HH:mm:ss': 'YYYY-MM-DD HH:mm:ss',
    YYYY: 'YYYY',
    'YYYY-MM': 'YYYY-MM',
    'YYYY-MM-DD': 'YYYY-MM-DD',
    'YYYY-MM-DD HH:mm': 'YYYY-MM-DD HH:mm',
    'YYYY-MM-DD HH:mm:ss': 'YYYY-MM-DD HH:mm:ss',
  };
  return formatObj[format] || 'YYYY-MM-DD HH:mm:ss';
}

export function getParamList(templateJson, data?, rowKey = 'id') {
  if (!templateJson?.length) return []
  for (let i = 0; i < templateJson.length; i++) {
    const e = templateJson[i]
    if (e.sourceType == 1 && data) {
      e.defaultValue = data[e.relationField] || data[e.relationField] == 0 || data[e.relationField] == false ? data[e.relationField] : ''
    }
    if (e.sourceType == 4 && e.relationField == '@formId') e.defaultValue = data[rowKey] || ''
  }
  return templateJson
}

export function getAmountChinese(val) {
  if (!val && val !== 0) return '';
  if (val == 0) return '零元整';
  const regExp = /[a-zA-Z]/;
  if (regExp.test(val)) return '数字较大溢出';
  let amount = +val;
  if (isNaN(amount)) return '';
  if (amount < 0) amount = Number(amount.toString().split('-')[1]);
  const NUMBER = ['零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖'];
  const N_UNIT1 = ['', '拾', '佰', '仟'];
  const N_UNIT2 = ['', '万', '亿', '兆'];
  const D_UNIT = ['角', '分', '厘', '毫'];
  let [integer, decimal] = amount.toString().split('.');
  if (integer && (integer.length > 15 || integer.indexOf('e') > -1)) return '数字较大溢出';
  let res = '';
  // 整数部分
  if (integer) {
    let zeroCount = 0;
    for (let i = 0, len = integer.length; i < len; i++) {
      const num = integer.charAt(i);
      const pos = len - i - 1; // 排除个位后 所处的索引位置
      const q = pos / 4;
      const m = pos % 4;
      if (num === '0') {
        zeroCount++;
      } else {
        if (zeroCount > 0 && m !== 3) res += NUMBER[0];
        zeroCount = 0;
        res += NUMBER[parseInt(num)] + N_UNIT1[m];
      }
      if (m == 0 && zeroCount < 4) res += N_UNIT2[Math.floor(q)];
    }
  }
  if (Number(integer) != 0) res += '元';
  // 小数部分
  if (parseInt(decimal)) {
    for (let i = 0; i < 4; i++) {
      const num = decimal.charAt(i);
      if (parseInt(num)) res += NUMBER[num] + D_UNIT[i];
    }
  } else {
    res += '整';
  }
  if (val < 0) res = '负数' + res;
  return res;
}
// 转千位分隔
export function thousandsFormat(num) {
  if (num === 0) return '0';
  if (!num) return '';
  const numArr = num.toString().split('.');
  numArr[0] = numArr[0].replace(/\B(?=(\d{3})+(?!\d))/g, ',');
  return numArr.join('.');
}
export function toDecimal(num: number = 0) {
  const sign = num == (num = Math.abs(num));
  num = Math.floor(num * 100 + 0.50000000001);
  const cents = num % 100;
  let value: string = Math.floor(num / 100).toString();
  const centsStr: string = cents < 10 ? '0' + cents : cents.toString();
  for (let i = 0; i < Math.floor((value.length - (1 + i)) / 3); i++)
    value = value.substring(0, value.length - (4 * i + 3)) + '' + value.substring(value.length - (4 * i + 3));
  return (sign ? '' : '-') + value + '.' + centsStr;
}

export function toFileSize(size) {
  if (size == null || size == '') return '';
  if (size < 1024) return toDecimal(size) + ' 字节';
  else if (size >= 1024 && size < 1048576) return toDecimal(size / 1024) + ' KB';
  else if (size >= 1048576 && size < 1073741824) return toDecimal(size / 1024 / 1024) + ' MB';
  else if (size >= 1073741824) return toDecimal(size / 1024 / 1024 / 1024) + ' GB';
}

export function getDateTimeUnit(format) {
  if (format == 'YYYY' || format == 'yyyy') return 'year'
  if (format == 'YYYY-MM' || format == 'yyyy-MM') return 'month'
  if (format == 'YYYY-MM-DD' || format == 'yyyy-MM-dd') return 'day'
  if (format == 'YYYY-MM-DD HH:mm' || format == 'yyyy-MM-dd HH:mm') return 'minute'
  if (format == 'YYYY-MM-DD HH:mm:ss' || format == 'yyyy-MM-dd HH:mm:ss') return 'second'
  return 'day'
}
export function getTimeUnit(key) {
  if (key == 1) return 'year'
  if (key == 2) return 'month'
  if (key == 3) return 'day'
  if (key == 4) return 'hour'
  if (key == 5) return 'minute'
  if (key == 6) return 'second'
  return 'day'
}

export const onlineUtils = {
  // 获取用户信息
  getUserInfo() {
    const userStore = useUserStore()
    const userInfo: OnlineUserInfo = userStore.getUserInfo()
    userInfo.token = userStore.access_token
    return userInfo
  },
  // 获取设备信息
  getDeviceInfo() {
    const deviceInfo = { vueVersion: '3', origin: 'pc' }
    return deviceInfo
  },
  // 请求
  // request(url: string, method: string, data = {}, headers = {}) {
  //   return defHttp[method ? method.toLowerCase() : 'get']({ url, data, headers })
  // },
  // 路由跳转
  route(url: string) {
    if (!url) return
    router.push(url)
  },
  // 消息提示
  toast(message: string | number, type: string = 'info', duration: number = 3000) {
    const { createMessage } = useMessage()
    if (!isString(message) && !isNumber(message)) return
    const newDuration = duration / 1000
    const config = { content: message, type, duration: newDuration }
    createMessage[type] && createMessage[type](config)
  }
}

// 数据类型转换
export function getDataTypeText(val) {
  let text = val;
  switch (val) {
    case 'varchar':
      text = '字符串';
      break;
    case 'int':
      text = '整型';
      break;
    case 'datetime':
      text = '日期时间';
      break;
    case 'decimal':
      text = '浮点';
      break;
    case 'bigint':
      text = '长整型';
      break;
    case 'text':
      text = '文本';
      break;
    default:
      text = val;
      break;
  }
  return text;
}
export function getLaunchFlowParamList(transferList, data?, rowKey = 'id') {
  transferList = cloneDeep(transferList);
  if (!transferList?.length) return [];
  for (let i = 0; i < transferList.length; i++) {
    const e = transferList[i];
    if (e.sourceType == 1) {
      if (e.sourceValue == '@formId') {
        e.sourceValue = data[rowKey] || '';
      } else {
        e.sourceValue = data[e.sourceValue] || data[e.sourceValue] == 0 || data[e.sourceValue] == false ? data[e.sourceValue] : '';
      }
    }
  }
  return transferList;
}
