<template>
  <div class="home">
    <div class="top-left">
      <h1>Welcome, {{ aname }} to STT</h1><br/>
      <div class="rectangles">
        <div class="card last-request">
          <h3><i class="pi pi-sign-out icon-with-bg1" style="margin-right: 0.5rem;"></i>Leave Balance</h3>
          <p>{{ leaveBalance }} days</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-stopwatch icon-with-bg2" style="margin-right: 0.5rem;"></i>Hours Worked Today</h3>
          <p>{{ hoursToday }} hours</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-clock icon-with-bg3" style="margin-right: 0.5rem;"></i>Hours Worked This Month</h3>
          <p>{{ hoursMonth }} hours</p>
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
  </div>
</template>

<script>
import { ref, onMounted, computed } from "vue";
import api from '@/api';

export default {
  name: "Home",
  setup() {
    const chartData = ref();
    const chartOptions = ref();
    const errorMessage = ref(null);
    const userId = ref(null);
    const leaveBalance = ref();
    const lastLeaveRequest = ref(null);

    // JWT decoding function
    const decodeJwt = (token) => {
      try {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(
          atob(base64)
            .split('')
            .map((c) => {
              return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
            })
            .join('')
        );
        return JSON.parse(jsonPayload);
      } catch (error) {
        console.error('Error decoding JWT:', error);
        return null;
      }
    };

    // Computed property to map status codes to labels
    const getStatusLabel = computed(() => (status) => {
      switch (status) {
        case 0:
          return "Pending";
        case 1:
          return "Approved";
        case 2:
          return "Rejected";
        default:
          return "Unknown";
      }
    });

    // Decode token and fetch data on component mount
    onMounted(async () => {
      chartData.value = setChartData();
      chartOptions.value = setChartOptions();

      const token = localStorage.getItem('accessToken');
      if (token) {
        const decoded = decodeJwt(token);
        console.log('Decoded token:', decoded);
        if (decoded) {
          const emailKey = 'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name';
          const email = decoded[emailKey];
          if (!email) {
            errorMessage.value = 'Invalid token: email not found';
            console.error('Invalid token: email not found');
            return;
          }

          try {
            const response = await api.post('/api/UserTenants/get-id-by-email', email,
              {
                headers: {
                  Authorization: `Bearer ${token}`
                }
              }
            );
            userId.value = response.data;
            if (!userId.value) {
              errorMessage.value = 'Invalid response: userId not found';
              console.error('Invalid response: userId not found');
            } else {
              console.log('Extracted userId:', userId.value);

              // Fetch leave balance
              const balanceResponse = await api.get(`/api/LeaveRequests/balance/${userId.value}`, {
                headers: {
                  Authorization: `Bearer ${token}`
                }
              });
              leaveBalance.value = balanceResponse.data.balance;
              console.log('Leave Balance:', leaveBalance.value);

              // Fetch last leave request status
              const lastResponse = await api.get(`/api/LeaveRequests/last/${userId.value}`, {
                headers: {
                  Authorization: `Bearer ${token}`
                }
              });
              lastLeaveRequest.value = { status: lastResponse.data };
              console.log('Last Leave Request Status:', lastLeaveRequest.value);
            }
          } catch (error) {
            errorMessage.value = 'Error fetching data from API';
            console.error('Error fetching data:', error);
          }
        } else {
          errorMessage.value = 'Invalid token';
          console.error('Invalid token');
        }
      } else {
        errorMessage.value = 'No token found in localStorage';
        console.error('No token found in localStorage');
      }
    });

    const setChartData = () => {
      const documentStyle = getComputedStyle(document.documentElement);
      return {
        labels: ['Monday', 'Tuesday', 'Wensday', 'Thursday', 'Friday', 'Saturday', 'Sunday'],
        datasets: [
          {
            type: 'bar',
            label: 'Worked Hours',
            backgroundColor: '#35D300',
            data: [8, 6, 7.5, 8, 7, , ]
          },
          {
            type: 'bar',
            label: 'Stopped Hours',
            backgroundColor: '#FF0000',
            data: [1, 1.5, 1.5, 1.5, 2, , ]
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
            labels: {
              color: textColor,
              font: { size: 14 },
              boxWidth: 20,
              usePointStyle: false,
              pointStyle: 'circle',
              generateLabels: function(chart) {
                return chart.data.datasets.map((dataset, i) => ({
                  text: dataset.label,
                  fillStyle: dataset.backgroundColor,
                  strokeStyle: 'rgba(0, 0, 0, 0.2)',
                  lineWidth: 1,
                  hidden: !chart.isDatasetVisible(i),
                  index: i
                }));
              }
            },
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
          tooltip: { mode: 'index', intersect: false }
        },
        scales: {
          x: { stacked: true, ticks: { color: textColorSecondary }, grid: { color: surfaceBorder } },
          y: { stacked: true, ticks: { color: textColorSecondary, beginAtZero: true }, grid: { color: surfaceBorder } }
        },
        layout: { padding: { left: 60, right: 20, top: 20, bottom: 10 } }
      };
    };

    return { chartData, chartOptions, errorMessage, userId, leaveBalance, lastLeaveRequest, getStatusLabel };
  },
  data() {
    return {
      aname: localStorage.getItem('username') || 'Guest',
      hoursToday: 4.5,
      hoursMonth: 120,
      recentRequests: [],
      upcomingLeave: null
    };
  },
  mounted() {
    Promise.all([
      api.get('/api/TimeTracking/today').then(res => { this.hoursToday = res.data.hours; }),
      api.get('/api/TimeTracking/monthly-total').then(res => { this.hoursMonth = res.data.hours; })
    ]).catch(error => console.error('Error fetching data:', error));
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

.center-button {
  position: absolute;
  left: 50%;
  transform: translate(-50%, -50%);
  gap: 1.5rem !important;
}

button {
  padding: 0.7rem 1.5rem;
  font-size: 1.2rem;
  cursor: pointer;
  background-color: #007bff00 !important;
  color: #35D300 !important;
  border: 1px solid #35D300 !important;
  border-radius: 10px;
  transition: background-color 0.2s, color 0.2s;
  margin-top: 55rem;
}

button:hover {
  background-color: #35D300 !important;
  color: white !important;
  border-color: white !important;
}

/* Responsive design for smaller screens */
@media (max-width: 800px) {
  .rectangles {
    flex-direction: column;
    align-items: flex-start;
  }

  .card {
    width: 100%;
    max-width: 300px;
  }

  .chart-container {
    width: 100%;
    max-width: 100%;
    margin: 20px;
  }

  .center-button {
    position: relative;
    top: auto;
    left: auto;
    transform: none;
    margin-top: 5rem;
  }
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