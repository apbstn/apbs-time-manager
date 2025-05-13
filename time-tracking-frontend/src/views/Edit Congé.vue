<template>
    <div>
        <h2>Modifier Congé</h2>
        <form @submit.prevent="submitConge">
            <div class="form-group">
                <label>Date de Début :</label>
                <input type="date" v-model="demande.startDate" class="form-control" required />
            </div>
            <div class="form-group">
                <label>Date de Fin :</label>
                <input type="date" v-model="demande.endDate" class="form-control" required />
            </div>
            <div class="form-group">
                <label>Type de Congé :</label>
                <select v-model="demande.typeConge" class="form-control" required>
                    <option value="" disabled selected>Type de congé</option>
                    <option value="Maladie">Maladie</option>
                    <option value="Vacances">Vacances</option>
                    <option value="Maternité">Maternité</option>
                    <option value="Autre">Autre</option>
                </select>
            </div>
            <div class="form-group">
                <label>Raison :</label>
                <textarea v-model="demande.reason" class="form-control" placeholder="Entrez la raison du congé"
                    required></textarea>
            </div>
            <div class="button-group">
                <button type="submit" class="btn btn-success">Soumettre</button>
                <button type="button" class="btn btn-danger" @click="cancelRequest">Annuler</button>
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
                startDate: "",
                endDate: "",
                typeConge: "",
                reason: "",
                employeId: "",
            },
            demandeId: null,
        };
    },
    async mounted() {

        this.demandeId = this.$route.params.id;

        if (!this.demandeId) {
            console.error("ID de la demande de congé non trouvé.");
            this.$router.push("/listdemande");
            return;
        }


        try {
            const response = await axios.get(`https://localhost:44386/api/DemandesConge/${this.demandeId}`);
            this.demande = response.data;

            this.demande.startDate = new Date(response.data.startDate).toISOString().split("T")[0];
            this.demande.endDate = new Date(response.data.endDate).toISOString().split("T")[0];

        } catch (error) {
            console.error("Erreur lors de la récupération de la demande de congé :", error);
        }
    },
    methods: {
        formatLocalDate(dateString) {
            const date = new Date(dateString);
            const year = date.getFullYear();
            const month = (date.getMonth() + 1).toString().padStart(2, "0");
            const day = date.getDate().toString().padStart(2, "0");
            return `${year}-${month}-${day}`;
        },

        async submitConge() {
            try {
                if (!this.demandeId) {
                    alert("Erreur : ID de la demande de congé manquant.");
                    return;
                }

                await axios.put(`https://localhost:44386/api/DemandesConge/${this.demandeId}`, this.demande);
                this.$router.push("/conges/ListDemande");
            } catch (error) {
                console.error("Erreur lors de la mise à jour de la demande de congé :", error);
            }
        },
        cancelRequest() {
            this.$router.push("/conges/ListDemande");
        },
    },
};
</script>

<style scoped>
h2 {
    text-align: center;
    margin-bottom: 40px;
    font-size: 40px;
}

.form-group {
    margin-bottom: 15px;
}

.button-group {
    display: flex;
    gap: 10px;
}

.btn {
    flex: 1;
}

.btn-success {
    background-color: green;
    color: white;
}

.btn-danger {
    background-color: red;
    color: white;
}

.btn-danger:hover {
    background-color: #c82333;
}

.btn-success:hover {
    background-color: rgb(6, 108, 2);
}
</style>