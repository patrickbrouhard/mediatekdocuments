using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class PublicTests
    {
        private const string id = "00001";
        private const string libelle = "Jeunesse";

        [TestMethod]
        public void Public_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Public lePublic = new Public(id, libelle);

            // Assurer
            Assert.AreEqual(id, lePublic.Id);
            Assert.AreEqual(libelle, lePublic.Libelle);
        }

        [TestMethod]
        public void ToString_DoitRetournerLibelle()
        {
            // Arranger
            Public lePublic = new Public(id, libelle);

            // Agir
            string resultat = lePublic.ToString();

            // Assurer
            Assert.AreEqual(libelle, resultat);
        }
    }
}