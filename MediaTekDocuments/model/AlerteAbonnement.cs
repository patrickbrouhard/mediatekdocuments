using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Représente un abonnement arrivant à expiration (avec titre de la revue)
    /// </summary>
    public class AlerteAbonnement
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("titre")]
        public string Titre { get; set; }

        [JsonProperty("dateFinAbonnement")]
        public DateTime? DateFinAbonnement { get; set; }
    }
}