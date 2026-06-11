<template>
  <a-drawer
    v-model:open="drawerStore.open"
    :closable="false"
    :root-style="{ color: 'blue' }"
    style="color: rgb(72, 124, 98)"
    :title="$t('common.StatusSettings')"
    placement="right"
    @after-open-change="afterOpenChange"
    class="PublicDrawerClass"
  >
    <p>{{ $t('common.Language') }}</p>
    <p>
      <a-select
        style="width: 200px"
        :value="navigationStore.language"
        :options="languageOptions"
        :disabled="isSwitchingLanguage"
        @change="switchLanguage"
      />
    </p>
    <p>{{ $t('common.MenuWidth') }}</p>
    <a-input-number v-model:value="menuWidthValue">
      <template #addonAfter>px</template>
    </a-input-number>
  </a-drawer>
  <div v-if="isSwitchingLanguage" class="language-switch-mask">
    <a-spin size="large" />
  </div>
</template>
<script setup>
import { useDrawerStore, useNavigationStore, useUserStore } from '@/store/index.js'
import { useRouteStore } from '@/store/mainRouter'
import { loadLocaleMessages } from '@/lang/i18n.js'
import { watch, ref, reactive } from 'vue'
import { useI18n } from 'vue-i18n'
import { message } from 'ant-design-vue'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper.js'
import { useGlobalState } from '@/shared/useGlobalState'
import { getLanguageList, setDefaultLanguage } from '@/api/commonFun'

/* 是否运行在 qiankun 主应用中 */
const isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__
const { globalStore } = useGlobalState()

/* 状态管理对象 */
let navigationStore
if (isQiankun) {
  // 在 qiankun 场景下，通过一个代理对象转发到主应用的 navigationStore
  navigationStore = reactive({
    get language() {
      return globalStore.value?.navigationStore?.language ?? 'zh'
    },
    set language(val) {
      const ns = globalStore.value?.navigationStore
      if (ns && typeof ns.setLanguage === 'function') {
        ns.setLanguage(val)
      }
    },
    setLanguage(val) {
      const ns = globalStore.value?.navigationStore
      if (ns && typeof ns.setLanguage === 'function') {
        ns.setLanguage(val)
      }
    }
  })
} else {
  // 独立运行子应用时，仍然使用本地的 navigationStore
  navigationStore = useNavigationStore()
}

const drawerStore = useDrawerStore()
const userStore = useUserStore()
const routeStore = useRouteStore()
const i18n = useI18n({ useScope: 'global' })

const LANGUAGE_CACHE_KEY = 'newsubservices_language_options_cache'
const LANGUAGE_CACHE_TTL = 24 * 60 * 60 * 1000
const defaultLanguageOptions = [
  { value: 'en', label: 'English' },
  { value: 'zh', label: '简体中文' }
]
const languageOptions = ref([...defaultLanguageOptions])
const isSwitchingLanguage = ref(false)
const loadingLanguageOptions = ref(false)

const normalizeLanguageOption = (item) => {
  const value = item?.langCode ?? item?.LangCode ?? item?.code ?? item?.Code ?? item?.value ?? item?.Value
  const label = item?.langName ?? item?.LangName ?? item?.name ?? item?.Name ?? item?.label ?? item?.Label ?? value

  if (!value) return null
  return {
    value,
    label
  }
}

const extractLanguageOptions = (response) => {
  if (!response?.success || !Array.isArray(response?.data)) return []
  return response.data.map(normalizeLanguageOption).filter(Boolean)
}

const readLanguageOptionsCache = () => {
  try {
    const raw = localStorage.getItem(LANGUAGE_CACHE_KEY)
    if (!raw) return null

    const parsed = JSON.parse(raw)
    if (!Array.isArray(parsed?.options) || !parsed?.timestamp) return null
    if (Date.now() - parsed.timestamp > LANGUAGE_CACHE_TTL) return null

    return parsed.options
  } catch {
    return null
  }
}

const writeLanguageOptionsCache = (options) => {
  try {
    localStorage.setItem(
      LANGUAGE_CACHE_KEY,
      JSON.stringify({
        timestamp: Date.now(),
        options
      })
    )
  } catch {
  }
}

const loadLanguageOptions = async () => {
  if (loadingLanguageOptions.value) return

  const cachedOptions = readLanguageOptionsCache()
  if (cachedOptions?.length) {
    languageOptions.value = cachedOptions
  }

  loadingLanguageOptions.value = true
  try {
    const response = await getLanguageList()
    const options = extractLanguageOptions(response)
    if (options.length > 0) {
      languageOptions.value = options
      writeLanguageOptionsCache(options)
      return
    }
    languageOptions.value = [...defaultLanguageOptions]
  } catch {
    languageOptions.value = [...defaultLanguageOptions]
  } finally {
    loadingLanguageOptions.value = false
  }
}

const initCachedLanguageOptions = () => {
  const cachedOptions = readLanguageOptionsCache()
  if (cachedOptions?.length) {
    languageOptions.value = cachedOptions
  }
}
initCachedLanguageOptions()

/* 语言设置 */
const switchLanguage = async (language) => {
  if (isSwitchingLanguage.value) return

  const previousLanguage = i18n.locale.value || navigationStore.language || 'zh'
  if (language === previousLanguage) {
    navigationStore.setLanguage(language)
    return
  }

  isSwitchingLanguage.value = true
  try {
    const result = await setDefaultLanguage(language)
    const resultPayload = result?.data ?? result
    if (!resultPayload?.success || resultPayload?.code !== 200) {
      navigationStore.setLanguage(previousLanguage)
      i18n.locale.value = previousLanguage
      const errorMessage = resultPayload?.msg || resultPayload?.message || '切换语言失败，请稍后重试'
      message.error(errorMessage)
      return
    }

    let routeSourceData = null
    if (userStore.access_token) {
      routeSourceData = await routeStore.loadRoutes(userStore.userRoles, language)
    }

    await loadLocaleMessages(i18n, language, {
      includeRouteMessages: Boolean(userStore.access_token),
      routeSourceData
    })

    navigationStore.setLanguage(language)
    i18n.locale.value = language
  } catch (error) {
    navigationStore.setLanguage(previousLanguage)
    i18n.locale.value = previousLanguage
    const errorMessage = error?.msg || error?.message || '加载语言资源失败，请稍后重试'
    message.error(errorMessage)
  } finally {
    isSwitchingLanguage.value = false
  }
}

const afterOpenChange = (bool) => {
  drawerStore.setOpen(bool)
  if (bool) {
    loadLanguageOptions()
  }
}

/* 菜单栏宽度 */
const menuWidthValue = ref(drawerStore.menuwidth)
watch(
  menuWidthValue,
  (newVal) => {
    drawerStore.setMenuWidth(newVal)
  },
  { immediate: false }
)

</script>
<style lang="scss">
.PublicDrawerClass {
  p {
    margin: 10px 0px !important;
  }
}

.language-switch-mask {
  position: fixed;
  inset: 0;
  z-index: 3000;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(0, 0, 0, 0.35);
}
</style>
