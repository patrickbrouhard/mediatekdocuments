using MediaTekDocuments.model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class ExemplaireTests
    {
        private const int numero = 42;
        private readonly DateTime dateAchat = new DateTime(2024, 2, 1);
        private const string photo = "url_de_la_photo.jpg";
        private const string idEtat = "00001";
        private const string libelleEtat = "Neuf";
        private const string idDocument = "00005";

        [TestMethod]
        public void Exemplaire_Constructeur_DevraitInitialiserProprietes()
        {
            // Arranger & Agir
            Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, libelleEtat, idDocument);

            // Assurer
            Assert.AreEqual(numero, exemplaire.Numero);
            Assert.AreEqual(dateAchat, exemplaire.DateAchat);
            Assert.AreEqual(photo, exemplaire.Photo);
            Assert.AreEqual(idEtat, exemplaire.IdEtat);
            Assert.AreEqual(libelleEtat, exemplaire.LibelleEtat);
            Assert.AreEqual(idDocument, exemplaire.Id); // Note : le paramètre idDocument assigne la propriété Id
        }
    }
}