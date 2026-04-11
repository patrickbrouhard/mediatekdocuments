using Newtonsoft.Json;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Suivi (état d'une commande document)
    /// </summary>
    public class Suivi
    {
        [JsonProperty("idSuivi")]
        public int IdSuivi { get; }

        [JsonProperty("libelleEtat")]
        public string LibelleEtat { get; }

        public Suivi(int idSuivi, string libelleEtat)
        {
            IdSuivi = idSuivi;
            LibelleEtat = libelleEtat;
        }
    }
}