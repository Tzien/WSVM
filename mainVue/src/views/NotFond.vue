<template>
  <div class="not-found-container">
    <div class="error-code">404</div>
    <div class="error-message">Page Not Found</div>
    <div class="description">
      The page you are looking for might have been removed, had its name changed, or is temporarily unavailable.
    </div>
    <router-link to="/" class="home-link">Go to Home</router-link>
  </div>
</template>

<script setup>
</script>

<style scoped lang="scss">
// 整体容器样式
.not-found-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  min-height: calc(100vh);
  background-color: #FFF;
  font-family: Arial, sans-serif;
  color: #333;

  // 错误代码 404 样式
.error-code {
    font-size: 10rem;
    font-weight: bold;
    color: #e74c3c;
    margin-bottom: 20px;
    animation: fadeInDown 1s ease both;
  }

  // 错误信息样式
.error-message {
    font-size: 3rem;
    margin-bottom: 40px;
    animation: fadeInUp 1s ease both;
  }

  // 描述信息样式
.description {
    text-align: center;
    width: 60%;
    font-size: 1.2rem;
    line-height: 1.6;
    margin-bottom: 60px;
    animation: fadeIn 1.5s ease both;
  }

  // 回家按钮样式
.home-link {
    display: inline-block;
    padding: 15px 30px;
    background-color: #3498db;
    color: #fff;
    text-decoration: none;
    font-size: 1.2rem;
    border-radius: 5px;
    transition: background-color 0.3s ease;
    animation: fadeIn 1.5s ease both;

    &:hover {
      background-color: #2980b9;
    }

  }
}

// 定义动画
@keyframes fadeInDown {
  0% {
    opacity: 0;
    transform: translateY(-50px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeInUp {
  0% {
    opacity: 0;
    transform: translateY(50px);
  }
  100% {
    opacity: 1;
    transform: translateY(0);
  }
}

@keyframes fadeIn {
  0% {
    opacity: 0;
  }
  100% {
    opacity: 1;
  }
}
</style>