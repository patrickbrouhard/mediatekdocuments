using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class LivreTests
    {
        private const string id = "00001";
        private const string titre = "titreLivre";
        private const string image = "imageLivre";
        private const string isbn = "123456789";
        private const string auteur = "AuteurTest";
        private const string collection = "CollectionTest";
        private const string idGenre = "10001";
        private const string genre = "Humour";
        private const string idPublic = "00002";
        private const string lePublic = "Adulte";
        private const string idRayon = "LV001";
        private const string rayon = "Rayon de test";

        [TestMethod]
        public void Livre_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, lePublic, idRayon, rayon);

            // Assurer
            Assert.AreEqual(id, livre.Id);
            Assert.AreEqual(titre, livre.Titre);
            Assert.AreEqual(image, livre.Image);
            Assert.AreEqual(isbn, livre.Isbn);
            Assert.AreEqual(auteur, livre.Auteur);
            Assert.AreEqual(collection, livre.Collection);
            Assert.AreEqual(idGenre, livre.IdGenre);
            Assert.AreEqual(genre, livre.Genre);
            Assert.AreEqual(idPublic, livre.IdPublic);
            Assert.AreEqual(lePublic, livre.Public);
            Assert.AreEqual(idRayon, livre.IdRayon);
            Assert.AreEqual(rayon, livre.Rayon);
        }

        [TestMethod]
        public void Endpoint_DoitRetournerLivre()
        {
            // Arranger
            Livre livre = new Livre(id, titre, image, isbn, auteur, collection, idGenre, genre, idPublic, lePublic, idRayon, rayon);

            // Agir
            string endpoint = livre.Endpoint;

            // Assurer
            Assert.AreEqual("livre", endpoint);
        }
    }
}