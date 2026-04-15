using MediaTekDocuments.commands;
using System;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.model
{
    public static class CommandeFactory
    {
        public static Commande Creer(CreerCommandeCommand cmd)
        {
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

                default:
                    throw new ArgumentException("Type de média non supporté");
            } 
        }
    }
}