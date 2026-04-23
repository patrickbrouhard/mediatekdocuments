using MediaTekDocuments.commands;
using System;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.model
{
    public static class CommandeFactory
    {
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