using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class AlerteAbonnementTests
    {
        [TestMethod]
        public void AlerteAbonnement_SetEtGetProprietes_DevraientEtreCorrects()
        {
            // Arranger
            AlerteAbonnement alerte = new AlerteAbonnement();
            string expectedId = "10001";
            string expectedTitre = "Titre de la revue";
            DateTime expectedDateFin = new DateTime(2025, 1, 1);

            // Agir
            alerte.Id = expectedId;
            alerte.Titre = expectedTitre;
            alerte.DateFinAbonnement = expectedDateFin;

            // Assurer
            Assert.AreEqual(expectedId, alerte.Id);
            Assert.AreEqual(expectedTitre, alerte.Titre);
            Assert.AreEqual(expectedDateFin, alerte.DateFinAbonnement);
        }
    }
}