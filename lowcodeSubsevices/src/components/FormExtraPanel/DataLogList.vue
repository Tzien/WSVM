<template>
  <div class="form-extra-log">
    <ScrollContainer v-loading="loading" ref="infiniteBody" class="form-extra-comment-main">
      <Timeline class="form-extra-log-list" v-if="list.length">
        <TimelineItem v-for="item in list">
          <template #dot>
            <span class="tag" :style="{ background: getTimeLineTagColor(item.type) }"></span>
          </template>
          <span>{{ item.creatorTime }}</span>
          <div class="time-item-container">
            <div class="time-item-head">
              <a-avatar :size="24" :src="apiUrl + item.headIcon" class="head-avatar" />
              <span class="head-name">{{ item.creatorUserName }}</span>
              <a-tag :color="getNodeStatusColor(item.type)" :bordered="false" class="head-status">{{ getNodeStatusContent(item.type) }}</a-tag>
            </div>
            <div class="time-item-log" v-if="item.type">
              <div class="time-item-log-total">
                有
                <span>{{ item.dataLog.length }}</span>
                处更改
              </div>
              <div class="time-item-log-item" v-for="log in item.dataLog">
                <div class="time-item-log-item-label">{{ log.fieldName }}：</div>
                <div class="time-item-log-item-value">
                  <template v-if="log.nameModified">
                    <span class="time-item-log-item-value-modify">已修改</span>
                  </template>
                  <template v-else>
                    <template v-if="log.ceriKey === 'table'">
                      <span class="time-item-log-item-value-more" @click="toDetail(log)">修改详情</span>
                    </template>
                    <template v-else-if="log.ceriKey === 'sign'">
                      <span class="time-item-log-item-value-more line-through" v-if="log.oldData" @click="handlePreview(log.oldData)">旧签名</span>
                      <template v-if="log.newData">
                        修改为
                        <span class="time-item-log-item-value-more" @click="handlePreview(log.newData)">新签名</span>
                      </template>
                    </template>
                    <template v-else-if="log.ceriKey === 'signature'">
                      <span class="time-item-log-item-value-more line-through" v-if="log.oldData" @click="handlePreview(apiUrl + log.oldData)">旧签章</span>
                      <template v-if="log.newData">
                        修改为
                        <span class="time-item-log-item-value-more" @click="handlePreview(apiUrl + log.newData)">新签章</span>
                      </template>
                    </template>
                    <template v-else>
                      <span class="time-item-log-item-value-old" v-if="log.oldData">{{ log.oldData }}</span>
                      <template v-if="log.newData">
                        修改为
                        <span class="time-item-log-item-value-new">{{ log.newData }}</span>
                      </template>
                    </template>
                  </template>
                </div>
              </div>
            </div>
          </div>
        </TimelineItem>
      </Timeline>
      <ceri-empty v-if="!list.length" />
    </ScrollContainer>
    <ChildLogModal @register="registerChildLogModal" />
  </div>
</template>
<script lang="ts" setup>
  // import { getDataLogList } from '@/api/onlineDev/visualDev';
  import { ref, reactive, toRefs, onMounted, nextTick } from 'vue';
  import { ScrollContainer, ScrollActionType } from '@/components/Container';
  import { useGlobSetting } from '@/hooks/setting';
  import { createImgPreview } from '@/components/Preview/index';
  import { Timeline, TimelineItem } from 'ant-design-vue';
  import ChildLogModal from './modal/ChildLogModal.vue';
  import { useModal } from '@/components/Modal';

  interface State {
    list: any[];
    loading: boolean;
    finish: boolean;
    listQuery: any;
  }

  const props = defineProps({
    modelId: { type: String, default: '' },
    formDataId: { type: [String, Number], default: '' },
  });

  const globSetting = useGlobSetting();
  const apiUrl = ref(globSetting.apiUrl);
  const infiniteBody = ref<Nullable<ScrollActionType>>(null);
  const state = reactive<State>({
    list: [],
    loading: false,
    finish: false,
    listQuery: {
      currentPage: 1,
      pageSize: 20,
      sort: 'desc',
      sidx: '',
    },
  });
  const { list, loading } = toRefs(state);
  const [registerChildLogModal, { openModal: openChildLogModal }] = useModal();

  function bindScroll() {
    const bodyRef = infiniteBody.value;
    const vBody = bodyRef?.getScrollWrap();
    vBody?.addEventListener('scroll', () => {
      if (vBody.scrollHeight - vBody.clientHeight - vBody.scrollTop <= 200 && !state.loading && !state.finish) {
        state.listQuery.currentPage += 1;
        init();
      }
    });
  }
  function getNodeStatusColor(type) {
    return type == 1 ? 'blue' : 'success';
  }
  function getTimeLineTagColor(type) {
    return type == 1 ? '#0177FF' : '#08AF28';
  }
  function getNodeStatusContent(type) {
    return type == 1 ? '编辑' : '新建';
  }
  function handlePreview(image) {
    createImgPreview({ imageList: [image] });
  }
  function toDetail(data) {
    openChildLogModal(true, data);
  }
  function init() {
    if (!props.modelId || !props.formDataId) return;
    state.loading = true;
    const query = { ...state.listQuery, modelId: props.modelId, dataId: props.formDataId };
    // getDataLogList(query)
    //   .then(res => {
    //     if (res.data.list.length < state.listQuery.pageSize) state.finish = true;
    //     const list = res.data.list.map(o => ({ ...o, dataLog: o.dataLog ? JSON.parse(o.dataLog) : [] }));
    //     state.list = [...state.list, ...list];
    //     state.loading = false;
    //   })
    //   .catch(() => {
    //     state.loading = false;
    //   });
  }

  onMounted(() => {
    state.list = [];
    state.finish = false;
    init();
    nextTick(() => {
      bindScroll();
    });
  });
</script>
