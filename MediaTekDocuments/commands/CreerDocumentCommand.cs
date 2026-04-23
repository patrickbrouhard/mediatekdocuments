using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.commands
{
    /// <summary>
    /// Représente une commande pour créer un document, incluant les informations nécessaires selon le type de média
    /// (livre, DVD ou revue).
    /// </summary>
    /// <remarks>Utilisez cette classe pour transmettre toutes les données requises lors de la création d'un
    /// nouveau document dans le système. Certains champs sont spécifiques à un type de média et peuvent rester non
    /// renseignés pour les autres types. Par exemple, les propriétés 'Isbn', 'Auteur' et 'Collection' concernent
    /// uniquement les livres, tandis que 'Duree', 'Realisateur' et 'Synopsis' sont propres aux DVD. 'Periodicite' et
    /// 'DelaiMiseADispo' sont utilisés pour les revues.</remarks>
    public class CreerDocumentCommand
    {
        public TypeMedia Type { get; set; }

        public string Id { get; set; }
        public string Titre { get; set; }
        public string Image { get; set; }

        public string IdGenre { get; set; }
        public string IdPublic { get; set; }
        public string IdRayon { get; set; }

        // Livre
        public string Isbn { get; set; }
        public string Auteur { get; set; }
        public string Collection { get; set; }

        // DVD
        public int? Duree { get; set; }
        public string Realisateur { get; set; }
        public string Synopsis { get; set; }

        // Revue
        public string Periodicite { get; set; }
        public int? DelaiMiseADispo { get; set; }
    }
}
