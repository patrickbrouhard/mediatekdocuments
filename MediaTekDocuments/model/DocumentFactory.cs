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
        /// <summary>
        /// Crée une nouvelle instance de document correspondant au type spécifié dans la commande.
        /// </summary>
        /// <remarks>Le type de document créé dépend de la valeur de la propriété Type de la commande. Les
        /// types pris en charge sont Livre, Dvd et Revue.</remarks>
        /// <param name="cmd">La commande contenant les informations nécessaires à la création du document, y compris l'identifiant, le
        /// type de média et les propriétés associées. Ne peut pas être null. Le type de média doit être pris en charge.</param>
        /// <returns>Une instance de la classe dérivée de Document correspondant au type de média spécifié dans la commande.</returns>
        /// <exception cref="ArgumentException">Levée si le type de média spécifié dans la commande n'est pas reconnu.</exception>
        public static Document Creer(CreerDocumentCommand cmd)
        {
            VerifierCoherence(cmd.Id, cmd.Type);

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

        /// <summary>
        /// Vérifie la cohérence entre l'identifiant fourni et le type de média spécifié.
        /// </summary>
        /// <param name="id">L'identifiant du média à vérifier. Peut être null ou vide lors de la création d'un nouveau document.</param>
        /// <param name="type">Le type de média auquel l'identifiant doit correspondre.</param>
        /// <exception cref="ArgumentException">Levée si l'identifiant ne commence pas par le chiffre correspondant au type de média spécifié.</exception>
        private static void VerifierCoherence(string id, TypeMedia type)
        {
            // Normal si c'est une création de document : id sera généré par l'API
            if (string.IsNullOrEmpty(id))
                return;

            // Vérifie que l'ID commence par le chiffre correspondant au type de média
            if ((type == TypeMedia.Livre && !id.StartsWith("0")) ||
                (type == TypeMedia.Revue && !id.StartsWith("1")) ||
                (type == TypeMedia.Dvd && !id.StartsWith("2")))
            {
                throw new ArgumentException("Incohérence entre le type et l'ID");
            }
        }
    }
}