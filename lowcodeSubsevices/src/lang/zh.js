import zhCNJson from '@/locales/lang/zh_CN.json'

const zh = {
  message: {
    /* 路由相关 */
    route: {
      homePage: '首页',
      ComprehensiveHomePage: '综合首页',
      RecentlyUsed: '最近使用',
      MyCollection: '我的收藏',
      MenuList: '菜单列表',
      Guidebook: '指导手册'
    },
    /* 各个弹窗标题相关 */
    modelTitle: {
      statusSettings: '状态设置'
    },
    /* 设置弹出抽屉相关 */
    setting: {
      language: '语言',
      theme: '主题',
      breadcrumbs: '面包屑',
      menuStyle: '菜单样式',
      off: '隐藏',
      on: '显示',
      menuWidth: '菜单栏宽度（样式非horizontal生效）'
    },
    drawer: {
      SaveAndClose: '保存并关闭',
      Save: '保存',
      Cancel: '取消',
      ChangePassword: '修改密码',
      TotalOf: '共',
      Items: '条'
    }
  }
}

function isPlainObject(value) {
  return Object.prototype.toString.call(value) === '[object Object]'
}

function deepMerge(target, source) {
  if (!isPlainObject(target) || !isPlainObject(source)) {
    return source
  }

  const merged = { ...target }
  for (const key in source) {
    if (Object.prototype.hasOwnProperty.call(source, key)) {
      merged[key] = deepMerge(merged[key], source[key])
    }
  }
  return merged
}

export default deepMerge(zh, zhCNJson)
