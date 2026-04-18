using MediaTekDocuments.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace MediaTekDocuments.UnitTests
{
    [TestClass]
    public class AbonnementServiceTests
    {
        private AbonnementService service;

        [TestInitialize]
        public void Init()
        {
            service = new AbonnementService();
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateDansIntervalle_RetourneTrue()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1),
                new DateTime(2024, 12, 31),
                new DateTime(2024, 6, 1) // date de parution dans l'intervalle
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateSurUneBorne_RetourneTrue()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1),
                new DateTime(2024, 12, 31),
                new DateTime(2024, 1, 1) // date de parution égale à la date de commande
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateAvantCommande_RetourneFalse()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1),
                new DateTime(2024, 12, 31),
                new DateTime(2023, 12, 31) // date de parution avant la date de commande
            );

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateApresFin_RetourneFalse()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1),
                new DateTime(2024, 12, 31),
                new DateTime(2025, 1, 1) // date de parution après la date de fin
            );

            Assert.IsFalse(result);
        }
    }
}
