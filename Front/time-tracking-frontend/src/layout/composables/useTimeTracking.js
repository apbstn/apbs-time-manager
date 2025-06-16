import { ref } from 'vue';
import api from '@/api';

export function useTimeTracking() {
  let state = {
    currentStatus: 'stop',
    statusMessage: '',
    userId: 'guest',
    timeLogs: []
  };

  const showStart = ref(true); // Reactive state for Start button visibility
  const showStop = ref(false); // Reactive state for Stop button visibility

  const decodeJwt = (token) => {
    try {
      if (!token) return null;
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(atob(base64).split('').map(c =>
        '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2)
      ).join(''));
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error('Failed to decode JWT:', error);
      return null;
    }
  };

  const initializeUserId = () => {
    const token = localStorage.getItem('accessToken');
    console.log('Token from localStorage:', token);
    if (token) {
      const decoded = decodeJwt(token);
      if (decoded) {
        state.userId = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || 'guest';
        console.log('Set state.userId to:', state.userId);
      } else {
        console.warn('Failed to decode token, using default userId:', state.userId);
      }
    } else {
      console.warn('No accessToken found, using default userId:', state.userId);
    }
    return state.userId;
  };

  const formatDate = (date) => {
    return date ? new Date(date).toISOString().split('T')[0] : 'No Date';
  };

const formatDuration = (totalHours) => {
  if (!totalHours || totalHours === 'null' || totalHours === '00:00:00') {
    return 'N/A'; // Display 'N/A' if no valid duration
  }
  const [h = 0, m = 0, s = 0] = totalHours.split(':').map(Number);
  if (isNaN(h) || isNaN(m) || isNaN(s)) {
    return 'Invalid Format'; // Handle malformed duration
  }
  const roundedSeconds = Math.floor(s); // Round down seconds to remove decimals
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
      showStop.value = latestType === 0;  // Show Stop if started (TM_TYPE is 0)
      console.log('Updated button visibility - Latest TM_TYPE:', latestType, 'showStart:', showStart.value, 'showStop:', showStop.value, 'at:', new Date().toLocaleString());
    } else {
      // Check if there was a previous active session before filtering
      const hasActiveSession = state.timeLogs.some(log => log.TM_TYPE === 0 && log.TM_ACTIV);
      showStart.value = !hasActiveSession; // Show Start only if no active session
      showStop.value = hasActiveSession;   // Show Stop if an active session exists
      console.log('No logs or filtered out, checking active session - showStart:', showStart.value, 'showStop:', showStop.value, 'at:', new Date().toLocaleString());
    }
  };

  const fetchTimeLogs = async () => {
    initializeUserId();
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
        const rawHours = log.totalHours || log.tm_TotalHours || log.TM_TOTALHOURS; // Try multiple field names
        console.log('Mapping log - raw type:', rawType, 'raw hours:', rawHours, 'for log:', log);
        return {
          tm_Id: log.tm_Id || log.TM_ID,
          TM_TIME: log.tm_Time || log.TM_TIME || log.time,
          TM_TOTALHOURS: rawHours || '00:00:00', // Default to '00:00:00' if null
          TM_TYPE: Number(rawType),
          TM_ACTIV: log.activ !== undefined ? log.activ : log.TM_ACTIV || false // Add TM_ACTIV if available
        };
      });

      if (state.timeLogs.length > 0) {
        const latestLog = state.timeLogs[state.timeLogs.length - 1];
        const latestType = latestLog.TM_TYPE;
        state.currentStatus = latestType === 0 ? 'start' : latestType === 1 ? 'pause' : 'stop';
        updateButtonVisibility(); // Update button visibility based on latest log
      } else {
        // Check unfiltered logs for an active session
        const latestLog = state.timeLogs[state.timeLogs.length - 1];
        if (latestLog && latestLog.TM_TYPE === 0 && latestLog.TM_ACTIV) {
          state.currentStatus = 'start';
          updateButtonVisibility(); // Show Stop if active session exists
        } else {
          state.currentStatus = 'stop';
          updateButtonVisibility(); // Default to Start if no active session
        }
      }
    } catch (err) {
      console.error('Error fetching time logs at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Failed to fetch logs.';
      state.currentStatus = 'error';
      updateButtonVisibility(); // Default to Start on error
    }
    console.log('Post-fetch state - showStart:', showStart.value, 'showStop:', showStop.value, 'at:', new Date().toLocaleString());
    return state;
  };

  const startTracking = async () => {
    initializeUserId();
    state.currentStatus = 'start';
    console.log('Starting tracking for userId:', state.userId, 'at:', new Date().toLocaleString());
    try {
      const res = await api.post(`/api/timelog/start/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = res.data || 'Started tracking.';
      console.log('Start response at:', new Date().toLocaleString(), ':', res.data);
      await new Promise(resolve => setTimeout(resolve, 2000));
      await fetchTimeLogs(); // This will update button visibility
    } catch (err) {
      console.error('Start tracking failed at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Start tracking failed.';
      state.currentStatus = 'error';
      updateButtonVisibility(); // Default to Start on error
    }
    return state;
  };

  const pauseTracking = async () => {
    initializeUserId();
    state.currentStatus = 'pause';
    console.log('Pausing tracking for userId:', state.userId, 'at:', new Date().toLocaleString());
    try {
      const res = await api.post(`/api/timelog/pause/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = res.data || 'Paused.';
      console.log('Pause response at:', new Date().toLocaleString(), ':', res.data);
      await new Promise(resolve => setTimeout(resolve, 2000));
      await fetchTimeLogs(); // This will update button visibility
    } catch (err) {
      console.error('Pause tracking failed at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Pause tracking failed.';
      state.currentStatus = 'error';
      updateButtonVisibility(); // Default to Start on error
    }
    return state;
  };

  const stopTracking = async () => {
    initializeUserId();
    state.currentStatus = 'stop';
    console.log('Stopping tracking for userId:', state.userId, 'at:', new Date().toLocaleString());
    try {
      const res = await api.post(`/api/timelog/stop/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = res.data || 'Stopped.';
      console.log('Stop response at:', new Date().toLocaleString(), ':', res.data);
      await new Promise(resolve => setTimeout(resolve, 2000));
      await fetchTimeLogs(); // This will update button visibility
    } catch (err) {
      console.error('Stop tracking failed at:', new Date().toLocaleString(), ':', err.message, 'Status:', err.response?.status);
      state.statusMessage = 'Stop tracking failed.';
      state.currentStatus = 'error';
      updateButtonVisibility(); // Default to Start on error
    }
    return state;
  };

  const initialize = async () => {
    initializeUserId();
    const initialState = await fetchTimeLogs();
    console.log('Initialization complete - showStart:', showStart.value, 'showStop:', showStop.value, 'at:', new Date().toLocaleString());
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
    showStart, // Expose for button visibility
    showStop   // Expose for button visibility
  };
}