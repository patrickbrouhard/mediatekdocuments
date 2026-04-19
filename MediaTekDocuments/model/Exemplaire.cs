using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Exemplaire (exemplaire d'une revue)
    /// </summary>
    public class Exemplaire
    {
        [JsonProperty("numero")]
        public int Numero { get; set; }
        [JsonProperty("photo")]
        public string Photo { get; set; }
        [JsonProperty("dateAchat")]
        public DateTime DateAchat { get; set; }
        [JsonProperty("idEtat")]
        public string IdEtat { get; set; }
        [JsonProperty("libelleEtat")]
        public string LibelleEtat { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }

        public Exemplaire(int numero, DateTime dateAchat, string photo, string idEtat, string libelleEtat, string idDocument)
        {
            this.Numero = numero;
            this.DateAchat = dateAchat;
            this.Photo = photo;
            this.IdEtat = idEtat;
            this.LibelleEtat = libelleEtat;
            this.Id = idDocument;
        }

    }
}
