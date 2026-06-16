<template>
  <div class="login">
    <div class="logo"></div>
    <div class="logincontrol">
      <div class="title">CeriOS-子应用登录</div>
      <a-form
        class="from"
        :model="formState"
        name="basic"
        :label-col="{ span: 5 }"
        :wrapper-col="{ span: 16 }"
        autocomplete="off"
        @finish="onFinish"
        @finishFailed="onFinishFailed"
      >
        <a-form-item label="账号" name="username" :rules="[{ required: true, message: '请输入账号!' }]">
          <a-input v-model:value="formState.username" />
        </a-form-item>
        <a-form-item label="密码" name="password" :rules="[{ required: true, message: '请输入密码!' }]">
          <a-input-password v-model:value="formState.password" />
        </a-form-item>

        <a-form-item :wrapper-col="{ offset: 6, span: 16 }">
          <a-button class="loginbtn" type="primary" html-type="submit">登录</a-button>
        </a-form-item>
      </a-form>
    </div>
  </div>
  <div class="tail">版权 © 2024 中冶京诚数字科技（北京）有限公司 all rights reserved</div>
</template>
<script setup>
import { reactive } from 'vue'
import { message } from 'ant-design-vue'
import { useUserStore, useRouteStore, useNavigationStore, useDrawerStore } from '../store/index'
import { usePassWordLogin } from '../api/user'
import { useRouter } from 'vue-router'
import CryptoJS from 'crypto-js'

const userStore = useUserStore()
const routeStore = useRouteStore()
const navigationStore = useNavigationStore()
const drawerStore = useDrawerStore()
const router = useRouter()
const currentTimeMillis = new Date().getTime()
const current = Math.floor(currentTimeMillis / 1000)
const expire = userStore.expiration_timestamp
if (userStore.access_token != '' && current <= expire) {
  router.push({ path: '/' })
}

const loginData = {
  ClientId: 'znpt',
  ClientSecret: 'secret',
  RememberLogin: false,
  LoginName: '',
  Password: ''
}

function parseJwt(token) {
  const base64Url = token.split('.')[1] // 获取 Payload 部分
  const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/') // 修复 Base64Url 的格式
  const jsonPayload = decodeURIComponent(
    atob(base64)
      .split('')
      .map((c) => `%${('00' + c.charCodeAt(0).toString(16)).slice(-2)}`)
      .join('')
  )
  return JSON.parse(jsonPayload) // 返回解析后的 JSON
}

const fetchData = async (loginname) => {
  const key = CryptoJS.enc.Utf8.parse('1234567890abcdef')
  const iv = CryptoJS.enc.Utf8.parse('1234567890abcdef')
  const pass = CryptoJS.AES.encrypt(loginData.Password, key, { iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 })
  loginData.Password = pass.toString()
  const tokenData = await usePassWordLogin(loginData)
  if (tokenData.code === 200 && tokenData.success) {
    let statement = parseJwt(tokenData.data.access_token)
    if (typeof statement.role === 'string') {
      statement.role = [statement.role]
    }
    userStore.setUserMessage(
      statement.ptid,
      loginname,
      tokenData.data.access_token,
      tokenData.data.refresh_token,
      tokenData.data.expires_in,
      statement.exp,
      statement.role,
      statement.alias
    )
    //获取动态路由
    await routeStore.loadRoutes(userStore.userRoles, navigationStore.language)

    // 动态添加路由
    routeStore.routes.forEach((route) => {
      router.addRoute(route)
    })

    message.success('登录成功!')
    navigationStore.removeAllTab()

    const firstMenu = routeStore.routes.filter((item) => item.path == `/${import.meta.env.VITE_APP_APPNAME}`)
    if (firstMenu.length > 0 && firstMenu[0].children.length > 0) {
      /* 添加tabs */
      navigationStore.addTabs({
        key: firstMenu[0].children[0].path,
        title: firstMenu[0].children[0].name,
        path: firstMenu[0].children[0].path,
        i18nKey: firstMenu[0].children[0].i18nKey
      })
    } else {
      message.error('该系统菜单数量为0,请配置好后重试')
    }
    drawerStore.changeSelected([`${import.meta.env.VITE_HOMEPAGEPATH}`])
    // 登录成功后跳转到根路径，让全局路由守卫按首页流程加载菜单
    router.push({ path: '/' })
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

<style scoped>
.login {
  position: relative;
  height: 96vh;
  background-color: #832a2a;
  background-image: url('@/assets/images/login_bg.png');
  background-size: cover;
  background-repeat: no-repeat;
  background-position: center center;
  /* background-position: 50px 100px; */
  /* scroll 随内容滚动 fixed */
  background-attachment: fixed;
}

.logo {
  position: absolute;
  top: 0;
  left: 0;
  width: 350px;
  height: 160px;
  background-color: rgba(103, 152, 90, 0);
  background-image: url('@/assets/images/NewLogo.png');
  /* background-image: url('@/assets/images/blacklogo.png'); */
  background-repeat: no-repeat;
  background-position: 47px 24px;
  background-size: 70%;
}

.logincontrol {
  position: absolute;
  top: 30vh;
  right: 20vh;
  border-radius: 15px;
  width: 600px;
  height: 380px;
  background-color: rgba(255, 255, 255);
}

.title {
  padding-top: 40px;
  padding-bottom: 20px;
  font-size: 50px;
  font-weight: bold;
  text-align: center;
}

.from {
  margin: 31px;
}

.tail {
  height: 4vh;
  font-size: 15px;
  font-weight: bold;
  text-align: center;
  line-height: 4vh;
}

.loginbtn {
  transition: all 0.1s;
}

.loginbtn:hover {
  transform: scale(1.1);
}

:where(.css-dev-only-do-not-override-3m4nqy).ant-btn {
  padding: 4px 114px;
}
</style>
