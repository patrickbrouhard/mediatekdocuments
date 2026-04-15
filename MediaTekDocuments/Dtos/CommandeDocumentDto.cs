using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.dto
{
    public class CommandeDocumentDto
    {
        [JsonProperty("idCommande")]
        public string IdCommande { get; set; }

        [JsonProperty("dateCommande")]
        public DateTime DateCommande { get; set; }

        [JsonProperty("montant")]
        public double Montant { get; set; }

        [JsonProperty("nbExemplaire")]
        public int NbExemplaire { get; set; }

        [JsonProperty("idSuivi")]
        public int IdSuivi { get; set; }

        [JsonProperty("libelleEtat")]
        public string LibelleEtat { get; set; }


        // Document commun
        [JsonProperty("idDocument")]
        public string IdDocument { get; set; }

        [JsonProperty("titre")]
        public string Titre { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("genreDocument")]
        public string Genre { get; set; }

        [JsonProperty("publicDocument")]
        public string Public { get; set; }

        [JsonProperty("rayonDocument")]
        public string Rayon { get; set; }


        // Spécifique Livre
        [JsonProperty("isbn")]
        public string Isbn { get; set; }

        [JsonProperty("auteur")]
        public string Auteur { get; set; }

        [JsonProperty("collection")]
        public string Collection { get; set; }


        // Spécifique DVD
        [JsonProperty("synopsis")]
        public string Synopsis { get; set; }

        [JsonProperty("realisateur")]
        public string Realisateur { get; set; }

        [JsonProperty("duree")]
        public int? Duree { get; set; }
    }
}