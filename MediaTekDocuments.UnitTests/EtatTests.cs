using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class EtatTests
    {
        private const string id = "00001";
        private const string libelle = "Neuf";

        [TestMethod]
        public void Etat_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Etat etat = new Etat(id, libelle);

            // Assurer
            Assert.AreEqual(id, etat.Id);
            Assert.AreEqual(libelle, etat.Libelle);
        }
    }
}