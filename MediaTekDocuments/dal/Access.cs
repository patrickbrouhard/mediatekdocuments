using MediaTekDocuments.commands;
using MediaTekDocuments.manager;
using MediaTekDocuments.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Xml.Linq;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.dal
{
    /// <summary>
    /// Classe d'accès aux données
    /// </summary>
    public class Access
    {
        /// <summary>
        /// adresse de l'API
        /// </summary>
        private static readonly string uriApi = ConfigurationManager.AppSettings["ApiBaseUrl"];
        /// <summary>
        /// instance unique de la classe
        /// </summary>
        private static Access instance = null;
        /// <summary>
        /// instance de ApiRest pour envoyer des demandes vers l'api et recevoir la réponse
        /// </summary>
        private readonly ApiRest api = null;
        /// <summary>
        /// méthode HTTP pour select
        /// </summary>
        private const string GET = "GET";
        /// <summary>
        /// méthode HTTP pour insert
        /// </summary>
        private const string POST = "POST";
        /// <summary>
        /// méthode HTTP pour update
        /// </summary>
        private const string PUT = "PUT";
        /// <summary>
        /// méthode HTTP pour delete
        /// </summary>
        private const string DELETE = "DELETE";
        private const string CHAMPS = "champs=";
        private const string RESULT = "result";

        /// <summary>
        /// méthode HTTP pour update

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            try
            {
                string authenticationString = ConfigurationManager
                    .ConnectionStrings["MediaTekDocuments.Properties.Settings.AuthString"]
                    .ConnectionString;

                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Création et retour de l'instance unique de la classe
        /// </summary>
        /// <returns>instance unique de la classe</returns>
        public static Access GetInstance()
        {
            if (instance == null)
            {
                instance = new Access();
            }
            return instance;
        }

        /// <summary>
        /// Retourne tous les genres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            IEnumerable<Genre> lesGenres = TraitementRecup<Genre>(GET, "genre", null);
            return new List<Categorie>(lesGenres);
        }

        /// <summary>
        /// Retourne tous les rayons à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            IEnumerable<Rayon> lesRayons = TraitementRecup<Rayon>(GET, "rayon", null);
            return new List<Categorie>(lesRayons);
        }

        /// <summary>
        /// Retourne toutes les catégories de public à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            IEnumerable<Public> lesPublics = TraitementRecup<Public>(GET, "public", null);
            return new List<Categorie>(lesPublics);
        }

        /// <summary>
        /// Retourne toutes les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne toutes les dvd à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            List<Dvd> lesDvd = TraitementRecup<Dvd>(GET, "dvd", null);
            return lesDvd;
        }

        /// <summary>
        /// Retourne toutes les revues à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            List<Revue> lesRevues = TraitementRecup<Revue>(GET, "revue", null);
            return lesRevues;
        }

        /// <summary>
        /// Retourne tous les états de suivi
        /// </summary>
        /// <returns>Liste d'objets Suivi</returns>
        public List<Suivi> GetAllSuivis()
        {
            List<Suivi> lesSuivis = TraitementRecup<Suivi>(GET, "suivi", null);
            return lesSuivis;
        }

        /// <summary>
        /// Récupère la liste complète des états disponibles.
        /// </summary>
        /// <returns>Une liste d'objets <see cref="Etat"/> représentant tous les états. La liste est vide s'il n'existe aucun
        /// état.</returns>
        public List<Etat> GetAllEtats()
        {
            List<Etat> lesEtats = TraitementRecup<Etat>(GET, "etat", null);
            return lesEtats;
        }

        /// <summary>
        /// Retourne les exemplaires d'une revue
        /// </summary>
        /// <param name="idDocument">id de la revue concernée</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            Debug.WriteLine("idDocument dans GetExemplairesDocument : " + idDocument);
            String jsonIdDocument = ConvertToJson("id", idDocument);
            Debug.WriteLine(jsonIdDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            Debug.WriteLine($"Nombre d'exemplaires récupérés : {lesExemplaires.Count}");
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion a pu se faire (retour != null)</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            String jsonExemplaire = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());
            try
            {
                List<Exemplaire> liste = TraitementRecup<Exemplaire>(POST, "exemplaire", CHAMPS + jsonExemplaire);
                return (liste != null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
        /// <summary>
        /// Exécute une commande API distante. 
        /// Crée pour pouvoir récupérer le json de l'API et en affichant 
        /// les champs "code" et "result" de la réponse. 
        /// Retourne true si le code de réponse est "200" et que le résultat est supérieur à zéro ; 
        /// sinon, retourne false.
        /// /// <param name="methode">La méthode HTTP à utiliser pour l'appel API, telle que "GET" ou "POST". Ne peut pas être null ou vide.</param>
        /// <param name="endpoint">L'URL ou le chemin de l'endpoint de l'API à appeler. Ne peut pas être null ou vide.</param>
        /// <param name="parametres">Les paramètres à inclure dans la requête API, sous forme de chaîne. Peut être vide si aucun paramètre n'est
        /// requis.</param>
        /// <returns>true si la commande API retourne un code de réponse "200" et un résultat supérieur à zéro ; sinon, false.</returns>
        private bool ExecuteCommande(string methode, string endpoint, string parametres)
        {
            try
            {
                JObject retour = api.RecupDistant(methode, endpoint, parametres);

                string code = (string)retour["code"];

                int result = 0;
                if (retour[RESULT] != null)
                {
                    int.TryParse(retour[RESULT].ToString(), out result);
                }

                Debug.WriteLine($"Code API : {code}");
                Debug.WriteLine($"Résultat API : {result}");

                return code == "200" && result > 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Catch de la méthode Access.ExecuteCommande");
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Ajoute un document à la base de données
        /// </summary>
        /// <remarks>Cette méthode tente de sérialiser et d'envoyer le document à un service distant. Si
        /// une erreur se produit lors de l'opération, la méthode retourne false et aucune exception n'est
        /// propagée.</remarks>
        /// <param name="document">Le document à ajouter. Ne peut pas être null.</param>
        /// <returns>true si le document a été ajouté avec succès ; sinon, false.</returns>
        public bool AjouterDocument(Document document)
        {
            string endpoint = document.Endpoint;
            Debug.WriteLine($"Endpoint pour la création : {endpoint}");

            string json = JsonConvert.SerializeObject(document, new CustomDateTimeConverter());
            Debug.WriteLine($"JSON envoyé pour la création : {json}");

            try
            {
                return ExecuteCommande(POST, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Met à jour un document existant dans la base de données
        /// </summary>
        /// <remarks>Cette méthode tente de mettre à jour le document spécifié via une requête HTTP PUT.
        /// Si une erreur se produit lors de la communication avec l'API ou si la réponse n'est pas valide, la méthode
        /// retourne false.</remarks>
        /// <param name="document">Le document à modifier. Ne peut pas être null. Les propriétés du document déterminent les champs mis à jour.</param>
        /// <returns>true si la modification du document a réussi ; sinon, false.</returns>
        public bool ModifierDocument(Document document)
        {
            string endpoint = $"{document.Endpoint}/{document.Id}";
            Debug.WriteLine($"Endpoint pour la modification : {endpoint}");

            string json = JsonConvert.SerializeObject(document, new CustomDateTimeConverter());
            Debug.WriteLine($"JSON envoyé pour la modification : {json}");

            try
            {
                return ExecuteCommande(PUT, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool SupprimerDocument(Document document)
        {
            string json = JsonConvert.SerializeObject(new
            {
                id = document.Id
            });

            string encodedJson = Uri.EscapeDataString(json);

            string endpoint = $"{document.Endpoint}/{encodedJson}";

            Debug.WriteLine($"Endpoint pour la suppression : {endpoint}");

            try
            {
                return ExecuteCommande(DELETE, endpoint, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Traitement de la récupération du retour de l'api, avec conversion du json en liste pour les select (GET)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methode">verbe HTTP (GET, POST, PUT, DELETE)</param>
        /// <param name="message">information envoyée dans l'url</param>
        /// <param name="parametres">paramètres à envoyer dans le body, au format "chp1=val1&chp2=val2&..."</param>
        /// <returns>liste d'objets récupérés (ou liste vide)</returns>
        private List<T> TraitementRecup<T>(String methode, String message, String parametres)
        {
            // transformation : préparation de la liste qui recevra les objets désérialisés
            List<T> liste = new List<T>();
            try
            {
                JObject retour = api.RecupDistant(methode, message, parametres);
                // extraction du code retourné
                String code = (String)retour["code"];
                if (code.Equals("200"))
                {
                    // dans le cas du GET (select), récupération de la liste d'objets
                    if (methode.Equals(GET))
                    {
                        String resultString = JsonConvert.SerializeObject(retour[RESULT]);
                        // construction de la liste d'objets à partir du retour de l'api
                        liste = JsonConvert.DeserializeObject<List<T>>(resultString, new CustomBooleanJsonConverter());
                    }
                }
                else
                {
                    Console.WriteLine("code erreur = " + code + " message = " + (String)retour["message"]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erreur lors de l'accès à l'API : " + e.Message);
                Environment.Exit(0);
            }
            return liste;
        }

        /// <summary>
        /// Convertit en json un couple nom/valeur
        /// </summary>
        /// <param name="nom"></param>
        /// <param name="valeur"></param>
        /// <returns>couple au format json</returns>
        private static String ConvertToJson(Object nom, Object valeur)
        {
            var dictionary = new Dictionary<object, object>
            {
                { nom, valeur }
            };
            return JsonConvert.SerializeObject(dictionary);
        }

        /// <summary>
        /// Modification du convertisseur Json pour gérer le format de date
        /// </summary>
        private sealed class CustomDateTimeConverter : IsoDateTimeConverter
        {
            public CustomDateTimeConverter()
            {
                base.DateTimeFormat = "yyyy-MM-dd";
            }
        }

        /// <summary>
        /// Modification du convertisseur Json pour prendre en compte les booléens
        /// classe trouvée sur le site :
        /// https://www.thecodebuzz.com/newtonsoft-jsonreaderexception-could-not-convert-string-to-boolean/
        /// </summary>
        private sealed class CustomBooleanJsonConverter : JsonConverter<bool>
        {
            public override bool ReadJson(JsonReader reader, Type objectType, bool existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                return Convert.ToBoolean(reader.ValueType == typeof(string) ? Convert.ToByte(reader.Value) : reader.Value);
            }

            public override void WriteJson(JsonWriter writer, bool value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        #region commandes

        public List<CommandeDocument> GetAllCommandesDocuments(TypeMedia type)
        {
            string endpoint;

            switch (type)
            {
                case TypeMedia.Livre:
                case TypeMedia.Dvd:
                    endpoint = "commandedocument/";
                    break;
                default:
                    throw new ArgumentException("TypeMedia invalide");
            }
            String jsonType = ConvertToJson("typemedia", type.ToString().ToLower());
            Debug.WriteLine(jsonType);
            return TraitementRecup<CommandeDocument>(GET, endpoint + jsonType, null);
        }

        public bool AjouterCommande(Commande commande)
        {
            string json = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());

            Debug.WriteLine("json dans AjouterCommande : " + json);

            try
            {
                return ExecuteCommande(POST, commande.Endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool ModifierCommande(Commande commande)
        {
            string endpoint = $"{commande.Endpoint}/{commande.Id}";
            string json = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());

            Debug.WriteLine("endpoint dans ModifierCommande : " + endpoint);
            Debug.WriteLine("json dans ModifierCommande : " + json);

            try
            {
                return ExecuteCommande(PUT, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool SupprimerCommande(Commande commande)
        {
            string json = JsonConvert.SerializeObject(new { id = commande.Id });
            string encodedJson = Uri.EscapeDataString(json);

            string endpoint = $"{commande.Endpoint}/{encodedJson}";

            try
            {
                return ExecuteCommande(DELETE, endpoint, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion

        #region abonnements

        public List<Abonnement> GetAllAbonnements()
        {
            return TraitementRecup<Abonnement>(GET, "abonnement", null);
        }

        public List<AlerteAbonnement> GetAbonnementsExpirantDans(int jours)
        {
            String jsonJours = ConvertToJson("jours", jours);
            Debug.WriteLine(jsonJours);
            return TraitementRecup<AlerteAbonnement>(GET, "abonnements_expirant_dans/" + jsonJours, null);
        }

        public bool AjouterAbonnement(Abonnement abonnement)
        {
            string json = JsonConvert.SerializeObject(abonnement, new CustomDateTimeConverter());
            try
            {
                return ExecuteCommande(POST, "abonnement", CHAMPS + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            string json = JsonConvert.SerializeObject(new { id = abonnement.Id });
            string encodedJson = Uri.EscapeDataString(json);
            string endpoint = $"abonnement/{encodedJson}";
            try
            {
                return ExecuteCommande(DELETE, endpoint, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion

        #region exemplaires

        /// <summary>
        /// Modifie l'exemplaire spécifié dans la collection ou la base de données distante.
        /// </summary>
        /// <param name="exemplaire">L'exemplaire à modifier. Ne peut pas être null et doit contenir un identifiant valide.</param>
        /// <returns>true si l'exemplaire a été modifié avec succès ; sinon, false.</returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            string endpoint = $"exemplaire/{exemplaire.Id}";
            string json = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());

            Debug.WriteLine($"Endpoint pour la modification de l'exemplaire : {endpoint}");
            Debug.WriteLine($"JSON envoyé pour la modification de l'exemplaire : {json}");

            try
            {
                return ExecuteCommande(PUT, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Supprime l'exemplaire spécifié de la collection ou de la base de données distante.
        /// </summary>
        /// <param name="exemplaire">L'exemplaire à supprimer. Ne peut pas être null et doit contenir un identifiant valide.</param>
        /// <returns>true si l'exemplaire a été supprimé avec succès ; sinon, false.</returns>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            string json = JsonConvert.SerializeObject(new
            {
                id = exemplaire.Id,
                numero = exemplaire.Numero
            });
            string encodedJson = Uri.EscapeDataString(json);
            string endpoint = $"exemplaire/{encodedJson}";

            Debug.WriteLine($"JSON pour la suppression de l'exemplaire : {json}");

            Debug.WriteLine($"Endpoint pour la suppression de l'exemplaire : {endpoint}");
            try
            {
                return ExecuteCommande(DELETE, endpoint, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        #endregion

        /// <summary>
        /// Authentifie un utilisateur à partir de son login et de son mot de passe
        /// Méthode spéciale car elle ne suit pas le même format que les autres (POST spécial)
        /// </summary>
        /// <param name="login">Login de l'utilisateur</param>
        /// <param name="password">Mot de passe de l'utilisateur</param>
        /// <returns>Objet Utilisateur contenant les informations de l'utilisateur si l'authentification réussit, sinon null</returns>
        public Utilisateur Authentifier(string login, string password)
        {
            string json = JsonConvert.SerializeObject(new
            {
                login,
                password
            });

            try
            {
                JObject retour = api.RecupDistant(POST, "authentification", CHAMPS + json);

                string code = (string)retour["code"];

                if (code == "200")
                {
                    string resultString = JsonConvert.SerializeObject(retour[RESULT]);
                    Utilisateur user = JsonConvert.DeserializeObject<Utilisateur>(resultString);
                    return user;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Erreur Authentification : " + ex.Message);
            }

            return null;
        }
    }
}
