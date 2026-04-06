using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.commands
{
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
