using static MediaTekDocuments.view.FrmMediatek;
using MediaTekDocuments.model;

namespace MediaTekDocuments.Services
{
    /// <summary>
    /// Classe statique centralisant les droits et autorisations selon le service auquel appartient l'utilisateur.
    /// </summary>
    public static class Permissions
    {
        /// <summary>
        /// Détermine si un utilisateur peut ouvrir l'application.
        /// </summary>
        /// <param name="service">Le service de l'utilisateur concerné.</param>
        /// <returns>True si le service a le droit d'ouvrir l'application (tous sauf Culture), sinon False.</returns>
        public static bool PeutOuvrirApp(Service service)
            => service != Service.Culture;

        /// <summary>
        /// Détermine si un utilisateur a le droit d'apporter des modifications au catalogue.
        /// </summary>
        /// <param name="service">Le service de l'utilisateur concerné.</param>
        /// <returns>True si le service est "Administratif" ou "Administrateur", sinon False.</returns>
        public static bool PeutModifierCatalogue(Service service)
            => service == Service.Administratif || service == Service.Administrateur;

        /// <summary>
        /// Détermine si un utilisateur peut consulter les informations du catalogue.
        /// </summary>
        /// <param name="service">Le service de l'utilisateur concerné.</param>
        /// <returns>True si le service est "Administratif", "Prets" ou "Administrateur", sinon False.</returns>
        public static bool PeutConsulterCatalogue(Service service)
            => service == Service.Administratif
            || service == Service.Prets
            || service == Service.Administrateur;

        /// <summary>
        /// Détermine si un utilisateur a les droits de gestion des commandes.
        /// </summary>
        /// <param name="service">Le service de l'utilisateur concerné.</param>
        /// <returns>True si le service est "Administratif" ou "Administrateur", sinon False.</returns>
        public static bool PeutGererCommandes(Service service)
            => service == Service.Administratif || service == Service.Administrateur;

        /// <summary>
        /// Détermine si un utilisateur a les droits de gestion des prêts.
        /// </summary>
        /// <param name="service">Le service de l'utilisateur concerné.</param>
        /// <returns>True si le service est "Prets" ou "Administrateur", sinon False.</returns>
        public static bool PeutGererPrets(Service service)
            => service == Service.Prets || service == Service.Administrateur;
    }
}
