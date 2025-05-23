<template>
  <div class="time-tracking">
    <div class="header">
      <h2>Time Tracking</h2>
      <button @click="fetchTimeLogs">Refresh</button>
    </div>
    <div class="status">
      <h3>{{ getStatusPhrase() }}</h3>
      <p v-if="localState.statusMessage">{{ localState.statusMessage }}</p>
    </div>
    <div class="logs">
      <h3>Time Logs ({{ localState.timeLogs.length }})</h3>
      <p v-if="!localState.timeLogs.length">No logs yet. Start tracking!</p>
      <ul v-else>
        <li v-for="log in localState.timeLogs" :key="log.tm_Id">
          <span>Log: {{ log.tm_Id }}</span>
          <span>Time: {{ formatDate(log.TM_TIME) }}</span>
          <span>Status: {{ displayStatus(log.TM_TYPE) }}</span>
          <span>Hours: {{ formatDuration(log.TM_TOTALHOURS) }}</span>
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
import { useTimeTracking } from '../layout/composables/useTimeTracking';

export default {
  setup() {
    const { initialize, fetchTimeLogs, startTracking, pauseTracking, stopTracking, getStatusPhrase, formatDate, formatDuration, getState } = useTimeTracking();
    return { initialize, fetchTimeLogs, startTracking, pauseTracking, stopTracking, getStatusPhrase, formatDate, formatDuration, getState };
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
  methods: {
    displayStatus(type) {
      console.log('Displaying TM_TYPE:', type);
      return type === 0 ? 'start' : type === 1 ? 'pause' : 'stop';
    },
    async fetchTimeLogs() {
      const updatedState = await this.fetchTimeLogs();
      this.localState = { ...updatedState };
    },
    async startTracking() {
      const updatedState = await this.startTracking();
      this.localState = { ...updatedState };
    },
    async pauseTracking() {
      const updatedState = await this.pauseTracking();
      this.localState = { ...updatedState };
    },
    async stopTracking() {
      const updatedState = await this.stopTracking();
      this.localState = { ...updatedState };
    }
  },
  async mounted() {
    const initialState = await this.initialize();
    this.localState = { ...initialState };
  }
};
</script>

<style scoped>
.time-tracking { padding: 1rem; }
.header { display: flex; justify-content: space-between; margin-bottom: 1rem; }
.status { margin-bottom: 1rem; }
.logs ul { list-style: none; padding: 0; }
.logs li { margin-bottom: 0.5rem; }
</style>