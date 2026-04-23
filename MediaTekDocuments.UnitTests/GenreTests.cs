using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class GenreTests
    {
        private const string id = "10000";
        private const string libelle = "Science-Fiction";

        [TestMethod]
        public void Genre_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Genre genre = new Genre(id, libelle);

            // Assurer
            Assert.AreEqual(id, genre.Id);
            Assert.AreEqual(libelle, genre.Libelle);
        }

        [TestMethod]
        public void ToString_DoitRetournerLibelle()
        {
            // Arranger
            Genre genre = new Genre(id, libelle);

            // Agir
            string resultat = genre.ToString();

            // Assurer
            Assert.AreEqual(libelle, resultat);
        }
    }
}