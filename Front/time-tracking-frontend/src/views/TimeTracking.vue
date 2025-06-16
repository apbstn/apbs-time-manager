<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2>Time Tracking</h2>
        <Button type="button" icon="pi pi-refresh" label="Refresh" @click="refreshData" class="add-button" />
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
                class="add-button" />
              
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
              <Tag :value="displayStatus(data.TM_TYPE)" :severity="getSeverity(data.TM_TYPE)" class="badge" />
            </template>
            <template #filter="{ filterModel }">
              <Select v-model="filterModel.value" :options="statusOptions" optionLabel="label" optionValue="value"
                placeholder="Select Status" showClear>
                <template #option="slotProps">
                  <Tag :value="slotProps.option.label" :severity="getSeverity(slotProps.option.value)" />
                </template>
                <template #value="slotProps">
                  <Tag v-if="slotProps.value !== null && slotProps.value !== undefined"
                    :value="displayStatus(slotProps.value)" :severity="getSeverity(slotProps.value)" />
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
  </div>
</template>

<script>
import { ref } from 'vue';
import { useTimeTracking } from '../layout/composables/useTimeTracking';
import { FilterMatchMode, FilterOperator } from '@primevue/core/api';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import IconField from 'primevue/iconfield';
import InputIcon from 'primevue/inputicon';
import DatePicker from 'primevue/datepicker';
import Select from 'primevue/select';
import Tag from 'primevue/tag';
import InputNumber from 'primevue/inputnumber';

export default {
  components: {
    DataTable,
    Column,
    Button,
    InputText,
    IconField,
    InputIcon,
    DatePicker,
    Select,
    Tag,
    InputNumber
  },
  setup() {
    const { initialize, fetchTimeLogs, getStatusPhrase, formatDate, formatDuration, getState } = useTimeTracking();
    const filters = ref();
    const loading = ref(false);
    const statuses = ref(['start', 'pause', 'stop']);
    const statusOptions = ref([
      { label: 'Start', value: 0 },
      { label: 'Pause', value: 1 },
      { label: 'Stop', value: 2 }
    ]);

    const initFilters = () => {
      filters.value = {
        global: { value: null, matchMode: FilterMatchMode.CONTAINS },
        TM_TIME: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.DATE_IS }] },
        TM_TYPE: { operator: FilterOperator.OR, constraints: [{ value: null, matchMode: FilterMatchMode.EQUALS }] },
        TM_TOTALHOURS: { operator: FilterOperator.AND, constraints: [{ value: null, matchMode: FilterMatchMode.GREATER_THAN_OR_EQUAL_TO }] }
      };
    };

    initFilters();

    return {
      initialize,
      fetchTimeLogs,
      getStatusPhrase,
      formatDate,
      formatDuration,
      getState,
      filters,
      loading,
      statuses,
      statusOptions,
      initFilters
    };
  },
  data() {
    return {
      localState: {
        currentStatus: 'stop',
        statusMessage: '',
        timeLogs: []
      }
    };
  },
  computed: {
    sortedTimeLogs() {
      return [...this.localState.timeLogs].sort((a, b) => {
        const dateA = new Date(a.TM_TIME);
        const dateB = new Date(b.TM_TIME);
        return dateB - dateA; // Most recent first
      });
    },
    filteredTimeLogs() {
      let logs = this.sortedTimeLogs;

      // Apply date filter if active
      const dateFilter = this.filters?.TM_TIME?.constraints?.[0];
      if (dateFilter?.value) {
        const filterDate = new Date(dateFilter.value);
        logs = logs.filter(log => {
          const logDate = new Date(log.TM_TIME);
          return logDate.toDateString() === filterDate.toDateString();
        });
      }

      return logs;
    }
  },
 methods: {
  displayStatus(type) {
    return type === 0 ? 'start' : type === 1 ? 'pause' : 'stop';
  },
  getSeverity(status) {
    switch (status) {
      case 0:
      case 'start':
        return 'success';
      case 1:
      case 'pause':
        return 'warning';
      case 2:
      case 'stop':
        return 'danger';
      default:
        return null;
    }
  },
  clearFilter() {
    this.initFilters();
  },
  async mounted() {
    this.loading = true;
    try {
      const initialState = await this.initialize();
      this.localState.timeLogs = initialState.timeLogs; // Explicitly set timeLogs
    } catch (error) {
      console.error('Error loading time tracking data:', error);
    }
    this.loading = false;
  },
  async refreshData() {
    this.loading = true;
    try {
      await this.fetchTimeLogs();
      const currentState = this.getState();
      this.localState.timeLogs = currentState.timeLogs; // Sync timeLogs
      this.localState.currentStatus = currentState.currentStatus;
      this.localState.statusMessage = currentState.statusMessage;
    } catch (error) {
      console.error('Error refreshing data:', error);
    }
    this.loading = false;
  }
}
};
</script>
<style scoped>
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

.add-button {
  border-radius: 6px;
  padding: 0.5rem 1rem;
  font-weight: 500;
  transition: background-color 0.2s, transform 0.1s;
  color: #35D300 !important;
  border-color: #35D300 !important;
  background-color: white;
}

.add-button:hover {
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

.badge {
  display: inline-flex;
  align-items: center;
  text-transform: uppercase;
  font-weight: 500;
  font-size: 12px;
  padding: 0.25rem 0.5rem;
  border-radius: 2px;
  color: #000000;
  background-color: white; /* Default background for all badges */
}

.badge::before {
  content: '';
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: 6px;
}

:deep(.p-tag-success).badge::before {
  background: #35D300;
}

:deep(.p-tag-warning).badge::before {
  background: #ffaa00;
}

:deep(.p-tag-danger).badge::before {
  background: #ff0000; /* Red circle for STOP */
}

:deep(.p-tag-success, .p-tag-warning, .p-tag-danger) {
  background: none !important; /* Remove default PrimeVue backgrounds */
  padding: 0 !important;
}

/* Override PrimeVue danger background to white */
:deep(.p-tag-danger) {
  background-color: white !important;
  color: #000000; /* Ensure text is readable */
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
