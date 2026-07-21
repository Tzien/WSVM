// vite.config.js
import { defineConfig } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/vite/dist/node/index.js";
import { createSvgIconsPlugin } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/vite-plugin-svg-icons/dist/index.mjs";
import path from "path";
import vue from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/@vitejs/plugin-vue/dist/index.mjs";
import qiankun from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/vite-plugin-qiankun/dist/index.js";
import Components from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/unplugin-vue-components/dist/vite.js";
import AutoImport from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/unplugin-auto-import/dist/vite.js";
import { ElementPlusResolver } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/unplugin-vue-components/dist/resolvers.js";
import { AntDesignVueResolver } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/unplugin-vue-components/dist/resolvers.js";
import { fileURLToPath, URL } from "node:url";
import vueJsx from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/@vitejs/plugin-vue-jsx/dist/index.mjs";

// build/config/themeConfig.ts
import { generate } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/@ant-design/colors/lib/index.js";
var primaryColor = "#1890ff";

// build/generate/generateModifyVars.ts
import { resolve } from "path";
import { generate as generate2 } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/@ant-design/colors/lib/index.js";
import { theme } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/ant-design-vue/lib/index.js";
import convertLegacyToken from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/ant-design-vue/lib/theme/convertLegacyToken.js";
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
import viteImagemin from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/vite-plugin-imagemin/dist/index.mjs";
import viteCompression from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/node_modules/vite-plugin-compression/dist/index.mjs";
var __vite_injected_original_import_meta_url = "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/cerios.core.lowcodeform/vite.config.js";
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
    qiankun(`lowcode-subApp`, {
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
  base: process.env.NODE_ENV === "development" ? "/" : "/subapp/lowcode",
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
    port: 8004,
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
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcuanMiLCAiYnVpbGQvY29uZmlnL3RoZW1lQ29uZmlnLnRzIiwgImJ1aWxkL2dlbmVyYXRlL2dlbmVyYXRlTW9kaWZ5VmFycy50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiY29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2Rpcm5hbWUgPSBcIkc6XFxcXERZWE1cXFxcXHU1RkFFXHU1MjREXHU3QUVGXFxcXG5ld0dpdFxcXFxjZXJpb3MuY29yZS5sb3djb2RlZm9ybVwiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiRzpcXFxcRFlYTVxcXFxcdTVGQUVcdTUyNERcdTdBRUZcXFxcbmV3R2l0XFxcXGNlcmlvcy5jb3JlLmxvd2NvZGVmb3JtXFxcXHZpdGUuY29uZmlnLmpzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9HOi9EWVhNLyVFNSVCRSVBRSVFNSU4OSU4RCVFNyVBQiVBRi9uZXdHaXQvY2VyaW9zLmNvcmUubG93Y29kZWZvcm0vdml0ZS5jb25maWcuanNcIjtpbXBvcnQgeyBkZWZpbmVDb25maWcgfSBmcm9tICd2aXRlJ1xyXG5pbXBvcnQgeyBjcmVhdGVTdmdJY29uc1BsdWdpbiB9IGZyb20gJ3ZpdGUtcGx1Z2luLXN2Zy1pY29ucydcclxuaW1wb3J0IHBhdGggZnJvbSAncGF0aCdcclxuaW1wb3J0IHZ1ZSBmcm9tICdAdml0ZWpzL3BsdWdpbi12dWUnXHJcbmltcG9ydCBxaWFua3VuIGZyb20gJ3ZpdGUtcGx1Z2luLXFpYW5rdW4nXHJcbmltcG9ydCBDb21wb25lbnRzIGZyb20gJ3VucGx1Z2luLXZ1ZS1jb21wb25lbnRzL3ZpdGUnXHJcbmltcG9ydCBBdXRvSW1wb3J0IGZyb20gJ3VucGx1Z2luLWF1dG8taW1wb3J0L3ZpdGUnXHJcbmltcG9ydCB7IEVsZW1lbnRQbHVzUmVzb2x2ZXIgfSBmcm9tICd1bnBsdWdpbi12dWUtY29tcG9uZW50cy9yZXNvbHZlcnMnXHJcbmltcG9ydCB7IEFudERlc2lnblZ1ZVJlc29sdmVyIH0gZnJvbSAndW5wbHVnaW4tdnVlLWNvbXBvbmVudHMvcmVzb2x2ZXJzJ1xyXG5pbXBvcnQgeyBmaWxlVVJMVG9QYXRoLCBVUkwgfSBmcm9tICdub2RlOnVybCdcclxuaW1wb3J0IHZ1ZUpzeCBmcm9tICdAdml0ZWpzL3BsdWdpbi12dWUtanN4J1xyXG5pbXBvcnQgeyBnZW5lcmF0ZU1vZGlmeVZhcnMgfSBmcm9tICcuL2J1aWxkL2dlbmVyYXRlL2dlbmVyYXRlTW9kaWZ5VmFycydcclxuXHJcbi8vIFx1NTZGRVx1NzI0N1x1NTM4Qlx1N0YyOVxyXG5pbXBvcnQgdml0ZUltYWdlbWluIGZyb20gJ3ZpdGUtcGx1Z2luLWltYWdlbWluJ1xyXG4vL1x1NjU4N1x1NEVGNlx1NTM4Qlx1N0YyOVxyXG5pbXBvcnQgdml0ZUNvbXByZXNzaW9uIGZyb20gJ3ZpdGUtcGx1Z2luLWNvbXByZXNzaW9uJ1xyXG4vLyBodHRwczovL3ZpdGVqcy5kZXYvY29uZmlnL1xyXG5leHBvcnQgZGVmYXVsdCBkZWZpbmVDb25maWcoe1xyXG4gIHBsdWdpbnM6IFtcclxuICAgIGNyZWF0ZVN2Z0ljb25zUGx1Z2luKHtcclxuICAgICAgLy8gXHU2MzA3XHU1QjlBXHU5NzAwXHU4OTgxXHU3RjEzXHU1QjU4XHU3Njg0XHU1NkZFXHU2ODA3XHU2NTg3XHU0RUY2XHU1OTM5XHJcbiAgICAgIGljb25EaXJzOiBbcGF0aC5yZXNvbHZlKHByb2Nlc3MuY3dkKCksICdzcmMvYXNzZXRzL2ljb25zJyldLFxyXG4gICAgICAvLyBcdTYzMDdcdTVCOUFzeW1ib2xJZFx1NjgzQ1x1NUYwRlxyXG4gICAgICBzeW1ib2xJZDogJ2ljb24tW2Rpcl0tW25hbWVdJyxcclxuICAgIH0pLFxyXG4gICAgdnVlSnN4KCksXHJcbiAgICAvLyBcdTU2RkVcdTcyNDdcdThENDRcdTZFOTBcdTUzOEJcdTdGMjlcclxuICAgIHZpdGVJbWFnZW1pbih7XHJcbiAgICAgIGdpZnNpY2xlOiB7XHJcbiAgICAgICAgLy8gZ2lmXHU1NkZFXHU3MjQ3XHU1MzhCXHU3RjI5XHJcbiAgICAgICAgb3B0aW1pemF0aW9uTGV2ZWw6IDMsIC8vIFx1OTAwOVx1NjJFOTFcdTUyMzAzXHU0RTRCXHU5NUY0XHU3Njg0XHU0RjE4XHU1MzE2XHU3RUE3XHU1MjJCXHJcbiAgICAgICAgaW50ZXJsYWNlZDogZmFsc2UgLy8gXHU5Njk0XHU4ODRDXHU2MjZCXHU2M0NGZ2lmXHU4RkRCXHU4ODRDXHU2RTEwXHU4RkRCXHU1RjBGXHU2RTMyXHU2N0QzXHJcbiAgICAgIH0sXHJcbiAgICAgIG9wdGlwbmc6IHtcclxuICAgICAgICAvLyBwbmdcclxuICAgICAgICBvcHRpbWl6YXRpb25MZXZlbDogNyAvLyBcdTkwMDlcdTYyRTkwXHU1MjMwN1x1NEU0Qlx1OTVGNFx1NzY4NFx1NEYxOFx1NTMxNlx1N0VBN1x1NTIyQlxyXG4gICAgICB9LFxyXG4gICAgICBtb3pqcGVnOiB7XHJcbiAgICAgICAgLy8ganBlZ1xyXG4gICAgICAgIHF1YWxpdHk6IDIwIC8vIFx1NTM4Qlx1N0YyOVx1OEQyOFx1OTFDRlx1RkYwQ1x1ODMwM1x1NTZGNFx1NEVDRTAoXHU2NzAwXHU1REVFKVx1NTIzMDEwMChcdTY3MDBcdTRGNzMpXHUzMDAyXHJcbiAgICAgIH0sXHJcbiAgICAgIHBuZ3F1YW50OiB7XHJcbiAgICAgICAgLy8gcG5nXHJcbiAgICAgICAgcXVhbGl0eTogWzAuOCwgMC45XSwgLy8gTWluXHU1NDhDbWF4XHU2NjJGXHU0RUNCXHU0RThFMChcdTY3MDBcdTVERUUpXHU1MjMwMShcdTY3MDBcdTRGNzMpXHU0RTRCXHU5NUY0XHU3Njg0XHU2NTcwXHU1QjU3XHVGRjBDXHU3QzdCXHU0RjNDXHU0RThFSlBFR1x1MzAwMlx1OEZCRVx1NTIzMFx1NjIxNlx1OEQ4NVx1OEZDN1x1NjcwMFx1OUFEOFx1OEQyOFx1OTFDRlx1NjI0MFx1OTcwMFx1NzY4NFx1NjcwMFx1NUMxMVx1OTFDRlx1NzY4NFx1OTg5Q1x1ODI3Mlx1MzAwMlx1NTk4Mlx1Njc5Q1x1OEY2Q1x1NjM2Mlx1NUJGQ1x1ODFGNFx1OEQyOFx1OTFDRlx1NEY0RVx1NEU4RVx1NjcwMFx1NEY0RVx1OEQyOFx1OTFDRlx1RkYwQ1x1NTZGRVx1NTBDRlx1NUMwNlx1NEUwRFx1NEYxQVx1ODhBQlx1NEZERFx1NUI1OFx1MzAwMlxyXG4gICAgICAgIHNwZWVkOiA0IC8vIFx1NTM4Qlx1N0YyOVx1OTAxRlx1NUVBNlx1RkYwQzEoXHU1RjNBXHU1MjlCKVx1NTIzMDExKFx1NjcwMFx1NUZFQilcclxuICAgICAgfSxcclxuICAgICAgc3Znbzoge1xyXG4gICAgICAgIHBsdWdpbnM6IFtcclxuICAgICAgICAgIC8vIHN2Z1x1NTM4Qlx1N0YyOVxyXG4gICAgICAgICAge1xyXG4gICAgICAgICAgICBuYW1lOiAncmVtb3ZlVmlld0JveCdcclxuICAgICAgICAgIH0sXHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIG5hbWU6ICdyZW1vdmVFbXB0eUF0dHJzJyxcclxuICAgICAgICAgICAgYWN0aXZlOiBmYWxzZVxyXG4gICAgICAgICAgfVxyXG4gICAgICAgIF1cclxuICAgICAgfVxyXG4gICAgfSksXHJcbiAgICB2aXRlQ29tcHJlc3Npb24oe1xyXG4gICAgICB2ZXJib3NlOiB0cnVlLCAvLyBcdTY2MkZcdTU0MjZcdTU3MjhcdTYzQTdcdTUyMzZcdTUzRjBcdTRFMkRcdThGOTNcdTUxRkFcdTUzOEJcdTdGMjlcdTdFRDNcdTY3OUNcclxuICAgICAgZGlzYWJsZTogZmFsc2UsXHJcbiAgICAgIHRocmVzaG9sZDogMTAyNDAsIC8vIFx1NTk4Mlx1Njc5Q1x1NEY1M1x1NzlFRlx1NTkyN1x1NEU4RVx1OTYwOFx1NTAzQ1x1RkYwQ1x1NUMwNlx1ODhBQlx1NTM4Qlx1N0YyOVx1RkYwQ1x1NTM1NVx1NEY0RFx1NEUzQWJcdUZGMENcdTRGNTNcdTc5RUZcdThGQzdcdTVDMEZcdTY1RjZcdThCRjdcdTRFMERcdTg5ODFcdTUzOEJcdTdGMjlcdUZGMENcdTRFRTVcdTUxNERcdTkwMDJcdTVGOTdcdTUxNzZcdTUzQ0RcclxuICAgICAgYWxnb3JpdGhtOiAnZ3ppcCcsIC8vIFx1NTM4Qlx1N0YyOVx1N0I5N1x1NkNENVx1RkYwQ1x1NTNFRlx1OTAwOVsnZ3ppcCdcdUZGMEMnIGJyb3RsaWNjb21wcmVzcyAnXHVGRjBDJ2RlZmxhdGUgJ1x1RkYwQydkZWZsYXRlUmF3J11cclxuICAgICAgZXh0OiAnLmd6JyxcclxuICAgICAgZGVsZXRlT3JpZ2luRmlsZTogZmFsc2UgLy8gXHU2RTkwXHU2NTg3XHU0RUY2XHU1MzhCXHU3RjI5XHU1NDBFXHU2NjJGXHU1NDI2XHU1MjIwXHU5NjY0KFx1NjIxMVx1NEUzQVx1NEU4Nlx1NzcwQlx1NTM4Qlx1N0YyOVx1NTQwRVx1NzY4NFx1NjU0OFx1Njc5Q1x1RkYwQ1x1NTE0OFx1OTAwOVx1NjJFOVx1NEU4NnRydWUpXHJcbiAgICB9KSxcclxuICAgIHZ1ZSgpLFxyXG4gICAgcWlhbmt1bihgbG93Y29kZS1zdWJBcHBgLCB7XHJcbiAgICAgIHVzZURldk1vZGU6IHRydWVcclxuICAgIH0pLFxyXG4gICAgLy9cdTUzRDZcdTZEODhcdTYzRDJcdTUxNjV2dWVcdTdFQzRcdTRFRjYtXHU4MUVBXHU1MkE4XHU2MzA5XHU5NzAwXHU1QkZDXHU1MTY1XHJcbiAgICBDb21wb25lbnRzKHtcclxuICAgICAgcmVzb2x2ZXJzOiBbXHJcbiAgICAgICAgQW50RGVzaWduVnVlUmVzb2x2ZXIoe1xyXG4gICAgICAgICAgaW1wb3J0U3R5bGU6IGZhbHNlIC8vIGNzcyBpbiBqc1xyXG4gICAgICAgIH0pLFxyXG4gICAgICAgIEVsZW1lbnRQbHVzUmVzb2x2ZXIoKVxyXG4gICAgICBdXHJcbiAgICB9KSxcclxuICAgIEF1dG9JbXBvcnQoe1xyXG4gICAgICByZXNvbHZlcnM6IFtFbGVtZW50UGx1c1Jlc29sdmVyKCldXHJcbiAgICB9KVxyXG4gIF0sXHJcbiAgY3NzOiB7XHJcbiAgICBwcmVwcm9jZXNzb3JPcHRpb25zOiB7XHJcbiAgICAgIGxlc3M6IHtcclxuICAgICAgICBtb2RpZnlWYXJzOiBnZW5lcmF0ZU1vZGlmeVZhcnMoKSxcclxuICAgICAgICBqYXZhc2NyaXB0RW5hYmxlZDogdHJ1ZVxyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfSxcclxuICBiYXNlOiBwcm9jZXNzLmVudi5OT0RFX0VOViA9PT0gJ2RldmVsb3BtZW50JyA/ICcvJyA6ICcvc3ViYXBwL2xvd2NvZGUnLFxyXG4gIGJ1aWxkOiB7XHJcbiAgICBzb3VyY2VtYXA6IHRydWUsIC8vIFx1NUJGOVx1NEU4RSBWaXRlXHJcbiAgICAvLyBcdTYzMDdcdTVCOUFcdThGOTNcdTUxRkFcdTc2RUVcdTVGNTVcclxuICAgIG91dERpcjogYGRpc3RgLFxyXG4gICAgdGFyZ2V0OiAnZXNuZXh0JywgLy8gXHU0RkVFXHU2NTM5XHU2MjUzXHU1MzA1XHU3NkVFXHU2ODA3XHU3M0FGXHU1ODgzXHJcbiAgICAvLyBcdTgxRUFcdTVCOUFcdTRFNDlcdTYyNTNcdTUzMDVcdTkxNERcdTdGNkVcclxuICAgIHJvbGx1cE9wdGlvbnM6IHtcclxuICAgICAgb3V0cHV0OiB7XHJcbiAgICAgICAgLy8gXHU5MTREXHU3RjZFIGNodW5rIFx1NTkyN1x1NUMwRlx1NTIwNlx1NzlCQlx1RkYwQ1x1NjNEMFx1NTM0N1x1NjAyN1x1ODBGRFxyXG4gICAgICAgIG1hbnVhbENodW5rcyhpZCkge1xyXG4gICAgICAgICAgaWYgKGlkLmluY2x1ZGVzKCdub2RlX21vZHVsZXMnKSkge1xyXG4gICAgICAgICAgICByZXR1cm4gJ3ZlbmRvcidcclxuICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuICAgIH1cclxuICB9LFxyXG4gIHNlcnZlcjoge1xyXG4gICAgcG9ydDogODAwNCxcclxuICAgIGhvc3Q6ICcwLjAuMC4wJyxcclxuICAgIGhlYWRlcnM6IHtcclxuICAgICAgJ0FjY2Vzcy1Db250cm9sLUFsbG93LU9yaWdpbic6ICcqJ1xyXG4gICAgfVxyXG4gIH0sXHJcbiAgcmVzb2x2ZToge1xyXG4gICAgYWxpYXM6IHtcclxuICAgICAgJ0AnOiBmaWxlVVJMVG9QYXRoKG5ldyBVUkwoJy4vc3JjJywgaW1wb3J0Lm1ldGEudXJsKSlcclxuICAgIH1cclxuICB9XHJcbn0pXHJcbiIsICJjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfZGlybmFtZSA9IFwiRzpcXFxcRFlYTVxcXFxcdTVGQUVcdTUyNERcdTdBRUZcXFxcbmV3R2l0XFxcXGNlcmlvcy5jb3JlLmxvd2NvZGVmb3JtXFxcXGJ1aWxkXFxcXGNvbmZpZ1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiRzpcXFxcRFlYTVxcXFxcdTVGQUVcdTUyNERcdTdBRUZcXFxcbmV3R2l0XFxcXGNlcmlvcy5jb3JlLmxvd2NvZGVmb3JtXFxcXGJ1aWxkXFxcXGNvbmZpZ1xcXFx0aGVtZUNvbmZpZy50c1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9pbXBvcnRfbWV0YV91cmwgPSBcImZpbGU6Ly8vRzovRFlYTS8lRTUlQkUlQUUlRTUlODklOEQlRTclQUIlQUYvbmV3R2l0L2Nlcmlvcy5jb3JlLmxvd2NvZGVmb3JtL2J1aWxkL2NvbmZpZy90aGVtZUNvbmZpZy50c1wiO2ltcG9ydCB7IGdlbmVyYXRlIH0gZnJvbSAnQGFudC1kZXNpZ24vY29sb3JzJztcblxuZXhwb3J0IGNvbnN0IHByaW1hcnlDb2xvciA9ICcjMTg5MGZmJztcblxuZXhwb3J0IGNvbnN0IGRhcmtNb2RlID0gJ2xpZ2h0JztcblxudHlwZSBGbiA9ICguLi5hcmc6IGFueSkgPT4gYW55O1xuXG50eXBlIEdlbmVyYXRlVGhlbWUgPSAnZGVmYXVsdCcgfCAnZGFyayc7XG5cbmV4cG9ydCBpbnRlcmZhY2UgR2VuZXJhdGVDb2xvcnNQYXJhbXMge1xuICBtaXhMaWdodGVuOiBGbjtcbiAgbWl4RGFya2VuOiBGbjtcbiAgdGlueWNvbG9yOiBhbnk7XG4gIGNvbG9yPzogc3RyaW5nO1xufVxuXG5leHBvcnQgZnVuY3Rpb24gZ2VuZXJhdGVBbnRDb2xvcnMoY29sb3I6IHN0cmluZywgdGhlbWU6IEdlbmVyYXRlVGhlbWUgPSAnZGVmYXVsdCcpIHtcbiAgcmV0dXJuIGdlbmVyYXRlKGNvbG9yLCB7XG4gICAgdGhlbWUsXG4gIH0pO1xufVxuXG5leHBvcnQgZnVuY3Rpb24gZ2V0VGhlbWVDb2xvcnMoY29sb3I/OiBzdHJpbmcpIHtcbiAgY29uc3QgdGMgPSBjb2xvciB8fCBwcmltYXJ5Q29sb3I7XG4gIGNvbnN0IGxpZ2h0Q29sb3JzID0gZ2VuZXJhdGVBbnRDb2xvcnModGMpO1xuICBjb25zdCBwcmltYXJ5ID0gbGlnaHRDb2xvcnNbNV07XG4gIGNvbnN0IG1vZGVDb2xvcnMgPSBnZW5lcmF0ZUFudENvbG9ycyhwcmltYXJ5LCAnZGFyaycpO1xuXG4gIHJldHVybiBbLi4ubGlnaHRDb2xvcnMsIC4uLm1vZGVDb2xvcnNdO1xufVxuXG5leHBvcnQgZnVuY3Rpb24gZ2VuZXJhdGVDb2xvcnMoeyBjb2xvciA9IHByaW1hcnlDb2xvciwgbWl4TGlnaHRlbiwgbWl4RGFya2VuLCB0aW55Y29sb3IgfTogR2VuZXJhdGVDb2xvcnNQYXJhbXMpIHtcbiAgY29uc3QgYXJyID0gbmV3IEFycmF5KDE5KS5maWxsKDApO1xuICBjb25zdCBsaWdodGVucyA9IGFyci5tYXAoKF90LCBpKSA9PiB7XG4gICAgcmV0dXJuIG1peExpZ2h0ZW4oY29sb3IsIGkgLyA1KTtcbiAgfSk7XG5cbiAgY29uc3QgZGFya2VucyA9IGFyci5tYXAoKF90LCBpKSA9PiB7XG4gICAgcmV0dXJuIG1peERhcmtlbihjb2xvciwgaSAvIDUpO1xuICB9KTtcblxuICBjb25zdCBhbHBoYUNvbG9ycyA9IGFyci5tYXAoKF90LCBpKSA9PiB7XG4gICAgcmV0dXJuIHRpbnljb2xvcihjb2xvcilcbiAgICAgIC5zZXRBbHBoYShpIC8gMjApXG4gICAgICAudG9SZ2JTdHJpbmcoKTtcbiAgfSk7XG5cbiAgY29uc3Qgc2hvcnRBbHBoYUNvbG9ycyA9IGFscGhhQ29sb3JzLm1hcChpdGVtID0+IGl0ZW0ucmVwbGFjZSgvXFxzL2csICcnKS5yZXBsYWNlKC8wXFwuL2csICcuJykpO1xuXG4gIGNvbnN0IHRpbnljb2xvckxpZ2h0ZW5zID0gYXJyXG4gICAgLm1hcCgoX3QsIGkpID0+IHtcbiAgICAgIHJldHVybiB0aW55Y29sb3IoY29sb3IpXG4gICAgICAgIC5saWdodGVuKGkgKiA1KVxuICAgICAgICAudG9IZXhTdHJpbmcoKTtcbiAgICB9KVxuICAgIC5maWx0ZXIoaXRlbSA9PiBpdGVtICE9PSAnI2ZmZmZmZicpO1xuXG4gIGNvbnN0IHRpbnljb2xvckRhcmtlbnMgPSBhcnJcbiAgICAubWFwKChfdCwgaSkgPT4ge1xuICAgICAgcmV0dXJuIHRpbnljb2xvcihjb2xvcilcbiAgICAgICAgLmRhcmtlbihpICogNSlcbiAgICAgICAgLnRvSGV4U3RyaW5nKCk7XG4gICAgfSlcbiAgICAuZmlsdGVyKGl0ZW0gPT4gaXRlbSAhPT0gJyMwMDAwMDAnKTtcbiAgcmV0dXJuIFsuLi5saWdodGVucywgLi4uZGFya2VucywgLi4uYWxwaGFDb2xvcnMsIC4uLnNob3J0QWxwaGFDb2xvcnMsIC4uLnRpbnljb2xvckRhcmtlbnMsIC4uLnRpbnljb2xvckxpZ2h0ZW5zXS5maWx0ZXIoaXRlbSA9PiAhaXRlbS5pbmNsdWRlcygnLScpKTtcbn1cbiIsICJjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfZGlybmFtZSA9IFwiRzpcXFxcRFlYTVxcXFxcdTVGQUVcdTUyNERcdTdBRUZcXFxcbmV3R2l0XFxcXGNlcmlvcy5jb3JlLmxvd2NvZGVmb3JtXFxcXGJ1aWxkXFxcXGdlbmVyYXRlXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJHOlxcXFxEWVhNXFxcXFx1NUZBRVx1NTI0RFx1N0FFRlxcXFxuZXdHaXRcXFxcY2VyaW9zLmNvcmUubG93Y29kZWZvcm1cXFxcYnVpbGRcXFxcZ2VuZXJhdGVcXFxcZ2VuZXJhdGVNb2RpZnlWYXJzLnRzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9HOi9EWVhNLyVFNSVCRSVBRSVFNSU4OSU4RCVFNyVBQiVBRi9uZXdHaXQvY2VyaW9zLmNvcmUubG93Y29kZWZvcm0vYnVpbGQvZ2VuZXJhdGUvZ2VuZXJhdGVNb2RpZnlWYXJzLnRzXCI7aW1wb3J0IHsgcHJpbWFyeUNvbG9yIH0gZnJvbSAnLi4vY29uZmlnL3RoZW1lQ29uZmlnJztcclxuaW1wb3J0IHsgcmVzb2x2ZSB9IGZyb20gJ3BhdGgnO1xyXG5pbXBvcnQgeyBnZW5lcmF0ZSB9IGZyb20gJ0BhbnQtZGVzaWduL2NvbG9ycyc7XHJcbmltcG9ydCB7IHRoZW1lIH0gZnJvbSAnYW50LWRlc2lnbi12dWUvbGliJztcclxuaW1wb3J0IGNvbnZlcnRMZWdhY3lUb2tlbiBmcm9tICdhbnQtZGVzaWduLXZ1ZS9saWIvdGhlbWUvY29udmVydExlZ2FjeVRva2VuJztcclxuY29uc3QgeyBkZWZhdWx0QWxnb3JpdGhtLCBkZWZhdWx0U2VlZCB9ID0gdGhlbWU7XHJcblxyXG5mdW5jdGlvbiBnZW5lcmF0ZUFudENvbG9ycyhjb2xvcjogc3RyaW5nLCB0aGVtZTogJ2RlZmF1bHQnIHwgJ2RhcmsnID0gJ2RlZmF1bHQnKSB7XHJcbiAgcmV0dXJuIGdlbmVyYXRlKGNvbG9yLCB7XHJcbiAgICB0aGVtZSxcclxuICB9KTtcclxufVxyXG5cclxuLyoqXHJcbiAqIGxlc3MgZ2xvYmFsIHZhcmlhYmxlXHJcbiAqL1xyXG5leHBvcnQgZnVuY3Rpb24gZ2VuZXJhdGVNb2RpZnlWYXJzKCkge1xyXG4gIGNvbnN0IHBhbGV0dGVzID0gZ2VuZXJhdGVBbnRDb2xvcnMocHJpbWFyeUNvbG9yKTtcclxuICBjb25zdCBwcmltYXJ5ID0gcGFsZXR0ZXNbNV07XHJcblxyXG4gIGNvbnN0IHByaW1hcnlDb2xvck9iajogUmVjb3JkPHN0cmluZywgc3RyaW5nPiA9IHt9O1xyXG5cclxuICBmb3IgKGxldCBpbmRleCA9IDA7IGluZGV4IDwgMTA7IGluZGV4KyspIHtcclxuICAgIHByaW1hcnlDb2xvck9ialtgcHJpbWFyeS0ke2luZGV4ICsgMX1gXSA9IHBhbGV0dGVzW2luZGV4XTtcclxuICB9XHJcblxyXG4gIGNvbnN0IG1hcFRva2VuID0gZGVmYXVsdEFsZ29yaXRobShkZWZhdWx0U2VlZCk7XHJcbiAgY29uc3QgdjNUb2tlbiA9IGNvbnZlcnRMZWdhY3lUb2tlbi5kZWZhdWx0KG1hcFRva2VuKTtcclxuICByZXR1cm4ge1xyXG4gICAgLi4udjNUb2tlbixcclxuICAgIC8vIFVzZWQgZm9yIGdsb2JhbCBpbXBvcnQgdG8gYXZvaWQgdGhlIG5lZWQgdG8gaW1wb3J0IGVhY2ggc3R5bGUgZmlsZSBzZXBhcmF0ZWx5XHJcbiAgICAvLyByZWZlcmVuY2U6ICBBdm9pZCByZXBlYXRlZCByZWZlcmVuY2VzXHJcbiAgICBoYWNrOiBgdHJ1ZTsgQGltcG9ydCAocmVmZXJlbmNlKSBcIiR7cmVzb2x2ZSgnc3JjL2Rlc2lnbi9jb25maWcubGVzcycpfVwiO2AsXHJcbiAgICAncHJpbWFyeS1jb2xvcic6IHByaW1hcnksXHJcbiAgICAuLi5wcmltYXJ5Q29sb3JPYmosXHJcbiAgICAnaW5mby1jb2xvcic6IHByaW1hcnksXHJcbiAgICAncHJvY2Vzc2luZy1jb2xvcic6IHByaW1hcnksXHJcbiAgICAnc3VjY2Vzcy1jb2xvcic6ICcjNTVEMTg3JywgLy8gIFN1Y2Nlc3MgY29sb3JcclxuICAgICdlcnJvci1jb2xvcic6ICcjRUQ2RjZGJywgLy8gIEZhbHNlIGNvbG9yXHJcbiAgICAnd2FybmluZy1jb2xvcic6ICcjRUZCRDQ3JywgLy8gICBXYXJuaW5nIGNvbG9yXHJcbiAgICAnYnRuLWluZm8tY29sb3InOiAnIzkwOTM5OScsXHJcbiAgICAndGV4dC1jb2xvci1zZWNvbmRhcnknOiAncmdiYSgwLCAwLCAwLCAwLjQ1KScsXHJcbiAgICAnYm9yZGVyLWNvbG9yLWJhc2UxJzogJyNmMGYwZjAnLFxyXG4gICAgJ2ZvbnQtc2l6ZS1iYXNlJzogJzE0cHgnLCAvLyAgTWFpbiBmb250IHNpemVcclxuICAgICdib3JkZXItcmFkaXVzLWJhc2UnOiAnMnB4JywgLy8gIENvbXBvbmVudC9mbG9hdCBmaWxsZXRcclxuICAgICdsaW5rLWNvbG9yJzogcHJpbWFyeSwgLy8gICBMaW5rIGNvbG9yXHJcbiAgICAnYXBwLWJhc2UtYmFja2dyb3VuZCc6ICcjZWFlY2YwJyxcclxuICAgICdhcHAtY29udGVudC1iYWNrZ3JvdW5kJzogJyNGMUY0RjgnLCAvLyAgIExpbmsgY29sb3JcclxuICAgICdhcHAtbWFpbi1iYWNrZ3JvdW5kJzogJyNlYmVlZjUnLFxyXG4gICAgJ3NlbGVjdGVkLWhvdmVyLWJnJzogJyNmNWY1ZjUnLFxyXG4gICAgJ2hvdmVyLWJhY2tncm91bmQnOiAnI2Y1ZjdmYScsXHJcbiAgfTtcclxufVxyXG4iXSwKICAibWFwcGluZ3MiOiAiO0FBQWtWLFNBQVMsb0JBQW9CO0FBQy9XLFNBQVMsNEJBQTRCO0FBQ3JDLE9BQU8sVUFBVTtBQUNqQixPQUFPLFNBQVM7QUFDaEIsT0FBTyxhQUFhO0FBQ3BCLE9BQU8sZ0JBQWdCO0FBQ3ZCLE9BQU8sZ0JBQWdCO0FBQ3ZCLFNBQVMsMkJBQTJCO0FBQ3BDLFNBQVMsNEJBQTRCO0FBQ3JDLFNBQVMsZUFBZSxXQUFXO0FBQ25DLE9BQU8sWUFBWTs7O0FDVjBXLFNBQVMsZ0JBQWdCO0FBRS9ZLElBQU0sZUFBZTs7O0FDRDVCLFNBQVMsZUFBZTtBQUN4QixTQUFTLFlBQUFBLGlCQUFnQjtBQUN6QixTQUFTLGFBQWE7QUFDdEIsT0FBTyx3QkFBd0I7QUFDL0IsSUFBTSxFQUFFLGtCQUFrQixZQUFZLElBQUk7QUFFMUMsU0FBUyxrQkFBa0IsT0FBZUMsU0FBNEIsV0FBVztBQUMvRSxTQUFPQyxVQUFTLE9BQU87QUFBQSxJQUNyQixPQUFBRDtBQUFBLEVBQ0YsQ0FBQztBQUNIO0FBS08sU0FBUyxxQkFBcUI7QUFDbkMsUUFBTSxXQUFXLGtCQUFrQixZQUFZO0FBQy9DLFFBQU0sVUFBVSxTQUFTLENBQUM7QUFFMUIsUUFBTSxrQkFBMEMsQ0FB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RUFDQSxNQUFNLFFBQVEsSUFBSSxhQUFhLGdCQUFnQixNQUFNO0FBQUEsRUFDckQsT0FBTztBQUFBLElBQ0wsV0FBVztBQUFBO0FBQUE7QUFBQSxJQUVYLFFBQVE7QUFBQSxJQUNSLFFBQVE7QUFBQTtBQUFBO0FBQUEsSUFFUixlQUFlO0FBQUEsTUFDYixRQUFRO0FBQUE7QUFBQSxRQUVOLGFBQWEsSUFBSTtBQUNmLGNBQUksR0FBRyxTQUFTLGNBQWMsR0FBRztBQUMvQixtQkFBTztBQUFBLFVBQ1Q7QUFBQSxRQUNGO0FBQUEsTUFDRjtBQUFBLElBQ0Y7QUFBQSxFQUNGO0FBQUEsRUFDQSxRQUFRO0FBQUEsSUFDTixNQUFNO0FBQUEsSUFDTixNQUFNO0FBQUEsSUFDTixTQUFTO0FBQUEsTUFDUCwrQkFBK0I7QUFBQSxJQUNqQztBQUFBLEVBQ0Y7QUFBQSxFQUNBLFNBQVM7QUFBQSxJQUNQLE9BQU87QUFBQSxNQUNMLEtBQUssY0FBYyxJQUFJLElBQUksU0FBUyx3Q0FBZSxDQUFDO0FBQUEsSUFDdEQ7QUFBQSxFQUNGO0FBQ0YsQ0FBQzsiLAogICJuYW1lcyI6IFsiZ2VuZXJhdGUiLCAidGhlbWUiLCAiZ2VuZXJhdGUiXQp9Cg==
