import { createI18n } from 'vue-i18n'
import { useNavigationStore } from '../store/navigation'
import { useGetPermissionAsync } from '../api/permission'
import { getCommonI18nValueList, getDefaultLanguage } from '../api/commonFun'
import { buildPermissionQuery } from '../utils/commonTools'
import messages from './index'

const baseMessages = JSON.parse(JSON.stringify(messages))

function isEnglishLocale(locale) {
  return `${locale || ''}`.toLowerCase().startsWith('en')
}

function resolveComposer(i18nOrComposer) {
  return i18nOrComposer?.global ?? i18nOrComposer
}

function deepClone(value) {
  return JSON.parse(JSON.stringify(value ?? {}))
}

function resolveLocaleTemplate(locale) {
  const fallbackLocale = isEnglishLocale(locale) ? 'en' : 'zh'
  const localeTemplate = baseMessages[locale] ?? baseMessages[fallbackLocale] ?? baseMessages.zh ?? baseMessages.en ?? {}
  return deepClone(localeTemplate)
}

function resolveCurrentLocaleMessage(i18nOrComposer, locale) {
  const composer = resolveComposer(i18nOrComposer)

  if (typeof composer?.getLocaleMessage === 'function') {
    const localeMessage = composer.getLocaleMessage(locale)
    if (localeMessage && typeof localeMessage === 'object') {
      return deepClone(localeMessage)
    }
  }

  if (typeof i18nOrComposer?.getLocaleMessage === 'function') {
    const localeMessage = i18nOrComposer.getLocaleMessage(locale)
    if (localeMessage && typeof localeMessage === 'object') {
      return deepClone(localeMessage)
    }
  }

  if (composer?.messages?.value?.[locale]) {
    return deepClone(composer.messages.value[locale])
  }

  if (i18nOrComposer?.messages?.value?.[locale]) {
    return deepClone(i18nOrComposer.messages.value[locale])
  }

  return null
}

function pickText(item, keys = []) {
  for (const key of keys) {
    const value = item?.[key]
    if (value !== undefined && value !== null && value !== '') return value
  }
  return ''
}

function resolveCode(item, keys = []) {
  for (const key of keys) {
    const value = item?.[key]
    if (value !== undefined && value !== null && value !== '') {
      return `${value}`
    }
  }
  return ''
}

function resolveMenuList(item) {
  if (Array.isArray(item?.menuDtos)) return item.menuDtos
  if (Array.isArray(item?.MenuDtos)) return item.MenuDtos
  return []
}

const SYSTEM_TEXT_KEYS = ['subSysName', 'SubSysName']
const MENU_TEXT_KEYS = ['functionName', 'FunctionName']

function extractRouteData(item, result = {}) {
  const sysCode = resolveCode(item, ['sysCode', 'SysCode'])

  if (!sysCode) {
    return result
  }

  result[`XT${sysCode}`] = pickText(item, SYSTEM_TEXT_KEYS)

  const menuDtos = resolveMenuList(item)
  if (menuDtos.length > 0) {
    menuDtos.forEach((menu) => {
      const functionCode = resolveCode(menu, ['functionCode', 'FunctionCode'])
      if (!functionCode) return

      result[`XT${functionCode}`] = pickText(menu, MENU_TEXT_KEYS)

      const childMenus = resolveMenuList(menu)
      if (childMenus.length > 0) {
        childMenus.forEach((subMenu) => {
          const subFunctionCode = resolveCode(subMenu, ['functionCode', 'FunctionCode'])
          if (!subFunctionCode) return

          extractRouteData(
            {
              sysCode: subFunctionCode,
              subSysName: pickText(subMenu, MENU_TEXT_KEYS),
              menuDtos: resolveMenuList(subMenu)
            },
            result
          )
        })
      }
    })
  }

  return result
}

function mergeRoutes(localeMessage, routeMessages) {
  if (!localeMessage.message) {
    localeMessage.message = {}
  }
  if (!localeMessage.message.route) {
    localeMessage.message.route = {}
  }
  Object.keys(routeMessages).forEach((key) => {
    localeMessage.message.route[key] = routeMessages[key]
  })
}

function setNestedValue(target, path, value) {
  const segments = path.split('.').filter(Boolean)
  if (!segments.length) return

  let cursor = target
  for (let index = 0; index < segments.length - 1; index += 1) {
    const key = segments[index]
    if (!cursor[key] || typeof cursor[key] !== 'object') {
      cursor[key] = {}
    }
    cursor = cursor[key]
  }
  cursor[segments[segments.length - 1]] = value
}

function extractCommonI18nList(response) {
  if (!response?.success || !Array.isArray(response?.data)) return []
  return response.data
}

function mergeCommonI18n(localeMessage, commonI18nList) {
  commonI18nList.forEach((item) => {
    const keyId = item?.keyId ?? item?.KeyId ?? item?.key ?? item?.Key ?? item?.i18nKey ?? item?.I18nKey
    const value = item?.value ?? item?.Value ?? item?.text ?? item?.Text ?? item?.content ?? item?.Content
    if (!keyId || value === undefined || value === null) return

    const normalizedKey = `${keyId}`.trim()
    if (!normalizedKey) return
    setNestedValue(localeMessage, normalizedKey, value)
  })
}

async function loadRemoteCommonMessages(locale) {
  const response = await getCommonI18nValueList(locale)
  return extractCommonI18nList(response)
}

function buildRouteMessages(permissionList) {
  const list = Array.isArray(permissionList) ? permissionList : []
  return list.reduce((acc, item) => extractRouteData(item, acc), {})
}

async function loadRemoteRouteMessages(locale, routeSourceData = null) {
  if (Array.isArray(routeSourceData)) {
    return buildRouteMessages(routeSourceData)
  }

  const obj = buildPermissionQuery({
    accurate: 1,
    roleids: [import.meta.env.VITE_SPECIALSIGN],
    langCode: locale ?? ''
  })
  const infoList = await useGetPermissionAsync(obj)
  return buildRouteMessages(infoList?.data)
}

function applyLocaleMessage(i18nOrComposer, locale, localeMessages) {
  const composer = resolveComposer(i18nOrComposer)

  if (typeof composer?.setLocaleMessage === 'function') {
    composer.setLocaleMessage(locale, localeMessages)
    return
  }

  if (typeof composer?.mergeLocaleMessage === 'function') {
    composer.mergeLocaleMessage(locale, localeMessages)
    return
  }

  if (typeof i18nOrComposer?.setLocaleMessage === 'function') {
    i18nOrComposer.setLocaleMessage(locale, localeMessages)
    return
  }

  if (typeof i18nOrComposer?.global?.setLocaleMessage === 'function') {
    i18nOrComposer.global.setLocaleMessage(locale, localeMessages)
    return
  }

  messages[locale] = localeMessages
}

export async function loadLocaleMessages(i18nOrComposer, locale, options = {}) {
  if (!i18nOrComposer || !locale) return

  const throwOnCommonError = options?.throwOnCommonError === true
  const includeCommonMessages = options?.includeCommonMessages !== false
  const includeRouteMessages = options?.includeRouteMessages !== false
  const routeSourceData = options?.routeSourceData
  const shouldRebuildLocaleMessages = includeCommonMessages && includeRouteMessages
  const currentLocaleMessages = resolveCurrentLocaleMessage(i18nOrComposer, locale)
  const localeMessages = shouldRebuildLocaleMessages
    ? resolveLocaleTemplate(locale)
    : currentLocaleMessages ?? resolveLocaleTemplate(locale)
  if (includeCommonMessages) {
    try {
      const commonI18nList = await loadRemoteCommonMessages(locale)
      mergeCommonI18n(localeMessages, commonI18nList)
    } catch (error) {
      console.warn(`[i18n] load common messages failed: ${locale}`, error)
      if (throwOnCommonError) {
        throw error
      }
    }
  }

  if (includeRouteMessages) {
    try {
      const routeMessages = await loadRemoteRouteMessages(locale, routeSourceData)
      mergeRoutes(localeMessages, routeMessages)
    } catch (error) {
      console.warn(`[i18n] load locale messages failed: ${locale}`, error)
    }
  }

  messages[locale] = localeMessages
  applyLocaleMessage(i18nOrComposer, locale, localeMessages)
}

export async function setupI18n() {
  // const { default: messages } = await import('./index') // 动态导入模块
  const navigationStore = useNavigationStore()
  // 先获取后端默认语言，确保第一次加载公共文案即使用该语言
  let initialLocale = navigationStore.language
  try {
    const resp = await getDefaultLanguage()
    const serverLang = resp?.data?.code
    if (serverLang) {
      initialLocale = serverLang
      if (navigationStore.language !== serverLang && typeof navigationStore.setLanguage === 'function') {
        navigationStore.setLanguage(serverLang)
      }
    }
  } catch (e) {
    initialLocale = initialLocale || 'zh'
  }

  const i18n = createI18n({
    legacy: false, //处理报错Uncaught (in promise) SyntaxError: Not available in legacy mode (at message-compiler.esm-bundler.js:54:19)
    locale: initialLocale, // 注意locale属性！  'zh'
    fallbackLocale: 'zh',
    messages
  })

  await loadLocaleMessages(i18n, initialLocale, {
    includeCommonMessages: true,
    includeRouteMessages: false
  })
  return i18n
}
