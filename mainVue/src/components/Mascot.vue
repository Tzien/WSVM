<template>
  <div class="MascotCss">
    <!-- 吉祥物图标 -->
    <div
      class="mascot-wrapper"
      :style="{ right: `${right}px`, bottom: `${bottom}px` }"
      @mouseenter="showTip = true"
      @mouseleave="showTip = false"
      @mousedown="startDrag"
      @click="openModal"
    >
      <img src="/src/assets/Mascot.png" alt="" class="mascot-img" draggable="false" />
      <div v-if="showTip" class="tooltip">您好，有什么需要帮助的吗？</div>
    </div>

    <!-- 抽屉组件：点击吉祥物后弹出 -->
    <a-drawer :bodyStyle="{ padding: '0' }" v-model:open="modalVisible" title="京小诚" placement="right" :width="1000" :closable="true">
      <iframe id="openwebui-frame" src="http://localhost:5173/" style="width: 100%; height: 99%; border: none" />
    </a-drawer>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const showTip = ref(false)
const modalVisible = ref(false)

const right = ref(100)
const bottom = ref(100)

let dragging = false
let startX = 0
let startY = 0
let startRight = 0
let startBottom = 0

let wasDragging = false

const startDrag = (e) => {
  e.preventDefault()
  dragging = true
  wasDragging = false

  showTip.value = false

  startX = e.clientX
  startY = e.clientY
  startRight = right.value
  startBottom = bottom.value

  window.addEventListener('mousemove', onDrag)
  window.addEventListener('mouseup', stopDrag)
}

const onDrag = (e) => {
  if (!dragging) return

  const deltaX = e.clientX - startX
  const deltaY = e.clientY - startY

  // 移动距离较大时，标记为拖动行为
  if (Math.abs(deltaX) > 3 || Math.abs(deltaY) > 3) {
    wasDragging = true
  }

  let newRight = startRight - deltaX
  let newBottom = startBottom - deltaY

  newRight = Math.min(Math.max(newRight, 0), window.innerWidth - 80)
  newBottom = Math.min(Math.max(newBottom, 0), window.innerHeight - 80)

  right.value = newRight
  bottom.value = newBottom
}

const stopDrag = () => {
  dragging = false
  window.removeEventListener('mousemove', onDrag)
  window.removeEventListener('mouseup', stopDrag)
}

const openModal = () => {
  if (wasDragging) return // 拖动时阻止触发弹窗
  modalVisible.value = true
  // 触发 iframe 登录的函数
  sendLoginMessageToIframe()
}

function sendLoginMessageToIframe() {
  window.addEventListener('message', (event) => {
    if (event.data?.type === 'READY_FOR_LOGIN') {
      const iframe = document.getElementById('openwebui-frame')
      if (iframe?.contentWindow) {
        //调用注册接口
        /* 验证用户是否注册过 */

        // iframe.contentWindow.postMessage(
        //   {
        //     type: 'SignUp_OPENWEBUI',
        //     payload: {
        //       username:'AAA2',
        //       email: 'AAA2@123.com',
        //       password: '123456'
        //     }
        //   },
        //   '*'
        // )

        //调用登录接口
        iframe.contentWindow.postMessage(
          {
            type: 'LOGIN_OPENWEBUI',
            payload: {
              email: '15930816859@163.com',
              password: '111111'
            }
          },
          '*'
        )
      }
    }
  })
}

const closeModal = () => {
  modalVisible.value = false
}
</script>

<style lang="scss">
.MascotCss {
  .mascot-wrapper {
    position: fixed;
    width: 80px;
    height: 80px;
    cursor: grab;
    z-index: 1000;
    user-select: none;
  }

  .mascot-wrapper:active {
    cursor: grabbing;
  }

  .mascot-img {
    width: 100%;
    height: 100%;
    object-fit: contain;
    pointer-events: none;
    user-select: none;
  }

  .tooltip {
    position: absolute;
    bottom: 100%;
    right: 0;
    margin-bottom: 8px;
    background-color: rgba(0, 0, 0, 0.75);
    color: white;
    padding: 6px 12px;
    border-radius: 4px;
    white-space: nowrap;
    font-size: 14px;
    user-select: none;
    pointer-events: none;
  }

  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100vw;
    height: 100vh;
    background: rgba(0, 0, 0, 0.6);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 2000;
  }

  .modal-content {
    background: white;
    padding: 30px 40px;
    border-radius: 10px;
    max-width: 90vw;
    max-height: 90vh;
    overflow-y: auto;
    text-align: center;
    box-shadow: 0 0 15px rgba(0, 0, 0, 0.3);
  }

  .modal-content button {
    margin-top: 20px;
    padding: 8px 16px;
    font-size: 16px;
    cursor: pointer;
  }
}
</style>
