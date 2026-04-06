using MediaTekDocuments.commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.model
{
    /// <summary>
    /// Fournit les méthodes pour créer des instances de documents en fonction du type de média.
    /// </summary>
    /// <remarks>Centralise la création des objets de type Livre, Dvd ou Revue selon le
    /// paramètre TypeMedia fourni. Elle simplifie l'instanciation en utilisant
    /// le Factory Pattern : https://www.youtube.com/watch?v=BJatgOiiht4&t=1199s</remarks>
    public static class DocumentFactory
    {
        public static Document Creer(CreerDocumentCommand cmd)
        {
            switch (cmd.Type)
            {
                case TypeMedia.Livre:
                    return new Livre(
                        cmd.Id,
                        cmd.Titre,
                        cmd.Image,
                        cmd.Isbn,
                        cmd.Auteur,
                        cmd.Collection,
                        cmd.IdGenre, "",
                        cmd.IdPublic, "",
                        cmd.IdRayon, ""
                    );

                case TypeMedia.Dvd:
                    return new Dvd(
                        cmd.Id,
                        cmd.Titre,
                        cmd.Image,
                        cmd.Duree ?? 0,
                        cmd.Realisateur,
                        cmd.Synopsis,
                        cmd.IdGenre, "",
                        cmd.IdPublic, "",
                        cmd.IdRayon, ""
                    );

                case TypeMedia.Revue:
                    return new Revue(
                        cmd.Id,
                        cmd.Titre,
                        cmd.Image,
                        cmd.IdGenre, "",
                        cmd.IdPublic, "",
                        cmd.IdRayon, "",
                        cmd.Periodicite,
                        cmd.DelaiMiseADispo ?? 0
                    );

                default:
                    throw new ArgumentException("TypeMedia inconnu");
            }
        }
    }
}