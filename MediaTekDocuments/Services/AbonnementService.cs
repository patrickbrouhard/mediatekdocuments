using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaTekDocuments.Services
{
    public class AbonnementService
    {
        public bool ParutionDansAbonnement(
            DateTime dateCommande,
            DateTime dateFin,
            DateTime dateParution
            )
        {
            return dateParution >= dateCommande
                && dateParution <= dateFin;
        }

        public bool PeutSupprimerAbonnement(Abonnement abonnement, List<Exemplaire> exemplaires)
        {
            foreach (var ex in exemplaires)
            {
                if (ParutionDansAbonnement(abonnement.DateCommande,
                                           abonnement.DateFinAbonnement,
                                           ex.DateAchat))
                {
                    return false; // un exemplaire tombe dans la période -> suppression interdite
                }
            }

            return true; // aucun exemplaire dans la période -> suppression autorisée
        }


    }
}
