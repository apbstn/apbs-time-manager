<template>
  <div class="time-tracking">
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2>Time Tracking</h2>
      </div>
    </div>

    <div class="card">
      <div class="status">
        <h3>{{ getStatusPhrase() }}</h3>
        <p v-if="localState.statusMessage">{{ localState.statusMessage }}</p>
      </div>
      <div class="logs">
        <div class="flex justify-content-between align-items-center">
          <h3>Time Logs ({{ localState.timeLogs.length }})</h3>
          <Button type="button" icon="pi pi-refresh" label="Refresh" @click="refreshData" outlined />
        </div>
        <DataTable v-model:filters="filters" :value="filteredTimeLogs" paginator showGridlines :rows="10"
          dataKey="tm_Id" filterDisplay="menu" :loading="loading"
          :globalFilterFields="['TM_TIME', 'TM_TYPE', 'TM_TOTALHOURS']">
          <template #header>
            <div class="flex justify-content-between">
              <Button type="button" icon="pi pi-filter-slash" label="Clear" outlined @click="clearFilter" />
              <IconField>
                <InputIcon>
                  <i class="pi pi-search" />
                </InputIcon>
                <InputText v-model="filters['global'].value" placeholder="Keyword Search" />
              </IconField>
            </div>
          </template>
          <template #empty> No logs yet. Start tracking! </template>
          <template #loading> Loading time logs. Please wait. </template>
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
              <Tag :value="displayStatus(data.TM_TYPE)" :severity="getSeverity(data.TM_TYPE)" class="badge"/>
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
    async refreshData() {
      this.loading = true;
      try {
        await this.fetchTimeLogs();
        const currentState = this.getState();
        console.log('Refreshed state:', currentState);
        this.localState = { ...currentState };
      } catch (error) {
        console.error('Error refreshing data:', error);
      }
      this.loading = false;
    }
  },
  async mounted() {
    this.loading = true;
    try {
      const initialState = await this.initialize();
      console.log('Initial state:', initialState);
      this.localState = { ...initialState };

      // Also try to fetch logs explicitly
      await this.fetchTimeLogs();
      const currentState = this.getState();
      console.log('Current state after fetchTimeLogs:', currentState);
      this.localState = { ...currentState };
    } catch (error) {
      console.error('Error loading time tracking data:', error);
    }
    this.loading = false;
  }
};
</script>
<style scoped>
.time-tracking {
  width: 100%;
  min-height: 100vh;
  padding: 2rem;
  background-color: #f9fafb;
  box-sizing: border-box;
}

.badge {
  display: inline-flex;
  align-items: center;
  text-transform: uppercase;
  font-weight: 500;
  font-size: 12px;
  padding: 0.25rem 0.5rem;
  border-radius: 2px;
  color: #1f2937; /* Text color */
  background: none !important; /* Ensure no background on badge */
}

.badge::before {
  content: '';
  display: inline-block;
  width: 8px;
  height: 8px;
  border-radius: 50%;
  margin-right: 6px; /* Space between circle and text */
}

/* Success (start) */
:deep(.p-tag-success).badge::before {
  background-color: #22c55e; /* Green for success */
}

/* Warning (pause) */
:deep(.p-tag-warning).badge::before {
  background-color: #f59e0b; /* Yellow for warning */
}

/* Danger (stop) */
:deep(.p-tag-danger).badge::before {
  background-color: #ef4444; /* Red for danger */
}

/* Override PrimeVue Tag component background */
:deep(.p-tag-success),
:deep(.p-tag-warning),
:deep(.p-tag-danger) {
  background: none !important; /* Remove PrimeVue Tag background */
  padding: 0; /* Remove default padding to align with badge */
}

.header-container {
  margin-bottom: 2rem;
}

.flex {
  display: flex;
}

.justify-content-between {
  justify-content: space-between;
}

.justify-content-end {
  justify-content: flex-end;
}

.align-items-center {
  align-items: center;
}

.card {
  width: 100%;
  padding: 1.5rem;
  background: linear-gradient(145deg, #ffffff, #f1f5f9);
  border-radius: 10px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  transition: transform 0.2s ease, box-shadow 0.2s ease;
}

.card:hover {
  transform: translateY(-2px);
  box-shadow: 0 6px 16px rgba(0, 0, 0, 0.1);
}

.status {
  margin-bottom: 1.5rem;
  padding: 1rem;
  background-color: #e6f0ff;
  border-radius: 8px;
  border-left: 4px solid #3b82f6;
}

.logs {
  margin-top: 1.5rem;
}

h2 {
  font-size: 2.25rem;
  font-weight: 700;
  margin: 0;
  color: #1f2937;
  letter-spacing: -0.025rem;
}

h3 {
  font-size: 1.5rem;
  font-weight: 600;
  margin: 0.5rem 0;
  color: #1f2937;
}

.text-center {
  text-align: center;
}

.text-muted {
  color: #6b7280;
  font-style: italic;
  font-size: 0.95rem;
}
</style> 
