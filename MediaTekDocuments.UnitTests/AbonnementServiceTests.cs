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
        private readonly AbonnementService service = new AbonnementService();

        [TestMethod]
        public void ParutionDansAbonnement_DateDansIntervalle_RetourneTrue()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2024, 6, 1, 0, 0, 0, DateTimeKind.Unspecified) // date de parution dans l'intervalle
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateSurUneBorne_RetourneTrue()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) // date de parution égale à la date de commande
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateAvantCommande_RetourneFalse()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Unspecified) // date de parution avant la date de commande
            );

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ParutionDansAbonnement_DateApresFin_RetourneFalse()
        {
            var result = service.ParutionDansAbonnement(
                new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2024, 12, 31, 0, 0, 0, DateTimeKind.Unspecified),
                new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Unspecified) // date de parution après la date de fin
            );

            Assert.IsFalse(result);
        }
    }
}
