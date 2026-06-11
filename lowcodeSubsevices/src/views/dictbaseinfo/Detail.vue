<template>
  <BasicModal v-bind="$attrs" @register="registerModal" :title="title" width="600px" :minHeight="100" :showOkBtn="false">
    <template #insertFooter>
    </template>
    <a-row class="dynamic-form ">
      <a-form :colon="false" layout="horizontal" labelAlign="right"  :labelCol="{ style: { width: '100px' } }" :model="dataForm" ref="formRef">
        <a-row :gutter="15">
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="DictType" :labelCol="{ style: { width: '100px' } }">
              <template #label>代码管理号</template>
              <CeriInput v-model:value="dataForm.DictType" :maskConfig='{"filler":"*","maskType":1,"prefixType":1,"prefixLimit":0,"prefixSpecifyChar":"","suffixType":1,"suffixLimit":0,"suffixSpecifyChar":"","ignoreChar":"","useUnrealMask":false,"unrealMaskLength":1}' disabled detailed />
            </a-form-item>
          </a-col>
          <a-col :span="12" class="ant-col-item" >
            <a-form-item name="DictBaseInfoId" :labelCol="{ style: { width: '100px' } }">
              <template #label>钢种代码</template>
              <CeriInput v-model:value="dataForm.DictBaseInfoId" :maskConfig='{"filler":"*","maskType":1,"prefixType":1,"prefixLimit":0,"prefixSpecifyChar":"","suffixType":1,"suffixLimit":0,"suffixSpecifyChar":"","ignoreChar":"","useUnrealMask":false,"unrealMaskLength":1}' disabled detailed />
            </a-form-item>
          </a-col>
          <a-col :span="12" class="ant-col-item" >
            <a-form-item name="FieldDecimal" :labelCol="{ style: { width: '100px' } }">
              <template #label>喷号码</template>
              <CeriInput v-model:value="dataForm.FieldDecimal" :maskConfig='{"filler":"*","maskType":1,"prefixType":1,"prefixLimit":0,"prefixSpecifyChar":"","suffixType":1,"suffixLimit":0,"suffixSpecifyChar":"","ignoreChar":"","useUnrealMask":false,"unrealMaskLength":1}' disabled detailed />
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="BelongSystem" :labelCol="{ style: { width: '100px' } }">
              <template #label>钢种分类</template>
              <p>{{ belongSystemLabel }}</p>
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="AllowEdit" :labelCol="{ style: { width: '100px' } }">
              <template #label>是否应用</template>
              <p>{{ allowEditLabel }}</p>
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="CreateTime" :labelCol="{ style: { width: '100px' } }">
              <template #label>插入时间</template>
              <p>{{ dataForm.CreateTime }}</p>
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="CreateName" :labelCol="{ style: { width: '100px' } }">
              <template #label>插入人员</template>
              <p>{{ dataForm.CreateName }}</p>
            </a-form-item>
          </a-col>
          <a-col :span="24" class="ant-col-item" >
            <a-form-item name="Remark" :labelCol="{ style: { width: '100px' } }">
              <template #label>备注</template>
              <p>{{ dataForm.Remark }}</p>
            </a-form-item>
          </a-col>
        </a-row>
      </a-form>
    </a-row>    
  </BasicModal>
</template>
<script lang="ts" setup>
  import { getDetail } from './helper/api';
  import { getConfigData } from '@/api/onlineDev/visualDev';
  import { reactive, toRefs, nextTick, ref, computed, unref, toRaw} from 'vue';
  import { thousandsFormat } from '@/utils/ceri';
  import dayjs from 'dayjs';
  import { CaretRightOutlined } from '@ant-design/icons-vue';
  import { buildUUID } from '@/utils/uuid';
  import { useI18n } from 'vue-i18n'
  import { getDataChange } from '@/api/onlineDev/visualDev';
  import ExtraRelationInfo from '@/components/CeriOS/RelationForm/src/ExtraRelationInfo.vue';
  import { BasicModal, useModal } from '@/components/Modal';

  interface State {
    dataForm: any;
    interfaceRes: any;
	extraOptions: any;
    extraData: any;
    title: string;
  }

  defineOptions({ name: 'Detail' });
  const [registerModal, { openModal, closeModal, setModalProps }] = useModal();
  const { t } = useI18n();

  const relationDetailRef = ref<any>(null);
  const state = reactive<State>({
    dataForm: {},
	interfaceRes:{
	},
	extraOptions:{
	},
	extraData:{
	},
    title: '详情',
  });
  const { title, dataForm } = toRefs(state);

  const belongSystemLabel = computed(() => {
    const v = (dataForm.value as any)?.BelongSystem;
    const map: Record<string, string> = {
      '1': '低合金高强度结构钢',
      '2': '优质碳素结构钢',
      '3': '其他钢种',
    };
    return v == null ? '' : map[String(v)] ?? String(v);
  });
  const allowEditLabel = computed(() => {
    const v = (dataForm.value as any)?.AllowEdit;
    const map: Record<string, string> = {
      '1': '是',
      '2': '否',
    };
    return v == null ? '' : map[String(v)] ?? String(v);
  });

  defineExpose({ init });

  function init(data) {
    state.dataForm.id = data.id;
    openModal();
    nextTick(() => {
      setTimeout(initData, 0);
    });
  }
  function initData() {
    changeLoading(true);
    if (state.dataForm.id) {
      getData(state.dataForm.id);
    } else {
      closeModal();
    }
  }
  function getData(id) {
    getDetail(id).then((res) => {
      const raw = res?.data || {};
      const mapped: any = { id: raw.id ?? raw.Id ?? id };
      Object.keys(raw || {}).forEach((k) => {
        if (!k) return;
        if (k === 'id') return;
        const pascalKey = k.replace(/^[a-z]/, (s) => s.toUpperCase());
        mapped[pascalKey] = raw[k];
      });
      if (mapped.CreateTime) {
        mapped.CreateTime = dayjs(mapped.CreateTime).format('YYYY-MM-DD HH:mm:ss');
      }
      Object.assign(state.dataForm, mapped);

      nextTick(() => {
        changeLoading(false);
      });
    });
  }
  function setFormProps(data) {
    setModalProps(data);
  }
  function changeLoading(loading) {
    setFormProps({ loading });
  }
function getParamList(key) {
    let templateJson: any[] = state.interfaceRes[key];
    if (!templateJson || !templateJson.length || !state.dataForm) return templateJson;
    for (let i = 0; i < templateJson.length; i++) {
        if (templateJson[i].relationField && templateJson[i].sourceType == 1) {
            templateJson[i].defaultValue = state.dataForm[templateJson[i].relationField + '_id'] || '';
        }
    }
    return templateJson;
}

</script>
