using MediaTekDocuments.commands;
using MediaTekDocuments.manager;
using MediaTekDocuments.model;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
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
        /// Représente l'URL de l'API à laquelle la classe Access se connecte.
        /// </summary>
        private static readonly string uriApi;
        /// <summary>
        /// Représente la source de l'API utilisée par la classe Access. Utilisée pour l'UI.
        /// </summary>
        private static readonly string ApiSource;

        /// <summary>
        /// Initialise les variables statiques uriApi et ApiSource en fonction de 
        /// la configuration de l'application et du mode de compilation (debug ou release). 
        /// Les valeurs sont récupérées à partir du fichier de configuration de l'application, 
        /// ce qui permet de changer facilement l'URL de l'API sans modifier le code.
        static Access()
        {
        #if DEBUG
            uriApi = ConfigurationManager.AppSettings["ApiLocalUrl"];
            ApiSource = "MDK-86 Local";
        #else
            uriApi = ConfigurationManager.AppSettings["ApiOnlineUrl"];
            ApiSource = "MDK-86 Online";
        #endif
        }

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
        /// <summary>
        /// nom du paramètre pour les données envoyées en POST/PUT
        /// </summary>
        private const string CHAMPS = "champs=";
        /// <summary>
        /// champ contenant le résultat dans le JSON retourné
        /// </summary>
        private const string RESULT = "result";

        /// <summary>
        /// Méthode privée pour créer un singleton
        /// initialise l'accès à l'API
        /// </summary>
        private Access()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console()
                    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
                    .WriteTo.File("logs/errorlog.txt",
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                    .CreateLogger();

                string authenticationString = ConfigurationManager
                    .ConnectionStrings["MediaTekDocuments.Properties.Settings.AuthString"]
                    .ConnectionString;

                api = ApiRest.GetInstance(uriApi, authenticationString);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Erreur fatale lors de l'initialisation de l'API REST.");
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// Récupère la source de l'API à partir du fichier de configuration de l'application.
        /// </summary>
        /// <returns>Un simple string représentant la source de l'API.</returns>
        public string GetApiSource()
        {
            return ApiSource;
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
        /// Retourne tous les livres à partir de la BDD
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            List<Livre> lesLivres = TraitementRecup<Livre>(GET, "livre", null);
            return lesLivres;
        }

        /// <summary>
        /// Retourne tous les dvd à partir de la BDD
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
        /// <returns>Une liste d'objets <see cref="Etat"/> représentant tous les états.</returns>
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
            Log.Debug("Récupération des exemplaires pour le document {IdDocument}", idDocument);
            String jsonIdDocument = ConvertToJson("id", idDocument);
            List<Exemplaire> lesExemplaires = TraitementRecup<Exemplaire>(GET, "exemplaire/" + jsonIdDocument, null);
            Log.Debug("{Count} exemplaires récupérés pour le document {IdDocument}", lesExemplaires.Count, idDocument);
            return lesExemplaires;
        }

        /// <summary>
        /// ecriture d'un exemplaire en base de données
        /// </summary>
        /// <param name="exemplaire">exemplaire à insérer</param>
        /// <returns>true si l'insertion s'est bien déroulée</returns>
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
                Log.Error(ex, "Erreur lors de la création de l'exemplaire.");
            }
            return false;
        }
        
        /// <summary>
        /// Exécute une commande API distante et analyse la réponse JSON.
        /// </summary>
        /// <param name="methode">La méthode HTTP à utiliser pour l'appel API (GET, POST, etc.)</param>
        /// <param name="endpoint">L'URL ou le chemin de l'endpoint de l'API à appeler</param>
        /// <param name="parametres">Les paramètres à inclure dans la requête API</param>
        /// <returns>true si la commande API retourne un code de réponse "200" et un résultat supérieur à zéro, sinon false.</returns>
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

                return code == "200" && result > 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'exécution de la commande {Methode} sur {Endpoint}", methode, endpoint);
                return false;
            }
        }

        /// <summary>
        /// Ajoute un document à la base de données
        /// </summary>
        /// <param name="document">Le document à ajouter.</param>
        /// <returns>true si le document a été ajouté avec succès ; sinon, false.</returns>
        public bool AjouterDocument(Document document)
        {
            string endpoint = document.Endpoint;
            string json = JsonConvert.SerializeObject(document, new CustomDateTimeConverter());
            
            try
            {
                return ExecuteCommande(POST, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'ajout du document.");
                return false;
            }
        }

        /// <summary>
        /// Met à jour un document existant dans la base de données via une requête HTTP PUT.
        /// </summary>
        /// <param name="document">Le document à modifier.</param>
        /// <returns>true si la modification a réussi ; sinon, false.</returns>
        public bool ModifierDocument(Document document)
        {
            string endpoint = $"{document.Endpoint}/{document.Id}";
            string json = JsonConvert.SerializeObject(document, new CustomDateTimeConverter());

            try
            {
                return ExecuteCommande(PUT, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la modification du document {Id}.", document.Id);
                return false;
            }
        }

        /// <summary>
        /// Supprime un document de la base de données via une requête HTTP DELETE.
        /// </summary>
        /// <param name="document">Le document à supprimer.</param>
        /// <returns>true si la suppression a réussi ; sinon, false.</returns>
        public bool SupprimerDocument(Document document)
        {
            string json = JsonConvert.SerializeObject(new
            {
                id = document.Id
            });

            string encodedJson = Uri.EscapeDataString(json);
            string endpoint = $"{document.Endpoint}/{encodedJson}";

            try
            {
                return ExecuteCommande(DELETE, endpoint, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la suppression du document {Id}.", document.Id);
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
                    Log.Error("code erreur = {Code} message = {Message}", code, (string)retour["message"]);
                }
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Erreur fatale lors de l'accès à l'API.");
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

        /// <summary>
        /// Récupère toutes les commandes liées à un type de media (livre ou dvd)
        /// </summary>
        /// <param name="type">Type de media concerné</param>
        /// <returns>Liste des commandes de documents</returns>
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
            return TraitementRecup<CommandeDocument>(GET, endpoint + jsonType, null);
        }

        /// <summary>
        /// Ajoute une commande en base de données.
        /// </summary>
        /// <param name="commande">L'objet commande à insérer</param>
        /// <returns>true si l'opération a réussi, false sinon</returns>
        public bool AjouterCommande(Commande commande)
        {
            string json = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());

            try
            {
                return ExecuteCommande(POST, commande.Endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'ajout de la commande.");
                return false;
            }
        }

        /// <summary>
        /// Modifie une commande existante en base de données.
        /// </summary>
        /// <param name="commande">La commande modifiée</param>
        /// <returns>true si la modification a réussi, false sinon</returns>
        public bool ModifierCommande(Commande commande)
        {
            string endpoint = $"{commande.Endpoint}/{commande.Id}";
            string json = JsonConvert.SerializeObject(commande, new CustomDateTimeConverter());

            try
            {
                return ExecuteCommande(PUT, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la modification de la commande {Id}.", commande.Id);
                return false;
            }
        }

        /// <summary>
        /// Supprime une commande de la base de données.
        /// </summary>
        /// <param name="commande">La commande à supprimer</param>
        /// <returns>true si la suppression a réussi, false sinon</returns>
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
                Log.Error(ex, "Erreur lors de la suppression de la commande {Id}.", commande.Id);
                return false;
            }
        }

        #endregion

        #region abonnements

        /// <summary>
        /// Récupère la liste de tous les abonnements.
        /// </summary>
        /// <returns>Liste des abonnements</returns>
        public List<Abonnement> GetAllAbonnements()
        {
            return TraitementRecup<Abonnement>(GET, "abonnement", null);
        }

        /// <summary>
        /// Récupère les alertes pour les abonnements expirant dans un délai précis.
        /// </summary>
        /// <param name="jours">Nombre de jours avant l'expiration</param>
        /// <returns>Liste d'alertes d'abonnement</returns>
        public List<AlerteAbonnement> GetAbonnementsExpirantDans(int jours)
        {
            String jsonJours = ConvertToJson("jours", jours);
            return TraitementRecup<AlerteAbonnement>(GET, "abonnements_expirant_dans/" + jsonJours, null);
        }

        /// <summary>
        /// Ajoute un nouvel abonnement en base de données.
        /// </summary>
        /// <param name="abonnement">L'abonnement à rajouter</param>
        /// <returns>true si l'ajout a réussi, false sinon</returns>
        public bool AjouterAbonnement(Abonnement abonnement)
        {
            string json = JsonConvert.SerializeObject(abonnement, new CustomDateTimeConverter());
            try
            {
                return ExecuteCommande(POST, "abonnement", CHAMPS + json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de l'ajout de l'abonnement.");
                return false;
            }
        }

        /// <summary>
        /// Supprime un abonnement de la base de données.
        /// </summary>
        /// <param name="abonnement">L'abonnement à supprimer</param>
        /// <returns>true si la suppression a réussi, false sinon</returns>
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
                Log.Error(ex, "Erreur lors de la suppression de l'abonnement {Id}.", abonnement.Id);
                return false;
            }
        }

        #endregion

        #region exemplaires

        /// <summary>
        /// Modifie un exemplaire dans la base de données distante.
        /// </summary>
        /// <param name="exemplaire">L'exemplaire à modifier.</param>
        /// <returns>true si l'exemplaire a été modifié avec succès ; sinon, false.</returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            string endpoint = $"exemplaire/{exemplaire.Id}";
            string json = JsonConvert.SerializeObject(exemplaire, new CustomDateTimeConverter());

            try
            {
                return ExecuteCommande(PUT, endpoint, CHAMPS + json);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la modification de l'exemplaire {Id}.", exemplaire.Id);
                return false;
            }
        }

        /// <summary>
        /// Supprime un exemplaire de la base de données distante.
        /// </summary>
        /// <param name="exemplaire">L'exemplaire à supprimer.</param>
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

            try
            {
                return ExecuteCommande(DELETE, endpoint, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur lors de la suppression de l'exemplaire {Id} (Numéro : {Numero}).", exemplaire.Id, exemplaire.Numero);
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
                Log.Error(ex, "Erreur lors de l'authentification de l'utilisateur {Login}.", login);
            }

            return null;
        }
    }
}
