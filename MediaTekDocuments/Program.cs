using MediaTekDocuments.model;
using MediaTekDocuments.view;
using System;
using System.Windows.Forms;

namespace MediaTekDocuments
{
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
                else
                {
                    return; // Fermer l'application si l'authentification échoue ou est annulée
                }
            }
        }
    }
}
