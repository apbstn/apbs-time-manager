<template>
  <div class="leave-request-container">
    <h2>Leave Request</h2>
    <form @submit.prevent="submitConge" class="leave-request-form">
      <div class="form-group">
        <label>Start Date :</label>
        <DatePicker v-model="icondisplay" showIcon fluid iconDisplay="input" required />
      </div>
      
      <div class="form-group">
        <label>End Date :</label>
        <DatePicker v-model="icondisplay" showIcon fluid iconDisplay="input" required />
      </div>
      
      <div class="form-group">
        <FloatLabel class="w-full md:w-56">
          <Select v-model="demande.typeConge" inputId="over_label" :options="leaveTypes" optionLabel="name"
            optionValue="value" class="w-full" />
          <label for="over_label">Type Of Leave</label>
        </FloatLabel>
      </div>
      
      <div class="form-group">
        <label>Raison :</label>
        <IftaLabel>
          <Textarea id="description" v-model="value" rows="5" cols="30" style="resize: none" />
          <label for="description">Description</label>
        </IftaLabel>
      </div>
      
      <div class="button-group">
        <button type="submit" class="btn btn-success">Send</button>
        <button type="button" class="btn btn-danger" @click="cancelRequest">Cancel</button>
      </div>
    </form>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      demande: {
        StartDate: "",
        EndDate: "",
        TypeConge: "",
        Reason: "",
        EmployeId: "",
      },
      leaveTypes: [
        { name: 'Maladie', value: 'Maladie' },
        { name: 'Vacances', value: 'Vacances' },
        { name: 'Maternité', value: 'Maternité' },
        { name: 'Autre', value: 'Autre' }
      ]
    };
  },
  mounted() {
    const userData = localStorage.getItem("user");
    if (userData) {
      const user = JSON.parse(userData);
      if (user.id) {
        this.demande.EmployeId = user.id;
      } else {
        console.error("ID de l'utilisateur introuvable.");
      }
    } else {
      console.error("Utilisateur non connecté ou ID introuvable.");
    }
  },
  methods: {
    async submitConge() {
      if (!this.demande.EmployeId) {
        alert("Erreur : ID de l'employé manquant.");
        return;
      }

      const user = JSON.parse(localStorage.getItem("user"));
      const token = user?.token;
      if (!token) {
        alert("Erreur : Token manquant, veuillez vous reconnecter.");
        return;
      }

      try {
        await axios.post("https://localhost:44386/api/DemandesConge", this.demande, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });
        this.$router.push("/listdemande");
      } catch (error) {
        console.error("Erreur lors de l'ajout de la demande de congé :", error);
        alert("Une erreur est survenue lors de la soumission de votre demande.");
      }
    },
    cancelRequest() {
      this.$router.push("/listdemande");
    },
  },
};
</script>
<style scoped>
.leave-request-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 2rem;
  background-color: #ffffff;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

h2 {
  text-align: left;
  margin-bottom: 2rem;
  font-size: 2rem;
  color: #2c3e50;
}

.leave-request-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 500;
  color: #495057;
}

/* Enhanced description textarea styling */
.form-group:has(#description) {
  margin-top: 1rem;
}

.form-group:has(#description) label {
  font-size: 1rem;
  margin-bottom: 0.5rem;
}

#description {
  min-height: 150px;  /* Increased from default */
  width: 100%;
  padding: 0.75rem;
  border: 1px solid #ced4da;
  border-radius: 6px;
  font-family: inherit;
  font-size: 1rem;
  transition: border-color 0.2s;
}

#description:focus {
  outline: none;
  border-color: #80bdff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}

.button-group {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;  /* Increased margin */
}

.btn {
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 6px;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  flex: 1;
  text-align: center;
  font-size: 1rem;
}

.btn-success {
  background-color: #28a745;
  color: white;
}

.btn-danger {
  background-color: #dc3545;
  color: white;
}

.btn-danger:hover {
  background-color: #c82333;
}

.btn-success:hover {
  background-color: #218838;
}

/* PrimeVue component adjustments */
.p-calendar, .p-inputtext, .p-dropdown {
  width: 100%;
}

.p-float-label {
  margin-top: 1rem;
}
</style>