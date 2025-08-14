<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2 style="font-size: 22px; color: #6B7280;">Time Tracking</h2>
        <div class="flex align-items-center gap-2">
          <Button type="button" icon="pi pi-plus" label="Enter Manual" @click="openManualDialog" class="refresh-button" v-tooltip.top="{
                  value: 'Enter Manual',
                  pt: {
                    arrow: { style: { borderBottomColor: '#000000' } },
                    text: '!bg-black !text-white !font-medium'
                  }
                }"/>
          <Button type="button" icon="pi pi-refresh" label="Refresh" @click="refreshData" class="refresh-button" v-tooltip.top="{
                  value: 'Refresh',
                  pt: {
                    arrow: { style: { borderBottomColor: '#000000' } },
                    text: '!bg-black !text-white !font-medium'
                  }
                }"/>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="status">
        <h3>{{ getStatusPhrase() }}</h3>
        <p v-if="localState.statusMessage">{{ localState.statusMessage }}</p>
      </div>
      <div class="logs">
        <InputText v-model="filters['global'].value" placeholder="Search time logs..." class="search-input" />
        <DataTable v-model:filters="filters" :value="filteredTimeLogs" paginator showGridlines :rows="10"
          dataKey="tm_Id" filterDisplay="menu" :loading="loading"
          :globalFilterFields="['TM_TIME', 'TM_TYPE', 'TM_TOTALHOURS']">
          <template #header>
            <div class="flex justify-content-between">
              <Button type="button" icon="pi pi-filter-slash" label="Clear" outlined @click="clearFilter"
                class="refresh-button" v-tooltip="{
                  value: 'Clear',
                  pt: {
                    arrow: { style: { borderBottomColor: '#000000' } },
                    text: '!bg-black !text-white !font-medium'
                  }
                }"/>
            </div>
          </template>
          <template #empty>
            <div class="text-center text-muted">No logs yet. Start tracking!</div>
          </template>
          <template #loading>
            <div class="text-center text-muted">Loading time logs. Please wait.</div>
          </template>
          <Column field="TM_TIME" header="Time" style="min-width: 12rem" filterField="TM_TIME" dataType="date"
            :showFilterMatchModes="false">
            <template #body="{ data }">
              {{ formatDate(data.TM_TIME) }}
            </template>
            <template #filter="{ filterModel }">
              <DatePicker v-model="filterModel.value" dateFormat="mm/dd/yy" placeholder="Select Date" showIcon />
            </template>
          </Column>
          <Column field="TM_TYPE" header="Status" style="min-width: 10rem" filterField="TM_TYPE">
            <template #body="{ data }">
              <Tag :value="displayStatus(data.TM_TYPE)" :severity="getSeverity(data.TM_TYPE)" class="status-badge" />
            </template>
            <template #filter="{ filterModel }">
              <Select v-model="filterModel.value" :options="statusOptions" optionLabel="label" optionValue="value"
                placeholder="Select Status" showClear>
                <template #option="slotProps">
                  <Tag :value="slotProps.option.label" :severity="getSeverity(slotProps.option.value)" class="status-badge" />
                </template>
                <template #value="slotProps">
                  <Tag v-if="slotProps.value !== null && slotProps.value !== undefined"
                    :value="displayStatus(slotProps.value)" :severity="getSeverity(slotProps.value)" class="status-badge" />
                </template>
              </Select>
            </template>
          </Column>
          <Column field="TM_TOTALHOURS" header="Hours" style="min-width: 10rem" filterField="TM_TOTALHOURS"
            dataType="numeric">
            <template #body="{ data }">
              {{ formatDuration(data.TM_TOTALHOURS) }}
            </template>
            <template #filter="{ filterModel }">
              <InputNumber v-model="filterModel.value" placeholder="Filter by hours" />
            </template>
          </Column>
        </DataTable>
      </div>
    </div>

    <!-- Manual Entry Dialog -->
    <!-- Manual Entry Dialog -->
<Dialog v-model:visible="manualDialogVisible" header="Enter Manual Log" class="stunning-dialog" style="width: 650px !important;" :modal="true">
    <Divider class="dialog-divider" />
    <p class="dialog-subtitle">Enter Manual Time Log Details Below</p>

    <div class="p-fluid form-container">
        <div class="field mb-4">
            <label for="manualDate" class="field-label">Date and Time</label>
            <DatePicker id="manualDate"
            style="width: 570px !important;"
                v-model="manualDate"
                
                showTime
                hourFormat="24"
                dateFormat="yy-mm-dd"
                placeholder="Select Date and Time"
                showIcon
            />
        </div>
        <div class="field mb-4">
            <label for="manualType" class="field-label">Type</label>
            <Select id="manualType"
            style="width: 570px !important;"
                v-model="manualType"
                :options="statusOptions"
                optionLabel="label"
                optionValue="value"
                placeholder="Select Type"
            />
        </div>
    </div>

    <Divider />

    <template #footer>
        <Button class="add-button1" label="Cancel" icon="pi pi-times" @click="manualDialogVisible = false" text />
        <Button class="add-button" label="Save" icon="pi pi-check" @click="saveManualLog" :disabled="!manualDate || manualType === null" />
    </template>
</Dialog>

  </div>
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useTimeTracking } from '../layout/composables/useTimeTracking';
import { FilterMatchMode, FilterOperator } from '@primevue/core/api';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import DatePicker from 'primevue/datepicker';
import Select from 'primevue/select';
import Tag from 'primevue/tag';
import InputNumber from 'primevue/inputnumber';
import Dialog from 'primevue/dialog';
import api from '@/api';

// Import components
const components = { DataTable, Column, Button, InputText, DatePicker, Select, Tag, InputNumber, Dialog };

// Use TimeTracking composable
const { initialize, fetchTimeLogs, getStatusPhrase, formatDate, formatDuration, getState } = useTimeTracking();

const filters = ref();
const loading = ref(false);
const statuses = ref(['start', 'pause', 'stop']);
const statusOptions = ref([
  { label: 'Start', value: 0 },
  { label: 'Pause', value: 1 },
  { label: 'Stop', value: 2 }
]);
const localState = ref({
  currentStatus: 'stop',
  statusMessage: '',
  timeLogs: []
});

const manualDialogVisible = ref(false);
const manualDate = ref(null);
const manualType = ref(null);

const initFilters = () => {
  filters.value = {
    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
    TM_TIME: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.DATE_IS }] },
    TM_TYPE: { operator: FilterOperator.OR, constraints: [{ value: null, matchMode: FilterMatchMode.EQUALS }] },
    TM_TOTALHOURS: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.GREATER_THAN_OR_EQUAL_TO }] }
  };
};

initFilters();

onMounted(async () => {
  loading.value = true;
  try {
    await fetchTimeLogs();
    const currentState = getState();
    localState.value.timeLogs = currentState.timeLogs;
    localState.value.currentStatus = currentState.currentStatus;
    localState.value.statusMessage = currentState.statusMessage;
    console.log('Loaded time logs:', localState.value.timeLogs);
  } catch (error) {
    console.error('Error loading time tracking data:', error);
    alert('Failed to load time logs');
  }
  loading.value = false;
});

// Computed properties
const sortedTimeLogs = computed(() => {
  return [...localState.value.timeLogs].sort((a, b) => {
    const dateA = new Date(a.TM_TIME);
    const dateB = new Date(b.TM_TIME);
    return dateB - dateA;
  });
});

const filteredTimeLogs = computed(() => {
  let logs = sortedTimeLogs.value;

  const dateFilter = filters.value?.TM_TIME?.constraints?.[0];
  if (dateFilter?.value) {
    const filterDate = new Date(dateFilter.value);
    logs = logs.filter(log => {
      const logDate = new Date(log.TM_TIME);
      return logDate.toDateString() === filterDate.toDateString();
    });
  }

  return logs;
});

// Methods
const displayStatus = (type) => {
  return type === 0 ? 'start' : type === 1 ? 'pause' : 'stop';
};

const getSeverity = (status) => {
  switch (status) {
    case 0:
      return 'success';
    case 1:
      return 'warn';
    case 2:
      return 'danger';
    default:
      return null;
  }
};

const clearFilter = () => {
  initFilters();
};

const refreshData = async () => {
  loading.value = true;
  try {
    await fetchTimeLogs();
    const currentState = getState();
    localState.value.timeLogs = currentState.timeLogs;
    localState.value.currentStatus = currentState.currentStatus;
    localState.value.statusMessage = currentState.statusMessage;
  } catch (error) {
    console.error('Error refreshing data:', error);
    alert('Failed to refresh time logs');
  }
  loading.value = false;
};

const openManualDialog = () => {
  manualDialogVisible.value = true;
  manualDate.value = null;
  manualType.value = null;
};

const saveManualLog = async () => {
  if (!manualDate.value || manualType.value === null) {
    alert('Date and Type are required');
    return;
  }

  const typeMap = {
    0: 'PE',
    1: 'P',
    2: 'PS'
  };

  const dto = {
    Time: manualDate.value.toISOString(),
    Type: typeMap[manualType.value]
  };
  const email = localStorage.getItem('email'); // TODO: Replace with actual email from auth context
  const id = await api.post('/api/UserTenants/get-id-by-email', email);
  const accountId = id.data; // TODO: Replace with actual accountId from auth context
  // Replace with actual accountId (e.g., from auth context or composable)
 // TODO: Get from auth context

  try {
    console.log('Manual log response:', dto);
    const accessToken = localStorage.getItem('accessToken'); 
    if (!accessToken) {
      console.log('No access token found');
    }

    const responsee = await api.post(`/api/timelog/manual/add/${accountId}`, dto,{
      headers: {
                Authorization: `Bearer ${accessToken}`
            }
    });
    
    console.log('Manual log response:', dto);
    console.log(responsee.data);
    console.log(responsee);

    const responseData = await responsee.data;
    console.log('Response status:', responsee.status, 'Response data:', responseData);

    if (responseData === "Log added successfully.") {
      console.log('==========================');
      await refreshData();
      console.log('==========================');
      manualDialogVisible.value = false;
      console.log('==========================');
      alert('Manual log added successfully');
    } else {
      const error = await responsee.data;
      alert(`Failed to add manual log: ${error.exception?.message || 'Unknown error'}`);
    }
  } catch (error) {
    console.error('Unexpected error saving manual log:', error);
    alert('Unexpected error adding manual log');
  }
};
</script>

<style scoped>
/* Dialog container styling */
.stunning-dialog {
    width: 90%;
    max-width: 650px;
    border-radius: 12px;
    box-shadow: 0 8px 24px rgba(0, 0, 0, 0.2);
    background: #ffffff;
}

/* Divider spacing */
.dialog-divider {
    margin: 1rem 0;
}

/* Subtitle styling */
.dialog-subtitle {
    margin: 0 0 1rem 0;
    color: #4b5563;
    font-size: 0.95rem;
    font-weight: 400;
    text-align: center;
}

/* Form container background and padding */
.form-container {
    padding: 1.5rem;
    background: #f9fafb;
    border-radius: 8px;
}

/* Each field block */
.field {
    margin-bottom: 1.5rem;
}

/* Label styling */
.field-label {
    font-weight: 600;
    font-size: 1rem;
    color: #1f2a44;
    margin-bottom: 0.5rem;
    display: block;
    text-align: left;
}

/* Make inputs full width */
:deep(.p-inputtext),
:deep(.p-calendar),
:deep(.p-dropdown) {
    width: 100% !important;
    border-radius: 6px;
    border: 1px solid #ced4da;
    transition: border-color 0.2s, box-shadow 0.2s;
}

/* Input focus effects */
:deep(.p-inputtext:focus),
:deep(.p-calendar:focus),
:deep(.p-dropdown:focus) {
    border-color: #6366f1;
    box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.2);
}



/* Existing styles remain unchanged */
.p-field {
  margin-bottom: 1rem;
  align-items: center;
}

.p-field label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  align-items: center !important;
}



.header-container {
  margin-bottom: 1.5rem;
}

.flex {
  display: flex;
}

.justify-content-between {
  justify-content: space-between;
}

.align-items-center {
  align-items: center;
}

.add-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #35D300 !important;
  border-color: #35D300 !important;
  background-color: rgba(255, 255, 255, 0);
}

.add-button:hover {
  transform: translateY(-1px);
  background-color: #35D300 !important;
  color: white !important;
  border-color: white !important;
}

.add-button1 {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #ff0000 !important;
  border-color: #ff0000 !important;
  background-color: white;
}

.add-button1:hover {
  transform: translateY(-1px);
  background-color: #ff0000 !important;
  color: white !important;
  border-color: white !important;
}



.search-input {
  border: 1px solid #ced4da;
  border-radius: 6px;
  width: 300px;
  margin-bottom: 1rem;
  padding: 0.5rem;
  transition: border-color 0.2s;
}

.search-input:focus {
  border-color: #6366f1;
  box-shadow: 0 0 0 3px rgba(99, 102, 241, 0.2);
}

h2 {
  font-size: 2rem;
  margin: 0;
}

h3 {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0.5rem 0;
  color: #000000;
}

.card {
  margin-top: 1rem;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  background: #ffffff;
  padding: 1.5rem;
}

.refresh-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #35D300 !important;
  border-color: #35D300 !important;
  background-color: rgba(255, 255, 255, 0);
}

.refresh-button:hover {
  transform: translateY(-1px);
  background-color: #35D300 !important;
  color: white !important;
  border-color: white !important;
}

.status {
  margin-bottom: 1.5rem;
  padding: 1rem;
  background: #e6f0ff;
  border-radius: 8px;
  border-left: 4px solid #35D300;
}

.logs {
  margin-top: 1.5rem;
}

.text-center {
  text-align: center;
}

.text-muted {
  color: #6b7280;
}

/* Status badge styling */
.status-badge {
  display: inline-flex !important;
  align-items: center !important;
  text-transform: uppercase;
  font-weight: 500;
  font-size: 12px;
  padding: 0.25rem 0.5rem;
  border-radius: 2px;
  color: #000000;
  background-color: white !important; /* Force white background */
}

.status-badge::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: 6px;
}

/* Specific colors for each severity */
.logs :deep(.p-tag-success).status-badge::before {
  background: #35D300 !important; /* Green for Start */
}

.logs :deep(.p-tag-warn).status-badge::before {
  background: #ff8000 !important; /* Orange for Pause */
}

.logs :deep(.p-tag-danger).status-badge::before {
  background: #ff0000 !important; /* Red for Stop */
}

/* Remove PrimeVue default backgrounds and ensure custom styling */
.logs :deep(.p-tag) {
  background: none !important;
  padding: 0 !important;
}

/* Ensure text color and background consistency */
.logs :deep(.p-tag-danger) {
  background-color: white !important;
  color: #000000 !important;
}

:deep(.p-inputtext) {
  border: 1px solid #ced4da;
  border-radius: 6px;
  padding: 0.5rem;
  transition: border-color 0.2s;
}

:deep(.p-inputtext:focus) {
  border-color: #35D300 !important;
  box-shadow: 0 0 0 3px #35D300 !important;
}
</style>