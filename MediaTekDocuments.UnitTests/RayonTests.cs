using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class RayonTests
    {
        private const string id = "RY001";
        private const string libelle = "Informatique";

        [TestMethod]
        public void Rayon_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Rayon rayon = new Rayon(id, libelle);

            // Assurer
            Assert.AreEqual(id, rayon.Id);
            Assert.AreEqual(libelle, rayon.Libelle);
        }

        [TestMethod]
        public void ToString_DoitRetournerLibelle()
        {
            // Arranger
            Rayon rayon = new Rayon(id, libelle);

            // Agir
            string resultat = rayon.ToString();

            // Assurer
            Assert.AreEqual(libelle, resultat);
        }
    }
}