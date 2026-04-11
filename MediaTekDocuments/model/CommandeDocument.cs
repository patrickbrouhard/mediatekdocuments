using Newtonsoft.Json;
using System;

namespace MediaTekDocuments.model
{
    public class CommandeDocument : Commande
    {
        [JsonProperty("nbExemplaire")]
        public int NbExemplaire { get; }

        public Document Document { get; }

        public Suivi Suivi { get; }

        [JsonIgnore]
        public string IdDocument => Document?.Id; // Propriété calculée pour le datagridview

        [JsonIgnore]
        public string LibelleSuivi => Suivi?.LibelleEtat; // Propriété calculée pour le datagridview

        [JsonIgnore]
        public override string Endpoint => "commandedocument";

        public CommandeDocument(
            string id,
            DateTime dateCommande,
            double montant,
            int nbExemplaire,
            Document document,
            Suivi suivi
        ) : base(id, dateCommande, montant)
        {
            NbExemplaire = nbExemplaire;
            Document = document;
            Suivi = suivi;
        }
    }
}