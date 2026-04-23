using MediaTekDocuments.commands;
using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class CommandeFactoryTests
    {
        [TestMethod]
        public void Creer_Livre_DoitRetournerCommandeDocument()
        {
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Livre,
                Id = "001",
                DateCommande = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                Montant = 100,
                NbExemplaire = 2,
                IdLivreDvd = "00001",
                IdSuivi = 1
            };

            var result = CommandeFactory.Creer(cmd);

            Assert.IsInstanceOfType(result, typeof(CommandeDocument));
        }

        [TestMethod]
        public void Creer_Dvd_DoitRetournerCommandeDocument()
        {
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Dvd,
                Id = "002",
                DateCommande = DateTime.Now,
                Montant = 50,
                NbExemplaire = 1,
                IdLivreDvd = "20001",
                IdSuivi = 1
            };

            var result = CommandeFactory.Creer(cmd);

            Assert.IsInstanceOfType(result, typeof(CommandeDocument));
        }

        [TestMethod]
        public void Creer_Revue_DoitRetournerAbonnement()
        {
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Revue,
                Id = "003",
                DateCommande = DateTime.Now,
                Montant = 75,
                DateFinAbonnement = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                IdRevue = "10001"
            };

            var result = CommandeFactory.Creer(cmd);

            Assert.IsInstanceOfType(result, typeof(Abonnement));
        }

        [TestMethod]
        public void Creer_CommandeDocument_DoitMapperLesProprietes()
        {
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Livre,
                Id = "004",
                DateCommande = new DateTime(2024, 2, 1, 0, 0, 0, DateTimeKind.Unspecified),
                Montant = 120,
                NbExemplaire = 5,
                IdLivreDvd = "00002",
                IdSuivi = 1
            };

            var result = (CommandeDocument)CommandeFactory.Creer(cmd);

            Assert.AreEqual(cmd.Id, result.Id);
            Assert.AreEqual(cmd.DateCommande, result.DateCommande);
            Assert.AreEqual(cmd.Montant, result.Montant);
            Assert.AreEqual(cmd.NbExemplaire, result.NbExemplaire);
            Assert.AreEqual(cmd.IdLivreDvd, result.IdLivreDvd);
            Assert.AreEqual(cmd.IdSuivi, result.IdSuivi);
        }

        [TestMethod]
        public void Creer_Abonnement_DoitMapperLesProprietes()
        {
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Revue,
                Id = "D005",
                DateCommande = new DateTime(2024, 3, 1, 0, 0, 0, DateTimeKind.Unspecified),
                Montant = 200,
                DateFinAbonnement = new DateTime(2025, 3, 1, 0, 0, 0, DateTimeKind.Unspecified),
                IdRevue = "10002"
            };

            var result = (Abonnement)CommandeFactory.Creer(cmd);

            Assert.AreEqual(cmd.Id, result.Id);
            Assert.AreEqual(cmd.DateCommande, result.DateCommande);
            Assert.AreEqual(cmd.Montant, result.Montant);
            Assert.AreEqual(cmd.DateFinAbonnement, result.DateFinAbonnement);
            Assert.AreEqual(cmd.IdRevue, result.IdRevue);
        }

        [TestMethod]
        public void Creer_Livre_AvecIdRevue_NeDevraitPasEtrePossible()
        {
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Livre,
                Id = "006",
                IdRevue = "10001" // incohérent
            };

            var result = CommandeFactory.Creer(cmd);

            Assert.IsNull((result as CommandeDocument).IdLivreDvd,
                "Une commande de livre ne devrait pas contenir d'id revue");
        }
    }
}