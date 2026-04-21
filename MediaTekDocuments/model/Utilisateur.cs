using MediaTekDocuments.Services;
using Newtonsoft.Json;

namespace MediaTekDocuments.model
{
    public class Utilisateur
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("idService")]
        public int IdService { get; set; }

        [JsonProperty("libelleService")]
        public string LibelleService { get; set; }

        [JsonIgnore]
        public string Endpoint => "authentification";

        [JsonIgnore]
        public Service sonService => (Service)IdService;

        public Utilisateur() { }

        public Utilisateur(int id, string login, int idService, string libelleService)
        {
            Id = id;
            Login = login;
            IdService = idService;
            LibelleService = libelleService;
        }

        public override string ToString()
        {
            return $"{Login} ({LibelleService})";
        }

        public bool PeutOuvrirApp()
        {
            return Permissions.PeutOuvrirApp(sonService);
        }

        public bool PeutModifierCatalogue()
        {
            return Permissions.PeutModifierCatalogue(sonService);
        }

        public bool PeutConsulterCatalogue()
        {
            return Permissions.PeutConsulterCatalogue(sonService);
        }

        public bool PeutGererCommandes()
        {
            return Permissions.PeutGererCommandes(sonService);
        }

        public bool PeutGererPrets()
        {
            return Permissions.PeutGererPrets(sonService);
        }
    }
}