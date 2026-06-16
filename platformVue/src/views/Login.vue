<template>
  <div class="login">
    <div class="logo"></div>
    <div class="logincontrol">
      <div class="title">CeriOS平台-子应用登录</div>
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
import { useUserStore, useRouteStore, useNavigationStore } from '../store/index'
import { usePassWordLogin } from '../api/user'
import { getI18n, loadLocaleMessages } from '../lang/i18n'
import { useRouter } from 'vue-router'

const userStore = useUserStore()
const routeStore = useRouteStore()
const navigationStore = useNavigationStore()
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
const fetchData = async (loginname) => {
  const tokenData = await usePassWordLogin(loginData)
  if (tokenData.code === 200 && tokenData.success) {
    let access_token = tokenData.data.access_token.split('.')
    let statement = JSON.parse(decodeURIComponent(window.atob(access_token[1].replace(/-/g, '+').replace(/_/g, '/'))))
    if (typeof statement.role === 'string') {
      statement.role = [statement.role]
    }
    userStore.setUserMessage(
      statement.ptid,
      loginname,
      tokenData.data.access_token,
      tokenData.data.refresh_token,
      tokenData.data.expires_in,
      statement.exp, //秒级
      statement.role
    )
    const currentLanguage = `${navigationStore.language || ''}`.trim() || 'zh'

    //获取动态路由
    await routeStore.loadRoutes(userStore.userRoles, currentLanguage)

    await loadLocaleMessages(getI18n(), currentLanguage, {
      includeCommonMessages: true,
      includeRouteMessages: false
    })

    await loadLocaleMessages(getI18n(), currentLanguage, {
      includeCommonMessages: false,
      includeRouteMessages: true,
      routeSourceData: routeStore.permissionSystemsCache
    })

    router.addRoute(routeStore.routes)

    message.success('登录成功!')
    navigationStore.removeAllTab()
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
  /* background-image: url('@/assets/images/logoA.png'); */
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
