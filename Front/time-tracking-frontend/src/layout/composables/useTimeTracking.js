import api from '@/api';

export function useTimeTracking() {
  let state = {
    currentStatus: 'stop',
    statusMessage: '',
    userId: 'guest',
    timeLogs: []
  };

  // Simplified JWT decoding
  const decodeJwt = (token) => {
    try {
      if (!token) return null;
      const base64Url = token.split('.')[1];
      if (!base64Url) return null;
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = atob(base64);
      return JSON.parse(jsonPayload);
    } catch (error) {
      console.error('JWT decoding error:', error.message);
      return null;
    }
  };

  // Initialize user ID from JWT
  const initializeUserId = () => {
    const token = localStorage.getItem('accessToken');
    console.log('Token from localStorage:', token);
    if (token) {
      const decoded = decodeJwt(token);
      console.log('Decoded token:', decoded);
      if (decoded && decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']) {
        state.userId = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || 'guest';
      } else {
        console.warn('No valid user ID in decoded token');
      }
    } else {
      console.warn('No accessToken found in localStorage');
    }
  };

  // Format date to human-readable string
  const formatDate = (date) => {
    return date ? new Date(date).toLocaleString() : 'No Date';
  };

  // Format duration
  const formatDuration = (totalHours) => {
    const [hours = 0, minutes = 0, seconds = 0] = (totalHours || '00:00:00').split(':').map(Number);
    return `${hours}h ${minutes}m ${seconds}s`;
  };

  // Get status phrase
  const getStatusPhrase = () => {
    const username = localStorage.getItem('username') || 'User';
    return {
      start: `${username}, you're ready to Start Tracking Time`,
      pause: `${username}, your tracking is Paused`,
      stop: `${username}, you're Not Tracking Time`,
      loading: `${username}, please wait...`,
      error: `${username}, an error occurred`
    }[state.currentStatus] || `${username}, status unknown`;
  };

  // Fetch time logs from API
  const fetchTimeLogs = async () => {
    initializeUserId();
    state.currentStatus = 'loading';
    console.log('Fetching logs for user:', state.userId);
    try {
      const response = await api.get(`api/timelog/${state.userId}`, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      console.log('Full API response:', JSON.stringify(response.data, null, 2));
      const logs = Array.isArray(response.data) ? response.data : [];
      state.timeLogs = logs.map((log) => ({
        tm_Id: log.tm_Id || log.TM_ID || 0,
        TM_TIME: log.tm_Time || log.TM_TIME || log.time || 'No Date',
        TM_TOTALHOURS: log.tm_TotalHours || log.TM_TOTALHOURS || '00:00:00',
        TM_TYPE: Number(log.TM_TYPE) || 2
      }));
      console.log('Processed logs with TM_TYPE:', state.timeLogs);
      if (state.timeLogs.length) {
        const latestType = state.timeLogs[state.timeLogs.length - 1].TM_TYPE;
        state.currentStatus = latestType === 0 ? 'start' : latestType === 1 ? 'pause' : 'stop';
        console.log('Status set to:', state.currentStatus, 'from TM_TYPE:', latestType);
      } else {
        state.currentStatus = 'stop';
        console.log('No logs, status set to stop');
      }
    } catch (error) {
      console.error('Fetch error:', error.message);
      state.statusMessage = 'Error fetching logs';
      state.currentStatus = 'error';
    }
    return state;
  };

  // Start time tracking
  const startTracking = async () => {
    state.currentStatus = 'start';
    console.log('Starting tracking');
    try {
      const response = await api.post(`api/timelog/start/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = response.data || 'Started';
      console.log('Start response:', response.data);
      await new Promise(resolve => setTimeout(resolve, 500));
      await fetchTimeLogs();
    } catch (error) {
      console.error('Start error:', error.message);
      state.statusMessage = 'Start failed';
      state.currentStatus = 'error';
    }
    return state;
  };

  // Pause time tracking
  const pauseTracking = async () => {
    if (state.currentStatus !== 'start') {
      state.statusMessage = 'Must start first';
      return state;
    }
    state.currentStatus = 'pause';
    console.log('Pausing tracking');
    try {
      const response = await api.post(`api/timelog/pause/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = response.data || 'Paused';
      console.log('Pause response:', response.data);
      await new Promise(resolve => setTimeout(resolve, 500));
      await fetchTimeLogs();
    } catch (error) {
      console.error('Pause error:', error.message);
      state.statusMessage = 'Pause failed';
      state.currentStatus = 'error';
    }
    return state;
  };

  // Stop time tracking
  const stopTracking = async () => {
    state.currentStatus = 'stop';
    console.log('Stopping tracking');
    try {
      const response = await api.post(`api/timelog/stop/${state.userId}`, {}, {
        headers: { Authorization: `Bearer ${localStorage.getItem('accessToken')}` }
      });
      state.statusMessage = response.data || 'Stopped';
      console.log('Stop response:', response.data);
      await new Promise(resolve => setTimeout(resolve, 500));
      await fetchTimeLogs();
    } catch (error) {
      console.error('Stop error:', error.message);
      state.statusMessage = 'Stop failed';
      state.currentStatus = 'error';
    }
    return state;
  };

  // Initialize on mount
  const initialize = async () => {
    initializeUserId();
    await fetchTimeLogs();
    return state;
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
    getState: () => state
  };
}