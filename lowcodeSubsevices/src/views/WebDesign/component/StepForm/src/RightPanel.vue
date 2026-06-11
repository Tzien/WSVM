<template>
  <div class="right-board common-board">
    <a-tabs v-model:activeKey="activeKey" :tabBarGutter="14" class="average-tabs">
      <a-tab-pane key="field" tab="组件属性"></a-tab-pane>
      <a-tab-pane key="style" tab="组件样式"></a-tab-pane>
      <a-tab-pane key="form" tab="表单属性" v-if="!formInfo.isKit"></a-tab-pane>
    </a-tabs>
    <div class="field-box" v-if="activeData?.__config__ && activeData.__config__.ceriKey">
      <ScrollContainer>
        <a-form :colon="false" layout="vertical" v-show="activeKey === 'field'" class="right-board-form">
          <a-form-item label="控件类型" v-if="!['tableGridTr', 'tableGridTd'].includes(activeData.__config__.ceriKey)">
            <a-input v-model:value="getCompName" disabled />
          </a-form-item>
          <a-form-item
            label="控件用途"
            v-if="ceriKey == 'calculate' || activeData.__config__.ceriKey == 'popupAttr' || activeData.__config__.ceriKey == 'relationFormAttr'">
            <ceri-select v-model:value="activeData.isStorage" :options="storageTypeOptions" @change="onStorageChange" />
          </a-form-item>
          <div v-if="(activeData.__vModel__ !== undefined && !noVModelList.includes(ceriKey)) || activeData.isStorage">
            <div v-if="getHasTable">
              <div v-if="!config.isSubTable">
                <a-form-item label="数据库表" v-if="ceriKey !== 'table'">
                  <ceri-select
                    v-model:value="activeData.__config__.tableName"
                    placeholder="请选择"
                    showSearch
                    :dropdownMatchSelectWidth="false"
                    :options="getAllTable.map(o => ({
                      ...o,
                      // StepBasic/StepDesign 侧传入的 formDbs 只有 tableName，无 table 字段，这里直接用 tableName 作为展示与取值依据
                      fullName: o.tableName,
                    }))"
                    :fieldNames="{ value: 'tableName' }"
                    @change="onTableChange">
                    <template #option="{ fullName, typeId }">
                      <span class="custom-option-left">{{ fullName }}</span>
                      <span class="custom-option-right">{{ typeId == '1' ? '主表' : '从表' }}</span>
                    </template>
                  </ceri-select>
                </a-form-item>
                <a-form-item>
                  <template #label>控件字段<BasicHelp :text="fieldText" /></template>
                  <ceri-input v-model:value="activeData.__vModel__" disabled v-if="ceriKey === 'table'" />
                  <ceri-select
                    v-model:value="activeData.__vModel__"
                    ref="fieldSelectRef"
                    placeholder="请输入数据库字段"
                    :options="fieldOptions"
                    :dropdownMatchSelectWidth="false"
                    showSearch
                    allowClear
                    :fieldNames="{ value: 'realField' }"
                    @dropdownVisibleChange="onFieldDropdownVisibleChange"
                    @change="onFieldChange($event, config.formId)"
                    v-else>
                    <template #dropdownRender="{ menuNode: menu }">
                      <v-nodes :vnodes="menu" />
                      <a-divider style="margin: 4px 0" />
                      <div class="select-item-add" @mousedown="e => e.preventDefault()" @click="handleAddField">
                        <a-button type="link" preIcon="icon-ym icon-ym-btn-add">添加字段</a-button>
                      </div>
                    </template>
                  </ceri-select>
                </a-form-item>
              </div>
              <div v-else>
                <a-form-item label="数据库表">
                  <ceri-select
                    v-model:value="activeData.__config__.relationTable"
                    placeholder="请选择"
                    :options="getAllTable.map(o => ({
                      ...o,
                      fullName: o.tableName,
                    }))"
                    :fieldNames="{ value: 'tableName' }"
                    showSearch
                    disabled>
                  </ceri-select>
                </a-form-item>
                <a-form-item>
                  <template #label>控件字段<BasicHelp :text="fieldText" /></template>
                  <ceri-select
                    v-model:value="activeData.__vModel__"
                    ref="childFieldSelectRef"
                    placeholder="请输入数据库字段"
                    :options="getSubTableField(config.relationTable)"
                    :dropdownMatchSelectWidth="false"
                    showSearch
                    allowClear
                    :fieldNames="{ value: 'field' }"
                    @dropdownVisibleChange="onFieldDropdownVisibleChange"
                    @change="onChildTableFieldChange($event, config.formId)">
                    <template #dropdownRender="{ menuNode: menu }">
                      <v-nodes :vnodes="menu" />
                      <a-divider style="margin: 4px 0" />
                      <div class="select-item-add" @mousedown="e => e.preventDefault()" @click="handleAddField">
                        <a-button type="link" preIcon="icon-ym icon-ym-btn-add">添加字段</a-button>
                      </div>
                    </template>
                  </ceri-select>
                </a-form-item>
              </div>
            </div>
            <div v-else>
              <a-form-item>
                <template #label>控件字段<BasicHelp :text="fieldText" /></template>
                <ceri-input
                  v-model:value="activeData.__vModel__"
                  placeholder="请输入数据库字段"
                  :disabled="ceriKey === 'table' || !!formInfo.isKit"
                  @change="onInputFieldChange($event, config.formId, config.parentVModel)" />
              </a-form-item>
            </div>
          </div>
          <a-form-item label="控件标题" v-if="!layoutList.includes(ceriKey) || ceriKey === 'table'">
            <a-input v-model:value="activeData.__config__.label"  placeholder="请输入" />
          </a-form-item>
          <a-form-item label="显示标题" v-if="!layoutList.includes(ceriKey)">
            <a-switch v-model:checked="activeData.__config__.showLabel" />
          </a-form-item>
          <a-form-item label="显示标题" v-if="ceriKey === 'table'">
            <a-switch v-model:checked="activeData.__config__.showTitle" />
          </a-form-item>
          <a-form-item label="卡片标题" v-if="ceriKey === 'card'">
            <ceri-i18n-input v-model:value="activeData.header" v-model:i18n="activeData.headerI18nCode" placeholder="请输入" />
          </a-form-item>
          <a-form-item label="标题提示" v-if="hasTipLabel(ceriKey)">
            <a-input v-model:value="activeData.__config__.tipLabel"  placeholder="请输入" />
          </a-form-item>
          <a-form-item label="占位提示" v-if="activeData.placeholder !== undefined && !systemList.includes(ceriKey)">
            <ceri-i18n-input v-model:value="activeData.placeholder" v-model:i18n="activeData.placeholderI18nCode" placeholder="请输入" />
          </a-form-item>
          <div v-if="ceriKey === 'table'">
            <a-form-item label="关联子表" v-if="getHasTable">
              <ceri-select
                v-model:value="activeData.__config__.tableName"
                placeholder="请选择"
                showSearch
                :dropdownMatchSelectWidth="false"
                :options="getSubTable.map(o => ({
                  ...o,
                  fullName: o.tableName,
                }))"
                :fieldNames="{ value: 'tableName' }"
                @change="onChildTableNameChange($event, config.formId)">
                <template #option="{ fullName, typeId }">
                  <span class="custom-option-left"> {{ fullName }}</span>
                  <span class="custom-option-right">{{ typeId == '1' ? '主表' : '从表' }}</span>
                </template>
              </ceri-select>
            </a-form-item>
          </div>
          <component :is="getRightComp" v-bind="getRightCompBind" :key="activeData.__config__.renderKey" />
          <a-form-item label="显示内容" v-if="ceriKey === 'currOrganize'">
            <a-select v-model:value="activeData.showLevel" placeholder="请选择">
              <a-select-option value="all">显示组织</a-select-option>
              <a-select-option value="last">显示部门</a-select-option>
            </a-select>
          </a-form-item>
          <div v-if="![...layoutList, ...noVModelList].includes(ceriKey) && ceriKey !== 'input'">
            <a-form-item label="是否禁用" v-if="!systemList.includes(ceriKey)">
              <a-switch v-model:checked="activeData.disabled" />
            </a-form-item>
            <a-form-item label="是否必填" v-if="!systemList.includes(ceriKey)">
              <a-switch v-model:checked="activeData.__config__.required" />
            </a-form-item>
            <a-form-item label="是否隐藏">
              <a-switch v-model:checked="activeData.__config__.noShow" />
            </a-form-item>
          </div>
          <a-form-item label="是否隐藏" v-if="['relationFormAttr', 'popupAttr', 'calculate'].includes(ceriKey) && activeData.isStorage">
            <a-switch v-model:checked="activeData.__config__.noShow" />
          </a-form-item>
          <div v-if="activeData.on">
            <a-divider>脚本事件<BasicHelp text="不支持代码生成" /></a-divider>
            <a-form-item :label="key" v-for="(_value, key) in activeData.on" :key="key">
              <a-button block @click="editFunc(key)">{{ getFuncText(key) }}</a-button>
            </a-form-item>
          </div>
        </a-form>
        <StylePane v-bind="getBindValue" v-show="activeKey === 'style'" />
        <FormAttrPane v-bind="getBindValue" :printTplOptions="printTplOptions" v-show="activeKey === 'form'" />
      </ScrollContainer>
    </div>
    <FieldModal @register="registerFieldModal" @updateOptions="updateFieldOptions" />
    <FormScript @register="registerScriptModal" :treeTitle="formInfo.fullName" :drawingList="drawingList" @confirm="updateScript" />
  </div>
</template>
<script lang="ts" setup>
  import { getDataModelFieldList } from '@/api/systemData/dataModel';
  // import { getDictionaryTypeSelector } from '@/api/systemData/dictionary';
  // import { getPrintDevSelector } from '@/api/system/printDev';
  import { reactive, toRefs, computed, ref, unref, watch, onMounted, nextTick,watchEffect } from 'vue';
  import { ScrollContainer } from '@/components/Container';
  import FieldModal from './components/FieldModal.vue';
  import FormScript from './components/FormScript.vue';
  import StylePane from './components/StylePane.vue';
  import FormAttrPane from './components/FormAttrPane.vue';
  import * as RightComp from './rightComponents';
  import { useModal } from '@/components/Modal';
  import { useGeneratorStore } from '@/store/generator';
  import { useMessage } from '@/hooks/web/useMessage';
  import { useAttrs } from '@/hooks/core/useAttrs';
  import type { ItemCfg } from './types/genItem';
  import { inputComponents, selectComponents, systemComponents, layoutComponents } from './helper/componentMap';
  import { layoutList, systemList, orgList } from './helper/rightPanel';
  import { noVModelList } from './helper/config';
  import { upperFirst } from 'lodash-es';
import { useCommonStore } from '@/store/index.js'
import { qiankunWindow } from 'vite-plugin-qiankun/dist/helper.js'
import { useGlobalState } from '@/shared/useGlobalState'

  var isQiankun = qiankunWindow.__POWERED_BY_QIANKUN__
var commonStore = ref({})
const { globalStore } = useGlobalState()
if (!isQiankun) {
  commonStore = useCommonStore()
} else {
  watchEffect(() => {
    if (globalStore.value) {
      commonStore.value = globalStore.value.commonStore
    }
  })
}
  defineOptions({ inheritAttrs: false });
  defineExpose({ setTabActiveKey });
  const props = defineProps(['activeData', 'formConf', 'drawingList', 'formInfo', 'dbType']);
  const attrs = useAttrs({ excludeDefaultKeys: false });
  const storageTypeOptions = [
    { id: 0, fullName: '展示数据' },
    { id: 1, fullName: '存储数据' },
  ];
  const VNodes = (_, { attrs }) => attrs.vnodes;
  const generatorStore = useGeneratorStore();
  const { createMessage } = useMessage();
  const [registerFieldModal, { openModal: openFieldModal }] = useModal();
  const [registerScriptModal, { openModal: openScriptModal }] = useModal();
  const fieldSelectRef = ref(null);
  const childFieldSelectRef = ref(null);
  const state = reactive({
    activeKey: 'field',
    fieldOptions: [],
    dictionaryOptions: [],
    printTplOptions: [],
    activeFunc: '',
  });
  const { activeKey, fieldOptions, printTplOptions } = toRefs(state);
  const getBindValue = computed(() => ({ ...props }));
  const config = computed<ItemCfg>(() => unref(props.activeData).__config__);
  const ceriKey = computed(() => unref(props.activeData).__config__?.ceriKey);
  const getCompName = computed(() => {
    const allComps = [...inputComponents, ...selectComponents, ...systemComponents, ...layoutComponents];
    const comp = allComps.filter(o => o.__config__.ceriKey === unref(ceriKey));
    if (!comp.length) return '';
    return comp[0].__config__.label;
  });
  const getRightComp = computed(() => {
    if (!unref(ceriKey)) return null;
    if (orgList.includes(unref(ceriKey))) return RightComp.ROrgRight;
    if (unref(ceriKey) === 'popupTableSelect') return RightComp.RPopupSelect;
    return RightComp['R' + upperFirst(unref(ceriKey))];
  });
  const getRightCompBind = computed(() => ({
    activeData: props.activeData,
    dicOptions: state.dictionaryOptions,
    ...unref(attrs),
    drawingOptions: unref(drawingOptions),
    drawingList: props.drawingList,
    formInfo: unref(props.formInfo),
  }));
  const drawingOptions = computed(() => {
    let list: any[] = [];
    const loop = (data, parent?) => {
      if (!data) return;
      if (data.__config__ && data.__config__.ceriKey !== 'table' && data.__config__.children && Array.isArray(data.__config__.children)) {
        loop(data.__config__.children, data);
      }
      if (Array.isArray(data)) data.forEach(d => loop(d, parent));
      if (data.__vModel__ && data.__config__.ceriKey !== 'table') {
        list.push({ id: data.__vModel__, fullName: data.__config__.label });
      }
    };
    loop(unref(props.drawingList));
    return list;
  });
  const getHasTable = computed(() => generatorStore.getHasTable);
  const getAllTable = computed({
    get() {
      console.info(111,generatorStore.getAllTable)
      return generatorStore.getAllTable;
    },
    set(val) {
      generatorStore.setAllTable(val);
    },
  });
  const getSubTable = computed({
    get() {
      return generatorStore.getSubTable;
    },
    set(val) {
      generatorStore.setSubTable(val);
    },
  });
  const getFormItemList = computed({
    get() {
      return generatorStore.getFormItemList;
    },
    set(val) {
      generatorStore.setFormItemList(val);
    },
  });
  const getMainTable = computed(() => {
    const allTable = unref(getAllTable);
    let item = allTable.filter(o => o.typeId == '1')[0];
    if (!item || !item.tableName) return '';
    return item.tableName;
  });
  const fieldText = computed(() => {
    return '不能使用【系统自动生成】字段，如：tenantid、id、foreignid、flowid、flowtaskid、deleteuserid、deletetime、deletemark、version、tenant_id、foreign_id、flow_id、flow_task_id、delete_user_id、delete_time、delete_mark、f_tenant_id、f_id、f_foreign_id、f_flow_id、f_flow_task_id、f_delete_user_id、f_delete_time、f_delete_mark、f_version';
  });

  watch(
    () => unref(props.formConf),
    val => {
      if (val.formStyle === 'word-form' && val.labelPosition === 'top') {
        val.labelPosition = 'left';
      }
    },
    { deep: true },
  );
  watch(
    () => unref(props.activeData),
    val => {
      if (!val || !val.__config__) return;
      if (!val.__config__.tableName && val.__config__.ceriKey !== 'table') {
        val.__config__.tableName = unref(getMainTable);
      }
      setDefaultOptions();
    },
  );
  function getSubTableField(key) {
    let item: Recordable = {};
    let list = unref(getSubTable).filter(o => o.tableName === key);
    if (list.length) {
      item = list[0];
    }
    let arr = [];
    if (item && item.fields) {
      arr = item.fields.filter(o => o.primaryKey != 1).map(o => ({ ...o, fullName: o.fieldName ? o.field + '(' + o.fieldName + ')' : o.field }));
    }
    return arr;
  }
  function onChildTableFieldChange(val, formId) {
    if (!val) return (props.activeData.__vModel__ = '');
    let boo = false;
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        const e = list[i];
        const config = e.__config__;
        if (config.ceriKey === 'table' && config.tableName === props.activeData.__config__.relationTable) {
          for (let j = 0; j < config.children.length; j++) {
            const child = config.children[j];
            if (child.__vModel__ === val && child.__config__.formId !== formId) {
              boo = true;
              break;
            }
          }
        }
        if (config && config.ceriKey != 'table' && config.children && Array.isArray(config.children)) {
          loop(config.children);
        }
      }
    };
    loop(unref(props.drawingList));
    if (boo) {
      createMessage.warning(`子表字段【${val}】已存在,请重新选择!`);
      props.activeData.__vModel__ = undefined;
      nextTick(() => {
        props.activeData.__vModel__ = '';
      });
      return;
    }
  }
  function onFieldChange(val, formId) {
    if (!val) return (props.activeData.__vModel__ = '');
    let boo = false;
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        const e = list[i];
        const config = e.__config__;
        if (e.__vModel__ === val && e.__config__.formId !== formId) {
          boo = true;
          break;
        }
        if (config && config.ceriKey != 'table' && config.children && Array.isArray(config.children)) {
          loop(config.children);
        }
      }
    };
    loop(unref(props.drawingList));
    if (boo) {
      createMessage.warning(`字段【${val}】已存在,请重新选择!`);
      props.activeData.__vModel__ = undefined;
      nextTick(() => {
        props.activeData.__vModel__ = '';
      });
      return;
    }
  }
  function onInputFieldChange(val, formId, parentVModel) {
    console.info(111,val,props.activeData.__config__)
    if (!val) return;
    const RegExp = /(^_([a-zA-Z0-9]_?)*$)|(^[a-zA-Z](_?[a-zA-Z0-9])*_?$)/;
    if (!RegExp.test(val)) {
      createMessage.warning(`请输入正确的字段`);
      return;
    }
    let boo = false;
    let childBoo = false;
    if (parentVModel) {
      const loop = (data, parent?) => {
        if (!data) return;
        if (data.__config__ && data.__config__.children && Array.isArray(data.__config__.children)) {
          loop(data.__config__.children, data);
        }
        if (Array.isArray(data)) data.forEach(d => loop(d, parent));
        if (parent?.__config__?.ceriKey == 'table') {
          if (
            data.__vModel__ &&
            data.__vModel__.toLowerCase() === val.toLowerCase() &&
            data.__config__.formId !== formId &&
            data.__config__.parentVModel === parentVModel
          ) {
            childBoo = true;
            return;
          }
        }
      };
      loop(unref(props.drawingList));
    } else {
      const loop = (data, parent?) => {
        if (!data) return;
        if (data.__config__?.ceriKey !== 'table' && data.__config__?.children && Array.isArray(data.__config__.children)) {
          loop(data.__config__.children, data);
        }
        if (Array.isArray(data)) data.forEach(d => loop(d, parent));
        if (data.__vModel__ && data.__vModel__.toLowerCase() === val.toLowerCase() && data.__config__.formId !== formId) {
          boo = true;
          return;
        }
      };
      loop(unref(props.drawingList));
    }
    if (boo) {
      createMessage.warning(`字段【${val}】已存在,请重新输入!`);
      props.activeData.__vModel__ = '';
      return;
    }
    if (childBoo) {
      createMessage.warning(`子表字段【${val}】已存在,请重新输入!`);
      props.activeData.__vModel__ = '';
      return;
    }
    props.activeData.__vModel__ = toggleVmodelCase(val);
  }
  function toggleVmodelCase(str) {
    const dbType = props.dbType || '';
    if (dbType.toLowerCase() === 'Oracle'.toLowerCase() || dbType.toLowerCase() === 'DM'.toLowerCase()) {
      return str.toUpperCase();
    }
    if (dbType.toLowerCase() === 'PostgreSQL'.toLowerCase() || dbType.toLowerCase() === 'KingBaseES'.toLowerCase()) {
      return str.toLowerCase();
    }
    return str;
  }
  function onStorageChange(val) {
    if (val == 0) {
      props.activeData.__vModel__ = '';
      props.activeData.__config__.noShow = false;
    } else {
      if (!unref(getHasTable)) {
        props.activeData.__vModel__ = toggleVmodelCase(`${props.activeData.__config__.ceriKey}Field${props.activeData.__config__.formId}`);
      } else {
        if (props.activeData.__config__.isSubTable && !props.activeData.__config__.relationTable) {
          const loop = (data, parent?) => {
            if (!data) return;
            if (data.__config__ && data.__config__.ceriKey !== 'table' && data.__config__.children && Array.isArray(data.__config__.children)) {
              loop(data.__config__.children, data);
            }
            if (Array.isArray(data)) data.forEach(d => loop(d, parent));
            if (data.__vModel__ === props.activeData.__config__.parentVModel) {
              props.activeData.__config__.relationTable = data.__config__.tableName;
            }
          };
          loop(unref(props.drawingList));
        }
        setDefaultOptions();
      }
    }
  }
  function onTableChange() {
    props.activeData.__vModel__ = '';
    setDefaultOptions();
  }

  function onFieldDropdownVisibleChange(open) {
    if (!open) return
    setDefaultOptions()
  }
  // 设计子表关联子表变化
  function onChildTableNameChange(tableName, formId) {
    if (!tableName) return;
    const drawingList = unref(props.drawingList) || [];
    let boo = false;
    const loop = list => {
      for (let i = 0; i < list.length; i++) {
        const e = list[i];
        const config = e.__config__;
        if (config.ceriKey === 'table' && config.tableName === tableName && config.formId !== formId) {
          boo = true;
          break;
        }
        if (config && config.ceriKey != 'table' && config.children && Array.isArray(config.children)) {
          loop(config.children);
        }
      }
    };
    loop(drawingList);
    if (boo) {
      createMessage.warning(`子表【${tableName}】已存在,请重新选择!`);
      props.activeData.__config__.tableName = undefined;
      nextTick(() => {
        props.activeData.__config__.tableName = '';
      });
      return;
    }
    for (let i = 0; i < props.activeData.__config__.children.length; i++) {
      props.activeData.__config__.children[i].__config__.relationTable = tableName;
      props.activeData.__config__.children[i].__vModel__ = '';
    }
  }
  function setDefaultOptions() {
    if (!unref(getHasTable)) return;
    let fieldOptions: any[] = [];
    if ((props.activeData.__vModel__ === undefined && props.activeData.isStorage != 1) || props.activeData.__config__.ceriKey === 'table') return;
    const allTable: any[] = unref(getAllTable) as any[];
    const mainTableName = unref(getMainTable);

    // 主表或未指定表名时，优先使用主表字段
    if (!props.activeData.__config__.tableName || props.activeData.__config__.tableName === mainTableName) {
      let mainFields: any[] = getFormItemList.value as any[];
      if (!mainFields.length) {
        const mainItem: any = allTable.filter(o => o.typeId == '1')[0];
        if (mainItem && mainItem.fields) {
          let rawFields: any = mainItem.fields;
          try {
            if (!Array.isArray(rawFields)) rawFields = JSON.parse(rawFields);
          } catch {
            rawFields = [];
          }
          mainFields = (rawFields || []).map((o: any) => ({
            ...o,
            field: o.field ?? o.name,
            fieldName: o.fieldName ?? o.columnName ?? o.name,
          }));
          getFormItemList.value = mainFields;
          mainItem.fields = mainFields;
        }
      }
      fieldOptions = (mainFields || []).map(o => ({ ...o, realField: o.field }));
    } else {
      let list = allTable.filter(o => o.tableName === props.activeData.__config__.tableName);
      if (!list.length) {
        props.activeData.__config__.tableName = mainTableName;
        const mainFields: any[] = (getFormItemList.value as any[]).map(o => ({ ...o, realField: o.field }));
        fieldOptions = mainFields;
        props.activeData.__vModel__ = '';
      } else {
        let item: any = list[0];
        let rawFields: any = item.fields || [];
        try {
          if (!Array.isArray(rawFields)) rawFields = JSON.parse(rawFields);
        } catch {
          rawFields = [];
        }
        const normalizedFields: any[] = (rawFields || []).map((o: any) => ({
          ...o,
          field: o.field ?? o.name,
          fieldName: o.fieldName ?? o.columnName ?? o.name,
        }));
        item.fields = normalizedFields;
        const options = normalizedFields.map(o => ({
          ...o,
          realField: 'ceri_' + props.activeData.__config__.tableName + '_ceri_' + o.field,
        }));
        fieldOptions = options;
      }
    }
    (state.fieldOptions as any[]) = fieldOptions
      .filter(o => o.primaryKey != 1)
      .map(o => ({ ...o, fullName: o.fieldName ? o.field + '(' + o.fieldName + ')' : o.field }));
  }
  function handleAddField() {
    let tableName = '';
    if (!props.activeData.__config__.isSubTable) {
      tableName = props.activeData.__config__.tableName;
      (unref(fieldSelectRef) as any)?.getSelectRef().blur();
    } else {
      tableName = props.activeData.__config__.relationTable;
      (unref(childFieldSelectRef) as any)?.getSelectRef().blur();
    }
    const linkId = props.formInfo.dbLinkId || '0';
    setTimeout(() => {
      openFieldModal(true, { linkId, tableName });
    }, 200);
  }
  async function updateFieldOptions() {
    let tableName = '';
    if (!props.activeData.__config__.isSubTable) {
      tableName = props.activeData.__config__.tableName;
    } else {
      tableName = props.activeData.__config__.relationTable;
    }
    const type = props.formInfo.type;
    const queryType = type == 3 || type == 4 || type == 5 ? '1' : '0';
    const res = await getDataModelFieldList(props.formInfo.dbLinkId, tableName, queryType);
    let fields = res.data.list;
    for (let i = 0; i < getAllTable.value.length; i++) {
      if (getAllTable.value[i].tableName === tableName) {
        getAllTable.value[i].fields = fields;
        break;
      }
    }
    if (!props.activeData.__config__.isSubTable) {
      if (props.activeData.__config__.tableName === unref(getMainTable)) getFormItemList.value = fields;
      setDefaultOptions();
    } else {
      getSubTable.value = getAllTable.value.filter(o => o.typeId == '0');
    }
  }
  function getFuncText(key) {
    let text = '';
    switch (key) {
      case 'change':
        text = '发生变化时触发';
        break;
      case 'blur':
        text = '失去焦点时触发';
        break;
      case 'click':
        text = '点击时触发';
        break;
      case 'tabClick':
        text = '面板点击时触发';
        break;
      default:
        text = '';
        break;
    }
    return text;
  }
  function editFunc(funcName) {
    state.activeFunc = funcName;
    openScriptModal(true, { text: props.activeData.on[state.activeFunc] });
  }
  function updateScript(data) {
    openScriptModal();
    props.activeData.on[state.activeFunc] = data;
  }
  // function getDictionaryType() {
  //   getDictionaryTypeSelector().then(res => {
  //     state.dictionaryOptions = res.data.list.filter(o => o.children && o.children.length);
  //   });
  // }
  // function getPrintTplList() {
  //   getPrintDevSelector().then(res => {
  //     state.printTplOptions = res.data.list.filter(o => o.children && o.children.length).map(o => ({ ...o, hasChildren: true }));
  //   });
  // }
  function hasTipLabel(ceriKey) {
    return ![...layoutList, ...systemList].includes(ceriKey) || ['table', 'card', 'billRule'].includes(ceriKey);
  }
  function setTabActiveKey(data) {
    state.activeKey = data;
  }

  onMounted(() => {
    // getDictionaryType();
    // getPrintTplList();
    if (!props.activeData || !props.activeData.__config__) return;
    if (!props.activeData.__config__.tableName && props.activeData.__config__.ceriKey !== 'table') {
      props.activeData.__config__.tableName = unref(getMainTable);
    }
    setDefaultOptions();

    console.info('getHasTable：',getHasTable,config)
  });
</script>
