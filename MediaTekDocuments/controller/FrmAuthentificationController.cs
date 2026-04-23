using MediaTekDocuments.dal;
using MediaTekDocuments.model;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôle le processus d'authentification des utilisateurs en utilisant l'accès aux données API.
    /// </summary>
    /// <remarks>Cette classe encapsule la logique d'authentification et délègue les opérations d'accès aux
    /// données à une instance unique d'Access. Elle est destinée à être utilisée en interne pour gérer la connexion des
    /// utilisateurs dans l'application.</remarks>
    internal class FrmAuthentificationController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmAuthentificationController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// Authentifie un utilisateur avec les informations de connexion fournies
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Utilisateur Authentifier(string login, string password)
        {
            return access.Authentifier(login, password);
        }
    }
}
