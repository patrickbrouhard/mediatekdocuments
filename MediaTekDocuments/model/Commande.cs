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
        public string Id { get; }

        [JsonProperty("dateCommande")]
        public DateTime DateCommande { get; }

        [JsonProperty("montant")]
        public double Montant { get; }

        [JsonIgnore]
        public abstract string Endpoint { get; }

        protected Commande(string id, DateTime dateCommande, double montant)
        {
            Id = id;
            DateCommande = dateCommande;
            Montant = montant;
        }
    }
}