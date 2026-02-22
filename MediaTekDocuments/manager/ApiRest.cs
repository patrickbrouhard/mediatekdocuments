using System;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace MediaTekDocuments.manager
{
    /// <summary>
    /// Classe utilitaire permettant d'interroger une API REST.
    /// Gère les requêtes HTTP (GET, POST, PUT, DELETE) ainsi qu'une éventuelle "basic authorization".
    /// Implémentée sous forme de Singleton pour réutiliser une seule instance de HttpClient.
    /// </summary>
    class ApiRest
    {
        /// <summary>
        /// Instance unique de la classe (pattern Singleton)
        /// </summary>
        private static ApiRest instance = null;
        /// <summary>
        /// Objet de connexion pour envoyer les requêtes à l'API et récupérer les réponses
        /// </summary>
        private readonly HttpClient httpClient;
        /// <summary>
        /// Canal http pour l'envoi du message et la récupération de la réponse
        /// </summary>
        private HttpResponseMessage httpResponse;

        /// <summary>
        /// Constructeur privé pour préparer la connexion (éventuellement sécurisée)
        /// Initialise le HttpClient avec l'URL de base de l'API
        /// et ajoute éventuellement un header "basic authorization".
        /// </summary>
        /// <param name="uriApi">URL de base de l'API</param>
        /// <param name="authenticationString">Chaîne d'authentification au format "login:motdepasse" (optionnel) </param>
        private ApiRest(String uriApi, String authenticationString="")
        {
            httpClient = new HttpClient() { BaseAddress = new Uri(uriApi) };
            // prise en compte dans l'url de l'authentification (basic authorization), si elle n'est pas vide
            if (!String.IsNullOrEmpty(authenticationString))
            {
                String base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(authenticationString));
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + base64EncodedAuthenticationString);
            }
        }

        /// <summary>
        /// Crée une instance unique de la classe
        /// </summary>
        /// <param name="uriApi">adresse de l'api</param>
        /// <param name="authenticationString">chaîne d'authentification (login:pwd)</param>
        /// <returns>Instance unique de ApiRest</returns>
        public static ApiRest GetInstance(String uriApi, String authenticationString)
        {
            if(instance == null)
            {
                instance = new ApiRest(uriApi, authenticationString);
            }
            return instance;
        }

        /// <summary>
        /// Envoie une requête HTTP à l'API et retourne la réponse sous forme de JSON.
        /// </summary>
        /// <param name="methode">verbe http (GET, POST, PUT, DELETE)</param>
        /// <param name="message">message à envoyer dans l'URL</param>
        /// <param name="parametres">contenu de variables à mettre dans body</param>
        /// <returns>liste d'objets (select) ou liste vide (ok) ou null si erreur</returns>
        public JObject RecupDistant(string methode, string message, String parametres)
        {
            // Préparation du contenu à envoyer dans le body (si présent)
            StringContent content = null;
            if(!(parametres is null))
            {
                content = new StringContent(parametres, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
            }
            // Envoi de la requête HTTP en fonction de la méthode choisie
            switch (methode)
            {
                case "GET":
                    httpResponse = httpClient.GetAsync(message).Result;
                    break;
                case "POST":
                    httpResponse = httpClient.PostAsync(message, content).Result;
                    break;
                case "PUT":
                    httpResponse = httpClient.PutAsync(message, content).Result;
                    break;
                case "DELETE":
                    httpResponse = httpClient.DeleteAsync(message).Result;
                    break;
                // Méthode inconnue → retour d'un objet JSON vide
                default:
                    return new JObject();
            }
            // Lecture du contenu de la réponse HTTP (au format texte JSON)
            var json = httpResponse.Content.ReadAsStringAsync().Result;

            // Conversion de la chaîne JSON en objet JObject exploitable
            return JObject.Parse(json);
        }

    }
}
