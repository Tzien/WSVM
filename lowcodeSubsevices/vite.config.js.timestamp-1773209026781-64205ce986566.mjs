// vite.config.js
import { defineConfig } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/vite/dist/node/index.js";
import { createSvgIconsPlugin } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/vite-plugin-svg-icons/dist/index.mjs";
import path from "path";
import vue from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/@vitejs/plugin-vue/dist/index.mjs";
import qiankun from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/vite-plugin-qiankun/dist/index.js";
import Components from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/unplugin-vue-components/dist/vite.js";
import AutoImport from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/unplugin-auto-import/dist/vite.js";
import { ElementPlusResolver } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/unplugin-vue-components/dist/resolvers.js";
import { AntDesignVueResolver } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/unplugin-vue-components/dist/resolvers.js";
import { fileURLToPath, URL } from "node:url";
import vueJsx from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/@vitejs/plugin-vue-jsx/dist/index.mjs";

// build/config/themeConfig.ts
import { generate } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/@ant-design/colors/lib/index.js";
var primaryColor = "#1890ff";

// build/generate/generateModifyVars.ts
import { resolve } from "path";
import { generate as generate2 } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/@ant-design/colors/lib/index.js";
import { theme } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/ant-design-vue/lib/index.js";
import convertLegacyToken from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/ant-design-vue/lib/theme/convertLegacyToken.js";
var { defaultAlgorithm, defaultSeed } = theme;
function generateAntColors(color, theme2 = "default") {
  return generate2(color, {
    theme: theme2
  });
}
function generateModifyVars() {
  const palettes = generateAntColors(primaryColor);
  const primary = palettes[5];
  const primaryColorObj = {};
  for (let index = 0; index < 10; index++) {
    primaryColorObj[`primary-${index + 1}`] = palettes[index];
  }
  const mapToken = defaultAlgorithm(defaultSeed);
  const v3Token = convertLegacyToken.default(mapToken);
  return {
    ...v3Token,
    // Used for global import to avoid the need to import each style file separately
    // reference:  Avoid repeated references
    hack: `true; @import (reference) "${resolve("src/design/config.less")}";`,
    "primary-color": primary,
    ...primaryColorObj,
    "info-color": primary,
    "processing-color": primary,
    "success-color": "#55D187",
    //  Success color
    "error-color": "#ED6F6F",
    //  False color
    "warning-color": "#EFBD47",
    //   Warning color
    "btn-info-color": "#909399",
    "text-color-secondary": "rgba(0, 0, 0, 0.45)",
    "border-color-base1": "#f0f0f0",
    "font-size-base": "14px",
    //  Main font size
    "border-radius-base": "2px",
    //  Component/float fillet
    "link-color": primary,
    //   Link color
    "app-base-background": "#eaecf0",
    "app-content-background": "#F1F4F8",
    //   Link color
    "app-main-background": "#ebeef5",
    "selected-hover-bg": "#f5f5f5",
    "hover-background": "#f5f7fa"
  };
}

// vite.config.js
import viteImagemin from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/vite-plugin-imagemin/dist/index.mjs";
import viteCompression from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/node_modules/vite-plugin-compression/dist/index.mjs";
var __vite_injected_original_import_meta_url = "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodesubservices/vite.config.js";
var vite_config_default = defineConfig({
  plugins: [
    createSvgIconsPlugin({
      // 指定需要缓存的图标文件夹
      iconDirs: [path.resolve(process.cwd(), "src/assets/icons")],
      // 指定symbolId格式
      symbolId: "icon-[dir]-[name]"
    }),
    vueJsx(),
    // 图片资源压缩
    viteImagemin({
      gifsicle: {
        // gif图片压缩
        optimizationLevel: 3,
        // 选择1到3之间的优化级别
        interlaced: false
        // 隔行扫描gif进行渐进式渲染
      },
      optipng: {
        // png
        optimizationLevel: 7
        // 选择0到7之间的优化级别
      },
      mozjpeg: {
        // jpeg
        quality: 20
        // 压缩质量，范围从0(最差)到100(最佳)。
      },
      pngquant: {
        // png
        quality: [0.8, 0.9],
        // Min和max是介于0(最差)到1(最佳)之间的数字，类似于JPEG。达到或超过最高质量所需的最少量的颜色。如果转换导致质量低于最低质量，图像将不会被保存。
        speed: 4
        // 压缩速度，1(强力)到11(最快)
      },
      svgo: {
        plugins: [
          // svg压缩
          {
            name: "removeViewBox"
          },
          {
            name: "removeEmptyAttrs",
            active: false
          }
        ]
      }
    }),
    viteCompression({
      verbose: true,
      // 是否在控制台中输出压缩结果
      disable: false,
      threshold: 10240,
      // 如果体积大于阈值，将被压缩，单位为b，体积过小时请不要压缩，以免适得其反
      algorithm: "gzip",
      // 压缩算法，可选['gzip'，' brotliccompress '，'deflate '，'deflateRaw']
      ext: ".gz",
      deleteOriginFile: false
      // 源文件压缩后是否删除(我为了看压缩后的效果，先选择了true)
    }),
    vue(),
    qiankun(`lowcodedemo-subApp`, {
      useDevMode: true
    }),
    //取消插入vue组件-自动按需导入
    Components({
      resolvers: [
        AntDesignVueResolver({
          importStyle: false
          // css in js
        }),
        ElementPlusResolver()
      ]
    }),
    AutoImport({
      resolvers: [ElementPlusResolver()]
    })
  ],
  css: {
    preprocessorOptions: {
      less: {
        modifyVars: generateModifyVars(),
        javascriptEnabled: true
      }
    }
  },
  base: process.env.NODE_ENV === "development" ? "/" : "/subapp/lowcodedemo",
  build: {
    sourcemap: true,
    // 对于 Vite
    // 指定输出目录
    outDir: `dist`,
    target: "esnext",
    // 修改打包目标环境
    // 自定义打包配置
    rollupOptions: {
      output: {
        // 配置 chunk 大小分离，提升性能
        manualChunks(id) {
          if (id.includes("node_modules")) {
            return "vendor";
          }
        }
      }
    }
  },
  server: {
    port: 8003,
    host: "0.0.0.0",
    headers: {
      "Access-Control-Allow-Origin": "*"
    }
  },
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", __vite_injected_original_import_meta_url))
    }
  }
});
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcuanMiLCAiYnVpbGQvY29uZmlnL3RoZW1lQ29uZmlnLnRzIiwgImJ1aWxkL2dlbmVyYXRlL2dlbmVyYXRlTW9kaWZ5VmFycy50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiY29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2Rpcm5hbWUgPSBcIkc6XFxcXERZWE1cXFxcXHU1RkFFXHU1MjREXHU3QUVGXFxcXG5ld0dpdFxcXFxjZXJpb3MuY29yZS5sb3djb2Rlc3Vic2VydmljZXNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfZmlsZW5hbWUgPSBcIkc6XFxcXERZWE1cXFxcXHU1RkFFXHU1MjREXHU3QUVGXFxcXG5ld0dpdFxcXFxjZXJpb3MuY29yZS5sb3djb2Rlc3Vic2VydmljZXNcXFxcdml0ZS5jb25maWcuanNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfaW1wb3J0X21ldGFfdXJsID0gXCJmaWxlOi8vL0c6L0RZWE0vJUU1JUJFJUFFJUU1JTg5JThEJUU3JUFCJUFGL25ld0dpdC9jZXJpb3MuY29yZS5sb3djb2Rlc3Vic2VydmljZXMvdml0ZS5jb25maWcuanNcIjtpbXBvcnQgeyBkZWZpbmVDb25maWcgfSBmcm9tICd2aXRlJ1xyXG5pbXBvcnQgeyBjcmVhdGVTdmdJY29uc1BsdWdpbiB9IGZyb20gJ3ZpdGUtcGx1Z2luLXN2Zy1pY29ucydcclxuaW1wb3J0IHBhdGggZnJvbSAncGF0aCdcclxuaW1wb3J0IHZ1ZSBmcm9tICdAdml0ZWpzL3BsdWdpbi12dWUnXHJcbmltcG9ydCBxaWFua3VuIGZyb20gJ3ZpdGUtcGx1Z2luLXFpYW5rdW4nXHJcbmltcG9ydCBDb21wb25lbnRzIGZyb20gJ3VucGx1Z2luLXZ1ZS1jb21wb25lbnRzL3ZpdGUnXHJcbmltcG9ydCBBdXRvSW1wb3J0IGZyb20gJ3VucGx1Z2luLWF1dG8taW1wb3J0L3ZpdGUnXHJcbmltcG9ydCB7IEVsZW1lbnRQbHVzUmVzb2x2ZXIgfSBmcm9tICd1bnBsdWdpbi12dWUtY29tcG9uZW50cy9yZXNvbHZlcnMnXHJcbmltcG9ydCB7IEFudERlc2lnblZ1ZVJlc29sdmVyIH0gZnJvbSAndW5wbHVnaW4tdnVlLWNvbXBvbmVudHMvcmVzb2x2ZXJzJ1xyXG5pbXBvcnQgeyBmaWxlVVJMVG9QYXRoLCBVUkwgfSBmcm9tICdub2RlOnVybCdcclxuaW1wb3J0IHZ1ZUpzeCBmcm9tICdAdml0ZWpzL3BsdWdpbi12dWUtanN4J1xyXG5pbXBvcnQgeyBnZW5lcmF0ZU1vZGlmeVZhcnMgfSBmcm9tICcuL2J1aWxkL2dlbmVyYXRlL2dlbmVyYXRlTW9kaWZ5VmFycydcclxuXHJcbi8vIFx1NTZGRVx1NzI0N1x1NTM4Qlx1N0YyOVxyXG5pbXBvcnQgdml0ZUltYWdlbWluIGZyb20gJ3ZpdGUtcGx1Z2luLWltYWdlbWluJ1xyXG4vL1x1NjU4N1x1NEVGNlx1NTM4Qlx1N0YyOVxyXG5pbXBvcnQgdml0ZUNvbXByZXNzaW9uIGZyb20gJ3ZpdGUtcGx1Z2luLWNvbXByZXNzaW9uJ1xyXG4vLyBodHRwczovL3ZpdGVqcy5kZXYvY29uZmlnL1xyXG5leHBvcnQgZGVmYXVsdCBkZWZpbmVDb25maWcoe1xyXG4gIHBsdWdpbnM6IFtcclxuICAgIGNyZWF0ZVN2Z0ljb25zUGx1Z2luKHtcclxuICAgICAgLy8gXHU2MzA3XHU1QjlBXHU5NzAwXHU4OTgxXHU3RjEzXHU1QjU4XHU3Njg0XHU1NkZFXHU2ODA3XHU2NTg3XHU0RUY2XHU1OTM5XHJcbiAgICAgIGljb25EaXJzOiBbcGF0aC5yZXNvbHZlKHByb2Nlc3MuY3dkKCksICdzcmMvYXNzZXRzL2ljb25zJyldLFxyXG4gICAgICAvLyBcdTYzMDdcdTVCOUFzeW1ib2xJZFx1NjgzQ1x1NUYwRlxyXG4gICAgICBzeW1ib2xJZDogJ2ljb24tW2Rpcl0tW25hbWVdJyxcclxuICAgIH0pLFxyXG4gICAgdnVlSnN4KCksXHJcbiAgICAvLyBcdTU2RkVcdTcyNDdcdThENDRcdTZFOTBcdTUzOEJcdTdGMjlcclxuICAgIHZpdGVJbWFnZW1pbih7XHJcbiAgICAgIGdpZnNpY2xlOiB7XHJcbiAgICAgICAgLy8gZ2lmXHU1NkZFXHU3MjQ3XHU1MzhCXHU3RjI5XHJcbiAgICAgICAgb3B0aW1pemF0aW9uTGV2ZWw6IDMsIC8vIFx1OTAwOVx1NjJFOTFcdTUyMzAzXHU0RTRCXHU5NUY0XHU3Njg0XHU0RjE4XHU1MzE2XHU3RUE3XHU1MjJCXHJcbiAgICAgICAgaW50ZXJsYWNlZDogZmFsc2UgLy8gXHU5Njk0XHU4ODRDXHU2MjZCXHU2M0NGZ2lmXHU4RkRCXHU4ODRDXHU2RTEwXHU4RkRCXHU1RjBGXHU2RTMyXHU2N0QzXHJcbiAgICAgIH0sXHJcbiAgICAgIG9wdGlwbmc6IHtcclxuICAgICAgICAvLyBwbmdcclxuICAgICAgICBvcHRpbWl6YXRpb25MZXZlbDogNyAvLyBcdTkwMDlcdTYyRTkwXHU1MjMwN1x1NEU0Qlx1OTVGNFx1NzY4NFx1NEYxOFx1NTMxNlx1N0VBN1x1NTIyQlxyXG4gICAgICB9LFxyXG4gICAgICBtb3pqcGVnOiB7XHJcbiAgICAgICAgLy8ganBlZ1xyXG4gICAgICAgIHF1YWxpdHk6IDIwIC8vIFx1NTM4Qlx1N0YyOVx1OEQyOFx1OTFDRlx1RkYwQ1x1ODMwM1x1NTZGNFx1NEVDRTAoXHU2NzAwXHU1REVFKVx1NTIzMDEwMChcdTY3MDBcdTRGNzMpXHUzMDAyXHJcbiAgICAgIH0sXHJcbiAgICAgIHBuZ3F1YW50OiB7XHJcbiAgICAgICAgLy8gcG5nXHJcbiAgICAgICAgcXVhbGl0eTogWzAuOCwgMC45XSwgLy8gTWluXHU1NDhDbWF4XHU2NjJGXHU0RUNCXHU0RThFMChcdTY3MDBcdTVERUUpXHU1MjMwMShcdTY3MDBcdTRGNzMpXHU0RTRCXHU5NUY0XHU3Njg0XHU2NTcwXHU1QjU3XHVGRjBDXHU3QzdCXHU0RjNDXHU0RThFSlBFR1x1MzAwMlx1OEZCRVx1NTIzMFx1NjIxNlx1OEQ4NVx1OEZDN1x1NjcwMFx1OUFEOFx1OEQyOFx1OTFDRlx1NjI0MFx1OTcwMFx1NzY4NFx1NjcwMFx1NUMxMVx1OTFDRlx1NzY4NFx1OTg5Q1x1ODI3Mlx1MzAwMlx1NTk4Mlx1Njc5Q1x1OEY2Q1x1NjM2Mlx1NUJGQ1x1ODFGNFx1OEQyOFx1OTFDRlx1NEY0RVx1NEU4RVx1NjcwMFx1NEY0RVx1OEQyOFx1OTFDRlx1RkYwQ1x1NTZGRVx1NTBDRlx1NUMwNlx1NEUwRFx1NEYxQVx1ODhBQlx1NEZERFx1NUI1OFx1MzAwMlxyXG4gICAgICAgIHNwZWVkOiA0IC8vIFx1NTM4Qlx1N0YyOVx1OTAxRlx1NUVBNlx1RkYwQzEoXHU1RjNBXHU1MjlCKVx1NTIzMDExKFx1NjcwMFx1NUZFQilcclxuICAgICAgfSxcclxuICAgICAgc3Znbzoge1xyXG4gICAgICAgIHBsdWdpbnM6IFtcclxuICAgICAgICAgIC8vIHN2Z1x1NTM4Qlx1N0YyOVxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBuYW1lOiAncmVtb3ZlVmlld0JveCdcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIG5hbWU6ICdyZW1vdmVFbXB0eUF0dHJzJyxcclxuICAgICAgICAgICAgYWN0aXZlOiBmYWxzZVxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF1cclxuICAgICAgfVxyXG4gICAgfSksXHJcbiAgICB2aXRlQ29tcHJlc3Npb24oe1xyXG4gICAgICB2ZXJib3NlOiB0cnVlLCAvLyBcdTY2MkZcdTU0MjZcdTU3MjhcdTYzQTdcdTUyMzZcdTUzRjBcdTRFMkRcdThGOTNcdTUxRkFcdTUzOEJcdTdGMjlcdTdFRDNcdTY3OUNcclxuICAgICAgZGlzYWJsZTogZmFsc2UsXHJcbiAgICAgIHRocmVzaG9sZDogMTAyNDAsIC8vIFx1NTk4Mlx1Njc5Q1x1NEY1M1x1NzlFRlx1NTkyN1x1NEU4RVx1OTYwOFx1NTAzQ1x1RkYwQ1x1NUMwNlx1ODhBQlx1NTM4Qlx1N0YyOVx1RkYwQ1x1NTM1NVx1NEY0RFx1NEUzQWJcdUZGMENcdTRGNTNcdTc5RUZcdThGQzdcdTVDMEZcdTY1RjZcdThCRjdcdTRFMERcdTg5ODFcdTUzOEJcdTdGMjlcdUZGMENcdTRFRTVcdTUxNERcdTkwMDJcdTVGOTdcdTUxNzZcdTUzQ0RcclxuICAgICAgYWxnb3JpdGhtOiAnZ3ppcCcsIC8vIFx1NTM4Qlx1N0YyOVx1N0I5N1x1NkNENVx1RkYwQ1x1NTNFRlx1OTAwOVsnZ3ppcCdcdUZGMEMnIGJyb3RsaWNjb21wcmVzcyAnXHVGRjBDJ2RlZmxhdGUgJ1x1RkYwQydkZWZsYXRlUmF3J11cclxuICAgICAgZXh0OiAnLmd6JyxcclxuICAgICAgZGVsZXRlT3JpZ2luRmlsZTogZmFsc2UgLy8gXHU2RTkwXHU2NTg3XHU0RUY2XHU1MzhCXHU3RjI5XHU1NDBFXHU2NjJGXHU1NDI2XHU1MjIwXHU5NjY0KFx1NjIxMVx1NEUzQVx1NEU4Nlx1NzcwQlx1NTM4Qlx1N0YyOVx1NTQwRVx1NzY4NFx1NjU0OFx1Njc5Q1x1RkYwQ1x1NTE0OFx1OTAwOVx1NjJFOVx1NEU4NnRydWUpXHJcbiAgICB9KSxcclxuICAgIHZ1ZSgpLFxyXG4gICAgcWlhbmt1bihgbG93Y29kZWRlbW8tc3ViQXBwYCwge1xyXG4gICAgICB1c2VEZXZNb2RlOiB0cnVlXHJcbiAgICB9KSxcclxuICAgIC8vXHU1M0Q2XHU2RDg4XHU2M0QyXHU1MTY1dnVlXHU3RUM0XHU0RUY2LVx1ODFFQVx1NTJBOFx1NjMwOVx1OTcwMFx1NUJGQ1x1NTE2NVxyXG4gICAgQ29tcG9uZW50cyh7XHJcbiAgICAgIHJlc29sdmVyczogW1xyXG4gICAgICAgIEFudERlc2lnblZ1ZVJlc29sdmVyKHtcclxuICAgICAgICAgIGltcG9ydFN0eWxlOiBmYWxzZSAvLyBjc3MgaW4ganNcclxuICAgICAgICB9KSxcclxuICAgICAgICBFbGVtZW50UGx1c1Jlc29sdmVyKClcclxuICAgICAgXVxyXG4gICAgfSksXHJcbiAgICBBdXRvSW1wb3J0KHtcclxuICAgICAgcmVzb2x2ZXJzOiBbRWxlbWVudFBsdXNSZXNvbHZlcigpXVxyXG4gICAgfSlcclxuICBdLFxyXG4gIGNzczoge1xyXG4gICAgcHJlcHJvY2Vzc29yT3B0aW9uczoge1xyXG4gICAgICBsZXNzOiB7XHJcbiAgICAgICAgbW9kaWZ5VmFyczogZ2VuZXJhdGVNb2RpZnlWYXJzKCksXHJcbiAgICAgICAgamF2YXNjcmlwdEVuYWJsZWQ6IHRydWVcclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0sXHJcbiAgYmFzZTogcHJvY2Vzcy5lbnYuTk9ERV9FTlYgPT09ICdkZXZlbG9wbWVudCcgPyAnLycgOiAnL3N1YmFwcC9sb3djb2RlZGVtbycsXHJcbiAgYnVpbGQ6IHtcclxuICAgIHNvdXJjZW1hcDogdHJ1ZSwgLy8gXHU1QkY5XHU0RThFIFZpdGVcclxuICAgIC8vIFx1NjMwN1x1NUI5QVx1OEY5M1x1NTFGQVx1NzZFRVx1NUY1NVxyXG4gICAgb3V0RGlyOiBgZGlzdGAsXHJcbiAgICB0YXJnZXQ6ICdlc25leHQnLCAvLyBcdTRGRUVcdTY1MzlcdTYyNTNcdTUzMDVcdTc2RUVcdTY4MDdcdTczQUZcdTU4ODNcclxuICAgIC8vIFx1ODFFQVx1NUI5QVx1NEU0OVx1NjI1M1x1NTMwNVx1OTE0RFx1N0Y2RVxyXG4gICAgcm9sbHVwT3B0aW9uczoge1xyXG4gICAgICBvdXRwdXQ6IHtcclxuICAgICAgICAvLyBcdTkxNERcdTdGNkUgY2h1bmsgXHU1OTI3XHU1QzBGXHU1MjA2XHU3OUJCXHVGRjBDXHU2M0QwXHU1MzQ3XHU2MDI3XHU4MEZEXHJcbiAgICAgICAgbWFudWFsQ2h1bmtzKGlkKSB7XHJcbiAgICAgICAgICBpZiAoaWQuaW5jbHVkZXMoJ25vZGVfbW9kdWxlcycpKSB7XHJcbiAgICAgICAgICAgIHJldHVybiAndmVuZG9yJ1xyXG4gICAgICAgICAgfVxyXG4gICAgICAgIH1cclxuICAgICAgfVxyXG4gICAgfVxyXG4gIH0sXHJcbiAgc2VydmVyOiB7XHJcbiAgICBwb3J0OiA4MDAzLFxyXG4gICAgaG9zdDogJzAuMC4wLjAnLFxyXG4gICAgaGVhZGVyczoge1xyXG4gICAgICAnQWNjZXNzLUNvbnRyb2wtQWxsb3ctT3JpZ2luJzogJyonXHJcbiAgICB9XHJcbiAgfSxcclxuICByZXNvbHZlOiB7XHJcbiAgICBhbGlhczoge1xyXG4gICAgICAnQCc6IGZpbGVVUkxUb1BhdGgobmV3IFVSTCgnLi9zcmMnLCBpbXBvcnQubWV0YS51cmwpKVxyXG4gICAgfVxyXG4gIH1cclxufSlcclxuIiwgImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJHOlxcXFxEWVhNXFxcXFx1NUZBRVx1NTI0RFx1N0FFRlxcXFxuZXdHaXRcXFxcY2VyaW9zLmNvcmUubG93Y29kZXN1YnNlcnZpY2VzXFxcXGJ1aWxkXFxcXGNvbmZpZ1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiRzpcXFxcRFlYTVxcXFxcdTVGQUVcdTUyNERcdTdBRUZcXFxcbmV3R2l0XFxcXGNlcmlvcy5jb3JlLmxvd2NvZGVzdWJzZXJ2aWNlc1xcXFxidWlsZFxcXFxjb25maWdcXFxcdGhlbWVDb25maWcudHNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfaW1wb3J0X21ldGFfdXJsID0gXCJmaWxlOi8vL0c6L0RZWE0vJUU1JUJFJUFFJUU1JTg5JThEJUU3JUFCJUFGL25ld0dpdC9jZXJpb3MuY29yZS5sb3djb2Rlc3Vic2VydmljZXMvYnVpbGQvY29uZmlnL3RoZW1lQ29uZmlnLnRzXCI7aW1wb3J0IHsgZ2VuZXJhdGUgfSBmcm9tICdAYW50LWRlc2lnbi9jb2xvcnMnO1xuXG5leHBvcnQgY29uc3QgcHJpbWFyeUNvbG9yID0gJyMxODkwZmYnO1xuXG5leHBvcnQgY29uc3QgZGFya01vZGUgPSAnbGlnaHQnO1xuXG50eXBlIEZuID0gKC4uLmFyZzogYW55KSA9PiBhbnk7XG5cbnR5cGUgR2VuZXJhdGVUaGVtZSA9ICdkZWZhdWx0JyB8ICdkYXJrJztcblxuZXhwb3J0IGludGVyZmFjZSBHZW5lcmF0ZUNvbG9yc1BhcmFtcyB7XG4gIG1peExpZ2h0ZW46IEZuO1xuICBtaXhEYXJrZW46IEZuO1xuICB0aW55Y29sb3I6IGFueTtcbiAgY29sb3I/OiBzdHJpbmc7XG59XG5cbmV4cG9ydCBmdW5jdGlvbiBnZW5lcmF0ZUFudENvbG9ycyhjb2xvcjogc3RyaW5nLCB0aGVtZTogR2VuZXJhdGVUaGVtZSA9ICdkZWZhdWx0Jykge1xuICByZXR1cm4gZ2VuZXJhdGUoY29sb3IsIHtcbiAgICB0aGVtZSxcbiAgfSk7XG59XG5cbmV4cG9ydCBmdW5jdGlvbiBnZXRUaGVtZUNvbG9ycyhjb2xvcj86IHN0cmluZykge1xuICBjb25zdCB0YyA9IGNvbG9yIHx8IHByaW1hcnlDb2xvcjtcbiAgY29uc3QgbGlnaHRDb2xvcnMgPSBnZW5lcmF0ZUFudENvbG9ycyh0Yyk7XG4gIGNvbnN0IHByaW1hcnkgPSBsaWdodENvbG9yc1s1XTtcbiAgY29uc3QgbW9kZUNvbG9ycyA9IGdlbmVyYXRlQW50Q29sb3JzKHByaW1hcnksICdkYXJrJyk7XG5cbiAgcmV0dXJuIFsuLi5saWdodENvbG9ycywgLi4ubW9kZUNvbG9yc107XG59XG5cbmV4cG9ydCBmdW5jdGlvbiBnZW5lcmF0ZUNvbG9ycyh7IGNvbG9yID0gcHJpbWFyeUNvbG9yLCBtaXhMaWdodGVuLCBtaXhEYXJrZW4sIHRpbnljb2xvciB9OiBHZW5lcmF0ZUNvbG9yc1BhcmFtcykge1xuICBjb25zdCBhcnIgPSBuZXcgQXJyYXkoMTkpLmZpbGwoMCk7XG4gIGNvbnN0IGxpZ2h0ZW5zID0gYXJyLm1hcCgoX3QsIGkpID0+IHtcbiAgICByZXR1cm4gbWl4TGlnaHRlbihjb2xvciwgaSAvIDUpO1xuICB9KTtcblxuICBjb25zdCBkYXJrZW5zID0gYXJyLm1hcCgoX3QsIGkpID0+IHtcbiAgICByZXR1cm4gbWl4RGFya2VuKGNvbG9yLCBpIC8gNSk7XG4gIH0pO1xuXG4gIGNvbnN0IGFscGhhQ29sb3JzID0gYXJyLm1hcCgoX3QsIGkpID0+IHtcbiAgICByZXR1cm4gdGlueWNvbG9yKGNvbG9yKVxuICAgICAgLnNldEFscGhhKGkgLyAyMClcbiAgICAgIC50b1JnYlN0cmluZygpO1xuICB9KTtcblxuICBjb25zdCBzaG9ydEFscGhhQ29sb3JzID0gYWxwaGFDb2xvcnMubWFwKGl0ZW0gPT4gaXRlbS5yZXBsYWNlKC9cXHMvZywgJycpLnJlcGxhY2UoLzBcXC4vZywgJy4nKSk7XG5cbiAgY29uc3QgdGlueWNvbG9yTGlnaHRlbnMgPSBhcnJcbiAgICAubWFwKChfdCwgaSkgPT4ge1xuICAgICAgcmV0dXJuIHRpbnljb2xvcihjb2xvcilcbiAgICAgICAgLmxpZ2h0ZW4oaSAqIDUpXG4gICAgICAgIC50b0hleFN0cmluZygpO1xuICAgIH0pXG4gICAgLmZpbHRlcihpdGVtID0+IGl0ZW0gIT09ICcjZmZmZmZmJyk7XG5cbiAgY29uc3QgdGlueWNvbG9yRGFya2VucyA9IGFyclxuICAgIC5tYXAoKF90LCBpKSA9PiB7XG4gICAgICByZXR1cm4gdGlueWNvbG9yKGNvbG9yKVxuICAgICAgICAuZGFya2VuKGkgKiA1KVxuICAgICAgICAudG9IZXhTdHJpbmcoKTtcbiAgICB9KVxuICAgIC5maWx0ZXIoaXRlbSA9PiBpdGVtICE9PSAnIzAwMDAwMCcpO1xuICByZXR1cm4gWy4uLmxpZ2h0ZW5zLCAuLi5kYXJrZW5zLCAuLi5hbHBoYUNvbG9ycywgLi4uc2hvcnRBbHBoYUNvbG9ycywgLi4udGlueWNvbG9yRGFya2VucywgLi4udGlueWNvbG9yTGlnaHRlbnNdLmZpbHRlcihpdGVtID0+ICFpdGVtLmluY2x1ZGVzKCctJykpO1xufVxuIiwgImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJHOlxcXFxEWVhNXFxcXFx1NUZBRVx1NTI0RFx1N0FFRlxcXFxuZXdHaXRcXFxcY2VyaW9zLmNvcmUubG93Y29kZXN1YnNlcnZpY2VzXFxcXGJ1aWxkXFxcXGdlbmVyYXRlXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJHOlxcXFxEWVhNXFxcXFx1NUZBRVx1NTI0RFx1N0FFRlxcXFxuZXdHaXRcXFxcY2VyaW9zLmNvcmUubG93Y29kZXN1YnNlcnZpY2VzXFxcXGJ1aWxkXFxcXGdlbmVyYXRlXFxcXGdlbmVyYXRlTW9kaWZ5VmFycy50c1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9pbXBvcnRfbWV0YV91cmwgPSBcImZpbGU6Ly8vRzovRFlYTS8lRTUlQkUlQUUlRTUlODklOEQlRTclQUIlQUYvbmV3R2l0L2Nlcmlvcy5jb3JlLmxvd2NvZGVzdWJzZXJ2aWNlcy9idWlsZC9nZW5lcmF0ZS9nZW5lcmF0ZU1vZGlmeVZhcnMudHNcIjtpbXBvcnQgeyBwcmltYXJ5Q29sb3IgfSBmcm9tICcuLi9jb25maWcvdGhlbWVDb25maWcnO1xyXG5pbXBvcnQgeyByZXNvbHZlIH0gZnJvbSAncGF0aCc7XHJcbmltcG9ydCB7IGdlbmVyYXRlIH0gZnJvbSAnQGFudC1kZXNpZ24vY29sb3JzJztcclxuaW1wb3J0IHsgdGhlbWUgfSBmcm9tICdhbnQtZGVzaWduLXZ1ZS9saWInO1xyXG5pbXBvcnQgY29udmVydExlZ2FjeVRva2VuIGZyb20gJ2FudC1kZXNpZ24tdnVlL2xpYi90aGVtZS9jb252ZXJ0TGVnYWN5VG9rZW4nO1xyXG5jb25zdCB7IGRlZmF1bHRBbGdvcml0aG0sIGRlZmF1bHRTZWVkIH0gPSB0aGVtZTtcclxuXHJcbmZ1bmN0aW9uIGdlbmVyYXRlQW50Q29sb3JzKGNvbG9yOiBzdHJpbmcsIHRoZW1lOiAnZGVmYXVsdCcgfCAnZGFyaycgPSAnZGVmYXVsdCcpIHtcclxuICByZXR1cm4gZ2VuZXJhdGUoY29sb3IsIHtcclxuICAgIHRoZW1lLFxyXG4gIH0pO1xyXG59XHJcblxyXG4vKipcclxuICogbGVzcyBnbG9iYWwgdmFyaWFibGVcclxuICovXHJcbmV4cG9ydCBmdW5jdGlvbiBnZW5lcmF0ZU1vZGlmeVZhcnMoKSB7XHJcbiAgY29uc3QgcGFsZXR0ZXMgPSBnZW5lcmF0ZUFudENvbG9ycyhwcmltYXJ5Q29sb3IpO1xyXG4gIGNvbnN0IHByaW1hcnkgPSBwYWxldHRlc1s1XTtcclxuXHJcbiAgY29uc3QgcHJpbWFyeUNvbG9yT2JqOiBSZWNvcmQ8c3RyaW5nLCBzdHJpbmc+ID0ge307XHJcblxyXG4gIGZvciAobGV0IGluZGV4ID0gMDsgaW5kZXggPCAxMDsgaW5kZXgrKykge1xyXG4gICAgcHJpbWFyeUNvbG9yT2JqW2BwcmltYXJ5LSR7aW5kZXggKyAxfWBdID0gcGFsZXR0ZXNbaW5kZXhdO1xyXG4gIH1cclxuXHJcbiAgY29uc3QgbWFwVG9rZW4gPSBkZWZhdWx0QWxnb3JpdGhtKGRlZmF1bHRTZWVkKTtcclxuICBjb25zdCB2M1Rva2VuID0gY29udmVydExlZ2FjeVRva2VuLmRlZmF1bHQobWFwVG9rZW4pO1xyXG4gIHJldHVybiB7XHJcbiAgICAuLi52M1Rva2VuLFxyXG4gICAgLy8gVXNlZCBmb3IgZ2xvYmFsIGltcG9ydCB0byBhdm9pZCB0aGUgbmVlZCB0byBpbXBvcnQgZWFjaCBzdHlsZSBmaWxlIHNlcGFyYXRlbHlcclxuICAgIC8vIHJlZmVyZW5jZTogIEF2b2lkIHJlcGVhdGVkIHJlZmVyZW5jZXNcclxuICAgIGhhY2s6IGB0cnVlOyBAaW1wb3J0IChyZWZlcmVuY2UpIFwiJHtyZXNvbHZlKCdzcmMvZGVzaWduL2NvbmZpZy5sZXNzJyl9XCI7YCxcclxuICAgICdwcmltYXJ5LWNvbG9yJzogcHJpbWFyeSxcclxuICAgIC4uLnByaW1hcnlDb2xvck9iaixcclxuICAgICdpbmZvLWNvbG9yJzogcHJpbWFyeSxcclxuICAgICdwcm9jZXNzaW5nLWNvbG9yJzogcHJpbWFyeSxcclxuICAgICdzdWNjZXNzLWNvbG9yJzogJyM1NUQxODcnLCAvLyAgU3VjY2VzcyBjb2xvclxyXG4gICAgJ2Vycm9yLWNvbG9yJzogJyNFRDZGNkYnLCAvLyAgRmFsc2UgY29sb3JcclxuICAgICd3YXJuaW5nLWNvbG9yJzogJyNFRkJENDcnLCAvLyAgIFdhcm5pbmcgY29sb3JcclxuICAgICdidG4taW5mby1jb2xvcic6ICcjOTA5Mzk5JyxcclxuICAgICd0ZXh0LWNvbG9yLXNlY29uZGFyeSc6ICdyZ2JhKDAsIDAsIDAsIDAuNDUpJyxcclxuICAgICdib3JkZXItY29sb3ItYmFzZTEnOiAnI2YwZjBmMCcsXHJcbiAgICAnZm9udC1zaXplLWJhc2UnOiAnMTRweCcsIC8vICBNYWluIGZvbnQgc2l6ZVxyXG4gICAgJ2JvcmRlci1yYWRpdXMtYmFzZSc6ICcycHgnLCAvLyAgQ29tcG9uZW50L2Zsb2F0IGZpbGxldFxyXG4gICAgJ2xpbmstY29sb3InOiBwcmltYXJ5LCAvLyAgIExpbmsgY29sb3JcclxuICAgICdhcHAtYmFzZS1iYWNrZ3JvdW5kJzogJyNlYWVjZjAnLFxyXG4gICAgJ2FwcC1jb250ZW50LWJhY2tncm91bmQnOiAnI0YxRjRGOCcsIC8vICAgTGluayBjb2xvclxyXG4gICAgJ2FwcC1tYWluLWJhY2tncm91bmQnOiAnI2ViZWVmNScsXHJcbiAgICAnc2VsZWN0ZWQtaG92ZXItYmcnOiAnI2Y1ZjVmNScsXHJcbiAgICAnaG92ZXItYmFja2dyb3VuZCc6ICcjZjVmN2ZhJyxcclxuICB9O1xyXG59XHJcbiJdLAogICJtYXBwaW5ncyI6ICI7QUFBdVcsU0FBUyxvQkFBb0I7QUFDcFksU0FBUyw0QkFBNEI7QUFDckMsT0FBTyxVQUFVO0FBQ2pCLE9BQU8sU0FBUztBQUNoQixPQUFPLGFBQWE7QUFDcEIsT0FBTyxnQkFBZ0I7QUFDdkIsT0FBTyxnQkFBZ0I7QUFDdkIsU0FBUywyQkFBMkI7QUFDcEMsU0FBUyw0QkFBNEI7QUFDckMsU0FBUyxlQUFlLFdBQVc7QUFDbkMsT0FBTyxZQUFZOzs7QUNWK1gsU0FBUyxnQkFBZ0I7QUFFcGEsSUFBTSxlQUFlOzs7QUNENUIsU0FBUyxlQUFlO0FBQ3hCLFNBQVMsWUFBQUEsaUJBQWdCO0FBQ3pCLFNBQVMsYUFBYTtBQUN0QixPQUFPLHdCQUF3QjtBQUMvQixJQUFNLEVBQUUsa0JBQWtCLFlBQVksSUFBSTtBQUUxQyxTQUFTLGtCQUFrQixPQUFlQyxTQUE0QixXQUFXO0FBQy9FLFNBQU9DLFVBQVMsT0FBTztBQUFBLElBQ3JCLE9BQUFEO0FBQUEsRUFDRixDQUFDO0FBQ0g7QUFLTyxTQUFTLHFCQUFxQjtBQUNu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
