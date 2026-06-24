<template>
  <div class="upload-img-container" :class="$attrs.class">
    <template v-if="!detailed">
      <a-upload v-model:file-list="fileList" v-bind="getBindValues" v-if="showType === 'button'">
        <a-button preIcon="icon-ym icon-ym-btn-upload" :disabled="disabled">{{ buttonText }}</a-button>
      </a-upload>
      <div class="upload-img__tip mb-10px" v-if="tipText && showType === 'button'">{{ tipText }}</div>
      <a-upload-dragger v-model:fileList="fileList" v-bind="getBindValues" v-if="showType === 'dragger'">
        <p class="ant-upload-drag-icon icon-ym icon-ym-btn-upload"></p>
        <p class="ant-upload-text">{{ t('component.upload.imgDragger') }}</p>
        <div class="upload-img__tip" v-if="tipText">{{ tipText }}</div>
      </a-upload-dragger>
    </template>
    <transition-group class="ant-upload-list ant-upload-list-picture-card" tag="div" name="list" v-if="!simple">
      <div class="ant-upload-list-picture-card-container" v-for="(file, index) in imgList" :key="file.fileId">
        <div class="ant-upload-list-item ant-upload-list-item-done ant-upload-list-item-list-type-picture-card">
          <div class="ant-upload-list-item-info">
            <a class="ant-upload-list-item-thumbnail">
              <img :src="getImageSrc(file, true)" class="ant-upload-list-item-image" />
            </a>
          </div>
          <span class="ant-upload-list-item-actions">
            <EyeOutlined @click="handlePreview(index)" />
            <DeleteOutlined @click="handleRemove(index)" v-if="!disabled && !detailed" />
          </span>
        </div>
      </div>
    </transition-group>
    <p class="link-text" @click="handlePreview(0)" v-if="simple">{{ t('component.upload.viewImage') }}</p>
    <template v-if="!detailed && showType !== 'button' && showType !== 'dragger'">
      <a-upload v-model:file-list="fileList" list-type="picture-card" v-bind="getBindValues">
        <div>
          <plus-outlined></plus-outlined>
          <div class="ant-upload-text" v-if="buttonText">{{ buttonText }}</div>
        </div>
      </a-upload>
      <div class="upload-img__tip" v-if="tipText">{{ tipText }}</div>
    </template>
  </div>
</template>

<script lang="ts" setup>
  import { Form, Upload as AUpload, UploadDragger as AUploadDragger } from 'ant-design-vue';
  import { PlusOutlined, EyeOutlined, DeleteOutlined } from '@ant-design/icons-vue';
  import type { UploadChangeParam, UploadFile } from 'ant-design-vue';
  import { computed, ref, unref, watch } from 'vue';
  import { uploadImgProps, imgItem, units } from './props';
  import { useGlobSetting } from '@/hooks/setting';
  // import { getToken } from '@/utils/auth';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useI18n } from 'vue-i18n';
  import { createImgPreview } from '@/components/Preview/index';

  defineOptions({ name: 'CeriUploadImg', inheritAttrs: false });
  const base64WithPrefixRegex = /^data:([a-zA-Z0-9]+\/[a-zA-Z0-9-.+]+);base64,([A-Za-z0-9+/]{4})*([A-Za-z0-9+/]{4}|[A-Za-z0-9+/]{3}=|[A-Za-z0-9+/]{2}==)$/;
  const absoluteUrlRegex = /^(https?:)?\/\//i;
  const props = defineProps(uploadImgProps);
  const emit = defineEmits(['update:value', 'change']);
  const { createMessage } = useMessage();
  const { t } = useI18n();
  const globSetting = useGlobSetting();
  const fileList = ref<UploadFile[]>([]);
  const imgList = ref<imgItem[]>([]);
  const loading = ref<boolean>(false);
  const apiUrl = ref(globSetting.apiUrl);
  const formItemContext = Form.useInjectFormItemContext();

  const getAction = computed(() => globSetting.uploadUrl + '/' + props.type);
  // const getHeaders = computed(() => ({ Authorization: getToken() as string }));
  const getUploadData = computed(() => ({
    pathType: props.pathType,
    sortRule: (props.sortRule || []).join(),
    timeFormat: props.timeFormat,
    folder: props.folder,
  }));
  const getBindValues = computed(() => {
    return {
      class: 'img-uploader-' + (props.showType || 'card'),
      accept: props.accept,
      disabled: props.disabled || props.detailed,
      showUploadList: false,
      multiple: props.limit != 1,
      maxCount: props.limit === 0 ? undefined : props.limit,
      action: unref(getAction),
      // headers: unref(getHeaders),
      data: unref(getUploadData),
      beforeUpload,
      onChange: handleChange,
    };
  });

  watch(
    () => props.value,
    val => {
      imgList.value = normalizeImgList(val);
    },
    { deep: true, immediate: true },
  );

  function normalizeImgList(val) {
    if (!val) return [];
    if (typeof val === 'string') {
      try {
        const parsed = JSON.parse(val);
        return normalizeImgList(parsed);
      } catch {
        return val ? [{ name: '', fileId: val, url: val, thumbUrl: '' }] : [];
      }
    }
    const list = Array.isArray(val) ? val : [val];
    return list.filter(o => o && (o.url || o.thumbUrl));
  }

  function toImageUrl(url?: string) {
    if (!url) return '';
    if (base64WithPrefixRegex.test(url) || absoluteUrlRegex.test(url)) return url;
    return apiUrl.value + url;
  }

  function getImageSrc(file: imgItem, useThumb = false) {
    const url = useThumb ? file.thumbUrl || file.url : file.url || file.thumbUrl;
    return toImageUrl(url);
  }

  function beforeUpload(file) {
    const isTopLimit = props.limit ? imgList.value.length >= props.limit : false;
    if (isTopLimit) {
      createMessage.error(t('component.upload.imageMaxNumber', [props.limit]));
      return AUpload.LIST_IGNORE;
    }
    const isAccept = new RegExp('image/*').test(file.type);
    if (!isAccept) {
      createMessage.error(t('component.upload.uploadImg'));
      return AUpload.LIST_IGNORE;
    }
    if (!props.fileSize) return true;
    const unitNum = units[props.sizeUnit];
    const isRightSize = file.size / unitNum < props.fileSize;
    if (!isRightSize) {
      createMessage.error(t('component.upload.imageMaxSize', { size: props.fileSize, unit: props.sizeUnit }));
      return AUpload.LIST_IGNORE;
    }
    return true;
  }

  function handleChange({ file }: UploadChangeParam) {
    if (file.status === 'uploading') return (loading.value = true);
    if (file.status === 'error') {
      loading.value = false;
      fileList.value = fileList.value.filter(o => o.uid != file.uid);
      createMessage.error(t('component.upload.uploadError'));
      return;
    }
    if (file.status === 'done') {
      loading.value = false;
      if (file.response.code === 200) {
        const isTopLimit = props.limit ? imgList.value.length >= props.limit : false;
        if (isTopLimit) {
          fileList.value = fileList.value.filter(o => o.uid != file.uid);
          createMessage.error(t('component.upload.imageMaxNumber', [props.limit]));
          return;
        }
        imgList.value.push({
          name: file.name,
          fileId: file.response.data.name,
          url: file.response.data.url,
          thumbUrl: file.response.data.thumbUrl,
        });
        emit('update:value', unref(imgList));
        emit('change', unref(imgList));
        formItemContext.onFieldChange();
      } else {
        fileList.value = fileList.value.filter(o => o.uid != file.uid);
        createMessage.error(file.response.msg);
      }
    }
  }
  function handlePreview(index: number) {
    const imageList = imgList.value.map(o => getImageSrc(o)).filter(Boolean);
    if (!imageList.length) return;
    createImgPreview({ imageList, index: Math.min(index, imageList.length - 1) });
  }
  function handleRemove(index: number) {
    fileList.value.splice(index, 1);
    imgList.value.splice(index, 1);
    emit('update:value', unref(imgList));
    emit('change', unref(imgList));
    formItemContext.onFieldChange();
  }
</script>
<style lang="less" scoped>
  .upload-img-container {
    .ant-upload-list {
      display: inline;
      .ant-upload-list-picture-card-container {
        display: inline-block;
        width: 100px;
        height: 100px;
        margin: 0 8px 8px 0;
        vertical-align: top;
        .ant-upload-list-item {
          border-radius: var(--border-radius);
          position: relative;
          height: 100%;
          padding: 0;
          overflow: hidden;
          &:hover {
            .ant-upload-list-item-info:before {
              opacity: 1;
            }
          }
          .ant-upload-list-item-info {
            position: relative;
            height: 100%;
            overflow: hidden;
            &::before {
              position: absolute;
              z-index: 1;
              width: 100%;
              height: 100%;
              background-color: rgba(0, 0, 0, 0.45);
              opacity: 0;
              transition: all 0.3s;
              content: ' ';
            }
            .ant-upload-list-item-thumbnail {
              height: 100%;
              .ant-upload-list-item-image {
                height: 100%;
                width: 100%;
                object-fit: unset;
              }
            }
          }
          .ant-upload-list-item-actions {
            position: absolute;
            top: 50%;
            left: 50%;
            z-index: 10;
            white-space: nowrap;
            transform: translate(-50%, -50%);
            opacity: 0;
            transition: all 0.3s;
            &:hover {
              opacity: 1;
            }
            .anticon-eye,
            .anticon-delete {
              color: rgba(255, 255, 255, 0.85);
              font-size: 20px;
              margin: 0 10px;
            }
          }
        }
      }
    }
    .img-uploader-card {
      width: 100px;
      :deep(.ant-upload) {
        width: 100px;
        height: 100px;
        .anticon {
          font-size: 26px;
          color: #8c939d;
        }
        .ant-upload-text {
          margin-top: 10px;
          color: #8c939d;
        }
      }
    }
    .img-uploader-button {
      display: block;
      margin-bottom: 10px;
    }
    .img-uploader-dragger {
      display: block;
      width: 300px;
      max-width: 100%;
      margin-bottom: 10px;
      .ant-upload-drag-icon {
        color: #8c939d;
        font-size: 28px;
        margin-bottom: 0;
      }
      .ant-upload-text {
        color: #8c939d;
        font-size: 12px;
      }
    }
    .list-enter-active,
    .list-leave-active {
      transition: all 1s ease;
    }
    .list-enter-from,
    .list-leave-to {
      opacity: 0;
      transform: translateY(-30px);
    }
    .upload-img__tip {
      line-height: 1.2;
      color: @text-color-secondary;
      font-size: 12px;
      word-break: break-all;
    }
  }
</style>
<style lang="less">
  .ant-table {
    .upload-img-container {
      .ant-upload-list {
        white-space: pre-wrap;
      }
    }
  }
</style>
