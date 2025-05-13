<template>
  <div>
    <div class="header-container">
      <div class="flex justify-content-between align-items-center mb-2">
        <h2>List of Leave Requests</h2>
      </div>
      
      <div class="search-container">
        <FloatLabel variant="on">
          <InputText 
            id="searchInput" 
            class="search-input" 
            v-model="searchQuery" 
            @input="filterConges"
            autocomplete="off" 
          />
          <label for="searchInput">Search</label>
        </FloatLabel>
      </div>
    </div>

    <div class="card">
      <DataTable 
        :value="sortedConges" 
        paginator 
        :rows="10" 
        dataKey="id" 
        tableStyle="min-width: 50rem" 
        :showGridlines="true"
      >
        <Column field="id" header="Id" sortable @sort="sortBy('id')">
          <template #body="{ data }">
            {{ data?.id }}
          </template>
          <template #sorticon="{ sorted, ascending }">
            <i class="pi" :class="{
              'pi-sort-alt': !sorted,
              'pi-sort-amount-up': sorted && ascending,
              'pi-sort-amount-down': sorted && !ascending
            }"></i>
          </template>
        </Column>
        <Column field="employeNom" header="Nom" sortable @sort="sortBy('employeNom')">
          <template #body="{ data }">
            {{ data?.employeNom }}
          </template>
        </Column>
        <Column field="employePrenom" header="Prénom" sortable @sort="sortBy('employePrenom')">
          <template #body="{ data }">
            {{ data?.employePrenom }}
          </template>
        </Column>
        <Column field="startDate" header="Début">
          <template #body="{ data }">
            {{ data ? formatDate(data.startDate) : '' }}
          </template>
        </Column>
        <Column field="endDate" header="Fin">
          <template #body="{ data }">
            {{ data ? formatDate(data.endDate) : '' }}
          </template>
        </Column>
        <Column field="nombreJours" header="Jours">
          <template #body="{ data }">
            {{ data?.nombreJours }}
          </template>
        </Column>
        <Column field="status" header="Statut">
          <template #body="{ data }">
            <Tag :value="getStatusText(data.status)" :severity="getStatusSeverity(data.status)" />
          </template>
        </Column>
        <Column field="typeConge" header="Type">
          <template #body="{ data }">
            {{ data?.typeConge }}
          </template>
        </Column>
        <Column field="reason" header="Raison">
          <template #body="{ data }">
            {{ data?.reason }}
          </template>
        </Column>
        <Column header="Actions">
          <template #body="{ data }">
            <Button 
              v-if="data?.status === 0" 
              icon="pi pi-check" 
              class="p-button-success p-button-sm mr-2" 
              @click="updateLeaveRequestStatus(data.id, 'approve')" 
            />
            <Button 
              v-if="data?.status === 0" 
              icon="pi pi-times" 
              class="p-button-danger p-button-sm" 
              @click="updateLeaveRequestStatus(data.id, 'reject')" 
            />
          </template>
        </Column>
        <template #empty>
          <div class="text-center text-muted">No leave requests found</div>
        </template>
      </DataTable>
    </div>
  </div>
</template>

<script>
import axios from "axios";

export default {
  data() {
    return {
      conges: [], 
      searchQuery: "", 
      sortField: "id", 
      sortDirection: "asc", 
      filteredConges: [], 
    };
  },
  computed: {
    
    sortedConges() {
      return [...this.filteredConges].sort((a, b) => {
        const modifier = this.sortDirection === "asc" ? 1 : -1;
        let aVal = a[this.sortField];
        let bVal = b[this.sortField];

        
        if (this.sortField === "id") {
          const [aPrefix, aSuffix] = aVal.split("-").map(Number);
          const [bPrefix, bSuffix] = bVal.split("-").map(Number);

          if (aPrefix === bPrefix) {
            return (aSuffix - bSuffix) * modifier;
          }
          return (aPrefix - bPrefix) * modifier;
        }

        
        if (typeof aVal === "number" && typeof bVal === "number") {
          return (aVal - bVal) * modifier;
        }

       
        aVal = aVal ? aVal.toString().toLowerCase() : "";
        bVal = bVal ? bVal.toString().toLowerCase() : "";
        if (aVal < bVal) return -1 * modifier;
        if (aVal > bVal) return 1 * modifier;
        return 0;
      });
    },
  },
  methods: {
    getStatusSeverity(status) {
      if (status === 0) return 'warning'
      if (status === 1) return 'success'
      if (status === 2) return 'danger'
      return null
    },
    
    async getAllLeaveRequests() {
      try {
        const response = await axios.get(
          "https://localhost:44386/api/DemandesConge/leave-requests"
        );
        this.conges = response.data;
        this.filterConges(); 
      } catch (error) {
        console.error("Erreur lors de la récupération des demandes :", error);
      }
    },
   
    filterConges() {
      const query = this.searchQuery.toLowerCase();
      this.filteredConges = this.conges.filter((demande) => {
        return (
          (demande.typeConge &&
            demande.typeConge.toLowerCase().includes(query)) ||
          (demande.employeNom &&
            demande.employeNom.toLowerCase().includes(query)) ||
          (demande.employePrenom &&
            demande.employePrenom.toLowerCase().includes(query))
        );
      });
    },
   
    sortBy(field) {
      if (this.sortField === field) {
        this.sortDirection = this.sortDirection === "asc" ? "desc" : "asc";
      } else {
        this.sortField = field;
        this.sortDirection = "asc";
      }
    },
    
    formatDate(date) {
      const d = new Date(date);
      return isNaN(d.getTime()) ? "Date invalide" : d.toLocaleDateString();
    },
    
    getStatusText(status) {
      if (status === 0) return "En attente";
      if (status === 1) return "Approuvé";
      if (status === 2) return "Rejeté";
      return "";
    },
    
    async updateLeaveRequestStatus(id, action) {
      try {
        const response = await axios.put(
          `https://localhost:44386/api/DemandesConge/leave-requests/${id}`,
          null,
          { params: { action } }
        );

        const updatedDemande = response.data;
        const index = this.conges.findIndex((d) => d.id === id);
        if (index !== -1) {
          this.conges.splice(index, 1, updatedDemande); 
          this.filterConges(); 
        }
        setTimeout(() => {
          this.getAllLeaveRequests();
        }, 10); 
      } catch (error) {
        console.error("Erreur lors de la mise à jour du statut :", error);
      }
    },
  },
  mounted() {
    this.getAllLeaveRequests(); 
  },
};
</script>

<style scoped>
.header-container {
  margin-bottom: 1.5rem;
}

.flex {
  display: flex;
}

.justify-content-between {
  justify-content: space-between;
}

.align-items-center {
  align-items: center;
}

.search-container {
  margin-top: 1rem;
  width: 300px;
}

.search-input {
  border: 1px solid #ced4da !important;
  border-radius: 4px !important;
  width: 100%;
}

h2 {
  font-size: 2rem;
  margin: 0;
}

.card {
  margin-top: 1rem;
}

.text-center {
  text-align: center;
}

.text-muted {
  color: #6c757d;
}

.mr-2 {
  margin-right: 0.5rem;
}
</style>