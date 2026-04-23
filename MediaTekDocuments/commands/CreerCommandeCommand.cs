using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.commands
{
    /// <summary>
    /// Représente une commande à créer, incluant les informations nécessaires pour les livres, les DVD ou les
    /// abonnements à des revues.
    /// </summary>
    /// <remarks>Utilisez cette classe pour transmettre les détails d'une nouvelle commande lors de la
    /// création d'une commande de média. Les propriétés pertinentes doivent être renseignées en fonction du type de
    /// média commandé. Par exemple, pour une commande de livre ou de DVD, renseignez les propriétés liées au livre ou
    /// au DVD ; pour un abonnement à une revue, renseignez les propriétés associées à la revue et à la période
    /// d'abonnement.</remarks>
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

        // Revues (Abonnements)
        public DateTime DateFinAbonnement { get; set; }
        public string IdRevue { get; set; }
    }
}
