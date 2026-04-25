using MediaTekDocuments.controller;
using MediaTekDocuments.model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MediaTekDocuments.view
{
    public partial class FrmAuthentification : Form
    {
        private readonly FrmAuthentificationController controller;

        /// <summary>
        /// Obtient l'utilisateur actuellement authentifié dans le système.
        /// </summary>
        public Utilisateur UtilisateurAuthentifie { get; private set; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe FrmAuthentification.
        /// </summary>
        public FrmAuthentification()
        {
            InitializeComponent();
            this.controller = new FrmAuthentificationController();
        }

        /// <summary>
        /// Gère l'événement de chargement du formulaire d'authentification. Cette méthode configure le titre de la fenêtre
        /// en fonction de la source de l'API et prépare le champ de saisie de l'utilisateur pour une entrée immédiate.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmAuthentification_Load(object sender, EventArgs e)
        {
            this.Text = controller.GetApiSource();
            textBoxUser.Focus();
            this.AcceptButton = buttonConnexion;
            PreRemplirChampsDebug();
        }

        // Méthode pour pré-remplir les champs de connexion en mode debug
        [System.Diagnostics.Conditional("DEBUG")]
        private void PreRemplirChampsDebug()
        {
            textBoxUser.Text = "root";
            textBoxPwd.Text = "test";
        }

        private void buttonConnexion_Click(object sender, EventArgs e)
        {
            string login = textBoxUser.Text.Trim();
            string password = textBoxPwd.Text;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Veuillez saisir votre identifiant et votre mot de passe");
                return;
            }

            var utilisateur = controller.Authentifier(login, password);


            if (utilisateur != null)
            {
                if (!utilisateur.PeutOuvrirApp())
                {
                    MessageBox.Show("Votre service n'a pas accès à cette application.");
                    return;
                }

                UtilisateurAuthentifie = utilisateur;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Login ou mot de passe incorrect.", "Erreur d'authentification", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
