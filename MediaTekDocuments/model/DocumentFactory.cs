using MediaTekDocuments.Dtos;
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
        public static Document Create(TypeMedia type, DocumentDto dto)
        {
            switch (type)
            {
                case TypeMedia.Livre:
                    return new Livre(
                        dto.Id,
                        dto.Titre,
                        dto.Image,
                        dto.Isbn,
                        dto.Auteur,
                        dto.Collection,
                        dto.IdGenre, "",
                        dto.IdPublic, "",
                        dto.IdRayon, ""
                    );

                case TypeMedia.Dvd:
                    return new Dvd(
                        dto.Id,
                        dto.Titre,
                        dto.Image,
                        dto.Duree ?? 0,
                        dto.Realisateur,
                        dto.Synopsis,
                        dto.IdGenre, "",
                        dto.IdPublic, "",
                        dto.IdRayon, ""
                    );

                case TypeMedia.Revue:
                    return new Revue(
                        dto.Id,
                        dto.Titre,
                        dto.Image,
                        dto.IdGenre, "",
                        dto.IdPublic, "",
                        dto.IdRayon, "",
                        dto.Periodicite,
                        dto.DelaiMiseADispo ?? 0
                    );

                default:
                    throw new ArgumentException("TypeMedia inconnu");
            }
        }
    }
}