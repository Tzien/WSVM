<template>
  <router-link to="/redirectToB">跳转到B项目</router-link>
</template>
<script>
// RedirectToB.vue
export default {
  mounted() {
    const targetUrl = 'http://localhost:8880/home'
    const dataToSend = {
      /* 传递给B项目的数据 */
    }
    window.location.href = `${targetUrl}?data=${encodeURIComponent(JSON.stringify(dataToSend))}`
  }
}
</script>
