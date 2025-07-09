<template>
  <div class="home">
    <div class="top-left">
      <h1>Welcome, {{ aname }} To STT</h1>
      <div class="rectangles">
        <div class="card last-request">
          <h3><i class="pi pi-sign-out icon-with-bg" style="margin-right: 0.5rem;"></i>Leave Balance</h3>
          <p>{{ leaveBalance }} days</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-stopwatch icon-with-bg" style="margin-right: 0.5rem;"></i>Hours Worked Today</h3>
          <p>{{ hoursToday }} hours</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-clock icon-with-bg" style="margin-right: 0.5rem;"></i>Hours Worked This Month</h3>
          <p>{{ hoursMonth }} hours</p>
        </div>
        <div class="card last-request">
          <h3><i class="pi pi-history icon-with-bg" style="margin-right: 0.5rem;"></i>Last Leave Request</h3>
          <p v-if="lastLeaveRequest">{{ lastLeaveRequest.status }}</p>
          <p v-else>No recent requests.</p>
        </div>
      </div>
    </div>
  <div class="center-button">
      <router-link to="/listdemande">
        <button>Make LeaveRequest</button>
      </router-link>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
      <router-link to="/time-tracking">
        <button>Start Tracking</button>
      </router-link>
    </div>
    
  </div>

</template>

<script>
import api from '@/api';

export default {
  name: "Home",
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
  /* margin-bottom: 1.5rem; */
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

  margin: 30px;
  width: 270px;
  height: 120px;
  /* box-shadow: 10px 10px 10px rgba(0, 0, 0, 0.224); */
  text-align: left;
  position: relative; /* Enable absolute positioning for child elements */
}

.card h3 {
  font-size: 16px;
  font-weight: 600;
  color: #40A120;
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
  background: #0003D41a;
  border-color: #35D300;
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

 .center-button {
    position: relative;
    top: auto;
    left: auto;
    transform: none;
    margin-top: 5rem;
  } 
}

.icon-with-bg {
  background: #40A120;
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