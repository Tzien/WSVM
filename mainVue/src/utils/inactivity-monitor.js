// plugins/inactivity-monitor.js
let config = {}
let timer = null
let lastActive = Date.now()
let warned = false

function updateActivity() {
  lastActive = Date.now()
  warned = false
}

function checkTimeout() {
  const now = Date.now()
  const inactiveTime = now - lastActive
  const remaining = config.inactivityLimit - inactiveTime

  if (remaining <= 0) {
    stop()
    config.onTimeout && config.onTimeout()
  } else if (!warned && remaining <= config.warningTime) {
    config.onWarning && config.onWarning()
    warned = true
  }
}

function setupListeners() {
  const events = ['mousemove', 'keydown', 'click', 'scroll', 'touchstart']
  events.forEach(event => window.addEventListener(event, updateActivity))
  document.addEventListener('visibilitychange', () => {
    if (document.visibilityState === 'visible') {
      updateActivity()
    }
  })
}

function start() {
  if (timer) return
  timer = setInterval(checkTimeout, config.checkInterval)
  setupListeners()
  updateActivity()
}

function stop() {
  if (timer) clearInterval(timer)
  timer = null
}

export default {
  install(app, options = {}) {
    config = {
      inactivityLimit: 20 * 60 * 1000, // 默认 20分钟
      warningTime: 10 * 60 * 1000,
      checkInterval:2 * 60 * 1000, //两分钟检查一次
      ...options
    }

    start()

    app.config.globalProperties.$inactivityMonitor = {
      stop,
      restart: () => {
        stop()
        start()
      }
    }
  }
}
