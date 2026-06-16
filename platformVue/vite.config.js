import { fileURLToPath, URL } from "node:url";
import { defineConfig, loadEnv } from "vite";
import vue from "@vitejs/plugin-vue";
import qiankun from "vite-plugin-qiankun";
import AutoImport from "unplugin-auto-import/vite";
import Components from "unplugin-vue-components/vite";
import {
  ElementPlusResolver,
  AntDesignVueResolver,
} from "unplugin-vue-components/resolvers";

// import { visualizer } from 'rollup-plugin-visualizer'
// 图片压缩
import viteImagemin from "vite-plugin-imagemin";
//文件压缩
import viteCompression from "vite-plugin-compression";

export default defineConfig(({ mode }) => {
  // 加载 .env 文件的内容
  const env = loadEnv(mode, process.cwd());
  return {
    plugins: [
      // 图片资源压缩
      viteImagemin({
        gifsicle: {
          // gif图片压缩
          optimizationLevel: 3, // 选择1到3之间的优化级别
          interlaced: false, // 隔行扫描gif进行渐进式渲染
        },
        optipng: {
          // png
          optimizationLevel: 7, // 选择0到7之间的优化级别
        },
        mozjpeg: {
          // jpeg
          quality: 20, // 压缩质量，范围从0(最差)到100(最佳)。
        },
        pngquant: {
          // png
          quality: [0.8, 0.9], // Min和max是介于0(最差)到1(最佳)之间的数字，类似于JPEG。达到或超过最高质量所需的最少量的颜色。如果转换导致质量低于最低质量，图像将不会被保存。
          speed: 4, // 压缩速度，1(强力)到11(最快)
        },
        svgo: {
          plugins: [
            // svg压缩
            {
              name: "removeViewBox",
            },
            {
              name: "removeEmptyAttrs",
              active: false,
            },
          ],
        },
      }),
      viteCompression({
        verbose: true, // 是否在控制台中输出压缩结果
        disable: false,
        threshold: 10240, // 如果体积大于阈值，将被压缩，单位为b，体积过小时请不要压缩，以免适得其反
        algorithm: "gzip", // 压缩算法，可选['gzip'，' brotliccompress '，'deflate '，'deflateRaw']
        ext: ".gz",
        deleteOriginFile: false, // 源文件压缩后是否删除(我为了看压缩后的效果，先选择了true)
      }),
      // visualizer({
      //   open: true,
      //   filename: 'visualizer.html'
      // }),
      vue(),
      qiankun("platform-subApp", {
        useDevMode: true,
      }),
      /* ElementPlus自动导入 */
      AutoImport({
        resolvers: [ElementPlusResolver()],
      }),
      //取消插入vue组件-自动按需导入
      Components({
        resolvers: [
          AntDesignVueResolver({
            importStyle: false, // css in js
          }),
          ElementPlusResolver(),
        ],
      }),
    ],
    base: process.env.NODE_ENV === 'development' ? '/' : '/subapp/platform',
    build: {
      sourcemap: true, // 对于 Vite
      // 指定输出目录
      outDir: `dist`,
      target: "esnext", // 修改打包目标环境
      // 自定义打包配置
      rollupOptions: {
        output: {
          // 对齐 ids4：node_modules 统一打包为 vendor，避免过细分包导致 chunk 执行顺序/循环依赖问题
          manualChunks(id) {
            if (id.includes('node_modules')) {
              return 'vendor'
            }
          }
        },
      },
    },
    server: {
      port: 8001,
      headers: {
        "Access-Control-Allow-Origin": "*",
      },
    },
    resolve: {
      alias: {
        "@": fileURLToPath(new URL("./src", import.meta.url)),
      },
    },
  };
});
