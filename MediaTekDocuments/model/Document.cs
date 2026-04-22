
using Newtonsoft.Json;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Classe métier Document (réunit les infomations communes à tous les documents : Livre, Revue, Dvd)
    /// </summary>
    public abstract class Document
    {
        [JsonProperty("id")]
        public string Id { get; }
        [JsonProperty("titre")]
        public string Titre { get; }
        [JsonProperty("image")]
        public string Image { get; }
        [JsonProperty("idGenre")]
        public string IdGenre { get; }
        [JsonIgnore]
        public string Genre { get; }
        [JsonProperty("idPublic")]
        public string IdPublic { get; }
        [JsonIgnore]
        public string Public { get; }
        [JsonProperty("idRayon")]
        public string IdRayon { get; }
        [JsonIgnore]
        public string Rayon { get; }
        [JsonIgnore]
        public abstract string Endpoint { get; }

        protected Document(string id, string titre, string image, string idGenre, string genre, string idPublic, string lePublic, string idRayon, string rayon)
        {
            Id = id;
            Titre = titre;
            Image = image;
            IdGenre = idGenre;
            Genre = genre;
            IdPublic = idPublic;
            Public = lePublic;
            IdRayon = idRayon;
            Rayon = rayon;
        }
    }
}
