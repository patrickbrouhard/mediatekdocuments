using MediaTekDocuments.commands;
using System;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Fournit des méthodes utilitaires pour créer des instances de commandes à partir de paramètres fournis.
    /// </summary>
    /// <remarks>Cette classe ne peut pas être instanciée et expose uniquement des membres statiques. Elle
    /// centralise la logique de création des différents types de commandes selon le type de média spécifié.</remarks>
    public static class CommandeFactory
    {
        /// <summary>
        /// Crée une nouvelle commande en fonction du type de média spécifié dans la commande.
        /// </summary>
        /// <param name="cmd">La commande à traiter, contenant les informations nécessaires à la création d'une commande. Ne peut pas être
        /// null et doit contenir un type de média valide.</param>
        /// <returns>Une instance de Commande correspondant au type de média spécifié dans la commande. Retourne un objet
        /// CommandeDocument pour les livres ou DVD, ou un objet Abonnement pour les revues.</returns>
        /// <exception cref="ArgumentException">Levée si le type de média spécifié dans la commande n'est pas pris en charge.</exception>
        public static Commande Creer(CreerCommandeCommand cmd)
        {
            VerifierCoherence(cmd);

            switch (cmd.Type)
            {
                case TypeMedia.Livre:
                case TypeMedia.Dvd:
                    return new CommandeDocument(
                        cmd.Id,
                        cmd.DateCommande,
                        cmd.Montant,
                        cmd.NbExemplaire,
                        cmd.IdLivreDvd,
                        cmd.IdSuivi
                    );
                case TypeMedia.Revue:
                    return new Abonnement(
                        cmd.Id,
                        cmd.DateCommande,
                        cmd.Montant,
                        cmd.DateFinAbonnement,
                        cmd.IdRevue
                    );
                default:
                    throw new ArgumentException("Type de média non supporté");
            } 
        }

        /// <summary>
        /// Vérifie la cohérence des identifiants de média dans la commande spécifiée en fonction du type de média.
        /// </summary>
        /// <param name="cmd">La commande à valider. Doit contenir des identifiants appropriés selon le type de média.</param>
        /// <exception cref="ArgumentException">Levée si les identifiants de média ne sont pas cohérents avec le type de média spécifié dans la commande.</exception>
        private static void VerifierCoherence(CreerCommandeCommand cmd)
        {
            if (cmd.Type == TypeMedia.Revue)
            {
                if (string.IsNullOrEmpty(cmd.IdRevue))
                    throw new ArgumentException("IdRevue requis pour une revue");
            }
            else
            {
                if (!string.IsNullOrEmpty(cmd.IdRevue))
                    throw new ArgumentException("IdRevue ne doit pas être défini pour un livre ou DVD");

                if (string.IsNullOrEmpty(cmd.IdLivreDvd))
                    throw new ArgumentException("IdLivreDvd requis pour livre ou DVD");
            }
        }
    }
}