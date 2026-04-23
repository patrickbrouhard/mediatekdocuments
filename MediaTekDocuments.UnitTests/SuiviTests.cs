using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class SuiviTests
    {
        private const int idSuivi = 1;
        private const string libelleEtat = "En cours";

        [TestMethod]
        public void Suivi_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Suivi suivi = new Suivi(idSuivi, libelleEtat);

            // Assurer
            Assert.AreEqual(idSuivi, suivi.IdSuivi);
            Assert.AreEqual(libelleEtat, suivi.LibelleEtat);
        }
    }
}