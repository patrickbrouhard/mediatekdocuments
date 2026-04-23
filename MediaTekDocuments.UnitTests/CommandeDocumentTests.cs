using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class CommandeDocumentTests
    {
        private const string id = "00001";
        private readonly DateTime dateCommande = new DateTime(2024, 3, 15);
        private const double montant = 59.99;
        private const int nbExemplaire = 10;
        private const string idLivreDvd = "10001";
        private const int idSuivi = 2;
        private const string libelleSuivi = "Livrée";

        [TestMethod]
        public void CommandeDocument_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi);
            
            // On assigne manuellement la propriété qui n'est pas dans le constructeur
            commandeDocument.LibelleSuivi = libelleSuivi;

            // Assurer
            Assert.AreEqual(id, commandeDocument.Id);
            Assert.AreEqual(dateCommande, commandeDocument.DateCommande);
            Assert.AreEqual(montant, commandeDocument.Montant);
            Assert.AreEqual(nbExemplaire, commandeDocument.NbExemplaire);
            Assert.AreEqual(idLivreDvd, commandeDocument.IdLivreDvd);
            Assert.AreEqual(idSuivi, commandeDocument.IdSuivi);
            Assert.AreEqual(libelleSuivi, commandeDocument.LibelleSuivi);
        }

        [TestMethod]
        public void IdDocument_DoitRetournerIdLivreDvd()
        {
            // Arranger
            CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi);

            // Agir & Assurer
            Assert.AreEqual(idLivreDvd, commandeDocument.IdDocument);
        }

        [TestMethod]
        public void Endpoint_DoitRetournerCommandeDocument()
        {
            // Arranger
            CommandeDocument commandeDocument = new CommandeDocument(id, dateCommande, montant, nbExemplaire, idLivreDvd, idSuivi);

            // Agir
            string endpoint = commandeDocument.Endpoint;

            // Assurer
            Assert.AreEqual("commandedocument", endpoint);
        }
    }
}