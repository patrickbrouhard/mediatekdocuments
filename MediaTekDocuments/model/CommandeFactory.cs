using MediaTekDocuments.dto;
using System;

namespace MediaTekDocuments.model
{
    public static class CommandeFactory
    {
        public static CommandeDocument Creer(CommandeDocumentDto dto)
        {
            // 1. Création du suivi
            var suivi = new Suivi(dto.IdSuivi, dto.LibelleEtat);

            // 2. Déterminer le type de document
            Document document;

            if (dto.IdDocument.StartsWith("0"))
            {
                // Livre
                document = new Livre(
                    dto.IdDocument,
                    dto.Titre,
                    dto.Image,
                    dto.Isbn,
                    dto.Auteur,
                    dto.Collection,
                    "", dto.Genre,
                    "", dto.Public,
                    "", dto.Rayon
                );
            }
            else
            {
                // DVD
                document = new Dvd(
                    dto.IdDocument,
                    dto.Titre,
                    dto.Image,
                    dto.Duree ?? 0,
                    dto.Realisateur,
                    dto.Synopsis,
                    "", dto.Genre,
                    "", dto.Public,
                    "", dto.Rayon
                );
            }

            // 3. Création de la commande
            return new CommandeDocument(
                dto.IdCommande,
                dto.DateCommande,
                dto.Montant,
                dto.NbExemplaire,
                document,
                suivi
            );
        }
    }
}