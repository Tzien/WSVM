<template>
  <div class="login-container" :style="{ backgroundImage: `url(${loginBg})` }">
    <!-- 左侧图片区域 -->
    <div class="login-image-area">
      <div class="login-image-bg">
        <div class="login-image-content">
          <!-- Logo区域 -->
          <div class="login-logo" @click="handleLogoClick">
            <div class="login-logo-content">
              <img :src="newLogoA" alt="Logo" class="login-logo-img animate-fade-in" />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- 右侧登录表单 -->
    <div class="login-form-panel animate-slide-in-right">
      <div class="loginContent">
      <div class="logo"></div>
      <div class="WelcomeMessageBox">欢迎使用{{ drawerStore.platformName }}</div>
      <div class="loginFormContent">
        <div style="margin-top: 130px">
          <div class="platformNameBox">{{ drawerStore.platformName }}</div>
          <div class="formLoginTitle">登&emsp;录</div>
          <div style="color: #000; font-size: 24px; margin: 82px 0px 14px 130px; margin-top: 82px">账号密码登录</div>
          <a-form class="from" :model="formState" name="basic" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
            <a-form-item class="formItem1" label="" name="username" :rules="[{ required: true, message: '用户账号必填!' }]">
              <a-input style="height: 64px; font-size: 20px" v-model:value="formState.username" placeholder="请输入用户账号">
                <template #prefix><UserOutlined style="color: rgba(0, 0, 0, 0.25); font-size: 20px; margin-right: 10px" /></template>
              </a-input>
            </a-form-item>
            <a-form-item class="formItem" label="" name="password" :rules="[{ required: true, message: '用户密码必填!' }]">
              <a-input-password style="height: 64px; font-size: 20px" v-model:value="formState.password" placeholder="请输入用户密码">
                <template #prefix><LockOutlined style="color: rgba(0, 0, 0, 0.25); font-size: 20px; margin-right: 10px" /></template>
              </a-input-password>
            </a-form-item>
            <a-form-item style="text-align: center; margin-top: 68px">
              <a-button class="loginbtn" type="primary" html-type="submit">登&emsp;录</a-button>
              <div class="forgetTitle">忘记密码请联系管理员</div>
            </a-form-item>
          </a-form>
        </div>
      </div>
    </div>

      <!-- 版权信息 -->
      <div class="login-copyright animate-fade-in-up" style="animation-delay: 0.6s">
        <p class="text-xs text-gray-500 dark:text-gray-400">{{ coptRightTitle }}.</p>
      </div>
    </div>
  </div>
</template>
<script setup>
import { reactive, ref, onMounted, nextTick } from 'vue'
import CryptoJS from 'crypto-js'
import { message } from 'ant-design-vue'
import { useUserStore, useRouteStore, useNavigationStore, useSignalRStore, useDrawerStore } from '../store/index'
import { usePassWordLogin } from '../api/user'
import { useRouter } from 'vue-router'
import { useInsertSysLoginUserLogAsync } from '../api/userLoginLogs'
import { useGetPermissionAsync } from '../api/permission'
import { buildPermissionQuery } from '../utils/commonTools'
import { getPlatformNameApi } from '@/api/sysinfo.js'
import loginBg from '@/assets/images/login_bg.png'
import newLogoA from '@/assets/images/NewLogoA.png'
import loadingBg from '@/assets/images/LoginImg/BG.png'
import CImg from '@/assets/images/LoginImg/C.png'
import EImg from '@/assets/images/LoginImg/E.png'
import RImg from '@/assets/images/LoginImg/R.png'
import I3Img from '@/assets/images/LoginImg/i3.png'
import DImg from '@/assets/images/LoginImg/D1.png'
import I1Img from '@/assets/images/LoginImg/i1.png'
import g1Img from '@/assets/images/LoginImg/g1.png'
import I2Img from '@/assets/images/LoginImg/i2.png'
import t1Img from '@/assets/images/LoginImg/t1.png'
import a1Img from '@/assets/images/LoginImg/a1.png'
import l1Img from '@/assets/images/LoginImg/l1.png'
import { getNoReadMsgApi } from '../api/Msg/msg'
import { UserOutlined, LockOutlined } from '@ant-design/icons-vue'

const userStore = useUserStore()
const routeStore = useRouteStore()
const navigationStore = useNavigationStore()
const drawerStore = useDrawerStore()
const router = useRouter()
const signalRStore = useSignalRStore()

// 响应式变量
const loading = ref(false)
const rememberMe = ref(false)
let latestRoutePermissionSystems = []

function GetPlatformName() {
  getPlatformNameApi().then((res) => {
    if (res.code == 200 && res.success) {
      drawerStore.SetPlatformName(res.data)
      return
    }
    message.warn('获取系统名称异常，请联系管理员')
  })
}
GetPlatformName()

const coptRightTitle = import.meta.env.VITE_SYSCOPYRIGHT

const loginData = {
  ClientId: 'znpt',
  ClientSecret: 'secret',
  RememberLogin: false,
  LoginName: '',
  Password: ''
}

function base64UrlToUtf8Json_legacy(b64url) {
  let b64 = b64url.replace(/-/g, '+').replace(/_/g, '/')
  const pad = b64.length % 4
  if (pad) b64 += '='.repeat(4 - pad)

  // 关键：先 escape 再 decodeURIComponent
  const jsonStr = decodeURIComponent(escape(window.atob(b64)))
  return JSON.parse(jsonStr)
}

const fetchData = async (loginname) => {
  const key = CryptoJS.enc.Utf8.parse('1234567890abcdef')
  const iv = CryptoJS.enc.Utf8.parse('1234567890abcdef')
  const pass = CryptoJS.AES.encrypt(loginData.Password, key, { iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 })
  loginData.Password = pass.toString()
  const tokenData = await usePassWordLogin(loginData)
  if (tokenData.code === 200 && tokenData.success) {
    let access_token = tokenData.data.access_token.split('.')
    // let statement = JSON.parse(decodeURIComponent(window.atob(access_token[1].replace(/-/g, '+').replace(/_/g, '/'))))
    let statement = base64UrlToUtf8Json_legacy(access_token[1])
    if (typeof statement.role === 'string') {
      statement.role = [statement.role]
    }
    // 立即显示动画，防止被后续操作干扰
    loading.value = false

    // 直接创建原生DOM动画，确保显示
    const loadingDiv = document.createElement('div')
    loadingDiv.className = 'loading-overlay-native'
    loadingDiv.style.cssText = `position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background-color: #ffffff; background-image: url(${loadingBg}); background-repeat: no-repeat; background-position: center center; background-size: cover; display: flex; flex-direction: column; align-items: center; justify-content: center; z-index: 99999; overflow: hidden;`

    loadingDiv.innerHTML = `
      <div style="width: 120px; height: 120px; display: flex; align-items: flex-end; justify-content: center; margin-bottom: 30px; animation: logoFloat 3s ease-in-out infinite;">
        <div style="display: flex; align-items: flex-end; justify-content: center; gap: 0px; padding: 0 4px;">
          <img src="${CImg}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0s;" />
          <img src="${EImg}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.06s;" />
          <img src="${RImg}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.12s;" />
          <img src="${I3Img}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.18s;" />
          <img src="${DImg}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.24s;" />
          <img src="${I1Img}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.3s;" />
          <img src="${g1Img}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.36s;" />
          <img src="${I2Img}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.42s;" />
          <img src="${t1Img}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.48s;" />
          <img src="${a1Img}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.54s;" />
          <img src="${l1Img}" style="height: 26px; animation: logoRotate 1.2s ease-in-out infinite; animation-delay: 0.6s;" />
        </div>
      </div>
      <div style="color: #4E5969; font-size: 24px; font-weight: 600; margin-bottom: 10px; opacity: 0; animation: textFadeIn 1s ease-out 0.5s forwards;">${
        drawerStore.platformName || '系统管理平台'
      }</div>
      <div style="color: #4E5969; font-size: 16px; opacity: 0; animation: textFadeIn 1s ease-out 1s forwards;">正在加载系统资源...</div>
      <div style="width: 200px; height: 4px; background: #98B0DA; border-radius: 2px; margin-top: 30px; overflow: hidden; opacity: 0; animation: textFadeIn 1s ease-out 1.5s forwards;">
        <div style="height: 100%; background: linear-gradient(90deg, #0061D4, #0061D4); border-radius: 2px; width: 0%; animation: progressFill 2s ease-out 2s forwards;"></div>
      </div>
    `

    // 添加动画关键帧
    const style = document.createElement('style')
    style.textContent = `
      @keyframes logoFloat {
        0%, 100% { transform: translateY(0px) scale(1); }
        50% { transform: translateY(-10px) scale(1.05); }
      }
      @keyframes logoRotate {
        0%, 100% { transform: translateY(0); }
        80% { transform: translateY(20px); }
      }
      @keyframes textFadeIn {
        from { opacity: 0; transform: translateY(20px); }
        to { opacity: 1; transform: translateY(0); }
      }
      @keyframes progressFill {
        from { width: 0%; }
        to { width: 100%; }
      }
    `
    document.head.appendChild(style)

    document.body.appendChild(loadingDiv)

    // 防止页面滚动
    document.body.style.overflow = 'hidden'

    // 将引用挂到全局，供后续根据加载完成情况主动移除
    window.__MAIN_LOGIN_LOADING__ = {
      loadingDiv,
      style,
      canRemove: false,
      minTimeReached: false
    }

    // 5秒后移除遮罩（路由跳转交给业务逻辑处理，这里只负责清理 DOM）
    setTimeout(() => {
      const holder = window.__MAIN_LOGIN_LOADING__
      if (!holder) {
        return
      }
      holder.minTimeReached = true
      if (!holder.canRemove) {
        return
      }
      const { loadingDiv: div, style: styleEl } = holder
      if (div && document.body.contains(div)) {
        document.body.removeChild(div)
      }
      if (styleEl && document.head.contains(styleEl)) {
        document.head.removeChild(styleEl)
      }
      document.body.style.overflow = ''
      window.__MAIN_LOGIN_LOADING__ = null
    }, 5000)

    // 使用nextTick确保DOM更新后再执行后续逻辑
    nextTick(async () => {
      try {
        // 设置用户信息
        userStore.setUserMessage(
          statement.realname,
          statement.ptid,
          loginname,
          tokenData.data.access_token,
          tokenData.data.refresh_token,
          tokenData.data.expires_in,
          statement.exp, //秒级
          statement.role
        )

        // 获取动态路由
        const routePermissionSystems = await routeStore.loadRoutes(userStore.userRoles)
        latestRoutePermissionSystems = Array.isArray(routePermissionSystems) ? routePermissionSystems : []
        // 动态添加路由
        routeStore.routes.forEach((route) => {
          router.addRoute(route)
        })

        // 重置tabs
        navigationStore.removeAllTab()

        // 异步处理其他操作
        useInsertSysLoginUserLogAsync({ CreateId: statement.ptid, LoginName: loginname }).then((data) => {
          if (data.code === 200 && data.success) {
            userStore.setlogid(data.data)
          }
        })

        // 告诉后端要登录了
        signalRStore.connection.invoke('Login', statement.sub) //发送消息
        // 检查是否有未读消息
        signalRStore.setIdsUserId(statement.sub)
        signalRStore.setPtUserId(statement.ptid)
        const query = {
          userId: statement.sub
        }
        getNoReadMsgApi(query).then((res) => {
          if (res.code === 200 && res.success) {
            signalRStore.setMsgCount(res.total)
          }
        })

        // 调用动画处理函数
        showLoginSuccessAnimation()
      } catch (error) {
        console.error('处理登录后逻辑失败:', error)
        // 即使出错也要隐藏动画
        const holder = window.__MAIN_LOGIN_LOADING__
        if (holder) {
          holder.canRemove = true
          const { loadingDiv, style } = holder
          if (loadingDiv && document.body.contains(loadingDiv)) {
            document.body.removeChild(loadingDiv)
          }
          if (style && document.head.contains(style)) {
            document.head.removeChild(style)
          }
          document.body.style.overflow = ''
          window.__MAIN_LOGIN_LOADING__ = null
        }
        setTimeout(() => {
          router.push({ path: '/' })
        }, 3000)
      }
    })
  } else {
    message.error(tokenData.message)
    loading.value = false
  }
}
const formState = reactive({
  username: '',
  password: ''
})

// 新增的方法
function handleLogoClick() {
  console.log('Logo被点击')
  showLoadingAnimation()
}

function showLoadingAnimation() {
  // Logo点击后的效果
  message.info('欢迎使用' + (drawerStore.platformName || '系统管理平台'))
}

async function showLoginSuccessAnimation() {

  setTimeout(async () => {
    try {
      // 立即开始加载微应用系统信息，仅更新 userStore，实际 qiankun 初始化在 main.js 中进行
      const currentLanguage = navigationStore?.language ?? ''
      let sysInfoData = Array.isArray(latestRoutePermissionSystems) ? latestRoutePermissionSystems : []
      const canReuseRouteSystems =
        sysInfoData.length > 0 && sysInfoData.every((item) => typeof item?.subSysAccessUrl === 'string' && item.subSysAccessUrl)

      if (!canReuseRouteSystems) {
        const obj = buildPermissionQuery({
          accurate: 0,
          roleids: userStore.userRoles,
          langCode: currentLanguage
        })
        const sysInfo = await useGetPermissionAsync(obj)
        sysInfoData = Array.isArray(sysInfo?.data) ? sysInfo.data : []
      }

      sysInfoData.forEach((item) => {
        userStore.setSysCodeUrlItem(`${item.subSysAccessUrl}_${item.sysCode}`)
        userStore.setSysCodeItem(item.sysCode)
      })

      // 根据最新的微应用配置初始化 qiankun（只会在当前页面生命周期内执行一次）
      if (window.__init_qiankun__) {
        window.__init_qiankun__()
      }

      // 微应用信息和 qiankun 初始化完成后，直接跳转首页
      router.push({ path: '/' })

      const holder = window.__MAIN_LOGIN_LOADING__
      if (holder) {
        // 标记逻辑层已允许移除遮罩
        holder.canRemove = true
        // 若尚未达到最小展示时间，则交给 5 秒定时器处理
        if (!holder.minTimeReached) {
          return
        }
        const { loadingDiv, style } = holder
        if (loadingDiv && document.body.contains(loadingDiv)) {
          document.body.removeChild(loadingDiv)
        }
        if (style && document.head.contains(style)) {
          document.head.removeChild(style)
        }
        document.body.style.overflow = ''
        window.__MAIN_LOGIN_LOADING__ = null
      }
    } catch (error) {
      console.error('加载微应用失败:', error)
      // 即使出错也要隐藏动画
      const holder = window.__MAIN_LOGIN_LOADING__
      if (holder) {
        holder.canRemove = true
        const { loadingDiv, style } = holder
        if (loadingDiv && document.body.contains(loadingDiv)) {
          document.body.removeChild(loadingDiv)
        }
        if (style && document.head.contains(style)) {
          document.head.removeChild(style)
        }
        document.body.style.overflow = ''
        window.__MAIN_LOGIN_LOADING__ = null
      }
      setTimeout(() => {
        router.push({ path: '/' })
      }, 3000) // 3秒后跳转
    }
  }, 100) // 100ms后开始加载资源
}

function handleForgetPassword() {
  message.info('忘记密码请联系管理员')
}

const onFinish = (values) => {
  loading.value = true
  loginData.LoginName = formState.username
  loginData.Password = formState.password
  fetchData(formState.username)
}
const onFinishFailed = (errorInfo) => {
  console.log('Failed:', errorInfo)
}
</script>

<style scoped>
/* 确保页面不会出现滚动条 */
html,
body {
  overflow: hidden;
  height: 100%;
}

/* 登录页面容器 */
.login-container {
  display: flex;
  min-height: 100vh;
  height: 100vh;
  flex: 1;
  user-select: none;
  overflow: hidden;
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  position: relative;
}

/* Logo区域样式 */
.login-logo {
  position: absolute;
  left: 0;
  top: 0;
  z-index: 20;
  display: flex;
  width: 100%;
  padding: 1rem;
}

.login-logo-content {
  color: #1f2937;
  display: flex;
  align-items: center;
}

.login-logo-img {
  object-fit: contain;
  margin-right: 0.5rem;
}

.login-logo-text {
  margin: 0;
  font-size: 1.25rem;
  font-weight: 500;
}

/* 登录表单面板 */
.login-form-panel {
  display: flex;
  flex-direction: column;
  justify-content: center;
  position: relative;
  padding: 2.5rem 1.5rem;
  flex: 1;
  height: 100vh;
  width: 50%;
  overflow: hidden;
  z-index: 10;
}

.login-form-container {
  margin-top: 1.5rem;
  width: 100%;
  max-width: 36rem;
  margin-left: auto;
  margin-right: auto;
  background-color: rgba(255, 255, 255, 0.9);
  backdrop-filter: blur(10px);
  border-radius: 16px;
  padding: 3rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.1);
}

/* 左侧图片区域 */
.login-image-area {
  position: relative;
  display: none;
  flex: 1;
  width: 50%;
  height: 100vh;
  overflow: hidden;
}

@media (min-width: 1024px) {
  .login-image-area {
    display: block;
  }
}

.login-image-bg {
  position: absolute;
  inset: 0;
  height: 100%;
  width: 100%;
}

.login-image-content {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 2rem;
  height: 100%;
  position: relative;
  z-index: 10;
}

@keyframes float {
  0%,
  100% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-20px);
  }
}

.animate-slide-in {
  animation: slideIn 0.5s ease-out;
}

.animate-slide-in-right {
  animation: slideInRight 0.8s ease-out;
}

.animate-fade-in-up {
  animation: fadeInUp 1s ease-out 0.3s both;
}

.animate-fade-in {
  animation: fadeIn 1.5s ease-out 0.5s both;
}

@keyframes slideIn {
  from {
    transform: translateX(-100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes slideInRight {
  from {
    transform: translateX(50px);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@keyframes fadeInUp {
  from {
    transform: translateY(20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

@keyframes fadeIn {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

/* 输入框聚焦动画 */
.login-input:focus {
  transform: scale(1.01);
  transition: all 0.3s ease;
}

/* 按钮悬停动画 */
.custom-login-button:hover:not(:disabled) {
  transform: translateY(-1px) scale(1.01);
  transition: all 0.3s ease;
}

/* 表单容器进入动画 */
.login-form-container {
  animation: containerFadeIn 1.2s ease-out 0.2s both;
}

@keyframes containerFadeIn {
  from {
    opacity: 0;
    transform: translateY(10px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

/* 表单样式 */
.login-form {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.login-title-area {
  margin-bottom: 1rem;
}

.login-title {
  font-size: 2rem;
  font-weight: 700;
  color: #1f2937;
  margin-bottom: 0.5rem;
}

.login-subtitle {
  color: #6b7280;
  font-size: 1rem;
}

.login-form-item {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.login-label {
  font-size: 1rem;
  font-weight: 500;
  color: #374151;
  margin-bottom: 0.5rem;
}

.login-input-wrapper {
  position: relative;
}

.login-input-icon {
  position: absolute;
  left: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #9ca3af;
  z-index: 1;
  font-size: 1.25rem;
}

.login-input {
  width: 100%;
  padding: 1rem 1rem 1rem 3rem;
  border: 1px solid #d1d5db;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: all 0.2s ease;
  background-color: #ffffff;
}

.login-input:focus {
  outline: none;
  border-color: #3b82f6;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.1);
}

.login-options {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin: 0.5rem 0;
}

.login-remember {
  display: flex;
  align-items: center;
  font-size: 1rem;
  color: #6b7280;
  cursor: pointer;
}

.login-remember input {
  margin-right: 0.5rem;
}

.login-forgot {
  font-size: 1rem;
  color: #3b82f6;
  text-decoration: none;
}

.login-forgot:hover {
  text-decoration: underline;
}

/* 登录按钮 */
.custom-login-button {
  background: linear-gradient(45deg, #667eea 0%, #764ba2 100%);
  border: none;
  color: white;
  transition: all 0.3s ease;
  width: 100%;
  padding: 1.25rem 1.5rem;
  border-radius: 0.5rem;
  font-weight: 600;
  font-size: 1rem;
  cursor: pointer;
  margin-top: 0.5rem;
}

.custom-login-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 15px rgba(0, 0, 0, 0.2);
}

.custom-login-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
  transform: none;
}

.custom-login-button:focus {
  outline: none;
  box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.3);
}

.login-button-loading {
  margin-right: 0.5rem;
}

/* 版权信息 */
.login-copyright {
  position: absolute;
  bottom: 0.75rem;
  left: 50%;
  transform: translateX(-50%);
  text-align: center;
  font-size: 0.75rem;
  color: #6b7280;
}

/* 响应式调整 */
@media (max-width: 1024px) {
  .login-image-area {
    display: none;
  }

  .login-form-panel {
    width: 100% !important;
    height: 100vh;
  }
}

@media (max-width: 768px) {
  .login-container {
    flex-direction: column;
    height: 100vh;
  }

  .login-form-panel {
    width: 100% !important;
    height: 100vh;
    padding: 1rem;
  }

  .login-form-container {
    padding: 0 0.5rem;
  }

  .login-logo-content {
    margin-left: 0.5rem;
    margin-top: 0.5rem;
  }
}

@keyframes logoFloat {
  0%,
  100% {
    transform: translateY(0px) scale(1);
  }
  50% {
    transform: translateY(-10px) scale(1.05);
  }
}

@keyframes logoRotate {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}

@keyframes textFadeIn {
  from {
    opacity: 0;
    transform: translateY(20px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes progressFill {
  from {
    width: 0%;
  }
  to {
    width: 100%;
  }
}
</style>
