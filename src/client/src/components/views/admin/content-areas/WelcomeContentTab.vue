<template>
  <div class="welcome-content-tab">
    <DataTable
      :value="welcomeSettings.tabs"
      :rows="10"
      reorderable-rows
      @row-reorder="onRowReorder"
      paginator>
      <template #empty>
        No data to display.
      </template>
      <Column row-reorder />
      <Column
        field="title"
        header="Title" />
      <Column class="grid-action-col grid-action-col--2">
        <template #body="{ data }: { data: TabContent }">
            <GridActions
              :disabled="isSubmittingReorder || isSubmittingDelete"
              @edit="handleEdit(data)"
              @delete="handleDelete(data)"
              show-edit
              show-delete />
        </template>
      </Column>
      <template #paginatorstart>
        <Button label="Create Tab" @click="handleCreate" />
      </template>
    </DataTable>
    <FormDrawer
      v-model:visible="showFormDrawer"
      :is-submitting="welcomeSettingsTabFormRef?.isSubmitting"
      :title="editingTab === undefined ? 'Create Tab' : 'Edit Tab'"
      @reset="welcomeSettingsTabFormRef?.reset()"
      @submit="welcomeSettingsTabFormRef?.submit()">
      <WelcomeSettingsTabForm
        ref="welcomeSettingsTabFormRef"
        @submitted="showFormDrawer = false"
        :tab-title="editingTab?.title" />
      <Divider>Preview</Divider>
      <Tabs value="preview">
        <TabList>
          <Tab value="preview">{{ welcomeSettingsTabFormRef?.state.title }}</Tab>
        </TabList>
        <TabPanels>
          <TabPanel value="preview">
            <MarkdownContent :content="welcomeSettingsTabFormRef?.state.body ?? ''" />
          </TabPanel>
        </TabPanels>
      </Tabs>
    </FormDrawer>
  </div>
</template>

<script setup lang="ts">
import { useWelcomeSettingsStore } from '@/stores/settings/welcomeSettingsStore.ts'
import type { TabContent } from '@/api/resources/settingsApi.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { type DataTableRowReorderEvent, Divider, Tabs, TabList, Tab, TabPanels, TabPanel, DataTable, Column, Button } from 'primevue'
import FormDrawer from '@/components/form/FormDrawer.vue'
import WelcomeSettingsTabForm from '@/components/form/requestForms/WelcomeSettingsTabForm.vue'
import { type Ref, ref, useTemplateRef } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { notEmptyArray } from '@/validation/rules/arrayRules.ts'
import MarkdownContent from "@/components/controls/MarkdownContent.vue";

const welcomeSettings = useWelcomeSettingsStore()
const welcomeSettingsTabFormRef = useTemplateRef('welcomeSettingsTabFormRef')

const isSubmittingReorder = ref(false)
const onRowReorder = async (event: DataTableRowReorderEvent) => {
  const formRef = ref({ tabs: event.value })
  const validation = createFormValidation(formRef, {
    tabs: notEmptyArray()
  })
  isSubmittingReorder.value = true
  await welcomeSettings.update(formRef, validation)
  isSubmittingReorder.value = false
}

const showFormDrawer = ref(false)
const editingTab: Ref<TabContent | undefined> = ref(undefined)
const handleEdit = (tab: TabContent) => {
  editingTab.value = tab
  welcomeSettingsTabFormRef.value?.reset()
  showFormDrawer.value = true
}
const handleCreate = () => {
  editingTab.value = undefined
  welcomeSettingsTabFormRef.value?.reset()
  showFormDrawer.value = true
}

const isSubmittingDelete = ref(false)
const deletingTab: Ref<TabContent | undefined> = ref(undefined)
const handleDelete = async (tab: TabContent) => {
  deletingTab.value = tab
  const tabsCopy = welcomeSettings.tabs.filter((t) => t.title !== tab.title)
  const formRef = ref({ tabs: tabsCopy })
  const validation = createFormValidation(formRef, {
    tabs: notEmptyArray()
  })
  isSubmittingDelete.value = true
  await welcomeSettings.update(formRef, validation)
  isSubmittingDelete.value = false
}
</script>
