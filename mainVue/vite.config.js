import { fileURLToPath, URL } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import Components from 'unplugin-vue-components/vite'
import AutoImport from 'unplugin-auto-import/vite'
import { ElementPlusResolver, AntDesignVueResolver } from 'unplugin-vue-components/resolvers'
// import { visualizer } from 'rollup-plugin-visualizer'
// 图片压缩
import viteImagemin from 'vite-plugin-imagemin'
//文件压缩
import viteCompression from 'vite-plugin-compression'

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    // 图片资源压缩
    viteImagemin({
      gifsicle: {
        // gif图片压缩
        optimizationLevel: 3, // 选择1到3之间的优化级别
        interlaced: false // 隔行扫描gif进行渐进式渲染
      },
      optipng: {
        // png
        optimizationLevel: 3, // 选择0到7之间的优化级别
        enable: true
      },
      mozjpeg: {
        // jpeg
        quality: 75 // 压缩质量，范围从0(最差)到100(最佳)，调整为75以平衡质量和大小
      },
      pngquant: {
        // png
        quality: [0.8, 0.9], // 调整quality范围以获得更好的压缩效果
        speed: 1, // 压缩速度，1(强力)到11(最快)
        dithering: 80, // 调整错误扩散，控制视觉效果。80 是常用的值，过高会影响图像细节
        strip: true // 去除所有无用的元数据
      },
      svgo: {
        plugins: [
          // svg压缩
          {
            name: 'removeViewBox'
          },
          {
            name: 'removeEmptyAttrs',
            active: false
          }
        ]
      }
    }),
    viteCompression({
      verbose: true, // 是否在控制台中输出压缩结果
      disable: false,
      threshold: 4096, // 降低压缩阈值到4KB，使更多资源能够被压缩
      algorithm: 'gzip', // 压缩算法，可选['gzip'，' brotliccompress '，'deflate '，'deflateRaw']
      ext: '.gz',
      deleteOriginFile: false // 源文件压缩后是否删除(我为了看压缩后的效果，先选择了true)
    }),
    // visualizer({
    //   open: false,
    //   filename: 'visualizer.html'
    // }),
    vue(),
    AutoImport({
      resolvers: [ElementPlusResolver()]
    }),
    //取消插入vue组件-自动按需导入
    Components({
      resolvers: [
        AntDesignVueResolver({
          importStyle: false,
          resolveIcons: true,
          importCss: false
        }),
        ElementPlusResolver({
          importStyle: true
        })
      ],
      dts: false,
      dirs: ['src/components']
    })
  ],
  css: {
    preprocessorOptions: {
      less: {
        javascriptEnabled: true
      }
    },
    // 确保CSS正确处理
    postcss: {
      plugins: []
    }
  },
  base: '/',
  build: {
    sourcemap: true, // 对于 Vite
    // 指定输出目录
    outDir: 'dist',
    emptyOutDir:true,
    target: 'esnext', // 修改打包目标环境
    // 自定义打包配置
    rollupOptions: {
      output: {
        // 配置代码分割策略
        manualChunks: {
          'vue-vendor': ['vue', 'vue-router', 'pinia'],
          'ui-vendor': ['ant-design-vue', '@ant-design/icons-vue', 'element-plus'],
          'chart-vendor': ['echarts'],
          'table-vendor': ['@surely-vue/table'],
          'utils-vendor': ['lodash-es', 'axios', 'dayjs']
        },
        // 配置chunk文件输出格式
        chunkFileNames: (chunkInfo) => {
          const hash = chunkInfo.hash ? chunkInfo.hash.slice(0, 8) : ''
          return `assets/js/${chunkInfo.name}${hash ? '-' + hash : ''}.js`
        },
        // 配置入口文件输出格式
        entryFileNames: 'assets/js/[name]-[hash:8].js'
      }
    },
    // 配置构建优化选项
    chunkSizeWarningLimit: 2000,
    minify: 'terser',
    terserOptions: {
      compress: {
        // 调试阶段保留 console，方便查看主应用与子应用的日志链路
        drop_console: false,
        drop_debugger: false,
        // pure_funcs: ['console.log', 'console.info', 'console.debug', 'console.time', 'console.timeEnd'],
        passes: 2
      },
      mangle: {
        safari10: true
      }
    },
    // 配置资源缓存策略
    assetsInlineLimit: 8192 // 提高小文件内联阈值到8KB，减少HTTP请求
  },
  server: {
    port: 8081,
    headers: {
      // 重点1: 允许跨域访问子应用页面
      'Access-Control-Allow-Origin': '*'
    }
  },
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    }
  }
})
