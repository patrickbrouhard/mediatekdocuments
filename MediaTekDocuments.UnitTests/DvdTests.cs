using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class DvdTests
    {
        private const string id = "00002";
        private const string titre = "titreDvd";
        private const string image = "imageDvd";
        private const int duree = 120;
        private const string realisateur = "RealisateurTest";
        private const string synopsis = "Synopsis de test";
        private const string idGenre = "10002";
        private const string genre = "Action";
        private const string idPublic = "00003";
        private const string lePublic = "Tous publics";
        private const string idRayon = "DV001";
        private const string rayon = "Rayon de test DVD";

        [TestMethod]
        public void Dvd_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);

            // Assurer
            Assert.AreEqual(id, dvd.Id);
            Assert.AreEqual(titre, dvd.Titre);
            Assert.AreEqual(image, dvd.Image);
            Assert.AreEqual(duree, dvd.Duree);
            Assert.AreEqual(realisateur, dvd.Realisateur);
            Assert.AreEqual(synopsis, dvd.Synopsis);
            Assert.AreEqual(idGenre, dvd.IdGenre);
            Assert.AreEqual(genre, dvd.Genre);
            Assert.AreEqual(idPublic, dvd.IdPublic);
            Assert.AreEqual(lePublic, dvd.Public);
            Assert.AreEqual(idRayon, dvd.IdRayon);
            Assert.AreEqual(rayon, dvd.Rayon);
        }

        [TestMethod]
        public void Endpoint_DoitRetournerDvd()
        {
            // Arranger
            Dvd dvd = new Dvd(id, titre, image, duree, realisateur, synopsis, idGenre, genre, idPublic, lePublic, idRayon, rayon);

            // Agir
            string endpoint = dvd.Endpoint;

            // Assurer
            Assert.AreEqual("dvd", endpoint);
        }
    }
}