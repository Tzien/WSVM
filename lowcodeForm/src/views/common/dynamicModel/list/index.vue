<template>
  <div class="ceri-content-wrapper">
    <div class="ceri-content-wrapper-left" v-if="columnData.type === 2">
      <BasicLeftTree v-bind="getLeftTreeBindValue" ref="leftTreeRef" @reload="getTreeView()" @select="handleLeftTreeSelect" />
    </div>
    <div class="ceri-content-wrapper-center">
      <div class="ceri-content-wrapper-search-box" v-if="getSearchList?.length">
        <BasicForm
          @register="registerSearchForm"
          :schemas="getSearchList"
          @advanced-change="redoHeight"
          @submit="handleSearchSubmit"
          @reset="handleSearchReset"
          class="search-form"
        >
        </BasicForm>
      </div>
      <div class="ceri-content-wrapper-content" style="background-color: white">
        <!-- <a-tabs
          v-model:activeKey="tabActiveKey"
          class="ceri-content-wrapper-tabs"
          destroyInactiveTabPane
          @change="onTabChange"
          v-if="[1, 4].includes(columnData.type) && tabList.length">
          <a-tab-pane v-for="item in tabList" :key="item.id" :tab="item.fullName"></a-tab-pane>
        </a-tabs> -->
        <BasicTable @register="registerTable" v-bind="getTableBindValue" ref="tableRef" @columns-change="handleColumnChange">
          <template #tableTitle>
              <a-tooltip placement="top">
              <template #title>
                <span>{{ t('common.back') }}</span>
              </template>
              <ArrowLeftOutlined @click="goBack()" />
            </a-tooltip>
            <a-button
              v-for="item in state.headerBtnsList"
              :key="item.value"
              :type="item.value === 'add' ? 'primary' : 'link'"
              :preIcon="item.icon"
              @click="headBtnsHandle(item.value)"
            >
              {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
            </a-button>
            <a-button
              v-for="item in state.customHeaderBtnsList"
              :key="item.value"
              type="link"
              :preIcon="item.event?.btnIcon"
              @click="headCustomBtnsHandle(item)"
            >
              {{ item.labelI18nCode ? t(item.labelI18nCode, item.label) : item.label }}
            </a-button>
          </template>
          <template #toolbar v-if="columnData.hasSuperQuery && config.webType != '4'">
          
            <a-tooltip placement="top">
              <template #title>
                <span>{{ t('common.superQuery') }}</span>
              </template>
              <filter-outlined @click="openSuperQuery(true, { columnOptions: state.columnOptions, hidePlan: isDataManage })" />
            </a-tooltip>
          </template>
          <template #toolbarAfter v-if="!isDataManage">
            <ViewList :menuId="searchInfo.menuId" :viewList="viewList" @itemClick="handleViewClick" @reload="initViewList" />
            <ViewSetting :menuId="searchInfo.menuId" :viewList="viewList" :currentView="currentView" @reload="initViewList" />
          </template>
          <template #expandedRowRender="{ record }" v-if="[1, 2].includes(columnData.type) && getChildTableStyle === 2 && childColumnList.length">
            <a-tabs size="small">
              <a-tab-pane :key="cIndex" :tab="child.label" :label="child.label" v-for="(child, cIndex) in childColumnList">
                <BasicTable @register="registerChildTable" :ellipsis="!!columnData.showOverflow" :data-source="record[child.prop]" :columns="child.children">
                  <template #bodyCell="{ column = {}, record: childRecord }">
                    <template v-if="column.ceriKey === 'relationForm'">
                      <p class="link-text" @click="toDetail(column.modelId, childRecord[`${column.dataIndex}_id`], column.propsValue)">
                        {{ childRecord[column.dataIndex] }}
                      </p>
                    </template>
                    <template v-if="column.ceriKey === 'inputNumber'">
                      <ceri-input-number
                        v-model:value="childRecord[column.dataIndex]"
                        :precision="column.precision"
                        :thousands="column.thousands"
                        disabled
                        detailed
                      />
                    </template>
                    <template v-if="column.ceriKey === 'calculate'">
                      <ceri-calculate
                        v-model:value="childRecord[column.dataIndex]"
                        :isStorage="column.isStorage"
                        :precision="column.precision"
                        :thousands="column.thousands"
                        :roundType="column.roundType"
                        detailed
                      />
                    </template>
                    <template v-if="column.ceriKey === 'sign'">
                      <ceri-sign v-model:value="childRecord[column.dataIndex]" detailed />
                    </template>
                    <template v-if="column.ceriKey === 'signature'">
                      <ceri-signature v-model:value="childRecord[column.dataIndex]" detailed />
                    </template>
                    <template v-if="column.ceriKey === 'rate'">
                      <ceri-rate v-model:value="childRecord[column.dataIndex]" :count="column.count" :allowHalf="column.allowHalf" disabled />
                    </template>
                    <template v-if="column.ceriKey === 'slider'">
                      <ceri-slider v-model:value="childRecord[column.dataIndex]" :min="column.min" :max="column.max" :step="column.step" disabled />
                    </template>
                    <template v-if="column.ceriKey === 'uploadImg'">
                      <ceri-upload-img v-model:value="childRecord[column.dataIndex]" disabled detailed simple v-if="childRecord[column.dataIndex]?.length" />
                    </template>
                    <template v-if="column.ceriKey === 'uploadFile'">
                      <ceri-upload-file v-model:value="childRecord[column.dataIndex]" disabled detailed simple v-if="childRecord[column.dataIndex]?.length" />
                    </template>
                    <template v-if="column.ceriKey === 'input'">
                      <ceri-input
                        v-model:value="childRecord[column.dataIndex]"
                        :useMask="column.useMask"
                        :maskConfig="column.maskConfig"
                        :showOverflow="columnData.showOverflow"
                        detailed
                      />
                    </template>
                  </template>
                </BasicTable>
              </a-tab-pane>
            </a-tabs>
          </template>
          <template #bodyCell="{ column = {}, record, index }">
            <template v-if="column.flag === 'INDEX'">
              <div class="edit-row-action" v-if="columnData.type === 4 && !config.enableFlow">
                <span class="edit-row-index">{{ index + 1 }}</span>
                <i class="ym-custom ym-custom-arrow-expand" @click="handleRowForm(record)"></i>
              </div>
              <span v-else>{{ index + 1 }}</span>
            </template>
            <template v-if="columnData.type === 4">
              <template v-if="record.rowEdit">
                <template v-if="column.ceriKey === 'inputNumber'">
                  <ceri-input-number
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :min="column.min"
                    :max="column.max"
                    :step="column.step"
                    :controls="column.controls"
                    :addonBefore="column.addonBefore"
                    :addonAfter="column.addonAfter"
                    :precision="column.precision"
                    :thousands="column.thousands"
                    :disabled="column.disabled"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'calculate'">
                  <ceri-calculate
                    v-model:value="record[column.prop]"
                    :isStorage="column.isStorage"
                    :precision="column.precision"
                    :thousands="column.thousands"
                    :roundType="column.roundType"
                    detailed
                  />
                </template>
                <template v-else-if="column.ceriKey === 'rate'">
                  <ceri-rate v-model:value="record[column.prop]" :count="column.count" :allowHalf="column.allowHalf" :disabled="column.disabled" />
                </template>
                <template v-else-if="column.ceriKey === 'slider'">
                  <ceri-slider v-model:value="record[column.prop]" :min="column.min" :max="column.max" :step="column.step" :disabled="column.disabled" />
                </template>
                <template v-else-if="column.ceriKey === 'uploadImg'">
                  <ceri-upload-img
                    v-model:value="record[column.prop]"
                    :fileSize="column.fileSize"
                    :sizeUnit="column.sizeUnit"
                    :buttonText="column.buttonText"
                    :limit="column.limit"
                    :pathType="column.pathType"
                    :sortRule="column.sortRule"
                    :timeFormat="column.timeFormat"
                    :folder="column.folder"
                    :tipText="column.tipText"
                    :showType="column.showType"
                    :disabled="column.disabled"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'uploadFile'">
                  <ceri-upload-file
                    v-model:value="record[column.prop]"
                    :accept="column.accept"
                    :fileSize="column.fileSize"
                    :sizeUnit="column.sizeUnit"
                    :buttonText="column.buttonText"
                    :limit="column.limit"
                    :pathType="column.pathType"
                    :sortRule="column.sortRule"
                    :timeFormat="column.timeFormat"
                    :folder="column.folder"
                    :tipText="column.tipText"
                    :disabled="column.disabled"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'switch'">
                  <ceri-switch v-model:value="record[column.prop]" :disabled="column.disabled" />
                </template>
                <template v-else-if="column.ceriKey === 'timePicker'">
                  <ceri-time-picker
                    v-model:value="record[column.prop]"
                    :format="column.format"
                    :startTime="column.startTime"
                    :endTime="column.endTime"
                    :placeholder="column.placeholder"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'datePicker'">
                  <ceri-date-picker
                    v-model:value="record[column.prop]"
                    :format="column.format"
                    :startTime="column.startTime"
                    :endTime="column.endTime"
                    :allowClear="column.clearable"
                    :placeholder="column.placeholder"
                    :disabled="column.disabled"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'organizeSelect'">
                  <ceri-organize-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :selectType="column.selectType"
                    :ableIds="column.ableIds"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'depSelect'">
                  <ceri-dep-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :selectType="column.selectType"
                    :ableIds="column.ableIds"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'roleSelect'">
                  <ceri-role-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :selectType="column.selectType"
                    :ableIds="column.ableIds"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'groupSelect'">
                  <ceri-group-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :selectType="column.selectType"
                    :ableIds="column.ableIds"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'posSelect'">
                  <ceri-pos-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :selectType="column.selectType"
                    :ableIds="column.ableIds"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'userSelect'">
                  <ceri-user-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :selectType="['all', 'custom'].includes(column.selectType) ? column.selectType : 'all'"
                    :ableIds="column.ableIds"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'usersSelect'">
                  <ceri-users-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :selectType="column.selectType"
                    :ableIds="column.ableIds"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'areaSelect'">
                  <ceri-area-select
                    v-model:value="record[column.prop]"
                    :level="column.level"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                  />
                </template>
                <template v-else-if="['select', 'radio', 'checkbox'].includes(column.ceriKey)">
                  <ceri-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple || column.ceriKey === 'checkbox'"
                    :allowClear="column.clearable || ['radio', 'checkbox'].includes(column.ceriKey)"
                    :showSearch="column.filterable"
                    :disabled="column.disabled"
                    :options="column.options"
                    :fieldNames="column.props"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'cascader'">
                  <ceri-cascader
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :showSearch="column.filterable"
                    :disabled="column.disabled"
                    :options="column.options"
                    :fieldNames="column.props"
                    :showAllLevels="column.showAllLevels"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'treeSelect'">
                  <ceri-tree-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :showSearch="column.filterable"
                    :disabled="column.disabled"
                    :options="column.options"
                    :fieldNames="column.props"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'relationForm'">
                  <ceri-relation-form
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :modelId="column.modelId"
                    :columnOptions="column.columnOptions"
                    :relationField="column.relationField"
                    :hasPage="column.hasPage"
                    :pageSize="column.pageSize"
                    :popupType="column.popupType"
                    :popupTitle="column.popupTitle"
                    :popupWidth="column.popupWidth"
                    :propsValue="column.propsValue"
                    :queryType="column.queryType"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'popupSelect' || column.ceriKey === 'popupTableSelect'">
                  <ceri-popup-select
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :multiple="column.multiple"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :interfaceId="column.interfaceId"
                    :templateJson="column.templateJson"
                    :columnOptions="column.columnOptions"
                    :propsValue="column.propsValue"
                    :relationField="column.relationField"
                    :hasPage="column.hasPage"
                    :pageSize="column.pageSize"
                    :popupType="column.popupType"
                    :popupTitle="column.popupTitle"
                    :popupWidth="column.popupWidth"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'autoComplete'">
                  <ceri-auto-complete
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :interfaceId="column.interfaceId"
                    :relationField="column.relationField"
                    :templateJson="column.templateJson"
                    :total="column.total"
                  />
                </template>
                <template v-else-if="['input', 'textarea'].includes(column.ceriKey)">
                  <ceri-input
                    v-model:value="record[column.prop]"
                    :placeholder="column.placeholder"
                    :allowClear="column.clearable"
                    :disabled="column.disabled"
                    :readonly="column.readonly"
                    :prefixIcon="column.prefixIcon"
                    :suffixIcon="column.suffixIcon"
                    :addonBefore="column.addonBefore"
                    :addonAfter="column.addonAfter"
                    :maxlength="column.maxlength"
                    :showCount="column.showCount"
                    :showPassword="column.showPassword"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'location'">
                  <ceri-location
                    v-model:value="record[column.prop]"
                    :autoLocation="column.autoLocation"
                    :enableLocationScope="column.enableLocationScope"
                    :adjustmentScope="column.adjustmentScope"
                    :enableDesktopLocation="column.enableDesktopLocation"
                    :locationScope="column.locationScope"
                    :clearable="column.clearable"
                    :disabled="column.disabled"
                  />
                </template>
                <template v-else-if="column.ceriKey === 'sign'">
                  <ceri-sign v-model:value="record[column.prop]" :isInvoke="column.isInvoke" :disabled="column.disabled" />
                </template>
                <template v-else-if="column.ceriKey === 'signature'">
                  <ceri-signature v-model:value="record[column.prop]" :ableIds="column.ableIds" :disabled="column.disabled" />
                </template>
                <template v-else-if="systemComponentsList.includes(column.ceriKey)">
                  {{ record[column.prop + '_name'] || record[column.prop] }}
                </template>
                <template v-else>
                  {{ record[column.prop] }}
                </template>
              </template>
              <template v-else>
                <template v-if="column.ceriKey === 'inputNumber'">
                  <ceri-input-number
                    v-model:value="record[column.prop + '_name']"
                    :precision="column.precision"
                    :thousands="column.thousands"
                    disabled
                    detailed
                  />
                </template>
                <template v-else-if="column.ceriKey === 'calculate'">
                  <ceri-calculate
                    v-model:value="record[column.prop + '_name']"
                    :isStorage="column.isStorage"
                    :precision="column.precision"
                    :thousands="column.thousands"
                    :roundType="column.roundType"
                    detailed
                  />
                </template>
                <template v-else-if="column.ceriKey === 'sign'">
                  <ceri-sign v-model:value="record[column.prop + '_name']" detailed />
                </template>
                <template v-else-if="column.ceriKey === 'signature'">
                  <ceri-signature v-model:value="record[column.prop + '_name']" detailed />
                </template>
                <template v-else-if="column.ceriKey === 'rate'">
                  <ceri-rate v-model:value="record[column.prop + '_name']" :count="column.count" :allowHalf="column.allowHalf" disabled />
                </template>
                <template v-else-if="column.ceriKey === 'slider'">
                  <ceri-slider v-model:value="record[column.prop + '_name']" :min="column.min" :max="column.max" :step="column.step" disabled />
                </template>
                <template v-else-if="column.ceriKey === 'uploadImg'">
                  <ceri-upload-img v-model:value="record[column.prop + '_name']" disabled detailed simple v-if="record[column.prop]?.length" />
                </template>
                <template v-else-if="column.ceriKey === 'uploadFile'">
                  <ceri-upload-file v-model:value="record[column.prop + '_name']" disabled detailed simple v-if="record[column.prop]?.length" />
                </template>
                <template v-else-if="column.ceriKey === 'input'">
                  <ceri-input
                    v-model:value="record[column.prop + '_name']"
                    :useMask="column.useMask"
                    :maskConfig="column.maskConfig"
                    :showOverflow="columnData.showOverflow"
                    detailed
                  />
                </template>
                <template v-else-if="column.ceriKey === 'relationForm'">
                  <p class="link-text" @click="toDetail(column.modelId, record[`${column.prop}_id`], column.propsValue)">
                    {{ record[column.prop + '_name'] || record[column.prop] }}
                  </p>
                </template>
                <template v-else>
                  {{ record[column.prop + '_name'] || record[column.prop] }}
                </template>
              </template>
            </template>
            <template v-else>
              <template v-for="(item, index) in childColumnList" v-if="getChildTableStyle !== 2 && childColumnList.length">
                <template v-if="column.id?.includes('-') && item.children && item.children[0] && column.id === item.children[0]?.id">
                  <ChildTableColumn
                    :data="record[item.prop]"
                    :head="item.children"
                    @toggleExpand="toggleExpand(record, `${item.prop}Expand`)"
                    @toDetail="toDetail"
                    :expand="record[`${item.prop}Expand`]"
                    :showOverflow="columnData.showOverflow"
                    :key="index"
                  />
                </template>
              </template>
              <template v-if="!(record.top || column.id?.includes('-'))">
                <template v-if="column.ceriKey === 'relationForm'">
                  <p class="link-text" @click="toDetail(column.modelId, record[`${column.prop}_id`], column.propsValue)">{{ record[column.prop] }}</p>
                </template>
                <template v-if="column.ceriKey === 'inputNumber'">
                  <ceri-input-number v-model:value="record[column.prop]" :precision="column.precision" :thousands="column.thousands" disabled detailed />
                </template>
                <template v-if="column.ceriKey === 'calculate'">
                  <ceri-calculate
                    v-model:value="record[column.prop]"
                    :isStorage="column.isStorage"
                    :precision="column.precision"
                    :thousands="column.thousands"
                    :roundType="column.roundType"
                    detailed
                  />
                </template>
                <template v-if="column.ceriKey === 'sign'">
                  <ceri-sign v-model:value="record[column.prop]" detailed />
                </template>
                <template v-if="column.ceriKey === 'signature'">
                  <ceri-signature v-model:value="record[column.prop]" detailed />
                </template>
                <template v-if="column.ceriKey === 'rate'">
                  <ceri-rate v-model:value="record[column.prop]" :count="column.count" :allowHalf="column.allowHalf" disabled />
                </template>
                <template v-if="column.ceriKey === 'slider'">
                  <ceri-slider v-model:value="record[column.prop]" :min="column.min" :max="column.max" :step="column.step" disabled />
                </template>
                <template v-if="column.ceriKey === 'uploadImg'">
                  <ceri-upload-img v-model:value="record[column.prop]" disabled detailed simple v-if="record[column.prop]?.length" />
                </template>
                <template v-if="column.ceriKey === 'uploadFile'">
                  <ceri-upload-file v-model:value="record[column.prop]" disabled detailed simple v-if="record[column.prop]?.length" />
                </template>
                <template v-if="column.ceriKey === 'input'">
                  <ceri-input
                    v-model:value="record[column.prop]"
                    :useMask="column.useMask"
                    :maskConfig="column.maskConfig"
                    :showOverflow="columnData.showOverflow"
                    detailed
                  />
                </template>
              </template>
            </template>
            <template v-if="column.key === 'flowState' && config.enableFlow == 1 && (!record.top || columnData.type == 5)">
              <CeriTextTag :content="getFlowStatusContent(record.flowState)" :color="getFlowStatusColor(record.flowState)" />
            </template>
            <template v-if="column.key === 'action' && (!record.top || columnData.type == 5)">
              <TableAction :actions="getTableActions(record, index)" :dropDownActions="getDropDownActions(record, index)" />
            </template>
          </template>
          <template #summary v-if="columnData.showSummary && [1, 2, 4].includes(columnData.type)">
            <a-table-summary fixed>
              <a-table-summary-row>
                <template v-if="getHasBatchBtn">
                  <a-table-summary-cell :index="0" :col-span="2">{{ t('component.table.summary') }}</a-table-summary-cell>
                  <a-table-summary-cell :index="1" :col-span="0"></a-table-summary-cell>
                  <a-table-summary-cell v-for="(item, index) in getColumnSum" :key="index" :index="index + 2" :align="getSummaryCellAlign(index)">
                    {{ item }}
                  </a-table-summary-cell>
                  <a-table-summary-cell :index="getColumnSum.length + 2"></a-table-summary-cell>
                </template>
                <template v-else>
                  <a-table-summary-cell :index="0">{{ t('component.table.summary') }}</a-table-summary-cell>
                  <a-table-summary-cell v-for="(item, index) in getColumnSum" :key="index" :index="index + 1" :align="getSummaryCellAlign(index)">
                    {{ item }}
                  </a-table-summary-cell>
                  <a-table-summary-cell :index="getColumnSum.length + 1"></a-table-summary-cell>
                </template>
              </a-table-summary-row>
            </a-table-summary>
          </template>
        </BasicTable>
      </div>
    </div>
    <Form ref="formRef" @reload="reload" />
    <Detail ref="detailRef" />
    <!-- <CandidateModal @register="registerCandidate" @confirm="submitCandidate" /> -->
    <!-- <FlowParser @register="registerFlowParser" @reload="reload" /> -->
    <ExportModal @register="registerExportModal" @download="handleDownload" />
    <ImportModal @register="registerImportModal" @reload="reload" />
    <SuperQueryModal @register="registerSuperQueryModal" @superQuery="handleSuperQuery" />
    <CustomForm ref="customFormRef" @reload="reload" />
    <PrintSelect @register="registerPrintSelect" @change="handleShowBrowse" />
    <PrintBrowse @register="registerPrintBrowse" />
  </div>
</template>

<script lang="ts" setup>
import {
  getModelList,
  exportModel,
  batchDelete,
  getConfigData,
  createModel,
  updateModel,
  getModelInfo,
  getViewList,
  launchFlow
} from '@/api/onlineDev/visualDev'
import { create as createFlowForm, update as updateFlowForm } from '@/api/workFlow/task'
import { getCandidates } from '@/api/workFlow/task'
import { getDataInterfaceRes } from '@/api/systemData/dataInterface'
// import { getOrgByOrganizeCondition, getDepartmentSelectAsyncList } from '@/api/permission/organize';
import { ref, reactive, onMounted, toRefs, computed, unref, nextTick, toRaw, provide } from 'vue'
import { useMessage } from '@/hooks/web/useMessage'
import { useI18n } from 'vue-i18n'
import { useUserStore } from '@/store/user'
import { useBaseStore } from '@/store/base'
import { useModal } from '@/components/Modal'
// import { usePopup } from '@/components/Popup';
import { BasicLeftTree, TreeActionType } from '@/components/Tree'
import { BasicForm, useForm } from '@/components/Form'
import { BasicTable, useTable, TableAction, ActionItem, TableActionType, SorterResult } from '@/components/Table'
import Form from './Form.vue'
import CustomForm from './CustomForm.vue'
import Detail from './detail/index.vue'
import ChildTableColumn from './ChildTableColumn.vue'
// import FlowParser from '@/views/workFlow/components/FlowParser.vue';
// import CandidateModal from '@/views/workFlow/components/modal/CandidateModal.vue';
import { ExportModal, ImportModal, SuperQueryModal } from '@/components/CommonModal'
import { downloadByUrl } from '@/utils/file/download'
import { useRoute,useRouter } from 'vue-router'
import { getScriptFunc, onlineUtils, getDateTimeUnit, getTimeUnit, thousandsFormat, getParamList, getLaunchFlowParamList } from '@/utils/ceri'
import { FilterOutline ,ArrowLeftOutlined } from '@ant-design/icons-vue'
// import { getSearchFormSchemas } from '@/components/FormGenerator/src/helper/transform';
import { dyOptionsList, systemComponentsList } from '@/views/WebDesign/component/StepForm/src/helper/config'
import { CeriRelationForm } from '@/components/CeriOS'
import { cloneDeep } from 'lodash-es'
import PrintSelect from '@/components/PrintDesign/printSelect/index.vue'
import PrintBrowse from '@/components/PrintDesign/printBrowse/index.vue'
import dayjs from 'dayjs'
// import { usePermission } from '@/hooks/web/usePermission';
import { useDefineSetting } from '@/hooks/setting/useDefineSetting'
import ViewSetting from './components/ViewSetting.vue'
import ViewList from './components/ViewList.vue'

interface State {
  config: any
  columnData: any
  formConf: any
  headerBtnsList: any[]
  columnBtnsList: any[]
  customHeaderBtnsList: any[]
  customColumnBtnsList: any[]
  columnOptions: any[]
  treeFieldNames: any
  leftTreeData: any[]
  leftTreeLoading: boolean
  isLeftTreeLazy: boolean
  treeActiveId: string
  treeActiveNodePath: any
  columns: any[]
  complexColumns: any[]
  childColumnList: any[]
  exportList: any[]
  cacheList: any[]
  currRow: any
  workFlowFormData: any
  expandObj: any
  columnSettingList: any[]
  searchSchemas: any[]
  treeRelationObj: any
  treeQueryJson: any
  leftTreeActiveInfo: any
  customRow: any
  customCell: any
  tabActiveKey: any
  tabList: any[]
  tabQueryJson: any
  viewList: any[]
  currentView: any
}

const props = defineProps(['config', 'modelId', 'isPreview', 'isDataManage'])
const route = useRoute()
const router = useRouter()
// const { hasBtnP } = usePermission();
const { createMessage, createConfirm } = useMessage()
const { t } = useI18n()
const userStore = useUserStore()
const baseStore = useBaseStore()
// const [registerFlowParser, { openPopup: openFlowParser }] = usePopup();
const [registerExportModal, { openModal: openExportModal, closeModal: closeExportModal, setModalProps: setExportModalProps }] = useModal()
const [registerImportModal, { openModal: openImportModal }] = useModal()
const [registerSuperQueryModal, { openModal: openSuperQuery }] = useModal()
// const [registerCandidate, { openModal: openCandidateModal, closeModal: closeCandidateModal }] = useModal();
const [registerPrintSelect, { openModal: openPrintSelect }] = useModal()
const [registerPrintBrowse, { openModal: openPrintBrowse }] = useModal()
const leftTreeRef = ref<Nullable<TreeActionType>>(null)
const formRef = ref<any>(null)
const customFormRef = ref<any>(null)
const tableRef = ref<Nullable<TableActionType>>(null)
const detailRef = ref<any>(null)
const searchInfo = reactive({
  modelId: '',
  menuId: '',
  queryJson: '',
  superQueryJson: ''
})
const state = reactive<State>({
  config: {},
  columnData: {},
  formConf: {},
  headerBtnsList: [],
  columnBtnsList: [],
  customHeaderBtnsList: [],
  customColumnBtnsList: [],
  columnOptions: [],
  treeFieldNames: {
    children: 'children',
    title: 'fullName',
    key: 'id',
    isLeaf: 'isLeaf'
  },
  leftTreeData: [],
  leftTreeLoading: false,
  isLeftTreeLazy: false,
  treeActiveId: '',
  treeActiveNodePath: [],
  columns: [],
  complexColumns: [], // 复杂表头
  childColumnList: [],
  exportList: [],
  cacheList: [],
  currRow: {},
  workFlowFormData: {},
  expandObj: {},
  columnSettingList: [],
  searchSchemas: [],
  treeRelationObj: null,
  treeQueryJson: {},
  leftTreeActiveInfo: {},
  customRow: null,
  customCell: null,
  tabActiveKey: '',
  tabList: [],
  tabQueryJson: {},
  viewList: [],
  currentView: {}
})
const { columnData, childColumnList, tabActiveKey, tabList, viewList, currentView } = toRefs(state)
const { getFlowStatusContent, getFlowStatusColor } = useDefineSetting()
const [registerSearchForm, { updateSchema, resetFields, submit: searchFormSubmit }] = useForm({
  baseColProps: { span: 6 },
  showActionButtonGroup: true,
  showAdvancedButton: true,
  compact: true
})
const [
  registerTable,
  {
    reload,
    setLoading,
    insertTableDataRecord,
    updateTableDataRecord,
    deleteTableDataRecord,
    getFetchParams,
    getSelectRowKeys,
    getSelectRows,
    clearSelectedRowKeys,
    redoHeight
  }
] = useTable({
  api: getModelList,
  immediate: false,
  clickToRowSelect: false,
  tableSetting: { setting: false, redo: !props.isPreview },
  afterFetch: (data) => {
    // 行内编辑
    if (state.columnData.type === 4) {
      const list = data.map((o) => ({ ...o, rowEdit: false }))
      state.cacheList = cloneDeep(list)
      nextTick(() => {
        if (state.columnData.funcs?.afterOnload) setTableLoadFunc()
      })
      return list
    }
    let list = data.map((o) => ({
      ...o,
      ...state.expandObj
    }))
    state.cacheList = cloneDeep(list)
    // 分组表格
    if (state.columnData.type === 3) {
      list.map((o) => {
        if (o.children && o.children.length) {
          o.children = o.children.map((e) => ({
            ...e,
            ...state.expandObj
          }))
        }
      })
    }
    nextTick(() => {
      if (state.columnData.funcs?.afterOnload) setTableLoadFunc()
    })
    return list
  }
})
const [registerChildTable] = useTable({
  pagination: false,
  canResize: false,
  showTableSetting: false
})

provide('getLeftTreeActiveInfo', () => state.leftTreeActiveInfo)

const getHasBatchBtn = computed(
  () =>
    state.columnData.type != 5 &&
    (state.headerBtnsList.some((o) => ['batchRemove', 'batchPrint', 'download'].includes(o.value)) || state.customHeaderBtnsList.length)
)
const getLeftTreeBindValue = computed(() => {
  const key = +new Date()
  const data: any = {
    title: state.columnData.treeTitleI18nCode ? t(state.columnData.treeTitleI18nCode, state.columnData.treeTitle) : state.columnData.treeTitle,
    showSearch: state.columnData.hasTreeQuery && state.columnData.treeSyncType == 0 && !state.isLeftTreeLazy,
    fieldNames: state.treeFieldNames,
    defaultExpandAll: state.columnData.treeSyncType == 0 && !state.isLeftTreeLazy,
    treeData: state.leftTreeData,
    loading: state.leftTreeLoading,
    key
  }
  if (state.columnData.treeSyncType == 1 || state.isLeftTreeLazy) data.loadData = onLoadData
  return data
})
const getPagination = computed(() => {
  if ([3, 5].includes(state.columnData.type) || !state.columnData.hasPage) return false
  return { pageSize: state.columnData.pageSize }
})
const getChildTableStyle = computed(() => (state.columnData.type == 3 || state.columnData.type == 5 ? 1 : state.columnData.childTableStyle))
const getColumns = computed(() => {
  const columns = unref(getChildTableStyle) == 2 || state.columnData.type == 4 ? state.columns : state.complexColumns
  return props.isDataManage ? columns : setListValue(state.currentView?.columnList, columns, 'prop')
})
const getSearchList = computed(() => {
  const allSearchSchemas = cloneDeep(state.searchSchemas).map((o) => ({ ...o, show: true }))
  const searchSchemas = setListValue(state.currentView?.searchList, allSearchSchemas, 'field')
  buildSearchOptions(searchSchemas)
  return props.isDataManage ? allSearchSchemas : searchSchemas
})
const getRowKey = computed(() => (state.config.webType == 4 && state.columnData.viewKey ? state.columnData.viewKey : 'id'))
const getTableBindValue = computed(() => {
  let columns = unref(getColumns)
  if (state.config.enableFlow) {
    const boo = columns.some((o) => o.dataIndex === 'flowState')
    if (!boo)
      columns.push({
        title: t('component.table.status'),
        dataIndex: 'flowState',
        width: 100,
        align: 'center',
        fixed: columns.some((o) => o.fixed == 'right') ? 'right' : 'none'
      })
  }
  const defaultSortConfig = (state.columnData.defaultSortConfig || []).map((o) => (o.sort === 'desc' ? '-' : '') + o.field)
  const data: any = {
    pagination: unref(getPagination),
    searchInfo: unref(searchInfo),
    defSort: { sidx: defaultSortConfig.join(',') },
    sortFn: (sortInfo: SorterResult | SorterResult[]) => {
      if (Array.isArray(sortInfo)) {
        const sortList = sortInfo.map((o) => (o.order === 'descend' ? '-' : '') + o.field)
        return { sidx: sortList.join(',') }
      } else {
        const { field, order } = sortInfo
        if (field && order) {
          // 排序字段
          return { sidx: (order === 'descend' ? '-' : '') + field }
        } else {
          return {}
        }
      }
    },
    columns,
    clearSelectOnPageChange: true,
    ellipsis: !!state.columnData.showOverflow,
    isTreeTable: [3, 5].includes(state.columnData.type),
    bordered: (unref(getChildTableStyle) != 2 && !!state.childColumnList?.length) || !!state.columnData.complexHeaderList?.length,
    rowKey: unref(getRowKey)
  }
  if (unref(getHasBatchBtn)) {
    const rowSelection: any = { type: 'checkbox' }
    if (state.columnData.type === 3) rowSelection.getCheckboxProps = (record) => ({ disabled: !!record.top })
    data.rowSelection = rowSelection
  }
  if (state.columnBtnsList.length || state.customColumnBtnsList.length) {
    let customWidth = state.customColumnBtnsList.length ? 50 : 0
    if (state.columnData.type == 4 && state.config.enableFlow) customWidth += 50
    let columnBtnsLen = state.columnBtnsList.length
    const actionWidth = columnBtnsLen ? columnBtnsLen * 50 + customWidth : customWidth + 20
    data.actionColumn = {
      width: actionWidth,
      title: t('component.table.action'),
      dataIndex: 'action',
      fixed: 'right'
    }
  }
  if (state.customRow) data.customRow = state.customRow
  return data
})
const getSummaryColumn = computed(() => {
  let defaultColumns = unref(getColumns)
  // 处理列固定
  if (state.columnSettingList?.length) {
    for (let i = 0; i < defaultColumns.length; i++) {
      inner: for (let j = 0; j < state.columnSettingList.length; j++) {
        if (defaultColumns[i].dataIndex === state.columnSettingList[j].dataIndex) {
          defaultColumns[i].fixed = state.columnSettingList[j].fixed
          defaultColumns[i].visible = state.columnSettingList[j].visible
          break inner
        }
      }
    }
    defaultColumns = defaultColumns.filter((o) => o.visible)
  }
  let columns: any[] = []
  for (let i = 0; i < defaultColumns.length; i++) {
    const e = defaultColumns[i]
    if (e.ceriKey === 'table' || e.ceriKey === 'complexHeader') {
      if (e.children?.length) columns.push(...e.children)
    } else {
      columns.push(e)
    }
    if (e.fixed && e.children?.length) {
      for (let j = 0; j < e.children.length; j++) {
        e.children[j].fixed = e.fixed
      }
    }
  }
  const leftFixedList = columns.filter((o) => o.fixed === 'left')
  const rightFixedList = columns.filter((o) => o.fixed === 'right')
  const noFixedList = columns.filter((o) => o.fixed !== 'left' && o.fixed !== 'right')
  return [...leftFixedList, ...noFixedList, ...rightFixedList]
})
// 列表合计
const getColumnSum = computed(() => {
  const sums: any[] = []
  const isSummary = (key) => state.columnData.summaryField.includes(key)
  const useThousands = (key) => unref(getSummaryColumn).some((o) => o.__vModel__ === key && o.thousands)
  unref(getSummaryColumn).forEach((column, index) => {
    let sumVal = state.cacheList.reduce((sum, d) => sum + getCmpValOfRow(d, column.prop), 0)
    if (!isSummary(column.prop)) sumVal = ''
    sumVal = Number.isNaN(sumVal) ? '' : sumVal
    const realVal = sumVal && !Number.isInteger(sumVal) ? Number(sumVal).toFixed(2) : sumVal
    sums[index] = useThousands(column.prop) ? thousandsFormat(realVal) : realVal
  })
  if ([1, 2].includes(state.columnData.type) && unref(getChildTableStyle) === 2 && state.childColumnList.length) sums.unshift('')
  return sums
})

function getCmpValOfRow(row, key) {
  const isSummary = (key) => state.columnData.summaryField.includes(key)
  if (!state.columnData.summaryField.length || !isSummary(key)) return 0
  const target = row[key]
  if (!target) return 0
  const data = isNaN(target) ? 0 : Number(target)
  return data
}
function getSummaryCellAlign(index) {
  if (!unref(getSummaryColumn).length) return
  return unref(getSummaryColumn)[index]?.align || 'left'
}
function getTableActions(record, index): ActionItem[] {
  const list = state.columnBtnsList.map((o) => {
    const item: ActionItem = {
      label: o.labelI18nCode ? t(o.labelI18nCode, o.label) : o.label,
      onClick: columnBtnsHandle.bind(null, o.value, record)
    }
    if (o.value === 'remove') item.color = 'error'
    if (state.config.enableFlow) {
      if (o.value === 'edit') item.disabled = ![0, 8, 9].includes(record.flowState)
      if (o.value === 'remove') item.disabled = ![0, 9].includes(record.flowState)
      if (o.value === 'detail') item.disabled = !record.flowState
    } else {
      if (o?.event?.enableFunc) {
        const parameter = { row: record, rowIndex: index, onlineUtils }
        const func: any = getScriptFunc(o.event.enableFunc)
        item.disabled = (func && !func(parameter)) || false
      }
    }
    return item
  })
  if (record.rowEdit) {
    let editBtnList: ActionItem[] = [
      { label: t('common.saveText'), onClick: saveForRowEdit.bind(null, record, 0) },
      { label: t('common.cancelText'), color: 'error', onClick: cancelRowEdit.bind(null, record, index) }
    ]
    if (state.config.enableFlow) {
      editBtnList.push({ label: t('common.submitText'), onClick: submitForRowEdit.bind(null, record) })
    }
    return editBtnList
  }
  return list
}
function getDropDownActions(record, index): ActionItem[] {
  const list = state.customColumnBtnsList.map((o) => {
    const item: ActionItem = {
      label: o.labelI18nCode ? t(o.labelI18nCode, o.label) : o.label,
      onClick: customBtnsHandle.bind(null, o, record, index)
    }
    if (o?.event?.enableFunc) {
      const parameter = { row: record, rowIndex: index, onlineUtils }
      const func: any = getScriptFunc(o.event?.enableFunc)
      item.disabled = (func && !func(parameter)) || false
    }
    return item
  })
  return list
}
function openFlowPopup() {
  const data = { id: '', flowId: state.config.flowId, opType: '-1' }
  // openFlowParser(true, data);
}
function handleLeftTreeSelect(id, _node, nodePath) {
  if (props.isPreview) return
  if (state.treeActiveId == id) return
  state.treeActiveId = id
  state.treeActiveNodePath = nodePath
  let queryJson: any = {}
  let leftTreeActiveInfo: any = {}
  const isMultiple = !state.treeRelationObj ? false : state.treeRelationObj.searchMultiple
  if (state.treeRelationObj?.ceriKey && ['organizeSelect', 'cascader', 'areaSelect'].includes(state.treeRelationObj.ceriKey)) {
    let currValue = []
    if (state.columnData.treeDataSource === 'formField' && state.treeRelationObj.ceriKey === 'organizeSelect') {
      currValue = state.treeActiveNodePath[state.treeActiveNodePath.length - 1].organizeIds
    } else {
      currValue = state.treeActiveNodePath.map((o) => o[state.treeFieldNames.key])
    }
    queryJson = { [state.columnData.treeRelation]: isMultiple ? [currValue] : currValue }
    leftTreeActiveInfo = { [state.columnData.treeRelation]: state.treeRelationObj.multiple ? [currValue] : currValue }
  } else {
    queryJson = { [state.columnData.treeRelation]: isMultiple ? [state.treeActiveId] : state.treeActiveId }
    leftTreeActiveInfo = { [state.columnData.treeRelation]: state.treeRelationObj.multiple ? [state.treeActiveId] : state.treeActiveId }
  }
  state.treeQueryJson = queryJson
  state.leftTreeActiveInfo = leftTreeActiveInfo
  unref(getSearchList)?.length ? resetFields() : handleSearchSubmit({})
}
// 行内编辑新增
function addHandleForRowEdit() {
  let item = { rowEdit: true, id: 'ceriAdd' }
  const userInfo = userStore.getUserInfo
  const currDate = new Date()
  for (let i = 0; i < state.columnData.columnList.length; i++) {
    let e = state.columnData.columnList[i]
    const config = e.__config__
    if (config.defaultCurrent) {
      if (config.ceriKey === 'datePicker') {
        config.defaultValue = dayjs(currDate).startOf(getDateTimeUnit(e.format)).valueOf()
      }
      if (config.ceriKey === 'timePicker') {
        config.defaultValue = dayjs(currDate).format(e.format || 'HH:mm:ss')
      }
      if (config.ceriKey === 'organizeSelect' && userInfo.organizeIdList?.length) {
        config.defaultValue = e.multiple ? [userInfo.organizeIdList] : userInfo.organizeIdList
      }
      if (config.ceriKey === 'depSelect' && userInfo.departmentId) {
        config.defaultValue = e.multiple ? [userInfo.departmentId] : userInfo.departmentId
      }
      if (config.ceriKey === 'userSelect' && userInfo.userId) {
        config.defaultValue = e.multiple ? [userInfo.userId] : userInfo.userId
      }
      if (config.ceriKey === 'usersSelect' && userInfo.userId) {
        config.defaultValue = e.multiple ? [userInfo.userId + '--user'] : userInfo.userId + '--user'
      }
      if (config.ceriKey === 'posSelect' && userInfo.positionIds?.length) {
        config.defaultValue = e.multiple ? userInfo.positionIds.map((o) => o.id) : userInfo.positionIds[0].id
      }
      if (config.ceriKey === 'roleSelect' && userInfo.roleIds?.length) {
        config.defaultValue = e.multiple ? userInfo.roleIds : userInfo.roleIds[0]
      }
      if (config.ceriKey === 'groupSelect' && userInfo.groupIds?.length) {
        config.defaultValue = e.multiple ? userInfo.groupIds : userInfo.groupIds[0]
      }
      if (config.ceriKey === 'sign' && userInfo.signImg) {
        config.defaultValue = userInfo.signImg
      }
    }
    if (!config.isSubTable) item[e.__vModel__] = config.defaultValue
  }
  insertTableDataRecord(cloneDeep(item), 0)
}
// 新增
function addHandle() {
  // 行内编辑新增
  if (state.columnData.type === 4) return addHandleForRowEdit()
  // 带流程新增
  if (state.config.enableFlow == 1) return openFlowPopup()
  const data = {
    id: '',
    formConf: state.formConf,
    modelId: props.modelId,
    isPreview: props.isPreview,
    isDataManage: props.isDataManage,
    useFormPermission: state.columnData.useFormPermission,
    showMoreBtn: ![3, 5].includes(state.columnData.type),
    menuId: searchInfo.menuId,
    allList: state.cacheList
  }
  formRef.value?.init(data)
}
// 顶部按钮点击事件
function headBtnsHandle(key) {
  // 新建
  if (key === 'add') return addHandle()
  // 导出
  if (key == 'download')
    return openExportModal(true, {
      columnList: state.exportList,
      selectIds: getSelectRowKeys(),
      showExportSelected: state.config.webType != 4 || state.columnData.viewKey
    })
  // 导入
  if (key == 'upload') return openImportModal(true, { modelId: props.modelId, menuId: searchInfo.menuId, flowId: state.config.flowId })
  if (props.isPreview) return
  // 批量删除
  if (key === 'batchRemove') handleBatchRemove()
  // 批量打印
  if (key === 'batchPrint') handleBatchPrint()
}
// 顶部自定义按钮点击事件
function headCustomBtnsHandle(item) {
  if (props.isPreview) return
  const selectRows = getSelectRows()
  if (!selectRows.length && item.event.dataRequired) return createMessage.error(t('common.selectDataTip'))
  if (item.event.btnType == 2) handleScriptFunc(item.event, selectRows, 0)
  if (item.event.btnType == 3) handleBatchInterface(item.event, selectRows)
  if (item.event.btnType == 4) handleBatchLaunchFlow(item, selectRows)
}
// 导出
function handleDownload(data) {
  if (props.isPreview) {
    setExportModalProps({ confirmLoading: false })
    createMessage.warning('功能预览不支持数据导出')
    return
  }
  let query = { ...getFetchParams(), ...data }
  exportModel(props.modelId, query)
    .then((res) => {
      setExportModalProps({ confirmLoading: false })
      if (!res.data.url) return
      downloadByUrl({ url: res.data.url })
      closeExportModal()
    })
    .catch(() => {
      setExportModalProps({ confirmLoading: false })
    })
}
// 批量删除
function handleBatchRemove() {
  const ids = getSelectRowKeys()
  if (!ids.length) return createMessage.error(t('common.selectDataTip'))
  const query = { ids, flowId: state.config.flowId || '' }
  createConfirm({
    iconType: 'warning',
    title: t('common.tipTitle'),
    content: t('common.batchDelTip'),
    onOk: () => {
      batchDelete(props.modelId, query).then((res) => {
        createMessage.success(res.msg)
        clearSelectedRowKeys()
        reload()
      })
    }
  })
}
// 批量打印
function handleBatchPrint() {
  if (state.config.isPreview) return createMessage.warning('功能预览不支持打印')
  if (!state.columnData.printIds?.length) return createMessage.error('未配置打印模板')
  const ids = getSelectRowKeys()
  if (!ids.length) return createMessage.error(t('common.selectDataTip'))
  if (state.columnData.printIds?.length === 1) return handleShowBrowse(state.columnData.printIds[0])
  openPrintSelect(true, state.columnData.printIds)
}
function handleShowBrowse(id) {
  const formInfo: any[] = (getSelectRows() || []).map((o) => {
    const item: any = { formId: o.id }
    if (state.config.enableFlow) item.flowTaskId = o.flowTaskId || o.id
    return item
  })
  openPrintBrowse(true, { id, formInfo })
}
// 行按钮点击事件
function columnBtnsHandle(key, record) {
  if (key === 'edit') return updateHandle(record)
  if (key === 'detail') return goDetail(record)
  if (key == 'remove') handleDelete(record.id)
}
// 编辑
function updateHandle(record) {
  // 行内编辑
  if (state.columnData.type === 4) return editForRowEdit(record)
  if (state.config.enableFlow == 1) {
    let data = {
      id: record.flowTaskId || record.id,
      flowId: state.config.flowId,
      opType: '-1'
    }
    // openFlowParser(true, data);
  } else {
    const data = {
      id: record.id,
      formConf: state.formConf,
      modelId: props.modelId,
      isPreview: props.isPreview,
      isDataManage: props.isDataManage,
      useFormPermission: state.columnData.useFormPermission,
      showMoreBtn: ![3, 5].includes(state.columnData.type),
      menuId: searchInfo.menuId,
      allList: state.cacheList
    }
    formRef.value?.init(data)
  }
}
// 行内编辑
function editForRowEdit(record) {
  record.rowEdit = true
}
function cancelRowEdit(record, index) {
  const id = !record.id || record.id === 'ceriAdd' ? '' : record.id
  if (!id) return deleteTableDataRecord('ceriAdd')
  record.rowEdit = false
  const item = cloneDeep(state.cacheList[index])
  updateTableDataRecord(item.id, item)
}
// 行内编辑保存
function saveForRowEdit(record, status = 0, candidateData: any = null) {
  if (props.isPreview) return createMessage.warning('功能预览不支持数据保存')
  const id = !record.id || record.id === 'ceriAdd' ? '' : record.id
  if (state.config.enableFlow == 1) {
    let query = {
      id: record.flowTaskId || id,
      status: status || 0,
      formData: record,
      flowId: state.config.flowId,
      flowUrgent: 1
    }
    if (candidateData) query = { ...query, ...candidateData }
    const formMethod = id ? updateFlowForm : createFlowForm
    formMethod(query).then((res) => {
      createMessage.success(res.msg)
      // closeCandidateModal();
      reload({ page: 1 })
    })
  } else {
    const query = { id, data: JSON.stringify(record), menuId: searchInfo.menuId }
    const formMethod = id ? updateModel : createModel
    formMethod(props.modelId, query).then((res) => {
      createMessage.success(res.msg)
      reload({ page: 1 })
    })
  }
}
// 行内编辑提交审核
function submitForRowEdit(record) {
  if (props.isPreview) return createMessage.warning('功能预览不支持数据保存')
  state.currRow = record
  const id = !record.id || record.id === 'ceriAdd' ? '' : record.id
  state.workFlowFormData = {
    id: record.flowTaskId || id,
    formData: record,
    flowId: state.config.flowId
  }
  getCandidates(0, state.workFlowFormData).then((res) => {
    const candidateList = res?.data?.list.filter((o) => !o.isBranchFlow && o.isCandidates)
    const branchList = res.data.list.filter((o) => o.isBranchFlow)
    if (!candidateList.length && res.data.type == 3) {
      createConfirm({
        iconType: 'warning',
        title: '提示',
        content: '您确定要提交当前流程吗, 是否继续?',
        onOk: () => {
          saveForRowEdit(record, 1)
        }
      })
      return
    }
    // openCandidateModal(true, { branchList, candidateList, formData: state.workFlowFormData });
  })
}
// 选择候选人
function submitCandidate(data) {
  saveForRowEdit(state.currRow, 1, data)
}
// 查看详情
function goDetail(record) {
  if (state.config.enableFlow == 1) {
    const data = {
      id: record.flowTaskId || record.id,
      flowId: state.config.flowId,
      opType: 0
    }
    // openFlowParser(true, data);
  } else {
    const data = {
      id: record.id,
      formConf: state.formConf,
      modelId: props.modelId,
      menuId: searchInfo.menuId,
      useFormPermission: state.columnData.useFormPermission,
      title: state.config.fullName,
      isDataManage: props.isDataManage,
      hideExtra: props.isDataManage
    }
    detailRef.value?.init(data)
  }
}
function handleDelete(id) {
  createConfirm({
    iconType: 'warning',
    title: t('common.tipTitle'),
    content: t('common.delTip'),
    onOk: () => {
      const query = { ids: [id], flowId: state.config.flowId || '' }
      batchDelete(props.modelId, query).then((res) => {
        createMessage.success(res.msg)
        reload()
      })
    }
  })
}
// 行自定义按钮点击事件
function customBtnsHandle(item, record, index) {
  if (item.event.btnType == 1) handlePopup(item.event, record)
  if (item.event.btnType == 2) handleScriptFunc(item.event, record, index)
  if (item.event.btnType == 3) handleInterface(item.event, record)
  if (item.event.btnType == 4) handleLaunchFlow(item, record)
}
function handlePopup(item, record) {
  const data = {
    ...item,
    recordModelId: props.modelId,
    record: toRaw(record),
    isPreview: props.isPreview,
    webType: state.config.webType,
    rowKey: unref(getRowKey)
  }
  customFormRef.value?.init(data)
}
function handleScriptFunc(item, record, index) {
  const parameter = { data: record, index, refresh: reload, onlineUtils }
  const func: any = getScriptFunc(item.func)
  if (!func) return
  func(parameter)
}
function handleInterface(item, record) {
  const handlerData = () => {
    getModelInfo(props.modelId, record[unref(getRowKey)]).then((res) => {
      const dataForm = res.data || {}
      if (!dataForm.data) return
      const data = { ...JSON.parse(dataForm.data), id: record[unref(getRowKey)] }
      handlerInterface(data)
    })
  }
  const handlerInterface = (data) => {
    const query = { paramList: getParamList(item.templateJson, { ...data, id: record[unref(getRowKey)] }, unref(getRowKey)) || [] }
    getDataInterfaceRes(item.interfaceId, query).then((res) => {
      createMessage.success(res.msg)
      if (item.isRefresh) reload()
    })
  }
  const handleFun = () => {
    state.config.webType == '4' ? handlerInterface(record) : handlerData()
  }
  if (!item.useConfirm) return handleFun()
  createConfirm({
    iconType: 'warning',
    title: t('common.tipTitle'),
    content: item.confirmTitle || '确认执行此操作?',
    onOk: () => {
      handleFun()
    }
  })
}
function handleBatchInterface(item, records) {
  const getBatchParamList = (templateJson, data?) => {
    if (!templateJson?.length) return []
    for (let i = 0; i < templateJson.length; i++) {
      const e = templateJson[i]
      if (e.sourceType == 1 && data?.length) {
        e.defaultValue = data.map((o) => (state.config.webType == '4' ? o[e.relationField] : o[e.relationField + '_ceriId']))
      }
      if (e.sourceType == 4 && e.relationField == '@formId') e.defaultValue = data.map((o) => o.id)
    }
    return templateJson
  }
  const handlerInterface = (data) => {
    const query = { paramList: getBatchParamList(item.templateJson, data) || [] }
    getDataInterfaceRes(item.interfaceId, query).then((res) => {
      createMessage.success(res.msg)
      if (item.isRefresh) reload()
    })
  }
  if (!item.useConfirm) return handlerInterface(records)
  createConfirm({
    iconType: 'warning',
    title: t('common.tipTitle'),
    content: item.confirmTitle || '确认执行此操作?',
    onOk: () => {
      handlerInterface(records)
    }
  })
}
function handleLaunchFlow(item, record) {
  handleBatchLaunchFlow(item, [record])
}
function handleBatchLaunchFlow(item, records) {
  const launchFlowCfg = cloneDeep(item.event.launchFlow)
  let dataList: any[] = []
  for (let i = 0; i < records.length; i++) {
    dataList.push(getLaunchFlowParamList(launchFlowCfg.transferList, records[i], unref(getRowKey)))
  }
  const query = {
    template: launchFlowCfg.flowId,
    btnCode: item.value,
    currentUser: launchFlowCfg.currentUser,
    customUser: launchFlowCfg.customUser,
    initiator: launchFlowCfg.initiator,
    dataList
  }
  launchFlow(props.modelId, query).then((res) => {
    createMessage.success(res.msg)
  })
}
async function init() {
  state.config = {
    modelId: props.modelId,
    isPreview: props.isPreview,
    ...props.config
  }
  searchInfo.modelId = props.modelId
  searchInfo.menuId = route.meta.modelId as string
  if (props.isPreview) searchInfo.menuId = props.modelId
  // if (props.isPreview) searchInfo.menuId = '270579315303777093'

  if (!state.config.columnData || (state.config.webType != '4' && !state.config.formData)) return
  state.columnData = JSON.parse(state.config.columnData)
  if (state.columnData.type === 3) {
    state.columnData.columnList = state.columnData.columnList.filter((o) => o.prop != state.columnData.groupField)
  }
  state.formConf = state.config.formData ? JSON.parse(state.config.formData) : {}
  const customBtnsList = state.columnData.customBtnsList || []
  const columnBtnsList = state.columnData.columnBtnsList || []
  getHeaderBtnsList(state.columnData.btnsList || [])
  getColumnBtnsList(columnBtnsList, customBtnsList)
  state.columnOptions = state.columnData.columnOptions || []
  if (!unref(getPagination)) (searchInfo as any).pageSize = 1000000
  setLoading(true)
  if (state.columnData.funcs.rowStyle) {
    state.customRow = (record, index) => {
      const data = { row: record, rowIndex: index }
      const func: any = getScriptFunc(state.columnData.funcs.rowStyle)
      const style: any = func ? func(data) : null
      if (!style) return {}
      return { style }
    }
  }
  if (state.columnData.funcs.cellStyle) {
    state.customCell = (record, rowIndex, column) => {
      const data = { row: record, rowIndex, column, columnIndex: column.key }
      const func: any = getScriptFunc(state.columnData.funcs.cellStyle)
      const style: any = func ? func(data) : null
      if (!style) return {}
      return { style }
    }
  }
  getSearchSchemas()
  getColumnList()
  !props.isDataManage && (await initViewList())
  if (state.columnData.type == 4) buildOptions()
  if ([1, 4].includes(state.columnData.type)) getTabList()
  if (props.isPreview) return setLoading(false)
  if (state.columnData.type === 2) {
    state.treeFieldNames.key = state.columnData.treePropsValue || 'id'
    state.treeFieldNames.title = state.columnData.treePropsLabel || 'fullName'
    state.treeFieldNames.children = state.columnData.treePropsChildren || 'children'
    getTreeView()
  } else {
    nextTick(() => {
      unref(getSearchList)?.length ? searchFormSubmit() : handleSearchSubmit({})
    })
  }
}
async function getTreeView() {
  state.isLeftTreeLazy = false
  state.leftTreeLoading = true
  state.leftTreeActiveInfo = {}
  state.treeQueryJson = {}
  state.leftTreeData = []
  state.treeActiveId = ''
  state.treeActiveNodePath = []
  let leftTreeData: any[] = []
  if (state.columnData.treeDataSource === 'dictionary') {
    if (!state.columnData.treeDictionary) return (state.leftTreeLoading = false)
    leftTreeData = await baseStore.getDicDataSelector(state.columnData.treeDictionary)
  }
  if (state.columnData.treeDataSource === 'organize') {
    state.isLeftTreeLazy = true
    // const res = await getDepartmentSelectAsyncList();
    // leftTreeData = res.data.list;
  }
  if (state.columnData.treeDataSource === 'api') {
    if (!state.columnData.treePropsUrl) return (state.leftTreeLoading = false)
    const query = { paramList: getParamList(state.columnData.treeTemplateJson) || [] }
    const res = await getDataInterfaceRes(state.columnData.treePropsUrl, query)
    leftTreeData = Array.isArray(res.data) ? res.data : []
  }
  if (state.columnData.treeDataSource === 'formField') {
    const treeRelationObj: any = state.treeRelationObj
    const ceriKey = treeRelationObj?.__config__?.ceriKey || ''
    if (state.columnData.treeRelation && ['organizeSelect', 'depSelect'].includes(ceriKey)) {
      if (treeRelationObj.selectType === 'all') {
        state.isLeftTreeLazy = true
        // const res = await getDepartmentSelectAsyncList();
        // leftTreeData = res.data.list;
      }
      if (treeRelationObj.selectType === 'custom' && treeRelationObj.ableIds?.length) {
        const departIds = ceriKey === 'organizeSelect' ? treeRelationObj.ableIds.map((o) => o[o.length - 1]) : treeRelationObj.ableIds
        // const res = await getOrgByOrganizeCondition({ departIds });
        // leftTreeData = res.data.list;
      }
    }
  }
  state.leftTreeData = leftTreeData
  state.leftTreeLoading = false
  nextTick(() => {
    if (state.leftTreeData.length) leftTreeRef.value?.setExpandedKeys([state.leftTreeData[0].id])
    unref(getSearchList)?.length ? searchFormSubmit() : reload({ page: 1 })
  })
}
//获取标签面板数据、设置标签面板默认值
async function getTabList() {
  state.tabList = []
  if (!state.columnData.tabConfig || !state.columnData.tabConfig.on || !state.columnData.tabConfig.relationField) return
  state.columnData.tabConfig?.hasAllTab && state.tabList.push({ fullName: '全部', id: '' })
  const list: any[] = state.columnData.columnOptions.filter((o) => o.__vModel__ == state.columnData.tabConfig.relationField) || []
  if (list?.length) {
    if (list[0].__config__.dataType == 'dictionary' && list[0].__config__.dictionaryType) {
      const data = (await baseStore.getDicDataSelector(list[0].__config__.dictionaryType)) || []
      const options = list[0].props.value == 'enCode' ? data.map((o) => ({ ...o, id: o.enCode })) : data
      state.tabList = [...state.tabList, ...options]
    } else {
      state.tabList = [...state.tabList, ...list[0].options]
    }
  }
  if (state.tabList?.length) {
    state.tabActiveKey = state.tabList[0].id || ''
    state.tabQueryJson = { [state.columnData.tabConfig.relationField]: state.tabList[0].id }
  }
}
function onTabChange(val) {
  if (props.isPreview) return
  state.tabQueryJson = { [state.columnData.tabConfig.relationField]: val }
  unref(getSearchList)?.length ? resetFields() : handleSearchSubmit({})
}
function getHeaderBtnsList(btnsList) {
  btnsList = btnsList.filter((o) => o.show || !Reflect.has(o, 'show'))
  if (props.isPreview || props.isDataManage || !state.columnData.useBtnPermission) return (state.headerBtnsList = btnsList)
  // 过滤权限
  let btns: any[] = []
  // for (let i = 0; i < btnsList.length; i++) {
  //   if (hasBtnP('btn_' + btnsList[i].value)) btns.push(btnsList[i]);
  // }
  state.headerBtnsList = btns
}
function getColumnBtnsList(columnBtnsList, customBtnsList) {
  columnBtnsList = columnBtnsList.filter((o) => o.show || !Reflect.has(o, 'show'))
  let btns: any[] = []
  let customBtns: any[] = []
  if (props.isPreview || props.isDataManage || !state.columnData.useBtnPermission) {
    btns = columnBtnsList
    customBtns = customBtnsList
  } else {
    // 过滤权限
    const permissionList = userStore.getPermissionList
    const list = permissionList.filter((o) => o.modelId === searchInfo.menuId)
    const perBtnList = list[0] && list[0].button ? list[0].button : []
    for (let i = 0; i < columnBtnsList.length; i++) {
      inner: for (let j = 0; j < perBtnList.length; j++) {
        if ('btn_' + columnBtnsList[i].value === perBtnList[j].enCode) {
          btns.push(columnBtnsList[i])
          break inner
        }
      }
    }
    for (let i = 0; i < customBtnsList.length; i++) {
      inner: for (let j = 0; j < perBtnList.length; j++) {
        if (customBtnsList[i].value === perBtnList[j].enCode) {
          customBtns.push(customBtnsList[i])
          break inner
        }
      }
    }
  }
  state.columnBtnsList = btns
  state.customHeaderBtnsList = customBtns.filter((o) => o.event?.position === 2)
  state.customColumnBtnsList = customBtns.filter((o) => o.event?.position !== 2)
  if (state.columnData.type == 5) state.customHeaderBtnsList = []
}
function getSearchSchemas() {
  if (state.columnData.treeRelation) {
    for (let i = 0; i < state.columnData.columnOptions.length; i++) {
      const e = state.columnData.columnOptions[i]
      if (e.id === state.columnData.treeRelation) {
        state.treeRelationObj = { ...e, searchMultiple: false, ceriKey: e.__config__.ceriKey }
        break
      }
    }
  }
  // state.searchSchemas = getSearchFormSchemas(state.columnData.searchList);
}
function buildSearchOptions(searchSchemas) {
  searchSchemas.forEach((cur) => {
    const config = cur.__config__
    if (dyOptionsList.includes(config.ceriKey)) {
      if (config.dataType === 'dictionary' && config.dictionaryType) {
        baseStore.getDicDataSelector(config.dictionaryType).then((res) => {
          updateSchema([{ field: cur.field, componentProps: { options: res } }])
        })
      }
      if (config.dataType === 'dynamic' && config.propsUrl) {
        const query = { paramList: config.templateJson || [] }
        getDataInterfaceRes(config.propsUrl, query).then((res) => {
          const data = Array.isArray(res.data) ? res.data : []
          updateSchema([{ field: cur.field, componentProps: { options: data } }])
        })
      }
    }
    if ((Array.isArray(cur.value) && cur.value.length) || cur.value || cur.value === 0 || cur.value === false) cur.defaultValue = cur.value
  })
  return searchSchemas
}
// 获取列
function getColumnList() {
  let columnList: any[] = []
  if (props.isPreview || props.isDataManage || !state.columnData.useColumnPermission) {
    columnList = state.columnData.columnList
  } else {
    // 过滤权限
    const permissionList = userStore.getPermissionList
    const list = permissionList.filter((o) => o.modelId === searchInfo.menuId)
    const perColumnList = list[0] && list[0].column ? list[0].column : []
    for (let i = 0; i < state.columnData.columnList.length; i++) {
      inner: for (let j = 0; j < perColumnList.length; j++) {
        if (state.columnData.columnList[i].prop === perColumnList[j].enCode) {
          columnList.push(state.columnData.columnList[i])
          break inner
        }
      }
    }
  }
  let columns = columnList.map((o) => ({
    ...o,
    placeholder: state.columnData.type == 4 && o.placeholderI18nCode ? t(o.placeholderI18nCode, o.placeholder) : o.placeholder,
    title: o.labelI18nCode ? t(o.labelI18nCode, o.label) : o.label,
    dataIndex: o.prop,
    align: o.align,
    fixed: o.fixed == 'none' ? false : o.fixed,
    sorter: o.sortable ? { multiple: 1 } : o.sortable,
    width: o.width || 100,
    customCell: state.customCell || null
  }))
  if (state.columnData.type !== 3 && state.columnData.type !== 5) columns = getComplexColumns(columns)
  state.columns = columns.filter((o) => o.prop.indexOf('-') < 0)
  if (state.columnData.type == 4) buildRowRelation()
  getChildComplexColumns(columns)
}
function getComplexColumns(columns) {
  let complexHeaderList: any[] = state.columnData.complexHeaderList || []
  if (!complexHeaderList.length) return columns
  let childColumns: any[] = []
  let firstChildColumns: string[] = []
  for (let i = 0; i < complexHeaderList.length; i++) {
    const e = complexHeaderList[i]
    e.label = e.fullName
    e.labelI18nCode = e.fullNameI18nCode
    e.title = e.fullNameI18nCode ? t(e.fullNameI18nCode, e.fullName) : e.fullName
    e.align = e.align
    e.dataIndex = e.id
    e.prop = e.id
    e.children = []
    e.ceriKey = 'complexHeader'
    if (e.childColumns?.length) {
      childColumns.push(...e.childColumns)
      for (let k = 0; k < e.childColumns.length; k++) {
        const item = e.childColumns[k]
        for (let j = 0; j < columns.length; j++) {
          const o = columns[j]
          if (o.prop == item && o.fixed !== 'left' && o.fixed !== 'right') e.children.push({ ...o })
        }
      }
    }
    if (e.children.length) firstChildColumns.push(e.children[0].prop)
  }
  complexHeaderList = complexHeaderList.filter((o) => o.children.length)
  let list: any[] = []
  for (let i = 0; i < columns.length; i++) {
    const e = columns[i]
    if (!childColumns.includes(e.prop) || e.fixed === 'left' || e.fixed === 'right') {
      list.push(e)
    } else {
      if (firstChildColumns.includes(e.prop)) {
        const item = complexHeaderList.find((o) => o.childColumns.includes(e.prop))
        list.push(item)
      }
    }
  }
  return list
}
function getChildComplexColumns(columnList) {
  let list: any[] = []
  for (let i = 0; i < columnList.length; i++) {
    const e = columnList[i]
    if (!e.prop.includes('-')) {
      list.push(e)
    } else {
      let prop = e.prop.split('-')[0]
      let vModel = e.prop.split('-')[1]
      let label = e.label.split('-')[0]
      let childLabel = e.label.replace(label + '-', '')
      if (e.fullNameI18nCode && Array.isArray(e.fullNameI18nCode) && e.fullNameI18nCode[0]) label = t(e.fullNameI18nCode[0], label)
      let newItem = {
        align: 'center',
        ceriKey: 'table',
        prop,
        label,
        title: label,
        dataIndex: prop,
        children: [],
        customCell: state.customCell || null
      }
      e.dataIndex = vModel
      e.title = e.labelI18nCode ? t(e.labelI18nCode, childLabel) : childLabel
      if (!state.expandObj.hasOwnProperty(`${prop}Expand`)) state.expandObj[`${prop}Expand`] = false
      if (!list.some((o) => o.prop === prop)) list.push(newItem)
      for (let i = 0; i < list.length; i++) {
        if (list[i].prop === prop) {
          list[i].children.push(e)
          break
        }
      }
    }
  }
  if (unref(getChildTableStyle) != 2) getMergeList(list)
  getExportList(list)
  state.complexColumns = list
  state.childColumnList = list.filter((o) => o.ceriKey === 'table')
  // 子表分组展示宽度取100
  if (unref(getChildTableStyle) !== 2) {
    for (let i = 0; i < state.childColumnList.length; i++) {
      const e = state.childColumnList[i]
      if (e.children?.length) e.children = e.children.map((o) => ({ ...o, width: 100 }))
    }
  }
}
function getMergeList(list) {
  list.forEach((item) => {
    if (item.ceriKey === 'table' && item.children && item.children.length) {
      item.children.forEach((child, index) => {
        if (index == 0) {
          child.customCell = (record, rowIndex, column) => ({
            ...(state.customCell ? state.customCell(record, rowIndex, column) : {}),
            ...{
              rowspan: 1,
              colspan: item.children.length,
              class: 'child-table-box'
            }
          })
        } else {
          child.customCell = () => ({
            rowspan: 0,
            colspan: 0
          })
        }
      })
    }
  })
}
function getExportList(list) {
  let exportList: any[] = []
  for (let i = 0; i < list.length; i++) {
    if (list[i].ceriKey === 'table') {
      if (state.columnData.type != 4) exportList.push(...list[i].children)
    } else if (list[i].ceriKey === 'complexHeader') {
      exportList.push(...list[i].children)
    } else {
      exportList.push(list[i])
    }
  }
  state.exportList = exportList
}
function setTableLoadFunc() {
  const parameter = { data: state.cacheList, tableRef: tableRef.value, onlineUtils }
  const func: any = getScriptFunc(state.columnData.funcs.afterOnload)
  if (!func) return
  func(parameter)
}
function toggleExpand(row, field) {
  row[field] = !row[field]
}
// 关联表单查看详情
function toDetail(modelId, id, propsValue) {
  if (!id) return
  getConfigData(modelId).then((res) => {
    if (!res.data || !res.data.formData) return
    const formConf = JSON.parse(res.data.formData)
    formConf.popupType = 'general'
    formConf.customBtns = []
    formConf.hasPrintBtn = false
    const data = { id, formConf, modelId, propsValue }
    detailRef.value?.init(data)
  })
}
function handleColumnChange(data) {
  state.columnSettingList = data
}
// 左侧树异步加载
function onLoadData(node) {
  return new Promise((resolve: (value?: unknown) => void) => {
    if (state.columnData.treeDataSource === 'api') {
      if (!state.columnData.treeSyncInterfaceId) return resolve()
      if (state.columnData.treeSyncTemplateJson?.length) {
        for (let i = 0; i < state.columnData.treeSyncTemplateJson.length; i++) {
          const e = state.columnData.treeSyncTemplateJson[i]
          e.defaultValue = node[e.relationField] || ''
        }
      }
      const query = { paramList: getParamList(state.columnData.treeSyncTemplateJson) || [] }
      getDataInterfaceRes(state.columnData.treeSyncInterfaceId, query).then((res) => {
        const data = Array.isArray(res.data) ? res.data : []
        leftTreeRef.value?.updateNodeByKey(node.eventKey, { children: data, isLeaf: !data.length })
        resolve()
      })
    } else {
      // getDepartmentSelectAsyncList(node.id).then(res => {
      //   const list = res.data.list;
      //   leftTreeRef.value?.updateNodeByKey(node.eventKey, { children: list, isLeaf: !list.length });
      //   resolve();
      // });
    }
  })
}
//返回上一页面
function goBack(){
  router.push('/FormTableDesign')
}
// 高级查询
function handleSuperQuery(superQueryJson) {
  if (props.isPreview) return
  searchInfo.superQueryJson = superQueryJson
  reload({ page: 1 })
}
function handleSearchSubmit(data) {
  if (props.isPreview) return
  clearSelectedRowKeys()
  let obj = {}
  for (let [key, value] of Object.entries(data)) {
    if (value || value === 0 || value === false) {
      if (Array.isArray(value)) {
        if (value.length) obj[key] = value
      } else {
        obj[key] = value
      }
    }
  }
  obj = { ...obj, ...(state.treeQueryJson || {}), ...(state.tabQueryJson || {}) }
  searchInfo.queryJson = JSON.stringify(obj) === '{}' ? '' : JSON.stringify(obj)
  reload({ page: 1 })
}
function handleSearchReset() {
  searchFormSubmit()
}
function handleRowForm(record) {
  let fields: any[] = []
  for (let i = 0; i < unref(getColumns).length; i++) {
    const e = unref(getColumns)[i]
    if (e.children?.length) {
      for (let j = 0; j < e.children.length; j++) {
        const o = e.children[j]
        o.__config__.span = 24
        o.__config__.label = o.label
        fields.push(toRaw(o))
      }
    } else {
      e.__config__.span = 24
      e.__config__.label = e.label
      fields.push(toRaw(e))
    }
  }
  fields = fields.map((o) => {
    if (o.__config__?.templateJson && o.__config__?.templateJson.length) {
      o.__config__.templateJson = o.__config__.templateJson.map((o) => ({ ...o, relationField: '' }))
    }
    if (o.templateJson && o.templateJson.length) {
      o.templateJson = o.templateJson.map((o) => ({ ...o, relationField: '' }))
    }
    return o
  })
  const formConf = { ...state.formConf, fields, popupType: 'general' }
  const data = {
    id: record.id,
    formConf,
    modelId: props.modelId,
    isPreview: props.isPreview,
    isDataManage: props.isDataManage,
    useFormPermission: state.columnData.useFormPermission,
    showMoreBtn: false,
    menuId: searchInfo.menuId,
    allList: state.cacheList,
    formData: record
  }
  formRef.value?.init(data)
}
// 行内编辑获取选项
function buildOptions() {
  const loop = (list) => {
    for (let i = 0; i < list.length; i++) {
      const cur = list[i]
      if (cur.children?.length) loop(cur.children)
      const config = cur.__config__
      if (!config) continue
      if (dyOptionsList.includes(config.ceriKey)) {
        if (config.dataType === 'dictionary' && config.dictionaryType) {
          cur.options = []
          baseStore.getDicDataSelector(config.dictionaryType).then((res) => {
            cur.options = res
          })
        }
        if (config.dataType === 'dynamic' && config.propsUrl) {
          cur.options = []
          const query = { paramList: config.templateJson || [] }
          getDataInterfaceRes(config.propsUrl, query).then((res) => {
            cur.options = Array.isArray(res.data) ? res.data : []
          })
        }
      }
    }
  }
  loop(state.columns)
}
function buildRowRelation() {
  const loop = (list) => {
    for (let i = 0; i < list.length; i++) {
      let cur = list[i]
      if (cur.children?.length) loop(cur.children)
      const config = cur?.__config__
      if (!config) continue
      if (config.ceriKey === 'datePicker') {
        if (config.startTimeRule) {
          if (config.startTimeType == 1) cur.startTime = config.startTimeValue
          if (config.startTimeType == 3) cur.startTime = new Date().getTime()
          if (config.startTimeType == 4 || config.startTimeType == 5) {
            const type = getTimeUnit(config.startTimeTarget)
            const method = config.startTimeType == 4 ? 'subtract' : 'add'
            const startTime = dayjs()[method](config.startTimeValue, type)
            let realStartTime = startTime.startOf('day').valueOf()
            if (config.startTimeTarget == 4) realStartTime = startTime.startOf('minute').valueOf()
            if (config.startTimeTarget == 5) realStartTime = startTime.startOf('second').valueOf()
            if (config.startTimeTarget == 6) realStartTime = startTime.valueOf()
            cur.startTime = realStartTime
          }
        }
        if (config.endTimeRule) {
          if (config.endTimeType == 1) cur.endTime = config.endTimeValue
          if (config.endTimeType == 3) cur.endTime = new Date().getTime()
          if (config.endTimeType == 4 || config.endTimeType == 5) {
            const type = getTimeUnit(config.endTimeTarget)
            const method = config.endTimeType == 4 ? 'subtract' : 'add'
            const endTime = dayjs()[method](config.endTimeValue, type)
            let realEndTime = endTime.endOf('day').valueOf()
            if (config.endTimeTarget == 4) realEndTime = endTime.endOf('minute').valueOf()
            if (config.endTimeTarget == 5) realEndTime = endTime.endOf('second').valueOf()
            if (config.endTimeTarget == 6) realEndTime = endTime.valueOf()
            cur.endTime = realEndTime
          }
        }
      }
      if (config.ceriKey === 'timePicker') {
        if (config.startTimeRule) {
          if (config.startTimeType == 1) cur.startTime = config.startTimeValue || null
          if (config.startTimeType == 3) cur.startTime = dayjs().format(cur.format)
          if (config.startTimeType == 4 || config.startTimeType == 5) {
            const type = getTimeUnit(config.startTimeTarget + 3)
            const method = config.startTimeType == 4 ? 'subtract' : 'add'
            const startTime = dayjs()[method](config.startTimeValue, type).format(cur.format)
            cur.startTime = startTime
          }
        }
        if (config.endTimeRule) {
          if (config.endTimeType == 1) cur.endTime = config.endTimeValue || null
          if (config.endTimeType == 3) cur.endTime = dayjs().format(cur.format)
          if (config.endTimeType == 4 || config.endTimeType == 5) {
            const type = getTimeUnit(config.endTimeTarget + 3)
            const method = config.endTimeType == 4 ? 'subtract' : 'add'
            const endTime = dayjs()[method](config.endTimeValue, type).format(cur.format)
            cur.endTime = endTime
          }
        }
      }
    }
  }
  loop(state.columns)
}
async function initViewList(currentId = '') {
  await getViewList(searchInfo.menuId, userStore.userid).then((res) => {
    const columns: any[] = unref(getChildTableStyle) == 2 || state.columnData.type == 4 ? state.columns : state.complexColumns
    const searchList: any[] = state.searchSchemas.map((o) => ({ label: o.label, id: o.field, show: o.show }))
    const columnList: any[] = columns.map((o) => ({ label: o.label, id: o.prop, show: true, fixed: o.fixed || 'none', labelI18nCode: o.labelI18nCode }))
    state.viewList = (res.data || []).map((o) => {
      if (o.type == 0) return { ...o, searchList, columnList }
      return { ...o, searchList: o.searchList ? JSON.parse(o.searchList) : [], columnList: o.columnList ? JSON.parse(o.columnList) : [] }
    })
    if (currentId) {
      state.currentView = state.viewList.filter((o) => o.id === currentId)[0] || state.viewList[0]
    } else {
      state.currentView = state.viewList.filter((o) => o.status === 1)[0] || state.viewList[0]
    }
  })
}
function handleViewClick(item) {
  state.currentView = item
}
function setListValue(data: any[] = [], defaultData: any[] = [], key) {
  let list: any[] = []
  for (let i = 0; i < data.length; i++) {
    for (let j = 0; j < defaultData.length; j++) {
      if (data[i].show && data[i].id == defaultData[j][key]) list.push(defaultData[j])
    }
  }
  return list
}

onMounted(() => {
  init()
})
</script>
