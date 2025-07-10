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
          <p v-if="lastLeaveRequest">{{ lastLeaveRequest.status }}</p>
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
import { ref, onMounted } from "vue";
import api from '@/api';

export default {
  name: "Home",
  setup() {
    const chartData = ref();
    const chartOptions = ref();

    onMounted(() => {
      chartData.value = setChartData();
      chartOptions.value = setChartOptions();
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
          title: {
            display: true,
            text: 'Tracked Hours',
            color: textColor,
            font: {
              size: 16
            },
            padding: {
              bottom: 10
            }
          },
          legend: { // Legend code starts here
        position: 'left', // Positioned on the left
        labels: {
          color: textColor, // Text color
          font: {
            size: 14 // Increased font size
          },
          boxWidth: 20, // Size of the color box
          // padding: 15, // Space between items
          usePointStyle: false, // Use circular markers
          pointStyle: 'circle', // Circular style
          generateLabels: function(chart) {
            return chart.data.datasets.map((dataset, i) => ({
              text: dataset.label,
              fillStyle: dataset.backgroundColor,
              strokeStyle: 'rgba(0, 0, 0, 0.2)', // Subtle border
              lineWidth: 1, // Border width
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
          tooltip: {
            mode: 'index',
            intersect: false
          }
        },
        scales: {
          x: {
            stacked: true, // Original stacked setup
            ticks: {
              color: textColorSecondary
            },
            grid: {
              color: surfaceBorder
            }
          },
          y: {
            stacked: true, // Original stacked setup
            ticks: {
              color: textColorSecondary,
              beginAtZero: true // Ensure y-axis starts at 0
            },
            grid: {
              color: surfaceBorder
            }
          }
        },
        layout: {
          padding: {
            left: 60, // Increased for legend on left
            right: 20,
            top: 20, // Space for title
            bottom: 10
          }
        }
      };
    };

    return { chartData, chartOptions };
  },
  data() {
    return {
      aname: localStorage.getItem('username') || 'Guest',
      leaveBalance: 10, // Placeholder; replace with API call
      hoursToday: 4.5, // Placeholder; replace with API call
      hoursMonth: 120, // Placeholder; replace with API call
      recentRequests: [], // Placeholder; fetch recent leave requests
      upcomingLeave: null, // Placeholder; fetch upcoming leave
      lastLeaveRequest: null // Placeholder; fetch last leave request status
    };
  },
  mounted() {
    // Fetch leave balance, hours, recent requests, upcoming leave, and last leave request
    Promise.all([
      api.get('/api/UserTenants/leave-balance').then(res => { this.leaveBalance = res.data.balance; }),
      api.get('/api/TimeTracking/today').then(res => { this.hoursToday = res.data.hours; }),
      api.get('/api/TimeTracking/monthly-total').then(res => { this.hoursMonth = res.data.hours; }),
      api.get('/api/LeaveRequests/recent').then(res => { this.recentRequests = res.data.slice(0, 3); }),
      api.get('/api/LeaveRequests/upcoming').then(res => { this.upcomingLeave = res.data; }),
      api.get('/api/LeaveRequests/last').then(res => { this.lastLeaveRequest = res.data; })
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
  background: rgba(155, 155, 155, 0.234);
  border-color: #000000;
}

.chart-container {
  width: auto;
  max-width: auto;
  height: auto;
  margin: 30px; 
  padding: 20px;
  /* margin-right: 1px; */
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