import { ref } from 'vue';
import api from '@/api';

export function useTimeTracking() {
  let state = {
    currentStatus: 'stop',
    statusMessage: '',
    userId: 'guest',
    timeLogs: []
  };

  const showStart = ref(true);
  const showStop = ref(false);
  const showPause = ref(false);

  const getAccountId = async (email) => {
    try {
      const response = await api.post('/api/UserTenants/get-id-by-email', email);
      console.log('Account ID fetched:', response.data);
      return response.data;
    } catch (error) {
      console.error('Error fetching account ID:', error);
      throw error;
    }
  };

  const initializeUserId = async () => {
    const email = localStorage.getItem('email');
    console.log('Email from localStorage:', email);
    if (email) {
      try {
        const accountId = await getAccountId(email);
        state.userId = accountId || 'guest';
        console.log('Set state.userId to:', state.userId);
      } catch (error) {
        console.warn('Failed to fetch account ID, using default userId:', state.userId);
      }
    } else {
      console.warn('No email found, using default userId:', state.userId);
    }
    return state.userId;
  };

  const formatDate = (date) => {
    return date ? new Date(date).toISOString().split('T')[0] : 'No Date';
  };

  const formatDuration = (totalHours) => {
    if (!totalHours || totalHours === 'null' || totalHours === '00:00:00') {
      return 'N/A';
    }
    const [h = 0, m = 0, s = 0] = totalHours.split(':').map(Number);
    if (isNaN(h) || isNaN(m) || isNaN(s)) {
      return 'Invalid Format';
    }
    const roundedSeconds = Math.floor(s);
    return `${h}h ${m}m ${roundedSeconds}s`;
  };

  const getStatusPhrase = () => {
    const username = localStorage.getItem('username') || 'User';
    const phrases = {
      start: `${username}, you're tracking time.`,
      pause: `${username}, tracking is paused.`,
      stop: `${username}, not tracking right now.`,
      loading: `${username}, loading your time logs...`,
      error: `${username}, something went wrong.`,
    };
    return phrases[state.currentStatus] || 'Status unknown.';
  };

  const updateButtonVisibility = () => {
    if (state.timeLogs.length > 0) {
      const latestLog = state.timeLogs[state.timeLogs.length - 1];
      const latestType = latestLog.TM_TYPE;
      showStart.value = latestType !== 0; // Show Start if not started (TM_TYPE is 1 or 2)
      showStop.value = latestType === 0 && latestLog.TM_ACTIV; // Show Stop only if active
      showPause.value = latestType === 0 && latestLog.TM_ACTIV; // Show Pause only if active
      console.log('Updated button visibility - Latest TM_TYPE:', latestType, 'showStart:', showStart.value, 'showStop:', showStop.value, 'showPause:', showPause.value, 'at:', new Date().toLocaleString());
    } else {
      showStart.value = true;
      showStop.value = false;
      showPause.value = false;
      console.log('No logs, setting default visibility - showStart:', showStart.value, 'showStop:', showStop.value, 'showPause:', showPause.value, 'at:', new Date().toLocaleString());
    }
  };

  const fetchTimeLogs = async () => {
    await initializeUserId();
    state.currentStatus = 'loading';
    console.log('Fetching logs for userId:', state.userId, 'at:', new Date().toLocaleString());
    try {
      const res = await api.get(`/api/timelog/${state.userId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      console.log('Full API response at:', new Date().toLocaleString(), ':', JSON.stringify(res.data, null, 2));
      const logs = Array.isArray(res.data) ? res.data : [];
      state.timeLogs = logs.map(log => {
        const rawType = log.type !== undefined ? log.type : log.TM_TYPE;
        const rawHours = log.totalHours || log.tm_TotalHours || log.TM_TOTALHOURS;
        console.log('Mapping log - raw type:', rawType, 'raw hours:', rawHours, 'for log:', log);
        return {
          tm_Id: log.tm_Id || log.TM_ID,
          TM_TIME: log.tm_Time || log.TM_TIME || log.time,
          TM_TOTALHOURS: rawHours || '00:00:00',
          TM_TYPE: Number(rawType),
          TM_ACTIV: log.activ !== undefined ? log.activ : log.TM_ACTIV || false
        };
      });

      if (state.timeLogs.length > 0) {
        const latestLog = state.timeLogs[state.timeLogs.length - 1];
        const latestType = latestLog.TM_TYPE;
        state.currentStatus = latestType === 0 ? 'start' : latestType === 1 ? 'pause' : 'stop';
        updateButtonVisibility();
      } else {
        state.currentStatus = 'stop';
        updateButtonVisibility();
      }
    } catch (err) {
      console.error('Error fetching time logs at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Failed to fetch logs.';
      state.currentStatus = 'error';
      updateButtonVisibility();
    }
    console.log('Post-fetch state - showStart:', showStart.value, 'showStop:', showStop.value, 'showPause:', showPause.value, 'at:', new Date().toLocaleString());
    return state;
  };

  const startTracking = async () => {
    await initializeUserId();
    state.currentStatus = 'start';
    console.log('Starting tracking for userId:', state.userId, 'at:', new Date().toLocaleString());
    try {
      const res = await api.post(`/api/timelog/start/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = res.data || 'Started tracking.';
      console.log('Start response at:', new Date().toLocaleString(), ':', res.data);
      await new Promise(resolve => setTimeout(resolve, 2000));
      await fetchTimeLogs();
    } catch (err) {
      console.error('Start tracking failed at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Start tracking failed.';
      state.currentStatus = 'error';
      updateButtonVisibility();
    }
    return state;
  };

  const pauseTracking = async () => {
    await initializeUserId();
    state.currentStatus = 'pause';
    console.log('Pausing tracking for userId:', state.userId, 'at:', new Date().toLocaleString());
    try {
      const res = await api.post(`/api/timelog/pause/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = res.data || 'Paused tracking.';
      console.log('Pause response at:', new Date().toLocaleString(), ':', res.data);
      await new Promise(resolve => setTimeout(resolve, 2000));
      await fetchTimeLogs(); // Refresh logs to update state
    } catch (err) {
      console.error('Pause tracking failed at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Pause tracking failed.';
      state.currentStatus = 'error';
      updateButtonVisibility();
    }
    return state;
  };

  const stopTracking = async () => {
    await initializeUserId();
    state.currentStatus = 'stop';
    console.log('Stopping tracking for userId:', state.userId, 'at:', new Date().toLocaleString());
    try {
      const res = await api.post(`/api/timelog/stop/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = res.data || 'Stopped tracking.';
      console.log('Stop response at:', new Date().toLocaleString(), ':', res.data);
      await new Promise(resolve => setTimeout(resolve, 2000));
      await fetchTimeLogs();
    } catch (err) {
      console.error('Stop tracking failed at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Stop tracking failed.';
      state.currentStatus = 'error';
      updateButtonVisibility();
    }
    return state;
  };

  const initialize = async () => {
    await initializeUserId();
    const initialState = await fetchTimeLogs();
    console.log('Initialization complete - showStart:', showStart.value, 'showStop:', showStop.value, 'showPause:', showPause.value, 'at:', new Date().toLocaleString());
    return initialState;
  };

  return {
    initialize,
    fetchTimeLogs,
    startTracking,
    pauseTracking,
    stopTracking,
    getStatusPhrase,
    formatDate,
    formatDuration,
    getState: () => state,
    showStart,
    showStop,
    showPause
  };
}