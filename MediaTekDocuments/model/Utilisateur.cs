using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}