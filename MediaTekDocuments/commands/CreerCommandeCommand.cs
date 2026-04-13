using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.commands
{
    public class CreerCommandeCommand
    {
        public TypeMedia Type { get; set; }

        public string Id { get; set; }
        public DateTime DateCommande { get; set; }
        public double Montant { get; set; }

        // Livres et DVDs
        public int NbExemplaire { get; set; }
        public string IdLivreDvd { get; set; }
        public int IdSuivi { get; set; }
    }
}
