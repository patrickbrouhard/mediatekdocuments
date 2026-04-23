using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class RevueTests
    {
        private const string id = "00003";
        private const string titre = "titreRevue";
        private const string image = "imageRevue";
        private const string idGenre = "10003";
        private const string genre = "Information";
        private const string idPublic = "00004";
        private const string lePublic = "Adulte";
        private const string idRayon = "RV001";
        private const string rayon = "Rayon de test Revue";
        private const string periodicite = "Mensuelle";
        private const int delaiMiseADispo = 15;

        [TestMethod]
        public void Revue_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);

            // Assurer
            Assert.AreEqual(id, revue.Id);
            Assert.AreEqual(titre, revue.Titre);
            Assert.AreEqual(image, revue.Image);
            Assert.AreEqual(idGenre, revue.IdGenre);
            Assert.AreEqual(genre, revue.Genre);
            Assert.AreEqual(idPublic, revue.IdPublic);
            Assert.AreEqual(lePublic, revue.Public);
            Assert.AreEqual(idRayon, revue.IdRayon);
            Assert.AreEqual(rayon, revue.Rayon);
            Assert.AreEqual(periodicite, revue.Periodicite);
            Assert.AreEqual(delaiMiseADispo, revue.DelaiMiseADispo);
        }

        [TestMethod]
        public void Endpoint_DoitRetournerRevue()
        {
            // Arranger
            Revue revue = new Revue(id, titre, image, idGenre, genre, idPublic, lePublic, idRayon, rayon, periodicite, delaiMiseADispo);

            // Agir
            string endpoint = revue.Endpoint;

            // Assurer
            Assert.AreEqual("revue", endpoint);
        }
    }
}