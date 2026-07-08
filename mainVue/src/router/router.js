import { h } from 'vue'
import { AppleFilled } from '@ant-design/icons-vue'
import { createRouter, createWebHistory } from 'vue-router'

// 使用动态导入实现路由懒加载
const Login = () => import(/* webpackChunkName: "login" */ '@/views/Login.vue')
const HomePage = () => import(/* webpackChunkName: "home" */ '@/views/HomePage.vue')
import { useDrawerStore, useUserStore } from '../store/index'
const routes = [
  {
    path: '/',
    name: 'home',
    component: HomePage
  },
  {
    name: '登录',
    path: '/Login',
    alias: '/login',
    icon: () => h(AppleFilled),
    component: Login
  }
  // {
  //   path: '/:catchAll(.*)',
  //   name: 'not-found',
  //   component: () => import('../views/NotFond.vue')
  // }
]

const router = createRouter({
  history: createWebHistory('/'),
  // history: createWebHashHistory('/'),
  routes
})

router.beforeEach((to, from, next) => {
  const userStore = useUserStore()
  const currentTimeMillis = new Date().getTime()
  const current = Math.floor(currentTimeMillis / 1000)
  const expiration = userStore.expiration_timestamp === undefined ? 0 : userStore.expiration_timestamp

  // 统一大小写处理登录路由（/Login 与 /login 均视为登录页）
  const isLoginRoute = to.path.toLowerCase() === '/login'

  // 登录页本身不做登录态拦截，直接放行
  if (isLoginRoute) {
    next()
    return
  }

  if ((current >= expiration || userStore.access_token == '') && !/[?&]code=/.test(window.location.href)) {
    // window.location.href = '/Login'
    localStorage.clear()
    next({ path: '/Login' })
    return
  }

  const drawerStore = useDrawerStore()
  const isFromLogin = from.path.toLowerCase() === '/login'
  const isToHome = to.path === '/'
  if (isFromLogin && isToHome) {
    drawerStore.AddSelectFirstMenuPath(['/'])
    drawerStore.changeSelected(['/'])
    next()
    return
  }
  if (drawerStore.selectFirstMenuPath[0] != '/' && from.path == '/' && to.path == '/') {
    drawerStore.AddSelectFirstMenuPath(['/'])
    // window.location.href = '/'
    next({ path: '/' })
  } else {
    next()
  }
})
export default router
