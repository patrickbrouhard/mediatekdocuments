using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json.Serialization;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class AbonnementTests
    {
        [TestMethod]
        public void Constructeur_AvecParametres_DoitInitialiserLesProprietes()
        {
            string id = "00001";
            DateTime dateCommande = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            double montant = 99.99;
            DateTime dateFin = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            string idRevue = "10007";

            Abonnement abonnement = new Abonnement(id, dateCommande, montant, dateFin, idRevue);

            Assert.AreEqual(id, abonnement.Id);
            Assert.AreEqual(dateCommande, abonnement.DateCommande);
            Assert.AreEqual(montant, abonnement.Montant);
            Assert.AreEqual(dateFin, abonnement.DateFinAbonnement);
            Assert.AreEqual(idRevue, abonnement.IdRevue);
        }

        [TestMethod]
        public void Endpoint_DoitRetournerAbonnement()
        {
            Abonnement abonnement = new Abonnement();

            string endpoint = abonnement.Endpoint;

            Assert.AreEqual("abonnement", endpoint);
        }
    }
}