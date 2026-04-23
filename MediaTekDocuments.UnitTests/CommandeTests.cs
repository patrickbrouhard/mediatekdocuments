using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.Json.Serialization;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class CommandeTests
    {
        // Classe concrète pour tester Commande car Commande est abstraite
        private class CommandeTestable : Commande
        {
            public override string Endpoint => "test";

            public CommandeTestable() : base() { }

            public CommandeTestable(string id, DateTime dateCommande, double montant)
                : base(id, dateCommande, montant)
            {
            }
        }

        [TestMethod]
        public void Constructeur_AvecParametres_DoitInitialiserLesProprietes()
        {
            string id = "00001";
            DateTime date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
            double montant = 42.5;

            CommandeTestable commande = new CommandeTestable(id, date, montant);

            Assert.AreEqual(id, commande.Id);
            Assert.AreEqual(date, commande.DateCommande);
            Assert.AreEqual(montant, commande.Montant);
        }

        [TestMethod]
        public void Endpoint_DoitEtreDefiniParClasseDerivee()
        {
            CommandeTestable commande = new CommandeTestable();

            string endpoint = commande.Endpoint;

            Assert.AreEqual("test", endpoint);
        }
    }
}