using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Abonnement (revues)
    /// </summary>
    public class Abonnement : Commande
    {
        [JsonProperty("dateFinAbonnement")]
        public DateTime DateFinAbonnement { get; set; }

        [JsonProperty("idRevue")]
        public string IdRevue { get; set; }

        [JsonIgnore]
        public override string Endpoint => "abonnement";

        // Constructeur vide pour la désérialisation JSON
        public Abonnement() { }

        public Abonnement(
            string id,
            DateTime dateCommande,
            double montant,
            DateTime dateFinAbonnement,
            string idRevue
        ) : base(id, dateCommande, montant)
        {
            DateFinAbonnement = dateFinAbonnement;
            IdRevue = idRevue;
        }
    }
}