import { createI18n } from 'vue-i18n'
import { getCommonI18nValueList, getPageI18nValueList } from '../api/commonFun'
import { useGetPermissionAsync } from '../api/permission'

const isPlainObject = (value) => Object.prototype.toString.call(value) === '[object Object]'
const SYSTEM_TEXT_KEYS = ['subSysName', 'SubSysName']
const MENU_TEXT_KEYS = ['functionName', 'FunctionName']
let baseMessages = null
let i18nInstance = null
const pageLocaleMessagesCache = new Map()

const deepMerge = (target, source) => {
  if (!isPlainObject(target) || !isPlainObject(source)) {
    return source
  }

  const merged = { ...target }
  Object.keys(source).forEach((key) => {
    const targetValue = merged[key]
    const sourceValue = source[key]
    merged[key] = isPlainObject(targetValue) && isPlainObject(sourceValue) ? deepMerge(targetValue, sourceValue) : sourceValue
  })

  return merged
}

const deepClone = (value) => JSON.parse(JSON.stringify(value ?? {}))

const normalizeLocale = (locale) => `${locale || ''}`.trim()

const resolveComposer = (i18nOrComposer) => i18nOrComposer?.global ?? i18nOrComposer

const resolveCurrentLocaleMessage = (i18nOrComposer, locale) => {
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

const resolveCode = (item, keys = []) => {
  for (const key of keys) {
    const value = item?.[key]
    if (value !== undefined && value !== null && value !== '') {
      return `${value}`
    }
  }
  return ''
}

const pickText = (item, keys = []) => {
  for (const key of keys) {
    const value = item?.[key]
    if (value !== undefined && value !== null && value !== '') {
      return value
    }
  }
  return ''
}

const resolveMenuList = (item) => {
  if (Array.isArray(item?.menuDtos)) return item.menuDtos
  if (Array.isArray(item?.MenuDtos)) return item.MenuDtos
  return []
}

const extractRouteData = (item, result = {}) => {
  const sysCode = resolveCode(item, ['sysCode', 'SysCode'])
  if (!sysCode) return result

  result[`XT${sysCode}`] = pickText(item, SYSTEM_TEXT_KEYS)

  const menuDtos = resolveMenuList(item)
  menuDtos.forEach((menu) => {
    const functionCode = resolveCode(menu, ['functionCode', 'FunctionCode'])
    if (!functionCode) return

    result[`XT${functionCode}`] = pickText(menu, MENU_TEXT_KEYS)

    const childMenus = resolveMenuList(menu)
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
  })

  return result
}

const buildRouteMessages = (permissionList) => {
  const list = Array.isArray(permissionList) ? permissionList : []
  return list.reduce((acc, item) => extractRouteData(item, acc), {})
}

const ensureBaseMessages = async () => {
  if (baseMessages) return baseMessages
  const { default: messages } = await import('./index')
  baseMessages = messages
  return baseMessages
}

const resolveLocaleTemplate = async (locale) => {
  const messages = await ensureBaseMessages()
  const localeTemplate = messages[locale] ?? messages.zh ?? messages.en ?? {}
  return deepClone(localeTemplate)
}

const setNestedValue = (target, path, value) => {
  const segments = `${path}`.split('.').filter(Boolean)
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

const mergeCommonI18n = (localeMessage, commonI18nList) => {
  commonI18nList.forEach((item) => {
    const keyId = item?.keyId ?? item?.KeyId ?? item?.key ?? item?.Key ?? item?.i18nKey ?? item?.I18nKey
    const value = item?.value ?? item?.Value ?? item?.text ?? item?.Text ?? item?.content ?? item?.Content
    if (!keyId || value === undefined || value === null) return
    const normalizedKey = `${keyId}`.trim()
    if (!normalizedKey) return
    setNestedValue(localeMessage, normalizedKey, value)
  })
}

const mergeRoutes = (localeMessage, routeMessages) => {
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

const extractCommonI18nList = (response) => {
  if (!response?.success || !Array.isArray(response?.data)) return []
  return response.data
}

const loadRemoteCommonMessages = async (locale) => {
  const response = await getCommonI18nValueList(locale)
  return extractCommonI18nList(response)
}

const loadRemotePageMessages = async (locale, pageCode) => {
  const cacheKey = `${locale}__${pageCode}`
  if (pageLocaleMessagesCache.has(cacheKey)) {
    return pageLocaleMessagesCache.get(cacheKey)
  }

  const request = getPageI18nValueList(locale, pageCode)
    .then((response) => extractCommonI18nList(response))
    .catch((error) => {
      pageLocaleMessagesCache.delete(cacheKey)
      throw error
    })
  pageLocaleMessagesCache.set(cacheKey, request)
  return request
}

const loadRemoteRouteMessages = async (locale, routeSourceData = null) => {
  if (Array.isArray(routeSourceData)) {
    return buildRouteMessages(routeSourceData)
  }

  const query = {
    accurate: 1,
    roleids: [import.meta.env.VITE_SPECIALSIGN]
  }

  if (locale !== undefined && locale !== null && locale !== '') {
    query.langCode = locale
  }

  const response = await useGetPermissionAsync(query)
  return buildRouteMessages(response?.data)
}

const applyLocaleMessage = (i18nOrComposer, locale, localeMessages) => {
  const composer = resolveComposer(i18nOrComposer)

  if (typeof composer?.setLocaleMessage === 'function') {
    composer.setLocaleMessage(locale, localeMessages)
    return
  }

  if (typeof composer?.mergeLocaleMessage === 'function') {
    composer.mergeLocaleMessage(locale, localeMessages)
  }
}

const applySnapshotMessages = (messages, i18nSnapshot) => {
  if (!i18nSnapshot?.locale || !isPlainObject(i18nSnapshot?.messages)) {
    return messages
  }

  const snapshotLocale = i18nSnapshot.locale
  const localeMessages = messages[snapshotLocale] || {}
  return {
    ...messages,
    [snapshotLocale]: deepMerge(localeMessages, i18nSnapshot.messages)
  }
}

export async function loadLocaleMessages(i18nOrComposer, locale, options = {}) {
  if (!i18nOrComposer || !locale) return

  const normalizedLocale = normalizeLocale(locale)
  const includeCommonMessages = options?.includeCommonMessages !== false
  const includeRouteMessages = options?.includeRouteMessages !== false
  const routeSourceData = options?.routeSourceData
  const shouldRebuildLocaleMessages = includeCommonMessages && includeRouteMessages
  const currentLocaleMessages = resolveCurrentLocaleMessage(i18nOrComposer, normalizedLocale)
  const localeMessages = shouldRebuildLocaleMessages
    ? await resolveLocaleTemplate(normalizedLocale)
    : currentLocaleMessages ?? (await resolveLocaleTemplate(normalizedLocale))

  if (includeCommonMessages) {
    try {
      const commonI18nList = await loadRemoteCommonMessages(normalizedLocale)
      mergeCommonI18n(localeMessages, commonI18nList)
    } catch (error) {
      console.warn(`[platform i18n] load common messages failed: ${normalizedLocale}`, error)
    }
  }

  if (includeRouteMessages) {
    try {
      const routeMessages = await loadRemoteRouteMessages(normalizedLocale, routeSourceData)
      mergeRoutes(localeMessages, routeMessages)
    } catch (error) {
      console.warn(`[platform i18n] load route messages failed: ${normalizedLocale}`, error)
    }
  }

  applyLocaleMessage(i18nOrComposer, normalizedLocale, localeMessages)
}

export async function loadPageLocaleMessages(i18nOrComposer, locale, pageCode) {
  if (!i18nOrComposer || !locale || !pageCode) return

  const normalizedLocale = normalizeLocale(locale)
  const normalizedPageCode = `${pageCode}`.trim()
  if (!normalizedLocale || !normalizedPageCode || normalizedPageCode === 'unknown') return

  const currentLocaleMessages = resolveCurrentLocaleMessage(i18nOrComposer, normalizedLocale)
  const localeMessages = currentLocaleMessages ?? (await resolveLocaleTemplate(normalizedLocale))

  try {
    const pageI18nList = await loadRemotePageMessages(normalizedLocale, normalizedPageCode)
    mergeCommonI18n(localeMessages, pageI18nList)
    applyLocaleMessage(i18nOrComposer, normalizedLocale, localeMessages)
  } catch (error) {
    console.warn(`[platform i18n] load page messages failed: ${normalizedLocale}/${normalizedPageCode}`, error)
  }
}

export async function setupI18n(languageEx, i18nSnapshot) {
  const messages = await ensureBaseMessages()
  const mergedMessages = applySnapshotMessages(messages, i18nSnapshot)
  const targetLocale = normalizeLocale(languageEx || i18nSnapshot?.locale || 'zh')

  i18nInstance = createI18n({
    legacy: false,
    locale: targetLocale,
    messages: mergedMessages
  })

  if (!i18nSnapshot?.locale) {
    await loadLocaleMessages(i18nInstance, targetLocale, {
      includeCommonMessages: false,
      includeRouteMessages: false
    })
  }

  return i18nInstance
}

export function getI18n() {
  if (!i18nInstance) {
    throw new Error('i18n has not been initialized yet!')
  }
  return i18nInstance
}
