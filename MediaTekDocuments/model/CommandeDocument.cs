using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier CommandeDocument (livres et DVD)
    /// </summary>
    public class CommandeDocument : Commande
    {
        [JsonProperty("nbExemplaire")]
        public int NbExemplaire { get; set; }

        [JsonProperty("idLivreDvd")]
        public string IdLivreDvd { get; set; }

        [JsonProperty("idSuivi")]
        public int IdSuivi { get; set; }

        [JsonProperty("libelleEtat")]
        public string LibelleSuivi { get; set; }

        [JsonIgnore]
        public string IdDocument => IdLivreDvd;

        [JsonIgnore]
        public override string Endpoint => "commandedocument";

        // Constructeur vide car la désérialisation JSON instancie d'abord un objet vide avant de remplir les propriétés
        public CommandeDocument() { }

        public CommandeDocument(
            string id,
            DateTime dateCommande,
            double montant,
            int nbExemplaire,
            string idLivreDvd,
            int idSuivi
        ) : base(id, dateCommande, montant)
        {
            NbExemplaire = nbExemplaire;
            IdLivreDvd = idLivreDvd;
            IdSuivi = idSuivi;
        }
    }
}
