using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Commande
    /// </summary>
    public abstract class Commande
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("dateCommande")]
        public DateTime DateCommande { get; set; }

        [JsonProperty("montant")]
        public double Montant { get; set; }

        [JsonIgnore]
        public abstract string Endpoint { get; }

        // Constructeur vide car la désérialisation JSON instancie d'abord un objet vide avant de remplir les propriétés
        protected Commande() { }

        protected Commande(string id, DateTime dateCommande, double montant)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
        }
    }
}
