using MediaTekDocuments.model;
using MediaTekDocuments.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class UtilisateurTests
    {
        private const int id = 1;
        private const string login = "testUser";
        private const int idService = (int)Service.Administrateur;
        private const string libelleService = "Administrateur";

        [TestMethod]
        public void Utilisateur_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Utilisateur utilisateur = new Utilisateur(id, login, idService, libelleService);

            // Assurer
            Assert.AreEqual(id, utilisateur.Id);
            Assert.AreEqual(login, utilisateur.Login);
            Assert.AreEqual(idService, utilisateur.IdService);
            Assert.AreEqual(libelleService, utilisateur.LibelleService);
            Assert.AreEqual(Service.Administrateur, utilisateur.sonService);
        }

        [TestMethod]
        public void Endpoint_DoitRetournerAuthentification()
        {
            // Arranger
            Utilisateur utilisateur = new Utilisateur(id, login, idService, libelleService);

            // Agir
            string endpoint = utilisateur.Endpoint;

            // Assurer
            Assert.AreEqual("authentification", endpoint);
        }

        [TestMethod]
        public void ToString_DoitRetournerFormatLoginEtService()
        {
            // Arranger
            Utilisateur utilisateur = new Utilisateur(id, login, idService, libelleService);
            string attendu = "testUser (Administrateur)";

            // Agir
            string resultat = utilisateur.ToString();

            // Assurer
            Assert.AreEqual(attendu, resultat);
        }
    }
}