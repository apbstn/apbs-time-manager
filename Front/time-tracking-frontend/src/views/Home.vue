<template>
  <div class="home">
    <div class="top-left">
      <h1>Welcome, {{ aname }} to STT</h1><br />
      <div class="rectangles">
        <div class="card last-request">
          <h3><i class="pi pi-sign-out icon-with-bg1" style="margin-right: 0.5rem;"></i>Leave Balance left</h3>
          <p>{{ leaveBalance }} days</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-stopwatch icon-with-bg2" style="margin-right: 0.5rem;"></i>Hours Worked Today</h3>
          <p>{{ hoursToday }}</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-clock icon-with-bg3" style="margin-right: 0.5rem;"></i>Hours Worked This Month</h3>
          <p>{{ hoursMonth }}</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-history icon-with-bg4" style="margin-right: 0.5rem;"></i>Last Leave Request</h3>
          <p v-if="lastLeaveRequest">{{ getStatusLabel(lastLeaveRequest.status) }}</p>
          <p v-else>No recent requests.</p>
        </div>
      </div>
      <!-- Integrated Chart -->
      <div class="card chart-container">
        <Chart type="bar" :data="chartData" :options="chartOptions" class="h-[30rem]" />
      </div>
    </div>

    <!-- Tracking Reminder Popup -->
    <Dialog
      v-model:visible="showReminderDialog"
      header="Time Tracking Reminder"
      :modal="true"
      :closable="true"
      :style="{ width: '650px' }"
      @update:visible="dismissReminder"
    >
    <Divider class="df"/>
      <p>Don't forget to start tracking time!</p>
      <Divider/>
      <div class="dialog-footer">
        <Button
          label="Dismiss"
          class="p-button-text1"
          :style="{ color: '#35D300' }"
          @click="dismissReminder"
        />
      </div>
    </Dialog>
  </div>
</template>

<script>
import { ref, onMounted, computed } from 'vue';
import api from '@/api';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import Chart from 'primevue/chart';

export default {
  name: 'Home',
  components: {
    Dialog,
    Button,
    Chart
  },
  setup() {
    const chartData = ref();
    const chartOptions = ref();
    const errorMessage = ref(null);
    const userId = ref(null);
    const leaveBalance = ref(null);
    const lastLeaveRequest = ref(null);
    const hoursToday = ref('0h 00m');
    const hoursMonth = ref('0h 00m');
    const showReminderDialog = ref(false);
    const isStarting = ref(false);

    // Helper function to convert decimal hours to "Xh Ym" format
    const formatHoursToHm = (decimalHours) => {
      const hours = Math.floor(decimalHours);
      const minutes = Math.round((decimalHours - hours) * 60);
      return `${hours}h ${minutes.toString().padStart(2, '0')}m`;
    };

    const decodeJwt = (token) => {
      try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
          atob(base64)
            .split('')
            .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
            .join('')
        );
        return JSON.parse(jsonPayload);
      } catch (error) {
        console.error('Error decoding JWT:', error);
        return null;
      }
    };

    const getStatusLabel = computed(() => (status) => {
      switch (status) {
        case 0: return 'Pending';
        case 1: return 'Approved';
        case 2: return 'Rejected';
        default: return 'No Leave Request were found';
      }
    });

    const startTracking = async () => {
      if (!userId.value) {
        errorMessage.value = 'User ID not found';
        console.error('startTracking: User ID not found');
        return;
      }

      try {
        isStarting.value = true;
        const token = localStorage.getItem('accessToken');
        await api.post(`/api/timelog/start/${userId.value}`, {}, {
          headers: { Authorization: `Bearer ${token}` }
        });
        console.log('Tracking started for user:', userId.value);
        // Refresh today's hours
        const todayResponse = await api.get(`/api/timelog/today/${userId.value}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
        hoursToday.value = todayResponse.data;
        // Close the dialog
        dismissReminder();
      } catch (error) {
        console.error('Error starting tracking:', error.response?.data || error.message);
        errorMessage.value = error.response?.data?.message || 'Failed to start tracking';
      } finally {
        isStarting.value = false;
      }
    };

    const dismissReminder = () => {
      showReminderDialog.value = false;
      localStorage.setItem('showTrackingReminder', 'false');
      console.log('Reminder dismissed, showTrackingReminder set to false');
    };

    onMounted(async () => {
      chartOptions.value = setChartOptions();

      // Check for tracking reminder flag
      localStorage.setItem('showStopTrackingReminder',true)
      const showReminder = localStorage.getItem('showTrackingReminder') === 'true';
      if (showReminder) {
        showReminderDialog.value = true;
      }

      const token = localStorage.getItem('accessToken');
      if (token) {
        const decoded = decodeJwt(token);
        if (decoded) {
          const emailKey = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
          const email = decoded[emailKey];
          if (!email) {
            errorMessage.value = 'Invalid token: email not found';
            return;
          }

          try {
            const response = await api.post('/api/UserTenants/get-id-by-email', email, {
              headers: { Authorization: `Bearer ${token}` }
            });
            userId.value = response.data;
            if (userId.value) {
              const balanceResponse = await api.get(`/api/LeaveRequests/balance/${userId.value}`, {
                headers: { Authorization: `Bearer ${token}` }
              });
              leaveBalance.value = balanceResponse.data.balance;

              const lastResponse = await api.get(`/api/LeaveRequests/last/${userId.value}`, {
                headers: { Authorization: `Bearer ${token}` }
              });
              lastLeaveRequest.value = { status: lastResponse.data };

              const todayResponse = await api.get(`/api/timelog/today/${userId.value}`, {
                headers: { Authorization: `Bearer ${token}` }
              });
              hoursToday.value = todayResponse.data;

              const monthResponse = await api.get(`/api/timelog/monthly/${userId.value}`, {
                headers: { Authorization: `Bearer ${token}` }
              });
              hoursMonth.value = monthResponse.data || '0h 00m';

              console.log('Monthly hours:', hoursMonth.value);

              // Fetch weekly hours
              console.log('-------------------------------------------------');
              console.log('ID going to sent:', userId.value);
              console.log('-------------------------------------------------');
              const weeklyResponse = await api.get(`/api/timelog/weekly/${userId.value}`, {
                headers: { Authorization: `Bearer ${token}` }
              });
              const weeklyPauseResponse = await api.get(`/api/timelog/weeklyPause/${userId.value}`, {
                headers: { Authorization: `Bearer ${token}` }
              });
              chartData.value = setChartData(weeklyResponse.data, weeklyPauseResponse.data);
              console.log('-------------------------------------------------');
              console.log('Weekly Data:', weeklyResponse.data);
              console.log('Weekly Pause Data:', weeklyPauseResponse.data);
              console.log('-------------------------------------------------');
            }
          } catch (error) {
            errorMessage.value = 'Error fetching data from API';
            console.error('Error fetching data:', error);
          }
        }
      }
    });

    const setChartData = (weeklyData = {}, weeklyPauseData = {}) => {
      const days = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
      return {
        labels: days,
        datasets: [
          {
            type: 'bar',
            label: 'Worked Hours',
            backgroundColor: '#35D300',
            data: days.map(day => weeklyData[day] || 0),
          },
          {
            type: 'bar',
            label: 'Paused Hours',
            backgroundColor: '#ff8000',
            data: days.map(day => weeklyPauseData[day] || 0),
          }
        ]
      };
    };

    const setChartOptions = () => {
      const documentStyle = getComputedStyle(document.documentElement);
      const textColor = documentStyle.getPropertyValue('--p-text-color');
      const textColorSecondary = documentStyle.getPropertyValue('--p-text-muted-color');
      const surfaceBorder = documentStyle.getPropertyValue('--p-content-border-color');
      return {
        maintainAspectRatio: false,
        aspectRatio: 0.8,
        plugins: {
          title: { display: true, text: 'Tracked Hours', color: textColor, font: { size: 16 }, padding: { bottom: 10 } },
          legend: {
            position: 'left',
            labels: { color: textColor, font: { size: 14 }, boxWidth: 20, usePointStyle: false, pointStyle: 'circle' },
            onClick: (e, legendItem, legend) => {
              const index = legendItem.index;
              const ci = legend.chart;
              if (ci.isDatasetVisible(index)) {
                ci.hide(index);
                legendItem.hidden = true;
              } else {
                ci.show(index);
                legendItem.hidden = false;
              }
              ci.update();
            }
          },
          tooltip: {
            mode: 'index',
            intersect: false,
            callbacks: {
              label: function (context) {
                const label = context.dataset.label || '';
                const value = context.parsed.y;
                return `${label}: ${formatHoursToHm(value)}`;
              }
            }
          }
        },
        scales: {
          x: {
            stacked: true,
            ticks: { color: textColorSecondary },
            grid: { color: surfaceBorder }
          },
          y: {
            stacked: true,
            ticks: {
              color: textColorSecondary,
              beginAtZero: true,
              callback: function (value) {
                return formatHoursToHm(value);
              }
            },
            grid: { color: surfaceBorder }
          }
        },
        layout: { padding: { left: 60, right: 20, top: 20, bottom: 10 } }
      };
    };

    return { chartData, chartOptions, errorMessage, userId, leaveBalance, lastLeaveRequest, getStatusLabel, hoursToday, hoursMonth, showReminderDialog, startTracking, dismissReminder };
  },
  data() {
    return {
      aname: localStorage.getItem('username') || 'Guest'
    };
  }
};
</script>

<style scoped>
.home {
  position: relative;
  display: flex;
  flex-direction: column;
}

.top-left {
  text-align: left;
}

h1 {
  font-size: 22px;
  color: #6B7280;
}

.rectangles {
  display: flex;
  flex-wrap: wrap;
}

.card {
  background: #ffffff;
  border: 1px solid #ced4da;
  border-radius: 8px;
  margin: auto;
  width: 270px;
  height: 120px;
  box-shadow: 2px 2px 5px rgba(0, 0, 0, 0.224);
  text-align: left;
  position: relative;
}

.card h3 {
  font-size: 16px;
  font-weight: 600;
  color: #2d2d2d;
  margin-bottom: 0.5rem;
}

.card p {
  font-size: 1.2rem;
  color: #434343;
  position: absolute;
  bottom: 1rem;
  right: 1rem;
}

.last-request {
  background: rgba(226, 226, 226, 0.234);
  border-color: #000000;
}

.chart-container {
  width: auto;
  max-width: auto;
  height: auto;
  margin: 30px;
  padding: 20px;
}

.dialog-footer {
  display: flex;
  justify-content: flex-end;
  gap: 1rem;
  margin-top: 1rem;
}

:deep(.p-button.p-button-text) {
  color: #35D300 !important;
  background-color: transparent !important;
  border-color: #35D300 !important;
  border-radius: 1rem;
}

:deep(.p-button.p-button-text:hover) {
  background-color: #35D300 !important;
  color: white !important;
}

:deep(.p-button.p-button-text1) {
  color: #ff0000 !important;
  background-color: transparent !important;
  border-color: #ff0000 !important;
  border-radius: 1rem;
}

:deep(.p-button.p-button-text1:hover) {
  background-color: #ff0000 !important;
  color: white !important;
}

:deep(.p-dialog-header) {
  background: #f9fafb;
  color: #1f2937;
  font-weight: 600;
}

:deep(.p-dialog-content) {
  background: #ffffff;
  color: #1f2937;
  padding: 1rem;
}

.icon-with-bg1 {
  background: #35D300;
  border-radius: 20%;
  padding: 0.4rem;
  color: #ffffff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2.2rem;
  height: 2.2rem;
}

.icon-with-bg2 {
  background: #ff9d00;
  border-radius: 20%;
  padding: 0.4rem;
  color: #ffffff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2.2rem;
  height: 2.2rem;
}

.icon-with-bg3 {
  background: #ff00dd;
  border-radius: 20%;
  padding: 0.4rem;
  color: #ffffff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2.2rem;
  height: 2.2rem;
}
.df{
  padding: 0px !important;
}
.icon-with-bg4 {
  background: #0084ff;
  border-radius: 20%;
  padding: 0.4rem;
  color: #ffffff;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 2.2rem;
  height: 2.2rem;
}
</style>