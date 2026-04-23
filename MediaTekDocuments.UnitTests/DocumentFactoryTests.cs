using MediaTekDocuments.commands;
using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class DocumentFactoryTests
    {
        [TestMethod]
        public void Creer_Livre_DoitRetournerLivre()
        {
            var cmd = new CreerDocumentCommand
            {
                Type = TypeMedia.Livre,
                Id = "01234",
                Titre = "Test Livre",
                Image = "img.jpg",
                Isbn = "123",
                Auteur = "Auteur",
                Collection = "Collection",
                IdGenre = "G1",
                IdPublic = "P1",
                IdRayon = "R1"
            };

            var result = DocumentFactory.Creer(cmd);

            Assert.IsInstanceOfType(result, typeof(Livre));
        }

        [TestMethod]
        public void Creer_Dvd_DureeNull_DoitMettreZero()
        {
            var cmd = new CreerDocumentCommand
            {
                Type = TypeMedia.Dvd,
                Id = "20001",
                Titre = "Film",
                Image = "img.jpg",
                Duree = null,
                Realisateur = "Spielberg",
                Synopsis = "Test",
                IdGenre = "G1",
                IdPublic = "P1",
                IdRayon = "R1"
            };

            var result = (Dvd)DocumentFactory.Creer(cmd);

            Assert.AreEqual(0, result.Duree);
        }

        [TestMethod]
        public void Creer_Revue_DelaiNull_DoitMettreZero()
        {
            var cmd = new CreerDocumentCommand
            {
                Type = TypeMedia.Revue,
                Id = "10001",
                Titre = "Revue",
                Image = "img.jpg",
                Periodicite = "Mensuelle",
                DelaiMiseADispo = null,
                IdGenre = "G1",
                IdPublic = "P1",
                IdRayon = "R1"
            };

            var result = (Revue)DocumentFactory.Creer(cmd);

            Assert.AreEqual(0, result.DelaiMiseADispo);
        }

        [TestMethod]
        public void Creer_TypeEtIdIncoherent_DoitEtreDetecte()
        {
            var cmd = new CreerDocumentCommand
            {
                Type = TypeMedia.Livre, // Type est Livre
                Id = "20003", // ID de DVD
                Titre = "Incohérent",
                Image = "img.jpg",
                Isbn = "123",
                Auteur = "Auteur",
                Collection = "Collection",
                IdGenre = "G1",
                IdPublic = "P1",
                IdRayon = "R1"
            };

            var result = DocumentFactory.Creer(cmd);

            Assert.IsTrue(result.Id.StartsWith("0"),
                "Un livre doit avoir un ID commençant par 0");
        }
    }
}