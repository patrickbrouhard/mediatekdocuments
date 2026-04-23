using MediaTekDocuments.model;
using MediaTekDocuments.view;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments
{
    /// <summary>
    /// Point d'entrée principal de l'application MediaTekDocuments. Cette classe configure les paramètres d'affichage et
    /// initialise l'interface utilisateur principale après une authentification réussie.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Afficher le formulaire d'authentification
            using (var login = new FrmAuthentification())
            {
                var resultat = login.ShowDialog();

                if (resultat == DialogResult.OK)
                {
                    Utilisateur utilisateur = login.UtilisateurAuthentifie;
                    Application.Run(new FrmMediatek(utilisateur));
                }
            }
        }
    }
}
