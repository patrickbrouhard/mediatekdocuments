using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.Services
{
    /// <summary>
    /// Permet de valider si certaines actions (comme la suppression) sont autorisées
    /// en vérifiant les périodes et les exemplaires associés.
    /// </summary>
    public static class AbonnementService
    {
        /// <summary>
        /// Vérifie si une date de parution est comprise dans la période d'un abonnement.
        /// </summary>
        /// <param name="dateCommande">Date de début (ou date de commande) de l'abonnement.</param>
        /// <param name="dateFin">Date de fin de l'abonnement.</param>
        /// <param name="dateParution">Date de parution à vérifier.</param>
        /// <returns><c>true</c> si la date de parution se situe entre la date de commande et la date de fin (incluses) ; sinon, <c>false</c>.</returns>
        public static bool ParutionDansAbonnement(
            DateTime dateCommande,
            DateTime dateFin,
            DateTime dateParution
            )
        {
            return dateParution >= dateCommande
                && dateParution <= dateFin;
        }

        /// <summary>
        /// Détermine si un abonnement peut être supprimé en vérifiant qu'aucun exemplaire n'a été rattaché durant sa période de validité.
        /// </summary>
        /// <param name="abonnement">L'abonnement que l'on souhaite vérifier.</param>
        /// <param name="exemplaires">La liste des exemplaires associés de la revue concernée.</param>
        /// <returns>true si l'abonnement peut être supprimé (aucun exemplaire dans la période) ; sinon, false.</returns>
        public static bool PeutSupprimerAbonnement(Abonnement abonnement, List<Exemplaire> exemplaires)
        {
            bool ExisteExemplaireDansPeriode = exemplaires.Any(ex => 
            ParutionDansAbonnement(
                abonnement.DateCommande, 
                abonnement.DateFinAbonnement, 
                ex.DateAchat));

            return !ExisteExemplaireDansPeriode; // si un exemplaire existe dans la période, on ne peut pas supprimer l'abonnement
        }
    }
}
