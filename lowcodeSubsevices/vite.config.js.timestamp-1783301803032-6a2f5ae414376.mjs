// vite.config.js
import { defineConfig } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/vite/dist/node/index.js";
import { createSvgIconsPlugin } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/vite-plugin-svg-icons/dist/index.mjs";
import path from "path";
import vue from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/@vitejs/plugin-vue/dist/index.mjs";
import qiankun from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/vite-plugin-qiankun/dist/index.js";

// build/config/themeConfig.ts
import { generate } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/@ant-design/colors/lib/index.js";
var primaryColor = "#1890ff";

// build/generate/generateModifyVars.ts
import { resolve } from "path";
import { generate as generate2 } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/@ant-design/colors/lib/index.js";
import { theme } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/ant-design-vue/lib/index.js";
import convertLegacyToken from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/ant-design-vue/lib/theme/convertLegacyToken.js";
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
import Components from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/unplugin-vue-components/dist/vite.js";
import AutoImport from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/unplugin-auto-import/dist/vite.js";
import { ElementPlusResolver } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/unplugin-vue-components/dist/resolvers.js";
import { AntDesignVueResolver } from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/unplugin-vue-components/dist/resolvers.js";
import { fileURLToPath, URL } from "node:url";
import vueJsx from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/@vitejs/plugin-vue-jsx/dist/index.mjs";
import viteImagemin from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/vite-plugin-imagemin/dist/index.mjs";
import viteCompression from "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/node_modules/vite-plugin-compression/dist/index.mjs";
var __vite_injected_original_import_meta_url = "file:///G:/DYXM/%E5%BE%AE%E5%89%8D%E7%AB%AF/newGit/LowCodeWS/lowcodeSubsevices/cerios.core.lowcodesubservices/vite.config.js";
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
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcuanMiLCAiYnVpbGQvY29uZmlnL3RoZW1lQ29uZmlnLnRzIiwgImJ1aWxkL2dlbmVyYXRlL2dlbmVyYXRlTW9kaWZ5VmFycy50cyJdLAogICJzb3VyY2VzQ29udGVudCI6IFsiY29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2Rpcm5hbWUgPSBcIkc6XFxcXERZWE1cXFxcXHU1RkFFXHU1MjREXHU3QUVGXFxcXG5ld0dpdFxcXFxMb3dDb2RlV1NcXFxcbG93Y29kZVN1YnNldmljZXNcXFxcY2VyaW9zLmNvcmUubG93Y29kZXN1YnNlcnZpY2VzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJHOlxcXFxEWVhNXFxcXFx1NUZBRVx1NTI0RFx1N0FFRlxcXFxuZXdHaXRcXFxcTG93Q29kZVdTXFxcXGxvd2NvZGVTdWJzZXZpY2VzXFxcXGNlcmlvcy5jb3JlLmxvd2NvZGVzdWJzZXJ2aWNlc1xcXFx2aXRlLmNvbmZpZy5qc1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9pbXBvcnRfbWV0YV91cmwgPSBcImZpbGU6Ly8vRzovRFlYTS8lRTUlQkUlQUUlRTUlODklOEQlRTclQUIlQUYvbmV3R2l0L0xvd0NvZGVXUy9sb3djb2RlU3Vic2V2aWNlcy9jZXJpb3MuY29yZS5sb3djb2Rlc3Vic2VydmljZXMvdml0ZS5jb25maWcuanNcIjtpbXBvcnQgeyBkZWZpbmVDb25maWcgfSBmcm9tICd2aXRlJ1xyXG5pbXBvcnQgeyBjcmVhdGVTdmdJY29uc1BsdWdpbiB9IGZyb20gJ3ZpdGUtcGx1Z2luLXN2Zy1pY29ucydcclxuaW1wb3J0IHBhdGggZnJvbSAncGF0aCdcclxuaW1wb3J0IHZ1ZSBmcm9tICdAdml0ZWpzL3BsdWdpbi12dWUnXHJcbmltcG9ydCBxaWFua3VuIGZyb20gJ3ZpdGUtcGx1Z2luLXFpYW5rdW4nXHJcbmltcG9ydCB7IGdlbmVyYXRlTW9kaWZ5VmFycyB9IGZyb20gJy4vYnVpbGQvZ2VuZXJhdGUvZ2VuZXJhdGVNb2RpZnlWYXJzJ1xyXG5pbXBvcnQgQ29tcG9uZW50cyBmcm9tICd1bnBsdWdpbi12dWUtY29tcG9uZW50cy92aXRlJ1xyXG5pbXBvcnQgQXV0b0ltcG9ydCBmcm9tICd1bnBsdWdpbi1hdXRvLWltcG9ydC92aXRlJ1xyXG5pbXBvcnQgeyBFbGVtZW50UGx1c1Jlc29sdmVyIH0gZnJvbSAndW5wbHVnaW4tdnVlLWNvbXBvbmVudHMvcmVzb2x2ZXJzJ1xyXG5pbXBvcnQgeyBBbnREZXNpZ25WdWVSZXNvbHZlciB9IGZyb20gJ3VucGx1Z2luLXZ1ZS1jb21wb25lbnRzL3Jlc29sdmVycydcclxuaW1wb3J0IHsgZmlsZVVSTFRvUGF0aCwgVVJMIH0gZnJvbSAnbm9kZTp1cmwnXHJcbmltcG9ydCB2dWVKc3ggZnJvbSAnQHZpdGVqcy9wbHVnaW4tdnVlLWpzeCdcclxuXHJcbi8vIFx1NTZGRVx1NzI0N1x1NTM4Qlx1N0YyOVxyXG5pbXBvcnQgdml0ZUltYWdlbWluIGZyb20gJ3ZpdGUtcGx1Z2luLWltYWdlbWluJ1xyXG4vL1x1NjU4N1x1NEVGNlx1NTM4Qlx1N0YyOVxyXG5pbXBvcnQgdml0ZUNvbXByZXNzaW9uIGZyb20gJ3ZpdGUtcGx1Z2luLWNvbXByZXNzaW9uJ1xyXG4vLyBodHRwczovL3ZpdGVqcy5kZXYvY29uZmlnL1xyXG5leHBvcnQgZGVmYXVsdCBkZWZpbmVDb25maWcoe1xyXG4gIHBsdWdpbnM6IFtcclxuICAgIGNyZWF0ZVN2Z0ljb25zUGx1Z2luKHtcclxuICAgICAgLy8gXHU2MzA3XHU1QjlBXHU5NzAwXHU4OTgxXHU3RjEzXHU1QjU4XHU3Njg0XHU1NkZFXHU2ODA3XHU2NTg3XHU0RUY2XHU1OTM5XHJcbiAgICAgIGljb25EaXJzOiBbcGF0aC5yZXNvbHZlKHByb2Nlc3MuY3dkKCksICdzcmMvYXNzZXRzL2ljb25zJyldLFxyXG4gICAgICAvLyBcdTYzMDdcdTVCOUFzeW1ib2xJZFx1NjgzQ1x1NUYwRlxyXG4gICAgICBzeW1ib2xJZDogJ2ljb24tW2Rpcl0tW25hbWVdJ1xyXG4gICAgfSksXHJcbiAgICB2dWVKc3goKSxcclxuICAgIC8vIFx1NTZGRVx1NzI0N1x1OEQ0NFx1NkU5MFx1NTM4Qlx1N0YyOVxyXG4gICAgdml0ZUltYWdlbWluKHtcclxuICAgICAgZ2lmc2ljbGU6IHtcclxuICAgICAgICAvLyBnaWZcdTU2RkVcdTcyNDdcdTUzOEJcdTdGMjlcclxuICAgICAgICBvcHRpbWl6YXRpb25MZXZlbDogMywgLy8gXHU5MDA5XHU2MkU5MVx1NTIzMDNcdTRFNEJcdTk1RjRcdTc2ODRcdTRGMThcdTUzMTZcdTdFQTdcdTUyMkJcclxuICAgICAgICBpbnRlcmxhY2VkOiBmYWxzZSAvLyBcdTk2OTRcdTg4NENcdTYyNkJcdTYzQ0ZnaWZcdThGREJcdTg4NENcdTZFMTBcdThGREJcdTVGMEZcdTZFMzJcdTY3RDNcclxuICAgICAgfSxcclxuICAgICAgb3B0aXBuZzoge1xyXG4gICAgICAgIC8vIHBuZ1xyXG4gICAgICAgIG9wdGltaXphdGlvbkxldmVsOiA3IC8vIFx1OTAwOVx1NjJFOTBcdTUyMzA3XHU0RTRCXHU5NUY0XHU3Njg0XHU0RjE4XHU1MzE2XHU3RUE3XHU1MjJCXHJcbiAgICAgIH0sXHJcbiAgICAgIG1vempwZWc6IHtcclxuICAgICAgICAvLyBqcGVnXHJcbiAgICAgICAgcXVhbGl0eTogMjAgLy8gXHU1MzhCXHU3RjI5XHU4RDI4XHU5MUNGXHVGRjBDXHU4MzAzXHU1NkY0XHU0RUNFMChcdTY3MDBcdTVERUUpXHU1MjMwMTAwKFx1NjcwMFx1NEY3MylcdTMwMDJcclxuICAgICAgfSxcclxuICAgICAgcG5ncXVhbnQ6IHtcclxuICAgICAgICAvLyBwbmdcclxuICAgICAgICBxdWFsaXR5OiBbMC44LCAwLjldLCAvLyBNaW5cdTU0OENtYXhcdTY2MkZcdTRFQ0JcdTRFOEUwKFx1NjcwMFx1NURFRSlcdTUyMzAxKFx1NjcwMFx1NEY3MylcdTRFNEJcdTk1RjRcdTc2ODRcdTY1NzBcdTVCNTdcdUZGMENcdTdDN0JcdTRGM0NcdTRFOEVKUEVHXHUzMDAyXHU4RkJFXHU1MjMwXHU2MjE2XHU4RDg1XHU4RkM3XHU2NzAwXHU5QUQ4XHU4RDI4XHU5MUNGXHU2MjQwXHU5NzAwXHU3Njg0XHU2NzAwXHU1QzExXHU5MUNGXHU3Njg0XHU5ODlDXHU4MjcyXHUzMDAyXHU1OTgyXHU2NzlDXHU4RjZDXHU2MzYyXHU1QkZDXHU4MUY0XHU4RDI4XHU5MUNGXHU0RjRFXHU0RThFXHU2NzAwXHU0RjRFXHU4RDI4XHU5MUNGXHVGRjBDXHU1NkZFXHU1MENGXHU1QzA2XHU0RTBEXHU0RjFBXHU4OEFCXHU0RkREXHU1QjU4XHUzMDAyXHJcbiAgICAgICAgc3BlZWQ6IDQgLy8gXHU1MzhCXHU3RjI5XHU5MDFGXHU1RUE2XHVGRjBDMShcdTVGM0FcdTUyOUIpXHU1MjMwMTEoXHU2NzAwXHU1RkVCKVxyXG4gICAgICB9LFxyXG4gICAgICBzdmdvOiB7XHJcbiAgICAgICAgcGx1Z2luczogW1xyXG4gICAgICAgICAgLy8gc3ZnXHU1MzhCXHU3RjI5XHJcbiAgICAgICAgICB7XHJcbiAgICAgICAgICAgIG5hbWU6ICdyZW1vdmVWaWV3Qm94J1xyXG4gICAgICAgICAgfSxcclxuICAgICAgICAgIHtcclxuICAgICAgICAgICAgbmFtZTogJ3JlbW92ZUVtcHR5QXR0cnMnLFxyXG4gICAgICAgICAgICBhY3RpdmU6IGZhbHNlXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgXVxyXG4gICAgICB9XHJcbiAgICB9KSxcclxuICAgIHZpdGVDb21wcmVzc2lvbih7XHJcbiAgICAgIHZlcmJvc2U6IHRydWUsIC8vIFx1NjYyRlx1NTQyNlx1NTcyOFx1NjNBN1x1NTIzNlx1NTNGMFx1NEUyRFx1OEY5M1x1NTFGQVx1NTM4Qlx1N0YyOVx1N0VEM1x1Njc5Q1xyXG4gICAgICBkaXNhYmxlOiBmYWxzZSxcclxuICAgICAgdGhyZXNob2xkOiAxMDI0MCwgLy8gXHU1OTgyXHU2NzlDXHU0RjUzXHU3OUVGXHU1OTI3XHU0RThFXHU5NjA4XHU1MDNDXHVGRjBDXHU1QzA2XHU4OEFCXHU1MzhCXHU3RjI5XHVGRjBDXHU1MzU1XHU0RjREXHU0RTNBYlx1RkYwQ1x1NEY1M1x1NzlFRlx1OEZDN1x1NUMwRlx1NjVGNlx1OEJGN1x1NEUwRFx1ODk4MVx1NTM4Qlx1N0YyOVx1RkYwQ1x1NEVFNVx1NTE0RFx1OTAwMlx1NUY5N1x1NTE3Nlx1NTNDRFxyXG4gICAgICBhbGdvcml0aG06ICdnemlwJywgLy8gXHU1MzhCXHU3RjI5XHU3Qjk3XHU2Q0Q1XHVGRjBDXHU1M0VGXHU5MDA5WydnemlwJ1x1RkYwQycgYnJvdGxpY2NvbXByZXNzICdcdUZGMEMnZGVmbGF0ZSAnXHVGRjBDJ2RlZmxhdGVSYXcnXVxyXG4gICAgICBleHQ6ICcuZ3onLFxyXG4gICAgICBkZWxldGVPcmlnaW5GaWxlOiBmYWxzZSAvLyBcdTZFOTBcdTY1ODdcdTRFRjZcdTUzOEJcdTdGMjlcdTU0MEVcdTY2MkZcdTU0MjZcdTUyMjBcdTk2NjQoXHU2MjExXHU0RTNBXHU0RTg2XHU3NzBCXHU1MzhCXHU3RjI5XHU1NDBFXHU3Njg0XHU2NTQ4XHU2NzlDXHVGRjBDXHU1MTQ4XHU5MDA5XHU2MkU5XHU0RTg2dHJ1ZSlcclxuICAgIH0pLFxyXG4gICAgdnVlKCksXHJcbiAgICBxaWFua3VuKGBsb3djb2RlZGVtby1zdWJBcHBgLCB7XHJcbiAgICAgIHVzZURldk1vZGU6IHRydWVcclxuICAgIH0pLFxyXG4gICAgLy9cdTUzRDZcdTZEODhcdTYzRDJcdTUxNjV2dWVcdTdFQzRcdTRFRjYtXHU4MUVBXHU1MkE4XHU2MzA5XHU5NzAwXHU1QkZDXHU1MTY1XHJcbiAgICBDb21wb25lbnRzKHtcclxuICAgICAgcmVzb2x2ZXJzOiBbXHJcbiAgICAgICAgQW50RGVzaWduVnVlUmVzb2x2ZXIoe1xyXG4gICAgICAgICAgaW1wb3J0U3R5bGU6IGZhbHNlIC8vIGNzcyBpbiBqc1xyXG4gICAgICAgIH0pLFxyXG4gICAgICAgIEVsZW1lbnRQbHVzUmVzb2x2ZXIoKVxyXG4gICAgICBdXHJcbiAgICB9KSxcclxuICAgIEF1dG9JbXBvcnQoe1xyXG4gICAgICByZXNvbHZlcnM6IFtFbGVtZW50UGx1c1Jlc29sdmVyKCldXHJcbiAgICB9KVxyXG4gIF0sXHJcbiAgY3NzOiB7XHJcbiAgICBwcmVwcm9jZXNzb3JPcHRpb25zOiB7XHJcbiAgICAgIGxlc3M6IHtcclxuICAgICAgICBtb2RpZnlWYXJzOiBnZW5lcmF0ZU1vZGlmeVZhcnMoKSxcclxuICAgICAgICBqYXZhc2NyaXB0RW5hYmxlZDogdHJ1ZVxyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfSxcclxuICBiYXNlOiBwcm9jZXNzLmVudi5OT0RFX0VOViA9PT0gJ2RldmVsb3BtZW50JyA/ICcvJyA6ICcvc3ViYXBwL2xvd2NvZGVkZW1vJyxcclxuICBidWlsZDoge1xyXG4gICAgc291cmNlbWFwOiB0cnVlLCAvLyBcdTVCRjlcdTRFOEUgVml0ZVxyXG4gICAgLy8gXHU2MzA3XHU1QjlBXHU4RjkzXHU1MUZBXHU3NkVFXHU1RjU1XHJcbiAgICBvdXREaXI6IGBkaXN0YCxcclxuICAgIHRhcmdldDogJ2VzbmV4dCcsIC8vIFx1NEZFRVx1NjUzOVx1NjI1M1x1NTMwNVx1NzZFRVx1NjgwN1x1NzNBRlx1NTg4M1xyXG4gICAgLy8gXHU4MUVBXHU1QjlBXHU0RTQ5XHU2MjUzXHU1MzA1XHU5MTREXHU3RjZFXHJcbiAgICByb2xsdXBPcHRpb25zOiB7XHJcbiAgICAgIG91dHB1dDoge1xyXG4gICAgICAgIC8vIFx1OTE0RFx1N0Y2RSBjaHVuayBcdTU5MjdcdTVDMEZcdTUyMDZcdTc5QkJcdUZGMENcdTYzRDBcdTUzNDdcdTYwMjdcdTgwRkRcclxuICAgICAgICBtYW51YWxDaHVua3MoaWQpIHtcclxuICAgICAgICAgIGlmIChpZC5pbmNsdWRlcygnbm9kZV9tb2R1bGVzJykpIHtcclxuICAgICAgICAgICAgcmV0dXJuICd2ZW5kb3InXHJcbiAgICAgICAgICB9XHJcbiAgICAgICAgfVxyXG4gICAgICB9XHJcbiAgICB9XHJcbiAgfSxcclxuICBzZXJ2ZXI6IHtcclxuICAgIHBvcnQ6IDgwMDMsXHJcbiAgICBob3N0OiAnMC4wLjAuMCcsXHJcbiAgICBoZWFkZXJzOiB7XHJcbiAgICAgICdBY2Nlc3MtQ29udHJvbC1BbGxvdy1PcmlnaW4nOiAnKidcclxuICAgIH1cclxuICB9LFxyXG4gIHJlc29sdmU6IHtcclxuICAgIGFsaWFzOiB7XHJcbiAgICAgICdAJzogZmlsZVVSTFRvUGF0aChuZXcgVVJMKCcuL3NyYycsIGltcG9ydC5tZXRhLnVybCkpXHJcbiAgICB9XHJcbiAgfVxyXG59KVxyXG4iLCAiY29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2Rpcm5hbWUgPSBcIkc6XFxcXERZWE1cXFxcXHU1RkFFXHU1MjREXHU3QUVGXFxcXG5ld0dpdFxcXFxMb3dDb2RlV1NcXFxcbG93Y29kZVN1YnNldmljZXNcXFxcY2VyaW9zLmNvcmUubG93Y29kZXN1YnNlcnZpY2VzXFxcXGJ1aWxkXFxcXGNvbmZpZ1wiO2NvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9maWxlbmFtZSA9IFwiRzpcXFxcRFlYTVxcXFxcdTVGQUVcdTUyNERcdTdBRUZcXFxcbmV3R2l0XFxcXExvd0NvZGVXU1xcXFxsb3djb2RlU3Vic2V2aWNlc1xcXFxjZXJpb3MuY29yZS5sb3djb2Rlc3Vic2VydmljZXNcXFxcYnVpbGRcXFxcY29uZmlnXFxcXHRoZW1lQ29uZmlnLnRzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9HOi9EWVhNLyVFNSVCRSVBRSVFNSU4OSU4RCVFNyVBQiVBRi9uZXdHaXQvTG93Q29kZVdTL2xvd2NvZGVTdWJzZXZpY2VzL2Nlcmlvcy5jb3JlLmxvd2NvZGVzdWJzZXJ2aWNlcy9idWlsZC9jb25maWcvdGhlbWVDb25maWcudHNcIjtpbXBvcnQgeyBnZW5lcmF0ZSB9IGZyb20gJ0BhbnQtZGVzaWduL2NvbG9ycyc7XG5cbmV4cG9ydCBjb25zdCBwcmltYXJ5Q29sb3IgPSAnIzE4OTBmZic7XG5cbmV4cG9ydCBjb25zdCBkYXJrTW9kZSA9ICdsaWdodCc7XG5cbnR5cGUgRm4gPSAoLi4uYXJnOiBhbnkpID0+IGFueTtcblxudHlwZSBHZW5lcmF0ZVRoZW1lID0gJ2RlZmF1bHQnIHwgJ2RhcmsnO1xuXG5leHBvcnQgaW50ZXJmYWNlIEdlbmVyYXRlQ29sb3JzUGFyYW1zIHtcbiAgbWl4TGlnaHRlbjogRm47XG4gIG1peERhcmtlbjogRm47XG4gIHRpbnljb2xvcjogYW55O1xuICBjb2xvcj86IHN0cmluZztcbn1cblxuZXhwb3J0IGZ1bmN0aW9uIGdlbmVyYXRlQW50Q29sb3JzKGNvbG9yOiBzdHJpbmcsIHRoZW1lOiBHZW5lcmF0ZVRoZW1lID0gJ2RlZmF1bHQnKSB7XG4gIHJldHVybiBnZW5lcmF0ZShjb2xvciwge1xuICAgIHRoZW1lLFxuICB9KTtcbn1cblxuZXhwb3J0IGZ1bmN0aW9uIGdldFRoZW1lQ29sb3JzKGNvbG9yPzogc3RyaW5nKSB7XG4gIGNvbnN0IHRjID0gY29sb3IgfHwgcHJpbWFyeUNvbG9yO1xuICBjb25zdCBsaWdodENvbG9ycyA9IGdlbmVyYXRlQW50Q29sb3JzKHRjKTtcbiAgY29uc3QgcHJpbWFyeSA9IGxpZ2h0Q29sb3JzWzVdO1xuICBjb25zdCBtb2RlQ29sb3JzID0gZ2VuZXJhdGVBbnRDb2xvcnMocHJpbWFyeSwgJ2RhcmsnKTtcblxuICByZXR1cm4gWy4uLmxpZ2h0Q29sb3JzLCAuLi5tb2RlQ29sb3JzXTtcbn1cblxuZXhwb3J0IGZ1bmN0aW9uIGdlbmVyYXRlQ29sb3JzKHsgY29sb3IgPSBwcmltYXJ5Q29sb3IsIG1peExpZ2h0ZW4sIG1peERhcmtlbiwgdGlueWNvbG9yIH06IEdlbmVyYXRlQ29sb3JzUGFyYW1zKSB7XG4gIGNvbnN0IGFyciA9IG5ldyBBcnJheSgxOSkuZmlsbCgwKTtcbiAgY29uc3QgbGlnaHRlbnMgPSBhcnIubWFwKChfdCwgaSkgPT4ge1xuICAgIHJldHVybiBtaXhMaWdodGVuKGNvbG9yLCBpIC8gNSk7XG4gIH0pO1xuXG4gIGNvbnN0IGRhcmtlbnMgPSBhcnIubWFwKChfdCwgaSkgPT4ge1xuICAgIHJldHVybiBtaXhEYXJrZW4oY29sb3IsIGkgLyA1KTtcbiAgfSk7XG5cbiAgY29uc3QgYWxwaGFDb2xvcnMgPSBhcnIubWFwKChfdCwgaSkgPT4ge1xuICAgIHJldHVybiB0aW55Y29sb3IoY29sb3IpXG4gICAgICAuc2V0QWxwaGEoaSAvIDIwKVxuICAgICAgLnRvUmdiU3RyaW5nKCk7XG4gIH0pO1xuXG4gIGNvbnN0IHNob3J0QWxwaGFDb2xvcnMgPSBhbHBoYUNvbG9ycy5tYXAoaXRlbSA9PiBpdGVtLnJlcGxhY2UoL1xccy9nLCAnJykucmVwbGFjZSgvMFxcLi9nLCAnLicpKTtcblxuICBjb25zdCB0aW55Y29sb3JMaWdodGVucyA9IGFyclxuICAgIC5tYXAoKF90LCBpKSA9PiB7XG4gICAgICByZXR1cm4gdGlueWNvbG9yKGNvbG9yKVxuICAgICAgICAubGlnaHRlbihpICogNSlcbiAgICAgICAgLnRvSGV4U3RyaW5nKCk7XG4gICAgfSlcbiAgICAuZmlsdGVyKGl0ZW0gPT4gaXRlbSAhPT0gJyNmZmZmZmYnKTtcblxuICBjb25zdCB0aW55Y29sb3JEYXJrZW5zID0gYXJyXG4gICAgLm1hcCgoX3QsIGkpID0+IHtcbiAgICAgIHJldHVybiB0aW55Y29sb3IoY29sb3IpXG4gICAgICAgIC5kYXJrZW4oaSAqIDUpXG4gICAgICAgIC50b0hleFN0cmluZygpO1xuICAgIH0pXG4gICAgLmZpbHRlcihpdGVtID0+IGl0ZW0gIT09ICcjMDAwMDAwJyk7XG4gIHJldHVybiBbLi4ubGlnaHRlbnMsIC4uLmRhcmtlbnMsIC4uLmFscGhhQ29sb3JzLCAuLi5zaG9ydEFscGhhQ29sb3JzLCAuLi50aW55Y29sb3JEYXJrZW5zLCAuLi50aW55Y29sb3JMaWdodGVuc10uZmlsdGVyKGl0ZW0gPT4gIWl0ZW0uaW5jbHVkZXMoJy0nKSk7XG59XG4iLCAiY29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2Rpcm5hbWUgPSBcIkc6XFxcXERZWE1cXFxcXHU1RkFFXHU1MjREXHU3QUVGXFxcXG5ld0dpdFxcXFxMb3dDb2RlV1NcXFxcbG93Y29kZVN1YnNldmljZXNcXFxcY2VyaW9zLmNvcmUubG93Y29kZXN1YnNlcnZpY2VzXFxcXGJ1aWxkXFxcXGdlbmVyYXRlXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJHOlxcXFxEWVhNXFxcXFx1NUZBRVx1NTI0RFx1N0FFRlxcXFxuZXdHaXRcXFxcTG93Q29kZVdTXFxcXGxvd2NvZGVTdWJzZXZpY2VzXFxcXGNlcmlvcy5jb3JlLmxvd2NvZGVzdWJzZXJ2aWNlc1xcXFxidWlsZFxcXFxnZW5lcmF0ZVxcXFxnZW5lcmF0ZU1vZGlmeVZhcnMudHNcIjtjb25zdCBfX3ZpdGVfaW5qZWN0ZWRfb3JpZ2luYWxfaW1wb3J0X21ldGFfdXJsID0gXCJmaWxlOi8vL0c6L0RZWE0vJUU1JUJFJUFFJUU1JTg5JThEJUU3JUFCJUFGL25ld0dpdC9Mb3dDb2RlV1MvbG93Y29kZVN1YnNldmljZXMvY2VyaW9zLmNvcmUubG93Y29kZXN1YnNlcnZpY2VzL2J1aWxkL2dlbmVyYXRlL2dlbmVyYXRlTW9kaWZ5VmFycy50c1wiO2ltcG9ydCB7IHByaW1hcnlDb2xvciB9IGZyb20gJy4uL2NvbmZpZy90aGVtZUNvbmZpZyc7XHJcbmltcG9ydCB7IHJlc29sdmUgfSBmcm9tICdwYXRoJztcclxuaW1wb3J0IHsgZ2VuZXJhdGUgfSBmcm9tICdAYW50LWRlc2lnbi9jb2xvcnMnO1xyXG5pbXBvcnQgeyB0aGVtZSB9IGZyb20gJ2FudC1kZXNpZ24tdnVlL2xpYic7XHJcbmltcG9ydCBjb252ZXJ0TGVnYWN5VG9rZW4gZnJvbSAnYW50LWRlc2lnbi12dWUvbGliL3RoZW1lL2NvbnZlcnRMZWdhY3lUb2tlbic7XHJcbmNvbnN0IHsgZGVmYXVsdEFsZ29yaXRobSwgZGVmYXVsdFNlZWQgfSA9IHRoZW1lO1xyXG5cclxuZnVuY3Rpb24gZ2VuZXJhdGVBbnRDb2xvcnMoY29sb3I6IHN0cmluZywgdGhlbWU6ICdkZWZhdWx0JyB8ICdkYXJrJyA9ICdkZWZhdWx0Jykge1xyXG4gIHJldHVybiBnZW5lcmF0ZShjb2xvciwge1xyXG4gICAgdGhlbWUsXHJcbiAgfSk7XHJcbn1cclxuXHJcbi8qKlxyXG4gKiBsZXNzIGdsb2JhbCB2YXJpYWJsZVxyXG4gKi9cclxuZXhwb3J0IGZ1bmN0aW9uIGdlbmVyYXRlTW9kaWZ5VmFycygpIHtcclxuICBjb25zdCBwYWxldHRlcyA9IGdlbmVyYXRlQW50Q29sb3JzKHByaW1hcnlDb2xvcik7XHJcbiAgY29uc3QgcHJpbWFyeSA9IHBhbGV0dGVzWzVdO1xyXG5cclxuICBjb25zdCBwcmltYXJ5Q29sb3JPYmo6IFJlY29yZDxzdHJpbmcsIHN0cmluZz4gPSB7fTtcclxuXHJcbiAgZm9yIChsZXQgaW5kZXggPSAwOyBpbmRleCA8IDEwOyBpbmRleCsrKSB7XHJcbiAgICBwcmltYXJ5Q29sb3JPYmpbYHByaW1hcnktJHtpbmRleCArIDF9YF0gPSBwYWxldHRlc1tpbmRleF07XHJcbiAgfVxyXG5cclxuICBjb25zdCBtYXBUb2tlbiA9IGRlZmF1bHRBbGdvcml0aG0oZGVmYXVsdFNlZWQpO1xyXG4gIGNvbnN0IHYzVG9rZW4gPSBjb252ZXJ0TGVnYWN5VG9rZW4uZGVmYXVsdChtYXBUb2tlbik7XHJcbiAgcmV0dXJuIHtcclxuICAgIC4uLnYzVG9rZW4sXHJcbiAgICAvLyBVc2VkIGZvciBnbG9iYWwgaW1wb3J0IHRvIGF2b2lkIHRoZSBuZWVkIHRvIGltcG9ydCBlYWNoIHN0eWxlIGZpbGUgc2VwYXJhdGVseVxyXG4gICAgLy8gcmVmZXJlbmNlOiAgQXZvaWQgcmVwZWF0ZWQgcmVmZXJlbmNlc1xyXG4gICAgaGFjazogYHRydWU7IEBpbXBvcnQgKHJlZmVyZW5jZSkgXCIke3Jlc29sdmUoJ3NyYy9kZXNpZ24vY29uZmlnLmxlc3MnKX1cIjtgLFxyXG4gICAgJ3ByaW1hcnktY29sb3InOiBwcmltYXJ5LFxyXG4gICAgLi4ucHJpbWFyeUNvbG9yT2JqLFxyXG4gICAgJ2luZm8tY29sb3InOiBwcmltYXJ5LFxyXG4gICAgJ3Byb2Nlc3NpbmctY29sb3InOiBwcmltYXJ5LFxyXG4gICAgJ3N1Y2Nlc3MtY29sb3InOiAnIzU1RDE4NycsIC8vICBTdWNjZXNzIGNvbG9yXHJcbiAgICAnZXJyb3ItY29sb3InOiAnI0VENkY2RicsIC8vICBGYWxzZSBjb2xvclxyXG4gICAgJ3dhcm5pbmctY29sb3InOiAnI0VGQkQ0NycsIC8vICAgV2FybmluZyBjb2xvclxyXG4gICAgJ2J0bi1pbmZvLWNvbG9yJzogJyM5MDkzOTknLFxyXG4gICAgJ3RleHQtY29sb3Itc2Vjb25kYXJ5JzogJ3JnYmEoMCwgMCwgMCwgMC40NSknLFxyXG4gICAgJ2JvcmRlci1jb2xvci1iYXNlMSc6ICcjZjBmMGYwJyxcclxuICAgICdmb250LXNpemUtYmFzZSc6ICcxNHB4JywgLy8gIE1haW4gZm9udCBzaXplXHJcbiAgICAnYm9yZGVyLXJhZGl1cy1iYXNlJzogJzJweCcsIC8vICBDb21wb25lbnQvZmxvYXQgZmlsbGV0XHJcbiAgICAnbGluay1jb2xvcic6IHByaW1hcnksIC8vICAgTGluayBjb2xvclxyXG4gICAgJ2FwcC1iYXNlLWJhY2tncm91bmQnOiAnI2VhZWNmMCcsXHJcbiAgICAnYXBwLWNvbnRlbnQtYmFja2dyb3VuZCc6ICcjRjFGNEY4JywgLy8gICBMaW5rIGNvbG9yXHJcbiAgICAnYXBwLW1haW4tYmFja2dyb3VuZCc6ICcjZWJlZWY1JyxcclxuICAgICdzZWxlY3RlZC1ob3Zlci1iZyc6ICcjZjVmNWY1JyxcclxuICAgICdob3Zlci1iYWNrZ3JvdW5kJzogJyNmNWY3ZmEnLFxyXG4gIH07XHJcbn1cclxuIl0sCiAgIm1hcHBpbmdzIjogIjtBQUErYixTQUFTLG9CQUFvQjtBQUM1ZCxTQUFTLDRCQUE0QjtBQUNyQyxPQUFPLFVBQVU7QUFDakIsT0FBTyxTQUFTO0FBQ2hCLE9BQU8sYUFBYTs7O0FDSnNkLFNBQVMsZ0JBQWdCO0FBRTVmLElBQU0sZUFBZTs7O0FDRDVCLFNBQVMsZUFBZTtBQUN4QixTQUFTLFlBQUFBLGlCQUFnQjtBQUN6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
