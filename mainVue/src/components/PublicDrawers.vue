<template>
  <a-drawer v-model:open="drawerStore.open" :closable="false" :root-style="{ color: 'blue' }"
    style="color: rgb(72, 124, 98)" :title="$t('common.StatusSettings')" placement="right"
    @after-open-change="afterOpenChange" class="PublicDrawerClass">
    <!-- <p>{{ $t('message.setting.theme') }}</p>
    <a-switch :checked="drawerStore.theme === 'dark'" checked-children="Dark" un-checked-children="Light" @change="changeTheme" />
    <p>{{ $t('message.setting.breadcrumbs') }}</p>
    <div>
      <a-switch
        :checked="drawerStore.displaybread"
        :checked-children="$t('message.setting.on')"
        :un-checked-children="$t('message.setting.off')"
        @change="changeBread"
      />
    </div>

    <p>{{ $t('message.setting.menuStyle') }}</p>
    <a-space direction="vertical">
      <a-radio-group v-model:value="drawerStore.menumode" option-type="button" :options="plainOptions" />
    </a-space> -->
    <p>{{ $t('common.Language') }}</p>
    <p>

      <!-- <a-radio-group v-model:value="navigationStore.language" name="language">
        <a-radio-button :value="'en'" @click="switchLanguage('en')"> English </a-radio-button>
        <a-radio-button :value="'zh'" @click="switchLanguage('zh')"> 中文 </a-radio-button>
      </a-radio-group>
       -->
      <a-select style="width: 200px;" :value="navigationStore.language" :options="languageOptions" :disabled="isSwitchingLanguage"
        @change="switchLanguage" />

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
import { getLanguageList, setDefaultLanguage } from '@/api/commonFun'
import { loadLocaleMessages } from '@/lang/i18n.js'
import { useI18n } from 'vue-i18n'
import { message } from 'ant-design-vue'
import { watch } from 'vue'
import { ref } from 'vue'

/* 状态管理对象 */
const navigationStore = useNavigationStore()
const drawerStore = useDrawerStore()
const userStore = useUserStore()
const routeStore = useRouteStore()

const LANGUAGE_CACHE_KEY = 'mainapp_language_options_cache'
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
    return
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
const i18n = useI18n({ useScope: 'global' })
const switchLanguage = async (language) => {
  if (isSwitchingLanguage.value) return

  const previousLanguage = i18n.locale.value
  if (language === previousLanguage) {
    navigationStore.setLanguage(language)
    return
  }

  isSwitchingLanguage.value = true
  try {
    var result = await setDefaultLanguage(language)
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
      throwOnCommonError: true,
      includeCommonMessages: true,
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
const changeTheme = (checked) => {
  drawerStore.setTheme(checked ? 'dark' : 'light')
}
const changeBread = (checked) => {
  drawerStore.setBread(checked ? true : false)
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

const plainOptions = ['vertical', 'horizontal', 'inline']
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
