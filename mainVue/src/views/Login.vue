<template>
  <div class="MainLogin">
    <div class="loginContent">
      <div class="logo"></div>
      <div class="WelcomeMessageBox">{{ $t('common.welcomeTitle') }}{{ drawerStore.platformName }}</div>
      <div class="loginFormContent">
        <div style="margin-top: 130px">
          <div class="platformNameBox">{{ drawerStore.platformName }}</div>
          <div class="formLoginTitle">{{ $t('common.loginTitle') }}</div>
          <div style="color: #000; font-size: 24px; margin: 82px 0px 14px 130px; margin-top: 82px">{{ $t('common.AccountPasswordLogin') }}</div>
          <a-form class="from" :model="formState" name="basic" autocomplete="off" @finish="onFinish" @finishFailed="onFinishFailed">
            <a-form-item class="formItem1" label="" name="username" :rules="[{ required: true, message: $t('common.UserAccountRequired') }]">
              <a-input style="height: 64px; font-size: 20px" v-model:value="formState.username" :placeholder="$t('common.EnterUserAccount')">
                <template #prefix><UserOutlined style="color: rgba(0, 0, 0, 0.25); font-size: 20px; margin-right: 10px" /></template>
              </a-input>
            </a-form-item>
            <a-form-item class="formItem" label="" name="password" :rules="[{ required: true, message: $t('common.UserPasswordRequired') }]">
              <a-input-password style="height: 64px; font-size: 20px" v-model:value="formState.password" :placeholder="$t('common.EnterUserPassword')">
                <template #prefix><LockOutlined style="color: rgba(0, 0, 0, 0.25); font-size: 20px; margin-right: 10px" /></template>
              </a-input-password>
            </a-form-item>
            <a-form-item style="text-align: center; margin-top: 68px">
              <a-button class="loginbtn" type="primary" html-type="submit">{{ $t('common.loginTitle') }}</a-button>
              <div class="forgetTitle">{{ $t('common.ForgotPasswordTitle') }}</div>
            </a-form-item>
          </a-form>
        </div>
      </div>
    </div>
    <div class="tail1">
      <div class="tail">{{ coptRightTitle }}</div>
    </div>
  </div>

  <a-modal v-model:open="open" :keyboard="false" :mask-closable="false" :closable="false" :footer="null">
    <div>
      <div style="margin-top: 40px; display: flex; justify-content: center; align-items: center">
        <label class="passlabel" style="margin-right: 10px">{{ $t('common.Password') }}: </label>
        <a-input-password v-model:value="que" style="margin-right: 15px; width: 200px" placeholder="请输入修改密码"> </a-input-password>
        <a-button type="primary" @click="handleOkpass" style="width: 100px; margin-right: 5px">{{$t('common.Confirm')}}</a-button>
      </div>
      <div style="font-size: 14px; color: #8ba3be; margin-top: 30px; display: flex; justify-content: center; align-items: center">
        {{ $t('common.PasswordRules') }}
      </div>
    </div>
  </a-modal>
</template>
<script setup>
import { reactive, ref } from 'vue'
import CryptoJS from 'crypto-js'
import { message } from 'ant-design-vue'
import { useUserStore, useRouteStore, useNavigationStore, useSignalRStore, useDrawerStore } from '../store/index'
import { usePassWordLogin, useUpdatePassword } from '../api/user'
import { useRouter } from 'vue-router'
import { useInsertSysLoginUserLogAsync } from '../api/userLoginLogs'
import { registerMicroApps, start } from 'qiankun'
import { useGetPermissionAsync } from '../api/permission'
import { buildPermissionQuery } from '../utils/commonTools'
import { loadLocaleMessages } from '../lang/i18n'
import { getPlatformNameApi } from '@/api/sysinfo.js'
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
import { useI18n } from 'vue-i18n'
const ispwd = import.meta.env.VITE_IS_UPDATE_PWD

const userStore = useUserStore()
const routeStore = useRouteStore()
const navigationStore = useNavigationStore()
const drawerStore = useDrawerStore()
const router = useRouter()
const signalRStore = useSignalRStore()
const i18n = useI18n({ useScope: 'global' })
const currentTimeMillis = new Date().getTime()
const current = Math.floor(currentTimeMillis / 1000)
const expire = userStore.expiration_timestamp
if (userStore.access_token != '' && current <= expire) {
  router.push({ path: '/' })
}

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

const open = ref(false)
const que = ref('')
let ptId = ''
let pwd = ''
const handleOkpass = () => {
  const passwordPattern = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,15}$/
  if (!passwordPattern.test(que.value)) {
    message.warning('密码格式不正确！')
    return
  }
  useUpdatePassword({
    UserId: ptId,
    OriginalPassword: pwd,
    Password: que.value
  }).then((data) => {
    if (data.code === 200 && data.success) {
      //message.success(data.message)
      open.value = false
      ptId = ''
      pwd = ''
      que.value = ''
      message.success('修改成功，请登录！')
    } else {
      message.error(data.message)
    }
  })
}

const uptpwd = () => {
  open.value = true
}

const fetchData = async (loginname) => {
  pwd = loginData.Password
  const key = CryptoJS.enc.Utf8.parse('1234567890abcdef')
  const iv = CryptoJS.enc.Utf8.parse('1234567890abcdef')
  const pass = CryptoJS.AES.encrypt(loginData.Password, key, { iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 })
  loginData.Password = pass.toString()
  const tokenData = await usePassWordLogin(loginData)
  if (tokenData.code === 200 && tokenData.success) {
    const passwordPattern = /^(?=.*[a-zA-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,15}$/

    let access_token = tokenData.data.access_token.split('.')
    // let statement = JSON.parse(decodeURIComponent(window.atob(access_token[1].replace(/-/g, '+').replace(/_/g, '/'))))
    let statement = base64UrlToUtf8Json_legacy(access_token[1])
    if (typeof statement.role === 'string') {
      statement.role = [statement.role]
    }
    ptId = statement.ptid
    
    if (!passwordPattern.test(pwd) && ispwd === 'true') {
      message.warning(i18n.t('common.StrongPasswordHint'))
      uptpwd()
      return
    }

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
      <div style="color: #4E5969; font-size: 24px; font-weight: 600; margin-bottom: 10px; opacity: 0; animation: textFadeIn 1s ease-out 0.5s forwards;">${drawerStore.platformName || '系统管理平台'}</div>
      <div style="color: #4E5969; font-size: 16px; opacity: 0; animation: textFadeIn 1s ease-out 1s forwards;">${i18n.t('main.LoadingSystemTitle')}</div>
      <div style="width: 200px; height: 4px; background: #98B0DA; border-radius: 2px; margin-top: 30px; overflow: hidden; opacity: 0; animation: textFadeIn 1s ease-out 1.5s forwards;">
        <div style="height: 100%; background: linear-gradient(90deg, #0061D4, #0061D4); border-radius: 2px; width: 0%; animation: progressFill 2s ease-out 2s forwards;"></div>
      </div>
    `

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
    document.body.style.overflow = 'hidden'

    window.__MAIN_LOGIN_LOADING__ = {
      loadingDiv,
      style,
      canRemove: false,
      minTimeReached: false
    }

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
    }, 3000)

    try {
      userStore.setUserMessage(
        statement.realname,
        statement.ptid,
        loginname,
        tokenData.data.access_token,
        tokenData.data.refresh_token,
        tokenData.data.expires_in,
        statement.exp,
        statement.role
      )
      const routePermissionSystems = await routeStore.loadRoutes(userStore.userRoles)
      routeStore.routes.forEach((route) => {
        router.addRoute(route)
      })
      navigationStore.removeAllTab()
      useInsertSysLoginUserLogAsync({ CreateId: statement.ptid, LoginName: loginname }).then((data) => {
        if (data.code === 200 && data.success) {
          userStore.setlogid(data.data)
        }
      })
      signalRStore.connection.invoke('Login', statement.sub)
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
      message.success('登录成功!')

      const currentLanguage = navigationStore?.language ?? ''
      await loadLocaleMessages(i18n, currentLanguage, {
        includeCommonMessages: true,
        includeRouteMessages: true,
        routeSourceData: routePermissionSystems
      })
      let sysInfoData = Array.isArray(routePermissionSystems) ? routePermissionSystems : []
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

      var microApps = []
      sysInfoData.forEach((item) => {
        var microApp = {
          name: `${item.sysCode}-subApp`,
          entry: process.env.NODE_ENV === 'development' ? item.subSysAccessUrl : `/subapp/${item.sysCode}/`,
          activeRule: `/${item.sysCode}`,
          container: `#${item.sysCode}-container`
        }
        microApps.push(microApp)
        userStore.setSysCodeUrlItem(`${item.subSysAccessUrl}_${item.sysCode}`)
        userStore.setSysCodeItem(item.sysCode)
      })
      registerMicroApps(microApps, {
        beforeMount(app) {
          console.log('registerMicroApps : beforeMount', app)
        },
        afterMount(app) {
          console.log('registerMicroApps : afterMount', app)
        }
      })
      start({
        prefetch: true
      })
      router.push({ path: '/' })

      const holder = window.__MAIN_LOGIN_LOADING__
      if (holder) {
        holder.canRemove = true
        if (!holder.minTimeReached) {
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
      }
    } catch (error) {
      console.error('处理登录后逻辑失败:', error)
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
  } else {
    message.error(tokenData.message)
  }
}
const formState = reactive({
  username: '',
  password: ''
})
const onFinish = (values) => {
  loginData.LoginName = values.username
  loginData.Password = values.password
  fetchData(values.username)
}
const onFinishFailed = (errorInfo) => {
  console.log('Failed:', errorInfo)
}
</script>

<style lang="scss">
.MainLogin {
  position: relative;
  .loginContent {
    position: relative;
    height: 100vh;
    background-color: transparent;
    background-image: url('@/assets/images/login_bg.png');
    background-size: cover;
    background-repeat: no-repeat;
    background-position: bottom center;
    /* background-position: 50px 100px; */
    /* scroll 随内容滚动 fixed */
    background-attachment: fixed;
  }

  .logo {
    width: 350px;
    height: 160px;
    background-image: url('@/assets/images/NewLogoA.png');
    background-repeat: no-repeat;
    background-position: 80px 40px;
  }

  .WelcomeMessageBox {
    position: absolute;
    top: 140px;
    left: 80px;
    width: 50%;
    height: 160px;
    font-size: 30px;
    font-weight: 500;
    color: #4e5969;
  }

  .loginFormContent {
    position: absolute;
    top: 0vh;
    right: 0vh;
    width: 800px;
    height: 100vh;
    background: linear-gradient(to right, #f3f8ff99 60%, #fff);

    .platformNameBox {
      font-size: 48px;
      font-weight: 500;
      text-align: center;
      margin-bottom: 50px;
    }
    .formLoginTitle {
      font-size: 48px;
      font-weight: 700;
      text-align: center;
      margin-top: 40px;
    }

    .from {
      margin: 0px 30px 30px 30px;
      padding: 0px 100px 50px 100px;

      .formItem1 {
        margin: 0px 0px 30px 0px;
      }
      .formItem {
        margin: 30px 0px;
      }

      .loginbtn {
        font-size: 20px;
        width: 100%;
        height: 64px;
        transition: all 0.1s;
      }

      .formItem ::placeholder {
        font-size: 20px !important;
        color: #86909c;
      }
      .formItem1 ::placeholder {
        font-size: 20px !important;
        color: #86909c;
      }

      .loginbtn:hover {
        transform: scale(1.1);
      }

      .forgetTitle {
        font-size: 20px;
        color: #86909c;
        margin-top: 10px;
        text-align: left;
      }
    }
  }

  .tail1 {
    position: absolute;
    bottom: 10px; // 离底部距离，可根据需求调整
    width: 100%;
    text-align: center;
    .tail {
      font-size: 18px;
      color: #8ba3be;
    }
  }
}
</style>
