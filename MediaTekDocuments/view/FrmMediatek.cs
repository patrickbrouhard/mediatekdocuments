using MediaTekDocuments.commands;
using MediaTekDocuments.controller;
using MediaTekDocuments.dal;
using MediaTekDocuments.model;
using MediaTekDocuments.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.view

{
    /// <summary>
    /// Classe d'affichage
    /// </summary>
    public partial class FrmMediatek : Form
    {
        #region Commun
        private readonly FrmMediatekController controller;
        private readonly BindingSource bdgGenres = new BindingSource();
        private readonly BindingSource bdgPublics = new BindingSource();
        private readonly BindingSource bdgRayons = new BindingSource();

        // flag pour indiquer que le chargement est en cours afin d'éviter les bugs de rafraîchissement causés par
        // les événements combo/datagridview (répare comme ça les appels multiples à l'API lors du chargement de liste)
        // https://stackoverflow.com/questions/2793207/comobox-event-selectedvaluechanged
        private bool _isLoading = false;

        // liste des états (pour les exemplaires) : les récupérer une seule fois suffit.
        private List<Etat> lesEtats;

        // variable pour stocker l'opération en cours (ajout, modification, suppression)
        private Operation operationEnCours = Operation.None;

        public enum Operation
        {
            None,
            Ajouter,
            Modifier,
            Supprimer
        }

        public enum TypeMedia
        {
            None,
            Livre,
            Dvd,
            Revue
        }

        /// <summary>
        /// Constructeur : création du contrôleur lié à ce formulaire
        /// </summary>
        internal FrmMediatek()
        {
            InitializeComponent();
            this.controller = new FrmMediatekController();
        }

        private void FrmMediatek_Load(object sender, EventArgs e)
        {
            AfficherAlerteAbonnements();
            lesEtats = controller.GetAllEtats();
            RemplirComboEtats(lesEtats, comboBoxLivreEtats);
            RemplirComboEtats(lesEtats, comboBoxDvdEtats);
            RemplirComboEtats(lesEtats, comboBoxRevueEtats);
        }

        /// <summary>
        /// Affiche une boîte de dialogue d'alerte listant les abonnements arrivant à expiration dans les 30 prochains
        /// jours (s'il y en a).
        /// </summary>
        private void AfficherAlerteAbonnements()
        {
            var abonnementsArrivantAExpiration = controller.GetAbonnementsArrivantAExpiration();
            Debug.WriteLine($"Abonnements arrivant à expiration dans les 30 prochains jours : {abonnementsArrivantAExpiration.Count}");

            if (abonnementsArrivantAExpiration == null || abonnementsArrivantAExpiration.Count == 0)
                return;

            var message = new StringBuilder();
            message.AppendLine("Abonnements arrivant à expiration dans les 30 prochains jours :\n");

            foreach (var abonnement in abonnementsArrivantAExpiration)
            {
                message.AppendLine($"- {abonnement.Titre} (fin le {abonnement.DateFinAbonnement:dd/MM/yyyy})");
            }
            MessageBox.Show(message.ToString(), "Alerte abonnements", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Rempli un des 3 combo (genre, public, rayon)
        /// </summary>
        /// <param name="lesCategories">liste des objets de type Genre ou Public ou Rayon</param>
        /// <param name="bdg">bindingsource contenant les informations</param>
        /// <param name="cbx">combobox à remplir</param>
        public void RemplirComboCategorie(List<Categorie> lesCategories, BindingSource bdg, ComboBox cbx)
        {
            bdg.DataSource = lesCategories;
            cbx.DataSource = bdg;
            if (cbx.Items.Count > 0)
            {
                cbx.SelectedIndex = -1;
            }
        }

        public void RemplirComboEtats(List<Etat> lesEtats, ComboBox cbx)
        {
            lesEtats = lesEtats
                .OrderBy(e => e.Id)
                .ToList();

            cbx.DataSource = new List<Etat>(lesEtats); // copie pour éviter les bugs de rafraîchissement
            cbx.DisplayMember = "Libelle";
            cbx.ValueMember = "Id";
            cbx.SelectedIndex = -1;
        }

        private bool AreChampsObligatoiresValides(Genre unGenre, Public unPublic, Rayon unRayon)
        {
            string message = "";
            if (unGenre == null) message += "Genre invalide.\n";
            if (unPublic == null) message += "Public invalide.\n";
            if (unRayon == null) message += "Rayon invalide.\n";
            if (message != "")
            {
                MessageBox.Show(message, "Erreur de validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Affiche une boîte de dialogue indiquant le succès ou l'échec d'une opération à l'utilisateur.
        /// </summary>
        /// <param name="succes">Indique si l'opération a réussi.
        /// <param name="operation">Le nom ou la description de l'opération à afficher dans le message. La valeur par défaut est "Opération".</param>
        private void AfficheMessageSucces(bool succes, string operation = "Opération")
        {
            string message = succes
                ? $"{operation} effectuée avec succès."
                : $"{operation} n’a pas pu être réalisée.";

            string titre = succes ? "Succès" : "Erreur";
            MessageBox.Show(message, titre, MessageBoxButtons.OK,
                            succes ? MessageBoxIcon.Information : MessageBoxIcon.Error);
        }


        private bool CreerDocument(TypeMedia type, string id, string titre, string imageChemin, Genre genre, Public lePublic, Rayon rayon, bool isNewDoc)
        {
            int.TryParse(txbDvdDuree.Text, out int duree);
            int.TryParse(txbRevuesDateMiseADispo.Text, out int delai);

            var cmd = new CreerDocumentCommand
            {
                Type = type,

                Id = id,
                Titre = titre,
                Image = imageChemin,

                IdGenre = genre.Id,
                IdPublic = lePublic.Id,
                IdRayon = rayon.Id,

                Isbn = txbLivresIsbn.Text,
                Auteur = txbLivresAuteur.Text,
                Collection = txbLivresCollection.Text,

                Duree = duree,
                Realisateur = txbDvdRealisateur.Text,
                Synopsis = txbDvdSynopsis.Text,

                Periodicite = txbRevuesPeriodicite.Text,
                DelaiMiseADispo = delai
            };

            bool succes = controller.SauvegarderDocument(cmd, isNewDoc);
            return succes;
        }

        #endregion

        #region Onglet Livres
        private readonly BindingSource bdgLivresListe = new BindingSource();
        private readonly BindingSource bdgExemplairesLivresListe = new BindingSource();
        private List<Livre> lesLivres = new List<Livre>();
        private List<Exemplaire> lesExemplairesLivres = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Livres : 
        /// appel des méthodes pour remplir le datagrid des livres et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabLivres_Enter(object sender, EventArgs e)
        {
            SetModeExemplaireLivre(Operation.None);
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxLivresGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxLivresPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxLivresRayons);

            _isLoading = true;
            lesLivres = controller.GetAllLivres();
            RemplirLivresListeComplete();
            InitGridExemplaires(dataGridViewLivreExemplaires, bdgExemplairesLivresListe);
            _isLoading = false;

            DgvLivresListe_SelectionChanged(null, null);
        }

        private void SetModeLivre(Operation operation)
        {
            bool edition = operation == Operation.Ajouter || operation == Operation.Modifier;

            dgvLivresListe.Enabled = !edition;

            // Champs éditables
            txbLivresTitre.ReadOnly = !edition;
            txbLivresAuteur.ReadOnly = !edition;
            txbLivresCollection.ReadOnly = !edition;
            txbLivresGenre.ReadOnly = !edition;
            txbLivresPublic.ReadOnly = !edition;
            txbLivresRayon.ReadOnly = !edition;
            txbLivresImage.ReadOnly = !edition;

            // Champ protégé
            txbLivresNumero.ReadOnly = true;

            // Boutons
            buttonLivreAjouter.Enabled = !edition;
            buttonLivreModifier.Enabled = !edition;
            buttonLivreSupprimer.Enabled = !edition;

            buttonValiderLivre.Enabled = edition;
            buttonAnnulerLivre.Enabled = edition;

            if (edition)
            {
                txbLivresTitre.Focus();
            }
        }

        private (Genre genre, Public publicObj, Rayon rayon) GetLivreSelections()
        {
            var genre = bdgGenres.OfType<Genre>()
                .FirstOrDefault(g => g.Libelle == txbLivresGenre.Text);

            var publicObj = bdgPublics.OfType<Public>()
                .FirstOrDefault(p => p.Libelle == txbLivresPublic.Text);

            var rayon = bdgRayons.OfType<Rayon>()
                .FirstOrDefault(r => r.Libelle == txbLivresRayon.Text);

            return (genre, publicObj, rayon);
        }

        private void buttonLivreAjouter_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.Ajouter;

            VideLivresInfos();
            SetModeLivre(operationEnCours);
        }

        private void buttonLivreModifier_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbLivresNumero.Text))
            {
                MessageBox.Show("Veuillez sélectionner un livre à modifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeLivre(operationEnCours);
        }

        private void buttonLivreSupprimer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbLivresNumero.Text)) return;

            var livre = lesLivres.Find(l => l.Id == txbLivresNumero.Text);
            if (livre == null) return;

            var result = MessageBox.Show(
                $"Supprimer '{livre.Titre}' ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerDocument(TypeMedia.Livre, livre.Id);
                AfficheMessageSucces(success, "Suppression");

                TabLivres_Enter(null, null);
            }

            operationEnCours = Operation.None;
            SetModeLivre(Operation.None);
        }

        private void buttonValiderLivre_Click(object sender, EventArgs e)
        {
            if (operationEnCours != Operation.Ajouter && operationEnCours != Operation.Modifier)
            {
                MessageBox.Show("Aucune opération en cours.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (genre, publicObj, rayon) = GetLivreSelections();

            if (!AreChampsObligatoiresValides(genre, publicObj, rayon))
                return;

            bool isNew = operationEnCours == Operation.Ajouter;

            bool success = CreerDocument(
                TypeMedia.Livre,
                isNew ? null : txbLivresNumero.Text,
                txbLivresTitre.Text,
                txbLivresImage.Text,
                genre,
                publicObj,
                rayon,
                isNew
            );

            AfficheMessageSucces(success);

            operationEnCours = Operation.None;
            SetModeLivre(Operation.None);

            TabLivres_Enter(null, null);
        }

        private void buttonAnnulerLivre_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.None;

            SetModeLivre(Operation.None);
            TabLivres_Enter(null, null);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="livres">liste de livres</param>
        private void RemplirLivresListe(List<Livre> livres)
        {
            bdgLivresListe.DataSource = livres;
            dgvLivresListe.DataSource = bdgLivresListe;
            dgvLivresListe.Columns["isbn"].Visible = false;
            dgvLivresListe.Columns["idRayon"].Visible = false;
            dgvLivresListe.Columns["idGenre"].Visible = false;
            dgvLivresListe.Columns["idPublic"].Visible = false;
            dgvLivresListe.Columns["image"].Visible = false;
            dgvLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLivresListe.Columns["id"].DisplayIndex = 0;
            dgvLivresListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du livre dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbLivresNumRecherche.Text.Equals(""))
            {
                txbLivresTitreRecherche.Text = "";
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                Livre livre = lesLivres.Find(x => x.Id.Equals(txbLivresNumRecherche.Text));
                if (livre != null)
                {
                    List<Livre> livres = new List<Livre>() { livre };
                    RemplirLivresListe(livres);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirLivresListeComplete();
                }
            }
            else
            {
                RemplirLivresListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des livres dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TxbLivresTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbLivresTitreRecherche.Text.Equals(""))
            {
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
                txbLivresNumRecherche.Text = "";
                List<Livre> lesLivresParTitre;
                lesLivresParTitre = lesLivres.FindAll(x => x.Titre.ToLower().Contains(txbLivresTitreRecherche.Text.ToLower()));
                RemplirLivresListe(lesLivresParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxLivresGenres.SelectedIndex < 0 && cbxLivresPublics.SelectedIndex < 0 && cbxLivresRayons.SelectedIndex < 0
                    && txbLivresNumRecherche.Text.Equals(""))
                {
                    RemplirLivresListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du livre sélectionné
        /// </summary>
        /// <param name="livre">le livre</param>
        private void AfficheLivresInfos(Livre livre)
        {
            txbLivresAuteur.Text = livre.Auteur;
            txbLivresCollection.Text = livre.Collection;
            txbLivresImage.Text = livre.Image;
            txbLivresIsbn.Text = livre.Isbn;
            txbLivresNumero.Text = livre.Id;
            txbLivresGenre.Text = livre.Genre;
            txbLivresPublic.Text = livre.Public;
            txbLivresRayon.Text = livre.Rayon;
            txbLivresTitre.Text = livre.Titre;
            string image = livre.Image;
            try
            {
                pcbLivresImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbLivresImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du livre
        /// </summary>
        private void VideLivresInfos()
        {
            txbLivresAuteur.Text = "";
            txbLivresCollection.Text = "";
            txbLivresImage.Text = "";
            txbLivresIsbn.Text = "";
            txbLivresNumero.Text = "";
            txbLivresGenre.Text = "";
            txbLivresPublic.Text = "";
            txbLivresRayon.Text = "";
            txbLivresTitre.Text = "";
            pcbLivresImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresGenres.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Genre genre = (Genre)cbxLivresGenres.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresPublics.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Public lePublic = (Public)cbxLivresPublics.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirLivresListe(livres);
                cbxLivresRayons.SelectedIndex = -1;
                cbxLivresGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbxLivresRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxLivresRayons.SelectedIndex >= 0)
            {
                txbLivresTitreRecherche.Text = "";
                txbLivresNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxLivresRayons.SelectedItem;
                List<Livre> livres = lesLivres.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirLivresListe(livres);
                cbxLivresGenres.SelectedIndex = -1;
                cbxLivresPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du livre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading)
                return;

            if (dgvLivresListe.CurrentCell != null)
            {
                try
                {
                    Livre livre = (Livre)bdgLivresListe.List[bdgLivresListe.Position];
                    AfficheLivresInfos(livre);

                    var exemplaires = controller.GetExemplairesDocument(livre.Id)
                        .OrderByDescending(ex => ex.DateAchat)
                        .ToList();

                    if (exemplaires != null && exemplaires.Count > 0)
                    {
                        RemplirExemplairesLivresListe(exemplaires);
                    }
                    else
                    {
                        bdgExemplairesLivresListe.DataSource = null;
                        comboBoxLivreEtats.SelectedIndex = -1;
                    }
                }
                catch
                {
                    VideLivresZones();
                }
            }
            else
            {
                VideLivresInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des livres
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLivresAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirLivresListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des livres
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirLivresListeComplete()
        {
            RemplirLivresListe(lesLivres);
            VideLivresZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideLivresZones()
        {
            cbxLivresGenres.SelectedIndex = -1;
            cbxLivresRayons.SelectedIndex = -1;
            cbxLivresPublics.SelectedIndex = -1;
            txbLivresNumRecherche.Text = "";
            txbLivresTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgvLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideLivresZones();
            string titreColonne = dgvLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<Livre> sortedList = new List<Livre>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesLivres.OrderBy(o => o.Titre).ToList();
                    break;
                case "Collection":
                    sortedList = lesLivres.OrderBy(o => o.Collection).ToList();
                    break;
                case "Auteur":
                    sortedList = lesLivres.OrderBy(o => o.Auteur).ToList();
                    break;
                case "Genre":
                    sortedList = lesLivres.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesLivres.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesLivres.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirLivresListe(sortedList);
        }

        private void RemplirExemplairesLivresListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires == null)
            {
                bdgExemplairesLivresListe.DataSource = null;
                return;
            }

            lesExemplairesLivres = exemplaires;
            bdgExemplairesLivresListe.DataSource = null;
            bdgExemplairesLivresListe.DataSource = exemplaires;

            if (dataGridViewLivreExemplaires.Rows.Count > 0)
            {
                dataGridViewLivreExemplaires.ClearSelection();
                dataGridViewLivreExemplaires.Rows[0].Selected = true;
            }
            else
            {
                dataGridViewLivreExemplaires.ClearSelection();
            }
        }

        private void dataGridViewLivreExemplaires_SelectionChanged(object sender, EventArgs e)
        {
            if (bdgExemplairesLivresListe.Current is Exemplaire exemplaire)
            {
                comboBoxLivreEtats.SelectedValue = exemplaire.IdEtat;
            }
            else
            {
                comboBoxLivreEtats.SelectedIndex = -1;

            }
        }

        private void SetModeExemplaireLivre(Operation mode)
        {
            bool edition = mode == Operation.Modifier;

            comboBoxLivreEtats.Enabled = edition;
            buttonLivreExemplaireValider.Enabled = edition;
            buttonLivreExemplaireSupprimer.Enabled = edition;
            buttonLivreExemplaireAnnuler.Enabled = edition;

            buttonLivreExemplaireModifier.Enabled = !edition;
        }

        private void buttonLivreExemplaireModifier_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesLivresListe.Current is Exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeExemplaireLivre(operationEnCours);
        }

        private void buttonLivreExemplaireAnnuler_Click(object sender, EventArgs e)
        {
            if (bdgExemplairesLivresListe.Current is Exemplaire exemplaire)
            {
                comboBoxLivreEtats.SelectedValue = exemplaire.IdEtat;
            }

            operationEnCours = Operation.None;
            SetModeExemplaireLivre(operationEnCours);
        }

        private void buttonLivreExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesLivresListe.Current is Exemplaire exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            string nouvelIdEtat = comboBoxLivreEtats.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(nouvelIdEtat))
            {
                MessageBox.Show("Veuillez sélectionner un état.");
                return;
            }

            if (nouvelIdEtat == exemplaire.IdEtat)
            {
                MessageBox.Show("Aucune modification à enregistrer.");
                return;
            }

            Exemplaire exemplaireMaj = new Exemplaire(
                exemplaire.Numero,
                exemplaire.DateAchat,
                exemplaire.Photo,
                nouvelIdEtat,
                null,
                exemplaire.Id
            );

            try
            {
                bool success = controller.ModifierExemplaire(exemplaireMaj);

                if (!success)
                {
                    MessageBox.Show("La mise à jour a échoué.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour : " + ex.Message);
                return;
            }

            exemplaire.IdEtat = nouvelIdEtat;

            var etat = lesEtats.FirstOrDefault(et => et.Id == nouvelIdEtat);
            exemplaire.LibelleEtat = etat?.Libelle;

            bdgExemplairesLivresListe.ResetBindings(false);

            MessageBox.Show("État mis à jour avec succès.");

            operationEnCours = Operation.None;
            SetModeExemplaireLivre(operationEnCours);
        }

        private void buttonLivreExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesLivresListe.Current is Exemplaire exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            var confirmation = MessageBox.Show(
                $"Supprimer l'exemplaire n°{exemplaire.Numero} ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmation == DialogResult.Yes)
            {
                bool success = controller.SupprimerExemplaire(exemplaire);
                if (success)
                {
                    MessageBox.Show("Exemplaire supprimé avec succès.");
                    bdgExemplairesLivresListe.Remove(exemplaire);
                    comboBoxLivreEtats.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("La suppression de l'exemplaire a échoué.");
                    return;
                }
            }

            operationEnCours = Operation.None;
            SetModeExemplaireLivre(operationEnCours);
        }

        private void dataGridViewLivreExemplaires_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var sortedList = TrierExemplaires(
                dataGridViewLivreExemplaires,
                lesExemplairesLivres,
                e.ColumnIndex
            );

            RemplirExemplairesLivresListe(sortedList);
        }

        #endregion

        #region Onglet Dvd
        private readonly BindingSource bdgDvdListe = new BindingSource();
        private readonly BindingSource bdgExemplairesDvdListe = new BindingSource();
        private List<Dvd> lesDvd = new List<Dvd>();
        private List<Exemplaire> lesExemplairesDvd = new List<Exemplaire>();

        /// <summary>
        /// Ouverture de l'onglet Dvds : 
        /// appel des méthodes pour remplir le datagrid des dvd et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabDvd_Enter(object sender, EventArgs e)
        {
            SetModeExemplaireDvd(Operation.None);
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxDvdGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxDvdPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxDvdRayons);

            _isLoading = true;
            lesDvd = controller.GetAllDvd();
            RemplirDvdListeComplete();
            InitGridExemplaires(dataGridViewDvdExemplaires, bdgExemplairesDvdListe);
            _isLoading = false;

            dgvDvdListe_SelectionChanged(null, null);
        }

        private void SetModeDvd(Operation operation)
        {
            bool edition = operation == Operation.Ajouter || operation == Operation.Modifier;

            dgvDvdListe.Enabled = !edition;

            // Champs éditables
            txbDvdTitre.ReadOnly = !edition;
            txbDvdRealisateur.ReadOnly = !edition;
            txbDvdSynopsis.ReadOnly = !edition;
            txbDvdGenre.ReadOnly = !edition;
            txbDvdPublic.ReadOnly = !edition;
            txbDvdRayon.ReadOnly = !edition;
            txbDvdImage.ReadOnly = !edition;
            txbDvdDuree.ReadOnly = !edition;

            // Champ protégé
            txbDvdNumero.ReadOnly = true;

            // Boutons
            buttonDvdAjouter.Enabled = !edition;
            buttonDvdModifier.Enabled = !edition;
            buttonDvdSupprimer.Enabled = !edition;

            buttonValiderDvd.Enabled = edition;
            buttonAnnulerDvd.Enabled = edition;

            if (edition)
                txbDvdTitre.Focus();
        }

        private (Genre genre, Public publicObj, Rayon rayon) GetDvdSelections()
        {
            var genre = bdgGenres.OfType<Genre>()
                .FirstOrDefault(g => g.Libelle == txbDvdGenre.Text);

            var publicObj = bdgPublics.OfType<Public>()
                .FirstOrDefault(p => p.Libelle == txbDvdPublic.Text);

            var rayon = bdgRayons.OfType<Rayon>()
                .FirstOrDefault(r => r.Libelle == txbDvdRayon.Text);

            return (genre, publicObj, rayon);
        }
        private void buttonDvdAjouter_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.Ajouter;

            VideDvdInfos();
            SetModeDvd(operationEnCours);
        }

        private void buttonDvdModifier_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbDvdNumero.Text))
            {
                MessageBox.Show("Veuillez sélectionner un DVD à modifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeDvd(operationEnCours);
        }

        private void buttonDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbDvdNumero.Text)) return;

            var dvd = lesDvd.Find(d => d.Id == txbDvdNumero.Text);
            if (dvd == null) return;

            var result = MessageBox.Show(
                $"Supprimer '{dvd.Titre}' ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerDocument(TypeMedia.Dvd, dvd.Id);
                AfficheMessageSucces(success, "Suppression");

                operationEnCours = Operation.None;
                SetModeDvd(Operation.None);

                tabDvd_Enter(null, null);
            }
        }

        private void buttonValiderDvd_Click(object sender, EventArgs e)
        {
            if (operationEnCours != Operation.Ajouter && operationEnCours != Operation.Modifier)
            {
                MessageBox.Show("Aucune opération en cours.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (genre, publicObj, rayon) = GetDvdSelections();

            if (!AreChampsObligatoiresValides(genre, publicObj, rayon))
                return;

            bool isNew = operationEnCours == Operation.Ajouter;

            bool success = CreerDocument(
                TypeMedia.Dvd,
                isNew ? null : txbDvdNumero.Text,
                txbDvdTitre.Text,
                txbDvdImage.Text,
                genre,
                publicObj,
                rayon,
                isNew
            );

            AfficheMessageSucces(success);

            operationEnCours = Operation.None;
            SetModeDvd(Operation.None);

            tabDvd_Enter(null, null);
        }

        private void buttonAnnulerDvd_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.None;

            SetModeDvd(Operation.None);
            tabDvd_Enter(null, null);
        }


        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="Dvds">liste de dvd</param>
        private void RemplirDvdListe(List<Dvd> Dvds)
        {
            bdgDvdListe.DataSource = Dvds;
            dgvDvdListe.DataSource = bdgDvdListe;
            dgvDvdListe.Columns["idRayon"].Visible = false;
            dgvDvdListe.Columns["idGenre"].Visible = false;
            dgvDvdListe.Columns["idPublic"].Visible = false;
            dgvDvdListe.Columns["image"].Visible = false;
            dgvDvdListe.Columns["synopsis"].Visible = false;
            dgvDvdListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvDvdListe.Columns["id"].DisplayIndex = 0;
            dgvDvdListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage du Dvd dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbDvdNumRecherche.Text.Equals(""))
            {
                txbDvdTitreRecherche.Text = "";
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                Dvd dvd = lesDvd.Find(x => x.Id.Equals(txbDvdNumRecherche.Text));
                if (dvd != null)
                {
                    List<Dvd> Dvd = new List<Dvd>() { dvd };
                    RemplirDvdListe(Dvd);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirDvdListeComplete();
                }
            }
            else
            {
                RemplirDvdListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des Dvd dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbDvdTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbDvdTitreRecherche.Text.Equals(""))
            {
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
                txbDvdNumRecherche.Text = "";
                List<Dvd> lesDvdParTitre;
                lesDvdParTitre = lesDvd.FindAll(x => x.Titre.ToLower().Contains(txbDvdTitreRecherche.Text.ToLower()));
                RemplirDvdListe(lesDvdParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxDvdGenres.SelectedIndex < 0 && cbxDvdPublics.SelectedIndex < 0 && cbxDvdRayons.SelectedIndex < 0
                    && txbDvdNumRecherche.Text.Equals(""))
                {
                    RemplirDvdListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations du dvd sélectionné
        /// </summary>
        /// <param name="dvd">le dvd</param>
        private void AfficheDvdInfos(Dvd dvd)
        {
            txbDvdRealisateur.Text = dvd.Realisateur;
            txbDvdSynopsis.Text = dvd.Synopsis;
            txbDvdImage.Text = dvd.Image;
            txbDvdDuree.Text = dvd.Duree.ToString();
            txbDvdNumero.Text = dvd.Id;
            txbDvdGenre.Text = dvd.Genre;
            txbDvdPublic.Text = dvd.Public;
            txbDvdRayon.Text = dvd.Rayon;
            txbDvdTitre.Text = dvd.Titre;
            string image = dvd.Image;
            try
            {
                pcbDvdImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbDvdImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations du dvd
        /// </summary>
        private void VideDvdInfos()
        {
            txbDvdRealisateur.Text = "";
            txbDvdSynopsis.Text = "";
            txbDvdImage.Text = "";
            txbDvdDuree.Text = "";
            txbDvdNumero.Text = "";
            txbDvdGenre.Text = "";
            txbDvdPublic.Text = "";
            txbDvdRayon.Text = "";
            txbDvdTitre.Text = "";
            pcbDvdImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdGenres.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Genre genre = (Genre)cbxDvdGenres.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdPublics.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Public lePublic = (Public)cbxDvdPublics.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdRayons.SelectedIndex = -1;
                cbxDvdGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxDvdRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxDvdRayons.SelectedIndex >= 0)
            {
                txbDvdTitreRecherche.Text = "";
                txbDvdNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxDvdRayons.SelectedItem;
                List<Dvd> Dvd = lesDvd.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirDvdListe(Dvd);
                cbxDvdGenres.SelectedIndex = -1;
                cbxDvdPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations du dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading)
                return;

            if (dgvDvdListe.CurrentCell != null)
            {
                try
                {
                    Dvd dvd = (Dvd)bdgDvdListe.List[bdgDvdListe.Position];
                    AfficheDvdInfos(dvd);

                    var exemplaires = controller.GetExemplairesDocument(dvd.Id)
                        .OrderByDescending(ex => ex.DateAchat)
                        .ToList();

                    if (exemplaires != null && exemplaires.Count > 0)
                    {
                        RemplirExemplairesDvdsListe(exemplaires);

                    }
                    else
                    {
                        bdgExemplairesDvdListe.DataSource = null;
                        comboBoxDvdEtats.SelectedIndex = -1;
                    }
                }
                catch
                {
                    VideDvdZones();
                }
            }
            else
            {
                VideDvdInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des Dvd
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDvdAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirDvdListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des Dvd
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirDvdListeComplete()
        {
            RemplirDvdListe(lesDvd);
            VideDvdZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideDvdZones()
        {
            cbxDvdGenres.SelectedIndex = -1;
            cbxDvdRayons.SelectedIndex = -1;
            cbxDvdPublics.SelectedIndex = -1;
            txbDvdNumRecherche.Text = "";
            txbDvdTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvDvdListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideDvdZones();
            string titreColonne = dgvDvdListe.Columns[e.ColumnIndex].HeaderText;
            List<Dvd> sortedList = new List<Dvd>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesDvd.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesDvd.OrderBy(o => o.Titre).ToList();
                    break;
                case "Duree":
                    sortedList = lesDvd.OrderBy(o => o.Duree).ToList();
                    break;
                case "Realisateur":
                    sortedList = lesDvd.OrderBy(o => o.Realisateur).ToList();
                    break;
                case "Genre":
                    sortedList = lesDvd.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesDvd.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesDvd.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirDvdListe(sortedList);
        }

        private void RemplirExemplairesDvdsListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires == null)
            {
                bdgExemplairesDvdListe.DataSource = null;
                return;
            }

            lesExemplairesDvd = exemplaires;
            bdgExemplairesDvdListe.DataSource = null;
            bdgExemplairesDvdListe.DataSource = exemplaires;

            if (dataGridViewDvdExemplaires.Rows.Count > 0)
            {
                dataGridViewDvdExemplaires.ClearSelection();
                dataGridViewDvdExemplaires.Rows[0].Selected = true;
            }
            else
            {
                dataGridViewDvdExemplaires.ClearSelection();
            }
        }

        private void dataGridViewDvdExemplaires_SelectionChanged(object sender, EventArgs e)
        {
            if (bdgExemplairesDvdListe.Current is Exemplaire exemplaire)
            {
                comboBoxDvdEtats.SelectedValue = exemplaire.IdEtat;
            }
            else
            {
                comboBoxDvdEtats.SelectedIndex = -1;
            }
        }

        private void SetModeExemplaireDvd(Operation operation)
        {
            bool edition = operation == Operation.Modifier;

            comboBoxDvdEtats.Enabled = edition;

            buttonDvdExemplaireModifier.Enabled = !edition;
            buttonDvdExemplaireSupprimer.Enabled = edition;

            buttonDvdExemplaireValider.Enabled = edition;
            buttonDvdExemplaireAnnuler.Enabled = edition;
        }

        private void buttonDvdExemplaireModifier_Click(object sender, EventArgs e)
        {
            if(!(bdgExemplairesDvdListe.Current is Exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeExemplaireDvd(operationEnCours);
        }

        private void buttonDvdExemplaireAnnuler_Click(object sender, EventArgs e)
        {
            if (bdgExemplairesDvdListe.Current is Exemplaire exemplaire)
            {
                comboBoxDvdEtats.SelectedValue = exemplaire.IdEtat;
            }

            operationEnCours = Operation.None;
            SetModeExemplaireDvd(operationEnCours);
        }

        private void buttonDvdExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesDvdListe.Current is Exemplaire exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            string nouvelIdEtat = comboBoxDvdEtats.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(nouvelIdEtat))
            {
                MessageBox.Show("Veuillez sélectionner un état.");
                return;
            }

            if (nouvelIdEtat == exemplaire.IdEtat)
            {
                MessageBox.Show("Aucune modification à enregistrer.");
                return;
            }

            Exemplaire exemplaireMaj = new Exemplaire(
                exemplaire.Numero,
                exemplaire.DateAchat,
                exemplaire.Photo,
                nouvelIdEtat,
                null,
                exemplaire.Id
            );

            try
            {
                bool success = controller.ModifierExemplaire(exemplaireMaj);

                if (!success)
                {
                    MessageBox.Show("La mise à jour a échoué.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour : " + ex.Message);
                return;
            }

            exemplaire.IdEtat = nouvelIdEtat;

            var etat = lesEtats.FirstOrDefault(et => et.Id == nouvelIdEtat);
            exemplaire.LibelleEtat = etat?.Libelle;

            bdgExemplairesDvdListe.ResetBindings(false);

            MessageBox.Show("État mis à jour avec succès.");

            operationEnCours = Operation.None;
            SetModeExemplaireDvd(operationEnCours);
        }

        private void buttonDvdExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesDvdListe.Current is Exemplaire exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            var confirmation = MessageBox.Show(
                "Voulez-vous vraiment supprimer cet exemplaire ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirmation == DialogResult.Yes)
            {
                bool success = controller.SupprimerExemplaire(exemplaire);
                if (success)
                {
                    MessageBox.Show("Exemplaire supprimé avec succès.");
                    bdgExemplairesDvdListe.Remove(exemplaire);
                    comboBoxDvdEtats.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("La suppression de l'exemplaire a échoué.");
                    return;
                }
            }

            operationEnCours = Operation.None;
            SetModeExemplaireDvd(operationEnCours);
        }

        private void dataGridViewDvdExemplaires_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var sortedList = TrierExemplaires(
                dataGridViewDvdExemplaires,
                lesExemplairesDvd,
                e.ColumnIndex
            );

            RemplirExemplairesDvdsListe(sortedList);
        }

        #endregion

        #region Onglet Revues
        private readonly BindingSource bdgRevuesListe = new BindingSource();
        private List<Revue> lesRevues = new List<Revue>();

        /// <summary>
        /// Ouverture de l'onglet Revues : 
        /// appel des méthodes pour remplir le datagrid des revues et des combos (genre, rayon, public)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabRevues_Enter(object sender, EventArgs e)
        {
            lesRevues = controller.GetAllRevues();
            RemplirComboCategorie(controller.GetAllGenres(), bdgGenres, cbxRevuesGenres);
            RemplirComboCategorie(controller.GetAllPublics(), bdgPublics, cbxRevuesPublics);
            RemplirComboCategorie(controller.GetAllRayons(), bdgRayons, cbxRevuesRayons);
            RemplirRevuesListeComplete();
        }

        private void SetModeRevue(Operation operation)
        {
            bool edition = operation == Operation.Ajouter || operation == Operation.Modifier;

            dgvRevuesListe.Enabled = !edition;

            // Champs éditables
            txbRevuesTitre.ReadOnly = !edition;
            txbRevuesPeriodicite.ReadOnly = !edition;
            txbRevuesDateMiseADispo.ReadOnly = !edition;
            txbRevuesGenre.ReadOnly = !edition;
            txbRevuesPublic.ReadOnly = !edition;
            txbRevuesRayon.ReadOnly = !edition;
            txbRevuesImage.ReadOnly = !edition;

            // ID protégé
            txbRevuesNumero.ReadOnly = true;

            // Boutons
            buttonRevueAjouter.Enabled = !edition;
            buttonRevueModifier.Enabled = !edition;
            buttonRevueSupprimer.Enabled = !edition;

            buttonValiderRevue.Enabled = edition;
            buttonAnnulerRevue.Enabled = edition;

            if (edition)
                txbRevuesTitre.Focus();
        }

        private (Genre genre, Public publicObj, Rayon rayon) GetRevueSelections()
        {
            var genre = bdgGenres.OfType<Genre>()
                .FirstOrDefault(g => g.Libelle == txbRevuesGenre.Text);

            var publicObj = bdgPublics.OfType<Public>()
                .FirstOrDefault(p => p.Libelle == txbRevuesPublic.Text);

            var rayon = bdgRayons.OfType<Rayon>()
                .FirstOrDefault(r => r.Libelle == txbRevuesRayon.Text);

            return (genre, publicObj, rayon);
        }

        private void buttonRevueAjouter_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.Ajouter;

            VideRevuesInfos();
            SetModeRevue(operationEnCours);
        }

        private void buttonRevueModifier_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbRevuesNumero.Text))
            {
                MessageBox.Show("Veuillez sélectionner une revue à modifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeRevue(operationEnCours);
        }

        private void buttonRevueSupprimer_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txbRevuesNumero.Text)) return;

            var revue = lesRevues.Find(r => r.Id == txbRevuesNumero.Text);
            if (revue == null) return;

            var result = MessageBox.Show(
                $"Supprimer '{revue.Titre}' ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerDocument(TypeMedia.Revue, revue.Id);
                AfficheMessageSucces(success, "Suppression");

                operationEnCours = Operation.None;
                SetModeRevue(Operation.None);

                tabRevues_Enter(null, null);
            }
        }

        private void buttonValiderRevue_Click(object sender, EventArgs e)
        {
            if (operationEnCours != Operation.Ajouter && operationEnCours != Operation.Modifier)
            {
                MessageBox.Show("Aucune opération en cours.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (genre, publicObj, rayon) = GetRevueSelections();

            if (!AreChampsObligatoiresValides(genre, publicObj, rayon))
                return;

            bool isNew = operationEnCours == Operation.Ajouter;

            bool success = CreerDocument(
                TypeMedia.Revue,
                isNew ? null : txbRevuesNumero.Text,
                txbRevuesTitre.Text,
                txbRevuesImage.Text,
                genre,
                publicObj,
                rayon,
                isNew
            );

            AfficheMessageSucces(success);

            operationEnCours = Operation.None;
            SetModeRevue(Operation.None);

            tabRevues_Enter(null, null);
        }

        private void buttonAnnulerRevue_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.None;

            SetModeRevue(Operation.None);
            tabRevues_Enter(null, null);
        }

        /// <summary>
        /// Remplit le dategrid avec la liste reçue en paramètre
        /// </summary>
        /// <param name="revues"></param>
        private void RemplirRevuesListe(List<Revue> revues)
        {
            bdgRevuesListe.DataSource = revues;
            dgvRevuesListe.DataSource = bdgRevuesListe;
            dgvRevuesListe.Columns["idRayon"].Visible = false;
            dgvRevuesListe.Columns["idGenre"].Visible = false;
            dgvRevuesListe.Columns["idPublic"].Visible = false;
            dgvRevuesListe.Columns["image"].Visible = false;
            dgvRevuesListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvRevuesListe.Columns["id"].DisplayIndex = 0;
            dgvRevuesListe.Columns["titre"].DisplayIndex = 1;
        }

        /// <summary>
        /// Recherche et affichage de la revue dont on a saisi le numéro.
        /// Si non trouvé, affichage d'un MessageBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesNumRecherche_Click(object sender, EventArgs e)
        {
            if (!txbRevuesNumRecherche.Text.Equals(""))
            {
                txbRevuesTitreRecherche.Text = "";
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbRevuesNumRecherche.Text));
                if (revue != null)
                {
                    List<Revue> revues = new List<Revue>() { revue };
                    RemplirRevuesListe(revues);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                    RemplirRevuesListeComplete();
                }
            }
            else
            {
                RemplirRevuesListeComplete();
            }
        }

        /// <summary>
        /// Recherche et affichage des revues dont le titre matche acec la saisie.
        /// Cette procédure est exécutée à chaque ajout ou suppression de caractère
        /// dans le textBox de saisie.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbRevuesTitreRecherche_TextChanged(object sender, EventArgs e)
        {
            if (!txbRevuesTitreRecherche.Text.Equals(""))
            {
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
                txbRevuesNumRecherche.Text = "";
                List<Revue> lesRevuesParTitre;
                lesRevuesParTitre = lesRevues.FindAll(x => x.Titre.ToLower().Contains(txbRevuesTitreRecherche.Text.ToLower()));
                RemplirRevuesListe(lesRevuesParTitre);
            }
            else
            {
                // si la zone de saisie est vide et aucun élément combo sélectionné, réaffichage de la liste complète
                if (cbxRevuesGenres.SelectedIndex < 0 && cbxRevuesPublics.SelectedIndex < 0 && cbxRevuesRayons.SelectedIndex < 0
                    && txbRevuesNumRecherche.Text.Equals(""))
                {
                    RemplirRevuesListeComplete();
                }
            }
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionné
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheRevuesInfos(Revue revue)
        {
            txbRevuesPeriodicite.Text = revue.Periodicite;
            txbRevuesImage.Text = revue.Image;
            txbRevuesDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbRevuesNumero.Text = revue.Id;
            txbRevuesGenre.Text = revue.Genre;
            txbRevuesPublic.Text = revue.Public;
            txbRevuesRayon.Text = revue.Rayon;
            txbRevuesTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbRevuesImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbRevuesImage.Image = null;
            }
        }

        /// <summary>
        /// Vide les zones d'affichage des informations de la reuve
        /// </summary>
        private void VideRevuesInfos()
        {
            txbRevuesPeriodicite.Text = "";
            txbRevuesImage.Text = "";
            txbRevuesDateMiseADispo.Text = "";
            txbRevuesNumero.Text = "";
            txbRevuesGenre.Text = "";
            txbRevuesPublic.Text = "";
            txbRevuesRayon.Text = "";
            txbRevuesTitre.Text = "";
            pcbRevuesImage.Image = null;
        }

        /// <summary>
        /// Filtre sur le genre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesGenres_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesGenres.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Genre genre = (Genre)cbxRevuesGenres.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Genre.Equals(genre.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur la catégorie de public
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesPublics_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesPublics.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Public lePublic = (Public)cbxRevuesPublics.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Public.Equals(lePublic.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesRayons.SelectedIndex = -1;
                cbxRevuesGenres.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Filtre sur le rayon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbxRevuesRayons_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbxRevuesRayons.SelectedIndex >= 0)
            {
                txbRevuesTitreRecherche.Text = "";
                txbRevuesNumRecherche.Text = "";
                Rayon rayon = (Rayon)cbxRevuesRayons.SelectedItem;
                List<Revue> revues = lesRevues.FindAll(x => x.Rayon.Equals(rayon.Libelle));
                RemplirRevuesListe(revues);
                cbxRevuesGenres.SelectedIndex = -1;
                cbxRevuesPublics.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Sur la sélection d'une ligne ou cellule dans le grid
        /// affichage des informations de la revue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRevuesListe.CurrentCell != null)
            {
                try
                {
                    Revue revue = (Revue)bdgRevuesListe.List[bdgRevuesListe.Position];
                    AfficheRevuesInfos(revue);
                }
                catch
                {
                    VideRevuesZones();
                }
            }
            else
            {
                VideRevuesInfos();
            }
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulPublics_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulRayons_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Sur le clic du bouton d'annulation, affichage de la liste complète des revues
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRevuesAnnulGenres_Click(object sender, EventArgs e)
        {
            RemplirRevuesListeComplete();
        }

        /// <summary>
        /// Affichage de la liste complète des revues
        /// et annulation de toutes les recherches et filtres
        /// </summary>
        private void RemplirRevuesListeComplete()
        {
            RemplirRevuesListe(lesRevues);
            VideRevuesZones();
        }

        /// <summary>
        /// vide les zones de recherche et de filtre
        /// </summary>
        private void VideRevuesZones()
        {
            cbxRevuesGenres.SelectedIndex = -1;
            cbxRevuesRayons.SelectedIndex = -1;
            cbxRevuesPublics.SelectedIndex = -1;
            txbRevuesNumRecherche.Text = "";
            txbRevuesTitreRecherche.Text = "";
        }

        /// <summary>
        /// Tri sur les colonnes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvRevuesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            VideRevuesZones();
            string titreColonne = dgvRevuesListe.Columns[e.ColumnIndex].HeaderText;
            List<Revue> sortedList = new List<Revue>();
            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Titre":
                    sortedList = lesRevues.OrderBy(o => o.Titre).ToList();
                    break;
                case "Periodicite":
                    sortedList = lesRevues.OrderBy(o => o.Periodicite).ToList();
                    break;
                case "DelaiMiseADispo":
                    sortedList = lesRevues.OrderBy(o => o.DelaiMiseADispo).ToList();
                    break;
                case "Genre":
                    sortedList = lesRevues.OrderBy(o => o.Genre).ToList();
                    break;
                case "Public":
                    sortedList = lesRevues.OrderBy(o => o.Public).ToList();
                    break;
                case "Rayon":
                    sortedList = lesRevues.OrderBy(o => o.Rayon).ToList();
                    break;
            }
            RemplirRevuesListe(sortedList);
        }
        #endregion

        #region Onglet Parutions
        private readonly BindingSource bdgExemplairesRevuesListe = new BindingSource();
        private List<Exemplaire> lesExemplairesRevues = new List<Exemplaire>();
        const string ETATNEUF = "00001";

        /// <summary>
        /// Ouverture de l'onglet : récupère le revues et vide tous les champs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabReceptionRevue_Enter(object sender, EventArgs e)
        {
            SetModeExemplaireRevue(Operation.None);

            _isLoading = true;
            lesRevues = controller.GetAllRevues();
            txbReceptionRevueNumero.Text = "";
            InitGridExemplaires(dgvReceptionExemplairesListe, bdgExemplairesRevuesListe);
            _isLoading = false;
        }

        /// <summary>
        /// Remplit le dategrid des exemplaires avec la liste reçue en paramètre
        /// </summary>
        /// <param name="exemplaires">liste d'exemplaires</param>
        private void RemplirReceptionExemplairesListe(List<Exemplaire> exemplaires)
        {
            if (exemplaires == null) 
            {
                bdgExemplairesRevuesListe.DataSource = null;
                return;
            }


            lesExemplairesRevues = exemplaires;
            bdgExemplairesRevuesListe.DataSource = null;
            bdgExemplairesRevuesListe.DataSource = exemplaires;

            if (dgvReceptionExemplairesListe.Rows.Count > 0)
            {
                dgvReceptionExemplairesListe.ClearSelection();
                dgvReceptionExemplairesListe.Rows[0].Selected = true;
            }
            else
            {
                dgvReceptionExemplairesListe.ClearSelection();
            }
        }

        /// <summary>
        /// Recherche d'un numéro de revue et affiche ses informations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionRechercher_Click(object sender, EventArgs e)
        {
            if (!txbReceptionRevueNumero.Text.Equals(""))
            {
                Revue revue = lesRevues.Find(x => x.Id.Equals(txbReceptionRevueNumero.Text));
                if (revue != null)
                {
                    AfficheReceptionRevueInfos(revue);
                }
                else
                {
                    MessageBox.Show("numéro introuvable");
                }
            }
        }

        /// <summary>
        /// Si le numéro de revue est modifié, la zone de l'exemplaire est vidée et inactive
        /// les informations de la revue son aussi effacées
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txbReceptionRevueNumero_TextChanged(object sender, EventArgs e)
        {
            txbReceptionRevuePeriodicite.Text = "";
            txbReceptionRevueImage.Text = "";
            txbReceptionRevueDelaiMiseADispo.Text = "";
            txbReceptionRevueGenre.Text = "";
            txbReceptionRevuePublic.Text = "";
            txbReceptionRevueRayon.Text = "";
            txbReceptionRevueTitre.Text = "";
            pcbReceptionRevueImage.Image = null;
            RemplirReceptionExemplairesListe(null);
            AccesReceptionExemplaireGroupBox(false);
        }

        /// <summary>
        /// Affichage des informations de la revue sélectionnée et les exemplaires
        /// </summary>
        /// <param name="revue">la revue</param>
        private void AfficheReceptionRevueInfos(Revue revue)
        {
            // informations sur la revue
            txbReceptionRevuePeriodicite.Text = revue.Periodicite;
            txbReceptionRevueImage.Text = revue.Image;
            txbReceptionRevueDelaiMiseADispo.Text = revue.DelaiMiseADispo.ToString();
            txbReceptionRevueNumero.Text = revue.Id;
            txbReceptionRevueGenre.Text = revue.Genre;
            txbReceptionRevuePublic.Text = revue.Public;
            txbReceptionRevueRayon.Text = revue.Rayon;
            txbReceptionRevueTitre.Text = revue.Titre;
            string image = revue.Image;
            try
            {
                pcbReceptionRevueImage.Image = Image.FromFile(image);
            }
            catch
            {
                pcbReceptionRevueImage.Image = null;
            }
            // affiche la liste des exemplaires de la revue
            AfficheReceptionExemplairesRevue();
        }

        /// <summary>
        /// Récupère et affiche les exemplaires d'une revue
        /// </summary>
        private void AfficheReceptionExemplairesRevue()
        {
            string idDocuement = txbReceptionRevueNumero.Text;
            lesExemplairesRevues = controller
                .GetExemplairesDocument(idDocuement)
                .OrderByDescending(e => e.DateAchat)
                .ToList();
            RemplirReceptionExemplairesListe(lesExemplairesRevues);
            AccesReceptionExemplaireGroupBox(true);
        }

        /// <summary>
        /// Permet ou interdit l'accès à la gestion de la réception d'un exemplaire
        /// et vide les objets graphiques
        /// </summary>
        /// <param name="acces">true ou false</param>
        private void AccesReceptionExemplaireGroupBox(bool acces)
        {
            grpReceptionExemplaire.Enabled = acces;
            txbReceptionExemplaireImage.Text = "";
            txbReceptionExemplaireNumero.Text = "";
            pcbReceptionExemplaireImage.Image = null;
            dtpReceptionExemplaireDate.Value = DateTime.Now;
        }

        /// <summary>
        /// Recherche image sur disque (pour l'exemplaire à insérer)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireImage_Click(object sender, EventArgs e)
        {
            string filePath = "";
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                // positionnement à la racine du disque où se trouve le dossier actuel
                InitialDirectory = Path.GetPathRoot(Environment.CurrentDirectory),
                Filter = "Files|*.jpg;*.bmp;*.jpeg;*.png;*.gif"
            };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filePath = openFileDialog.FileName;
            }
            txbReceptionExemplaireImage.Text = filePath;
            try
            {
                pcbReceptionExemplaireImage.Image = Image.FromFile(filePath);
            }
            catch
            {
                pcbReceptionExemplaireImage.Image = null;
            }
        }

        /// <summary>
        /// Enregistrement du nouvel exemplaire
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReceptionExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!txbReceptionExemplaireNumero.Text.Equals(""))
            {
                try
                {
                    int numero = int.Parse(txbReceptionExemplaireNumero.Text);
                    DateTime dateAchat = dtpReceptionExemplaireDate.Value;
                    string photo = txbReceptionExemplaireImage.Text;
                    string idEtat = ETATNEUF;
                    string etat = "neuf";
                    string idDocument = txbReceptionRevueNumero.Text;
                    Exemplaire exemplaire = new Exemplaire(numero, dateAchat, photo, idEtat, etat, idDocument);
                    if (controller.CreerExemplaire(exemplaire))
                    {
                        AfficheReceptionExemplairesRevue();
                    }
                    else
                    {
                        MessageBox.Show("numéro de publication déjà existant", "Erreur");
                    }
                }
                catch
                {
                    MessageBox.Show("le numéro de parution doit être numérique", "Information");
                    txbReceptionExemplaireNumero.Text = "";
                    txbReceptionExemplaireNumero.Focus();
                }
            }
            else
            {
                MessageBox.Show("numéro de parution obligatoire", "Information");
            }
        }

        /// <summary>
        /// Tri sur une colonne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvExemplairesListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var sortedList = TrierExemplaires(
                dgvReceptionExemplairesListe,
                lesExemplairesRevues,
                e.ColumnIndex
            );

            RemplirReceptionExemplairesListe(sortedList);
        }

        /// <summary>
        /// affichage de l'image de l'exemplaire suite à la sélection d'un exemplaire dans la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvReceptionExemplairesListe_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvReceptionExemplairesListe.CurrentCell != null)
            {
                if (bdgExemplairesRevuesListe.Current is Exemplaire exemplaire)
                {
                    comboBoxRevueEtats.SelectedValue = exemplaire.IdEtat;
                }
                else
                {
                    comboBoxRevueEtats.SelectedIndex = -1;
                }
            }
        }

        private void SetModeExemplaireRevue(Operation operation)
        {
            bool edition = operation == Operation.Modifier;

            comboBoxRevueEtats.Enabled = edition;
            buttonRevueExemplaireValider.Enabled = edition;
            buttonRevueExemplaireSupprimer.Enabled = edition;
            buttonRevueExemplaireAnnuler.Enabled = edition;

            buttonRevueExemplaireModifier.Enabled = !edition;
        }

        private void buttonRevueExemplaireModifier_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesRevuesListe.Current is Exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeExemplaireRevue(operationEnCours);
        }

        private void buttonRevueExemplaireAnnuler_Click(object sender, EventArgs e)
        {
            if (bdgExemplairesRevuesListe.Current is Exemplaire exemplaire)
            {
                comboBoxRevueEtats.SelectedValue = exemplaire.IdEtat;
            }

            operationEnCours = Operation.None;
            SetModeExemplaireRevue(operationEnCours);
        }
        private void buttonRevueExemplaireValider_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesRevuesListe.Current is Exemplaire exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            string nouvelIdEtat = comboBoxRevueEtats.SelectedValue?.ToString();

            if (string.IsNullOrEmpty(nouvelIdEtat))
            {
                MessageBox.Show("Veuillez sélectionner un état.");
                return;
            }

            if (nouvelIdEtat == exemplaire.IdEtat)
            {
                MessageBox.Show("Aucune modification à enregistrer.");
                return;
            }

            Exemplaire exemplaireMaj = new Exemplaire(
                exemplaire.Numero,
                exemplaire.DateAchat,
                exemplaire.Photo,
                nouvelIdEtat,
                null,
                exemplaire.Id
            );

            try
            {
                bool success = controller.ModifierExemplaire(exemplaireMaj);

                if (!success)
                {
                    MessageBox.Show("La mise à jour a échoué.");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la mise à jour : " + ex.Message);
                return;
            }

            exemplaire.IdEtat = nouvelIdEtat;

            var etat = lesEtats.FirstOrDefault(et => et.Id == nouvelIdEtat);
            exemplaire.LibelleEtat = etat?.Libelle;

            bdgExemplairesRevuesListe.ResetBindings(false);

            MessageBox.Show("État mis à jour avec succès.");

            operationEnCours = Operation.None;
            SetModeExemplaireRevue(operationEnCours);
        }

        private void buttonRevueExemplaireSupprimer_Click(object sender, EventArgs e)
        {
            if (!(bdgExemplairesRevuesListe.Current is Exemplaire exemplaire))
            {
                MessageBox.Show("Aucun exemplaire sélectionné.");
                return;
            }

            var confirmation = MessageBox.Show(
                $"Supprimer l'exemplaire n°{exemplaire.Numero} ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (confirmation == DialogResult.Yes)
            {
                bool success = controller.SupprimerExemplaire(exemplaire);
                if (success)
                {
                    MessageBox.Show("Exemplaire supprimé avec succès.");
                    bdgExemplairesRevuesListe.Remove(exemplaire);
                    comboBoxRevueEtats.SelectedIndex = -1;
                }
                else
                {
                    MessageBox.Show("La suppression de l'exemplaire a échoué.");
                    return;
                }
            }

            operationEnCours = Operation.None;
            SetModeExemplaireRevue(operationEnCours);
        }


        #endregion

        #region Commandes

        // Enum pour les états de suivi d'une commande
        public enum EtatSuivi
        {
            Initial = 0,
            EnCours = 1,
            Relancee = 2,
            Livree = 3,
            Reglee = 4
        }

        // Dictionnaire des transitions autorisées entre les états de suivi
        private static readonly Dictionary<EtatSuivi, List<EtatSuivi>> transitionsAutorisees = new Dictionary<EtatSuivi, List<EtatSuivi>>
        {
            // depuis "Initial", on peut passer uniquement à "En cours"
            { EtatSuivi.Initial, new List<EtatSuivi> { EtatSuivi.EnCours } },

            // depuis "En cours", on peut passer à "Relancée" ou "Livrée"
            { EtatSuivi.EnCours, new List<EtatSuivi> { EtatSuivi.Relancee, EtatSuivi.Livree } },

            // une fois relancée, seule la transition vers livrée est autorisée
            { EtatSuivi.Relancee, new List<EtatSuivi> { EtatSuivi.Livree } },

            // une fois livrée, seule la transition vers réglée est autorisée
            { EtatSuivi.Livree, new List<EtatSuivi> { EtatSuivi.Reglee } },

            // une fois réglée, pas de transition possible
            { EtatSuivi.Reglee, new List<EtatSuivi>() }
        };
        private static bool IsTransitionValide(EtatSuivi ancienEtat, EtatSuivi nouvelEtat)
        {
            // même état : pas de changement, donc valide
            if (ancienEtat == nouvelEtat) { return true; }
            // pas de transition définie pour l'état actuel : aucune transition autorisée
            if (!transitionsAutorisees.ContainsKey(ancienEtat)) { return false; }

            return transitionsAutorisees[ancienEtat].Contains(nouvelEtat);
        }

        private void ChargerSuivis()
        {
            var lesSuivis = controller.GetAllSuivis();

            comboBoxCommandeLivreEtat.DataSource = lesSuivis;
            comboBoxCommandeLivreEtat.DisplayMember = "LibelleEtat";
            comboBoxCommandeLivreEtat.ValueMember = "IdSuivi";

            comboBoxCommandeDvdEtat.DataSource = lesSuivis;
            comboBoxCommandeDvdEtat.DisplayMember = "LibelleEtat";
            comboBoxCommandeDvdEtat.ValueMember = "IdSuivi";
        }


        private void FiltrerEtatsSuivisDisponibles(CommandeDocument commande, ComboBox comboBox, Operation operationEnCours)
        {
            if (commande == null && operationEnCours != Operation.Ajouter) return;

            EtatSuivi etatActuel = operationEnCours == Operation.Ajouter
                ? EtatSuivi.Initial
                : (EtatSuivi)commande.IdSuivi;

            if (!transitionsAutorisees.ContainsKey(etatActuel)) return;

            // inclut également l'état actuel pour permettre de ne pas changer l'état. Bah oui. Evidemment.
            var etatsAutorises = transitionsAutorisees[etatActuel]
                .Append(etatActuel)
                .ToList();

            var tousLesSuivis = controller.GetAllSuivis();
            if (tousLesSuivis == null) return;
            comboBox.DataSource = null;

            // filtre les suivis pour n'afficher que ceux autorisés pour la transition (LINQ)
            var suivisFiltres = tousLesSuivis
                .Where(s => etatsAutorises.Contains((EtatSuivi)s.IdSuivi))
                .ToList();

            comboBox.DataSource = suivisFiltres;
            comboBox.DisplayMember = "LibelleEtat";
            comboBox.ValueMember = "IdSuivi";

            // réinitialise la sélection à l'état actuel de la commande
            comboBox.SelectedValue = (int)etatActuel;
        }

        #endregion

        #region OngletCommandesLivres

        private readonly BindingSource bdgCommandeLivresListe = new BindingSource();
        private List<CommandeDocument> lesCommandesLivres = new List<CommandeDocument>();

        private void tabCommandeLivre_Enter(object sender, EventArgs e)
        {
            lesCommandesLivres = controller.GetAllCommandesDocuments(TypeMedia.Livre);
            RemplirCommandesLivreListe(lesCommandesLivres);
            ChargerSuivis();
        }

        private void SetModeCommandeLivre(Operation operation)
        {
            bool edition = operation == Operation.Ajouter || operation == Operation.Modifier;

            dataGridViewCommandeLivresListe.Enabled = !edition;

            // Champs éditables
            dateTimePickerCommandeLivreDate.Enabled = edition;
            textBoxCommandeLivreMontant.ReadOnly = !edition;
            textBoxCommandeLivreNbExemplaires.ReadOnly = !edition;
            comboBoxCommandeLivreEtat.Enabled = edition;
            textBoxCommandeLivreRecherche.Enabled = !edition;

            // Modifiable uniquement à la création
            textBoxLivreNumeroDansCommande.ReadOnly = operation != Operation.Ajouter;

            // ID protégé
            textBoxLCommandeLivreNumero.ReadOnly = true;

            // Boutons
            buttonCommandeLivreAjouter.Enabled = !edition;
            buttonCommandeLivreModifier.Enabled = !edition;
            buttonCommandeLivreSupprimer.Enabled = !edition;

            buttonCommandeLivreValider.Enabled = edition;
            buttonCommandeLivreAnnuler.Enabled = edition;
            buttonCommandeLivreRechercher.Enabled = !edition;

            // Gestion du focus uniquement en mode édition
            if (edition)
            {
                Control focusControl = operation == Operation.Ajouter
                    ? textBoxLivreNumeroDansCommande
                    : textBoxCommandeLivreMontant;

                focusControl.Focus();
            }
        }

        private void buttonCommandeLivreAjouter_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.Ajouter;

            ViderCommande();
            if (textBoxCommandeLivreRecherche.Text != "")
            {
                textBoxLivreNumeroDansCommande.Text = textBoxCommandeLivreRecherche.Text.Trim();
                textBoxCommandeLivreRecherche.Text = "";
            }
            comboBoxCommandeLivreEtat.SelectedIndex = 0;
            SetModeCommandeLivre(operationEnCours);

            FiltrerEtatsSuivisDisponibles(null, comboBoxCommandeLivreEtat, Operation.Ajouter);
        }

        private void buttonCommandeLivreModifier_Click(object sender, EventArgs e)
        {
            if (bdgCommandeLivresListe.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.");
                return;
            }

            if (!(bdgCommandeLivresListe.Current is CommandeDocument commande))
            {
                MessageBox.Show("Veuillez sélectionner une commande à modifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeCommandeLivre(operationEnCours);

            RemplirCommande(commande);
            RemplirDetailsLivre(commande);

            FiltrerEtatsSuivisDisponibles(commande, comboBoxCommandeLivreEtat, operationEnCours);
        }

        private void buttonCommandeLivreSupprimer_Click(object sender, EventArgs e)
        {
            if (bdgCommandeLivresListe.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.");
                return;
            }

            if (!(bdgCommandeLivresListe.Current is CommandeDocument commande))
            {
                MessageBox.Show("Veuillez sélectionner une commande à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationEnCours = Operation.Supprimer;

            var result = MessageBox.Show(
                $"Supprimer la commande numéro '{commande.IdDocument}' ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerCommande(TypeMedia.Livre, commande.Id);
                if (success)
                {
                    MessageBox.Show("Commande supprimée");
                    lesCommandesLivres = controller.GetAllCommandesDocuments(TypeMedia.Livre);
                    RemplirCommandesLivreListe(lesCommandesLivres);
                    ViderDetailsLivre();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression");
                }
            }

            operationEnCours = Operation.None;
            tabCommandeLivre_Enter(null, null);
        }

        private void buttonCommandeLivreAnnuler_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.None;
            SetModeCommandeLivre(Operation.None);
            tabCommandeLivre_Enter(null, null);

            // repositionnement sur la première commande si la liste n'est pas vide
            if (lesCommandesLivres.Any())
            {
                bdgCommandeLivresListe.Position = 0;
            }
        }

        /// <summary>
        /// Gère la validation d'une commande de livre lors du clic sur le bouton de validation. Selon l'opération en
        /// cours, crée ou modifie une commande de livre et affiche un message de confirmation ou d'erreur.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCommandeLivreValider_Click(object sender, EventArgs e)
        {
            if (operationEnCours != Operation.Ajouter && operationEnCours != Operation.Modifier)
            {
                MessageBox.Show("Aucune opération en cours.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isNew = operationEnCours == Operation.Ajouter;
            string idCommande = textBoxLCommandeLivreNumero.Text;

            string messageErreur;
            if (!ValiderChampsCommandeLivre(isNew, out messageErreur))
            {
                MessageBox.Show(messageErreur);
                return;
            }

            // construction du command
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Livre,
                Id = textBoxLCommandeLivreNumero.Text,
                DateCommande = dateTimePickerCommandeLivreDate.Value,
                NbExemplaire = int.Parse(textBoxCommandeLivreNbExemplaires.Text),
                Montant = double.Parse(textBoxCommandeLivreMontant.Text),
                IdLivreDvd = textBoxLivreNumeroDansCommande.Text,
                IdSuivi = (int)comboBoxCommandeLivreEtat.SelectedValue
            };

            Debug.WriteLine(
                $"controller.SauvegarderCommande, isNew : {isNew} - Id : {cmd.Id}, Livre/Dvd : {cmd.IdLivreDvd}, NbExemplaires : {cmd.NbExemplaire}, Montant : {cmd.Montant}, Date : {cmd.DateCommande}, IdSuivi : {cmd.IdSuivi}"
            );

            bool success = controller.SauvegarderCommande(cmd, isNew);

            if (success)
            {
                MessageBox.Show(isNew ? "Commande créée" : "Commande modifiée");

                operationEnCours = Operation.None;
                SetModeCommandeLivre(Operation.None);

                // rechargement de la liste des commandes pour afficher les changements
                lesCommandesLivres = controller.GetAllCommandesDocuments(TypeMedia.Livre);
                RemplirCommandesLivreListe(lesCommandesLivres);

                // repositionnement sur la commande modifiée ou créée
                var updated = lesCommandesLivres.FirstOrDefault(c => c.Id == idCommande);
                if (updated != null)
                {
                    bdgCommandeLivresListe.Position = lesCommandesLivres.IndexOf(updated);
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de l'enregistrement");
            }
        }

        /// <summary>
        /// Valide les champs saisis pour une commande de document et indique si les données sont correctes pour
        /// l'enregistrement ou la modification.
        /// </summary>
        /// <param name="isNew">Indique si la commande est nouvelle ou s'il s'agit d'une modification existante</param>
        /// <param name="messageErreur">Contient le message d'erreur descriptif si la validation échoue</param>
        /// <returns>true si tous les champs de la commande sont valides</returns>
        private bool ValiderChampsCommandeLivre(bool isNew, out string messageErreur)
        {
            // Numéro de commande obligatoire si modification
            if (!isNew && string.IsNullOrWhiteSpace(textBoxLCommandeLivreNumero.Text))
            {
                messageErreur = "Numéro de commande obligatoire";
                return false;
            }

            // Document obligatoire
            if (string.IsNullOrWhiteSpace(textBoxLivreNumeroDansCommande.Text))
            {
                messageErreur = "Document obligatoire";
                return false;
            }

            // Vérifier que le livre existe
            var livre = lesLivres.FirstOrDefault(l => l.Id == textBoxLivreNumeroDansCommande.Text);
            if (livre == null)
            {
                messageErreur = "Ce livre n'existe pas.";
                return false;
            }

            // Nombre d'exemplaires
            if (!int.TryParse(textBoxCommandeLivreNbExemplaires.Text, out int nbExemplaires) || nbExemplaires <= 0)
            {
                messageErreur = "Nombre d'exemplaires invalide";
                return false;
            }

            // Montant
            if (!double.TryParse(textBoxCommandeLivreMontant.Text, out double montant) || montant < 0)
            {
                messageErreur = "Montant invalide";
                return false;
            }

            // État obligatoire
            if (comboBoxCommandeLivreEtat.SelectedValue == null)
            {
                messageErreur = "État obligatoire";
                return false;
            }

            EtatSuivi ancienEtat;
            EtatSuivi nouvelEtat = (EtatSuivi)((int)comboBoxCommandeLivreEtat.SelectedValue);

            if (!isNew)
            {
                var commande = lesCommandesLivres
                    .FirstOrDefault(c => c.Id == textBoxLCommandeLivreNumero.Text);

                // IdSuivi peut être casté car il correspond aux valeurs dans l'enum (ex: IdSuivi = 1 -> EnCours)
                ancienEtat = (EtatSuivi)commande.IdSuivi;

            }
            else { ancienEtat = EtatSuivi.Initial; }

            if (!IsTransitionValide(ancienEtat, nouvelEtat))
            {
                messageErreur = $"Transition d'état invalide : {ancienEtat} -> {nouvelEtat}";
                return false;
            }

            messageErreur = null;
            return true;
        }



        private void RemplirCommandesLivreListe(List<CommandeDocument> commandesDocument)
        {
            bdgCommandeLivresListe.DataSource = commandesDocument;

            dataGridViewCommandeLivresListe.AutoGenerateColumns = false;
            dataGridViewCommandeLivresListe.DataSource = bdgCommandeLivresListe;
            dataGridViewCommandeLivresListe.Columns.Clear();

            dataGridViewCommandeLivresListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Id",
                DataPropertyName = "Id"
            });

            dataGridViewCommandeLivresListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Id Document",
                DataPropertyName = "IdDocument"
            });

            dataGridViewCommandeLivresListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Exemplaires",
                DataPropertyName = "NbExemplaire"
            });

            dataGridViewCommandeLivresListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Etat",
                DataPropertyName = "LibelleSuivi"
            });

            dataGridViewCommandeLivresListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date",
                DataPropertyName = "DateCommande"
            });

            dataGridViewCommandeLivresListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Montant",
                DataPropertyName = "Montant"
            });

            dataGridViewCommandeLivresListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCommandeLivresListe.AutoResizeColumns();
        }

        private void RemplirDetailsLivre(CommandeDocument commande)
        {
            if (!lesLivres.Any())
            {
                lesLivres = controller.GetAllLivres();
            }

            var livre = lesLivres.FirstOrDefault(l => l.Id == commande.IdLivreDvd);

            if (livre != null)
            {
                RemplirDetailsLivreDepuisDocument(livre);
            }
            else
            {
                ViderDetailsLivre();
            }
        }

        private void RemplirDetailsLivreDepuisDocument(Document document)
        {
            if (document is Livre livre)
            {
                textBoxLivreNumero.Text = livre.Id;
                textBoxLivreIsbn.Text = livre.Isbn;
                textBoxLivreTitre.Text = livre.Titre;
                textBoxLivreAuteur.Text = livre.Auteur;
                textBoxLivreCollection.Text = livre.Collection;
                textBoxLivreGenre.Text = livre.Genre;
                textBoxLivrePublic.Text = livre.Public;
                textBoxLivreRayon.Text = livre.Rayon;
                textBoxLivreImage.Text = livre.Image;
            }
        }

        private void ViderDetailsLivre()
        {
            textBoxLivreNumero.Text = "";
            textBoxLivreIsbn.Text = "";
            textBoxLivreTitre.Text = "";
            textBoxLivreAuteur.Text = "";
            textBoxLivreCollection.Text = "";
            textBoxLivreGenre.Text = "";
            textBoxLivrePublic.Text = "";
            textBoxLivreRayon.Text = "";
            textBoxLivreImage.Text = "";
        }

        private void RemplirCommande(CommandeDocument commande)
        {
            if (commande == null) return;

            textBoxLCommandeLivreNumero.Text = commande.Id ?? "";
            dateTimePickerCommandeLivreDate.Value = commande.DateCommande;
            textBoxCommandeLivreMontant.Text = commande.Montant.ToString("0.00");

            textBoxLivreNumeroDansCommande.Text = commande.IdLivreDvd ?? "";
            textBoxCommandeLivreNbExemplaires.Text = commande.NbExemplaire.ToString();

            if (commande.IdSuivi != 0)
            {
                comboBoxCommandeLivreEtat.SelectedValue = commande.IdSuivi;
            }
        }

        private void ViderCommande()
        {
            textBoxLCommandeLivreNumero.Text = "";
            dateTimePickerCommandeLivreDate.Value = DateTime.Now;
            textBoxCommandeLivreMontant.Text = "";
            textBoxLivreNumeroDansCommande.Text = "";
            textBoxCommandeLivreNbExemplaires.Text = "";
            comboBoxCommandeLivreEtat.SelectedIndex = -1;
        }

        private void dataGridViewCommandeLivresListe_SelectionChanged(object sender, EventArgs e)
        {
            if (!(bdgCommandeLivresListe.Current is CommandeDocument commande))
            {
                ViderDetailsLivre();
                return;
            }

            RemplirCommande(commande);
            RemplirDetailsLivre(commande);
            if (operationEnCours != Operation.Ajouter)
            {
                FiltrerEtatsSuivisDisponibles(commande, comboBoxCommandeLivreEtat, operationEnCours);
            }
        }

        private void buttonCommandeLivreRechercher_Click(object sender, EventArgs e)
        {
            var input = textBoxCommandeLivreRecherche.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                RemplirCommandesLivreListe(lesCommandesLivres);
                ViderDetailsLivre();
                return;
            }

            // Recherche dans les commandes
            var commandesFiltrees = lesCommandesLivres
                .Where(c => c.IdDocument == input)
                .ToList();

            if (commandesFiltrees.Any())
            {
                RemplirCommandesLivreListe(commandesFiltrees);
                return;
            }

            // Si absent des commandes : Recherche dans les livres
            if (!lesLivres.Any())
            {
                lesLivres = controller.GetAllLivres();
            }

            var livre = lesLivres.FirstOrDefault(l => l.Id == input);

            if (livre != null)
            {
                dataGridViewCommandeLivresListe.ClearSelection();
                dataGridViewCommandeLivresListe.CurrentCell = null;

                RemplirDetailsLivreDepuisDocument(livre);
            }
            else
            {
                MessageBox.Show("Aucun livre trouvé", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RemplirCommandesLivreListe(lesCommandesLivres);
                ViderDetailsLivre();
            }
        }

        private void dataGridViewCommandeLivresListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dataGridViewCommandeLivresListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();

            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesCommandesLivres.OrderBy(o => o.Id).ToList();
                    break;
                case "Id Document":
                    sortedList = lesCommandesLivres.OrderBy(o => o.IdDocument).ToList();
                    break;
                case "Exemplaires":
                    sortedList = lesCommandesLivres.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Etat":
                    sortedList = lesCommandesLivres.OrderBy(o => o.LibelleSuivi).ToList();
                    break;
                case "Date":
                    sortedList = lesCommandesLivres.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesLivres.OrderBy(o => o.Montant).ToList();
                    break;
            }
            RemplirCommandesLivreListe(sortedList);
        }

        #endregion

        #region OngletCommandesDvds

        private readonly BindingSource bdgCommandeDvdsListe = new BindingSource();
        private List<CommandeDocument> lesCommandesDvds = new List<CommandeDocument>();

        private void tabCommandeDvd_Enter(object sender, EventArgs e)
        {
            lesCommandesDvds = controller.GetAllCommandesDocuments(TypeMedia.Dvd);
            RemplirCommandesDvdListe(lesCommandesDvds);
            ChargerSuivis();
        }

        private void SetModeCommandeDvd(Operation operation)
        {
            bool edition = operation == Operation.Ajouter || operation == Operation.Modifier;

            dataGridViewCommandeDvdsListe.Enabled = !edition;

            // Champs éditables
            dateTimePickerCommandeDvdDate.Enabled = edition;
            textBoxCommandeDvdMontant.ReadOnly = !edition;
            textBoxCommandeDvdNbExemplaires.ReadOnly = !edition;
            comboBoxCommandeDvdEtat.Enabled = edition;
            textBoxCommandeDvdRecherche.Enabled = !edition;

            // Modifiable uniquement à la création
            textBoxDvdNumeroDansCommande.ReadOnly = operation != Operation.Ajouter;

            // ID protégé
            textBoxLCommandeDvdNumero.ReadOnly = true;

            // Boutons
            buttonCommandeDvdAjouter.Enabled = !edition;
            buttonCommandeDvdModifier.Enabled = !edition;
            buttonCommandeDvdSupprimer.Enabled = !edition;

            buttonCommandeDvdValider.Enabled = edition;
            buttonCommandeDvdAnnuler.Enabled = edition;
            buttonCommandeDvdRechercher.Enabled = !edition;

            // Gestion du focus uniquement en mode édition
            if (edition)
            {
                Control focusControl = operation == Operation.Ajouter
                    ? textBoxDvdNumeroDansCommande
                    : textBoxCommandeDvdMontant;

                focusControl.Focus();
            }
        }

        private void buttonCommandeDvdAjouter_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.Ajouter;

            ViderCommandeDvd();
            if (textBoxCommandeDvdRecherche.Text != "")
            {
                textBoxDvdNumeroDansCommande.Text = textBoxCommandeDvdRecherche.Text.Trim();
                textBoxCommandeDvdRecherche.Text = "";
            }
            comboBoxCommandeDvdEtat.SelectedIndex = 0;
            SetModeCommandeDvd(operationEnCours);

            FiltrerEtatsSuivisDisponibles(null, comboBoxCommandeDvdEtat, Operation.Ajouter);
        }

        private void buttonCommandeDvdModifier_Click(object sender, EventArgs e)
        {
            if (bdgCommandeDvdsListe.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.");
                return;
            }

            if (!(bdgCommandeDvdsListe.Current is CommandeDocument commande))
            {
                MessageBox.Show("Veuillez sélectionner une commande à modifier.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationEnCours = Operation.Modifier;
            SetModeCommandeDvd(operationEnCours);

            RemplirCommandeDvd(commande);
            RemplirDetailsDvd(commande);

            FiltrerEtatsSuivisDisponibles(commande, comboBoxCommandeDvdEtat, Operation.Modifier);
        }

        private void buttonCommandeDvdSupprimer_Click(object sender, EventArgs e)
        {
            if (bdgCommandeDvdsListe.Count == 0)
            {
                MessageBox.Show("Aucune commande disponible.");
                return;
            }

            if (!(bdgCommandeDvdsListe.Current is CommandeDocument commande))
            {
                MessageBox.Show("Veuillez sélectionner une commande à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            operationEnCours = Operation.Supprimer;

            var result = MessageBox.Show(
                $"Supprimer la commande numéro '{commande.IdDocument}' ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerCommande(TypeMedia.Dvd, commande.Id);
                if (success)
                {
                    MessageBox.Show("Commande supprimée");
                    lesCommandesDvds = controller.GetAllCommandesDocuments(TypeMedia.Dvd);
                    RemplirCommandesDvdListe(lesCommandesDvds);
                    ViderDetailsDvd();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression");
                }
            }

            operationEnCours = Operation.None;
            tabCommandeDvd_Enter(null, null);
        }

        private void buttonCommandeDvdAnnuler_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.None;
            SetModeCommandeDvd(Operation.None);
            tabCommandeDvd_Enter(null, null);

            if (lesCommandesDvds.Any())
            {
                bdgCommandeDvdsListe.Position = 0;
            }
        }

        private void buttonCommandeDvdValider_Click(object sender, EventArgs e)
        {
            if (operationEnCours != Operation.Ajouter && operationEnCours != Operation.Modifier)
            {
                MessageBox.Show("Aucune opération en cours.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isNew = operationEnCours == Operation.Ajouter;
            string idCommande = textBoxLCommandeDvdNumero.Text;

            string messageErreur;
            if (!ValiderChampsCommandeDvd(isNew, out messageErreur))
            {
                MessageBox.Show(messageErreur);
                return;
            }

            // construction du command
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Dvd,
                Id = textBoxLCommandeDvdNumero.Text,
                DateCommande = dateTimePickerCommandeDvdDate.Value,
                NbExemplaire = int.Parse(textBoxCommandeDvdNbExemplaires.Text),
                Montant = double.Parse(textBoxCommandeDvdMontant.Text),
                IdLivreDvd = textBoxDvdNumeroDansCommande.Text,
                IdSuivi = (int)comboBoxCommandeDvdEtat.SelectedValue
            };

            Debug.WriteLine(
                $"controller.SauvegarderCommande, isNew : {isNew} - Id : {cmd.Id}, Livre/Dvd : {cmd.IdLivreDvd}, NbExemplaires : {cmd.NbExemplaire}, Montant : {cmd.Montant}, Date : {cmd.DateCommande}, IdSuivi : {cmd.IdSuivi}"
            );

            bool success = controller.SauvegarderCommande(cmd, isNew);

            if (success)
            {
                MessageBox.Show(isNew ? "Commande créée" : "Commande modifiée");

                operationEnCours = Operation.None;
                SetModeCommandeDvd(Operation.None);

                lesCommandesDvds = controller.GetAllCommandesDocuments(TypeMedia.Dvd);
                RemplirCommandesDvdListe(lesCommandesDvds);

                var updated = lesCommandesDvds.FirstOrDefault(c => c.Id == idCommande);
                if (updated != null)
                {
                    bdgCommandeDvdsListe.Position = lesCommandesDvds.IndexOf(updated);
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de l'enregistrement");
            }
        }

        private bool ValiderChampsCommandeDvd(bool isNew, out string messageErreur)
        {
            if (!isNew && string.IsNullOrWhiteSpace(textBoxLCommandeDvdNumero.Text))
            {
                messageErreur = "Numéro de commande obligatoire";
                return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxDvdNumeroDansCommande.Text))
            {
                messageErreur = "Document obligatoire";
                return false;
            }

            var dvd = lesDvd.FirstOrDefault(d => d.Id == textBoxDvdNumeroDansCommande.Text);
            if (dvd == null)
            {
                messageErreur = "Ce DVD n'existe pas.";
                return false;
            }

            if (!int.TryParse(textBoxCommandeDvdNbExemplaires.Text, out int nbExemplaires) || nbExemplaires <= 0)
            {
                messageErreur = "Nombre d'exemplaires invalide";
                return false;
            }

            if (!double.TryParse(textBoxCommandeDvdMontant.Text, out double montant) || montant < 0)
            {
                messageErreur = "Montant invalide";
                return false;
            }

            if (comboBoxCommandeDvdEtat.SelectedValue == null)
            {
                messageErreur = "État obligatoire";
                return false;
            }

            if (!isNew)
            {
                var commande = lesCommandesDvds
                    .FirstOrDefault(c => c.Id == textBoxLCommandeDvdNumero.Text);

                EtatSuivi nouvelEtat = (EtatSuivi)((int)comboBoxCommandeDvdEtat.SelectedValue);
                EtatSuivi ancienIdSuivi = (EtatSuivi)commande.IdSuivi;

                var transitionValide =
                    ancienIdSuivi == nouvelEtat
                    || transitionsAutorisees.ContainsKey(ancienIdSuivi)
                    && transitionsAutorisees[ancienIdSuivi].Contains(nouvelEtat);

                if (!transitionValide)
                {
                    messageErreur = $"Transition d'état invalide : {ancienIdSuivi} -> {nouvelEtat}";
                    return false;
                }
            }

            messageErreur = null;
            return true;
        }

        private void RemplirCommandesDvdListe(List<CommandeDocument> commandesDocument)
        {
            bdgCommandeDvdsListe.DataSource = commandesDocument;

            dataGridViewCommandeDvdsListe.AutoGenerateColumns = false;
            dataGridViewCommandeDvdsListe.DataSource = bdgCommandeDvdsListe;
            dataGridViewCommandeDvdsListe.Columns.Clear();

            dataGridViewCommandeDvdsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Id",
                DataPropertyName = "Id"
            });

            dataGridViewCommandeDvdsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Id Document",
                DataPropertyName = "IdDocument"
            });

            dataGridViewCommandeDvdsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Exemplaires",
                DataPropertyName = "NbExemplaire"
            });

            dataGridViewCommandeDvdsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Etat",
                DataPropertyName = "LibelleSuivi"
            });

            dataGridViewCommandeDvdsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date",
                DataPropertyName = "DateCommande"
            });

            dataGridViewCommandeDvdsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Montant",
                DataPropertyName = "Montant"
            });

            dataGridViewCommandeDvdsListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCommandeDvdsListe.AutoResizeColumns();
        }

        private void RemplirDetailsDvd(CommandeDocument commande)
        {
            if (!lesDvd.Any())
            {
                lesDvd = controller.GetAllDvd();
            }

            var dvd = lesDvd.FirstOrDefault(d => d.Id == commande.IdLivreDvd);

            if (dvd != null)
            {
                RemplirDetailsDvdDepuisDocument(dvd);
            }
            else
            {
                ViderDetailsDvd();
            }
        }

        private void RemplirDetailsDvdDepuisDocument(Document document)
        {
            if (document is Dvd dvd)
            {
                textBoxDvdNumero.Text = dvd.Id;
                textBoxDvdDuree.Text = dvd.Duree.ToString();
                textBoxDvdTitre.Text = dvd.Titre;
                textBoxDvdRealisateur.Text = dvd.Realisateur;
                textBoxDvdSynopsis.Text = dvd.Synopsis;
                textBoxDvdGenre.Text = dvd.Genre;
                textBoxDvdPublic.Text = dvd.Public;
                textBoxDvdRayon.Text = dvd.Rayon;
                textBoxDvdImage.Text = dvd.Image;
            }
        }

        private void ViderDetailsDvd()
        {
            textBoxDvdNumero.Text = "";
            textBoxDvdDuree.Text = "";
            textBoxDvdTitre.Text = "";
            textBoxDvdRealisateur.Text = "";
            textBoxDvdSynopsis.Text = "";
            textBoxDvdGenre.Text = "";
            textBoxDvdPublic.Text = "";
            textBoxDvdRayon.Text = "";
            textBoxDvdImage.Text = "";
        }

        private void RemplirCommandeDvd(CommandeDocument commande)
        {
            if (commande == null) return;

            textBoxLCommandeDvdNumero.Text = commande.Id ?? "";
            dateTimePickerCommandeDvdDate.Value = commande.DateCommande;
            textBoxCommandeDvdMontant.Text = commande.Montant.ToString("0.00");

            textBoxDvdNumeroDansCommande.Text = commande.IdLivreDvd ?? "";
            textBoxCommandeDvdNbExemplaires.Text = commande.NbExemplaire.ToString();

            if (commande.IdSuivi != 0)
            {
                comboBoxCommandeDvdEtat.SelectedValue = commande.IdSuivi;
            }
        }

        private void ViderCommandeDvd()
        {
            textBoxLCommandeDvdNumero.Text = "";
            dateTimePickerCommandeDvdDate.Value = DateTime.Now;
            textBoxCommandeDvdMontant.Text = "";
            textBoxDvdNumeroDansCommande.Text = "";
            textBoxCommandeDvdNbExemplaires.Text = "";
            comboBoxCommandeDvdEtat.SelectedIndex = -1;
        }

        private void dataGridViewCommandeDvdsListe_SelectionChanged(object sender, EventArgs e)
        {
            if (!(bdgCommandeDvdsListe.Current is CommandeDocument commande))
            {
                ViderDetailsDvd();
                return;
            }

            RemplirCommandeDvd(commande);
            RemplirDetailsDvd(commande);
            if (operationEnCours != Operation.Ajouter)
            {
                FiltrerEtatsSuivisDisponibles(commande, comboBoxCommandeDvdEtat, operationEnCours);
            }
        }

        private void buttonCommandeDvdRechercher_Click(object sender, EventArgs e)
        {
            var input = textBoxCommandeDvdRecherche.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                RemplirCommandesDvdListe(lesCommandesDvds);
                ViderDetailsDvd();
                return;
            }

            var commandesFiltrees = lesCommandesDvds
                .Where(c => c.IdDocument == input)
                .ToList();

            if (commandesFiltrees.Any())
            {
                RemplirCommandesDvdListe(commandesFiltrees);
                return;
            }

            if (!lesDvd.Any())
            {
                lesDvd = controller.GetAllDvd();
            }

            var dvd = lesDvd.FirstOrDefault(d => d.Id == input);

            if (dvd != null)
            {
                dataGridViewCommandeDvdsListe.ClearSelection();
                dataGridViewCommandeDvdsListe.CurrentCell = null;

                RemplirDetailsDvdDepuisDocument(dvd);
            }
            else
            {
                MessageBox.Show("Aucun DVD trouvé", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RemplirCommandesDvdListe(lesCommandesDvds);
                ViderDetailsDvd();
            }
        }

        private void dataGridViewCommandeDvdsListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dataGridViewCommandeDvdsListe.Columns[e.ColumnIndex].HeaderText;
            List<CommandeDocument> sortedList = new List<CommandeDocument>();

            switch (titreColonne)
            {
                case "Id":
                    sortedList = lesCommandesDvds.OrderBy(o => o.Id).ToList();
                    break;
                case "Id Document":
                    sortedList = lesCommandesDvds.OrderBy(o => o.IdDocument).ToList();
                    break;
                case "Exemplaires":
                    sortedList = lesCommandesDvds.OrderBy(o => o.NbExemplaire).ToList();
                    break;
                case "Etat":
                    sortedList = lesCommandesDvds.OrderBy(o => o.LibelleSuivi).ToList();
                    break;
                case "Date":
                    sortedList = lesCommandesDvds.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Montant":
                    sortedList = lesCommandesDvds.OrderBy(o => o.Montant).ToList();
                    break;
            }

            RemplirCommandesDvdListe(sortedList);
        }

        #endregion

        #region Abonnements Revues

        private readonly BindingSource bdgAbonnementsRevuesListe = new BindingSource();
        private List<Abonnement> lesAbonnementsRevues = new List<Abonnement>();

        private void tabCommandeRevue_Enter(object sender, EventArgs e)
        {
            SetModeCommandeRevue(Operation.None);
            lesAbonnementsRevues = controller.GetAllAbonnements();
            RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
        }

        private void SetModeCommandeRevue(Operation operation)
        {
            groupBoxRevueInfos.Enabled = false;

            bool creation = operation == Operation.Ajouter || operation == Operation.Modifier;

            dataGridViewAbonnementsListe.Enabled = !creation;

            // Champs éditables
            textBoxRevueNumeroDansCommande.ReadOnly = !creation;
            dateTimePickerCommandeRevueDate.Enabled = creation;
            dateTimePickerAbonnementFin.Enabled = creation;
            textBoxCommandeRevueMontant.ReadOnly = !creation;

            // ID protégé
            textBoxLCommandeRevueNumero.ReadOnly = true;

            // Boutons
            buttonAbonnementAjouter.Enabled = !creation;
            buttonAbonnementSupprimer.Enabled = !creation;

            buttonCommandeRevueValider.Enabled = creation;
            buttonCommandeRevueAnnuler.Enabled = creation;
            buttonAbonnementRechercher.Enabled = !creation;

            // focus
            Control focusControl = textBoxRevueNumeroDansCommande;
            focusControl.Focus();
        }

        private void buttonAbonnementAjouter_Click(object sender, EventArgs e)
        {
            ViderAbonnementRevue();
            operationEnCours = Operation.Ajouter;
            SetModeCommandeRevue(operationEnCours);

            var numeroDocument = "";

            if (bdgAbonnementsRevuesListe.Current is Abonnement abonnement)
            {
                numeroDocument = abonnement.IdRevue;
            }
            if (textBoxAbonnementRecherche.Text != "")
            {
                numeroDocument = textBoxAbonnementRecherche.Text.Trim();
                textBoxAbonnementRecherche.Text = "";
            }

            textBoxRevueNumeroDansCommande.Text = numeroDocument;
            dateTimePickerCommandeRevueDate.Value = DateTime.Now;
            dateTimePickerAbonnementFin.Value = DateTime.Now.AddMonths(12);

        }

        private void buttonAbonnementSupprimer_Click(object sender, EventArgs e)
        {
            if (bdgAbonnementsRevuesListe.Count == 0)
            {
                MessageBox.Show("Aucun abonnement disponible.");
                return;
            }

            if (!(bdgAbonnementsRevuesListe.Current is Abonnement abonnement))
            {
                MessageBox.Show("Veuillez sélectionner un abonnement à supprimer.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!controller.PeutSupprimerAbonnement(abonnement))
            {
                MessageBox.Show("Impossible de supprimer l'abonnement : des exemplaires sont encore dans la période.");
                return;
            }

            operationEnCours = Operation.Supprimer;

            var result = MessageBox.Show(
                $"Supprimer l'abonnement numéro '{abonnement.Id}' ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                bool success = controller.SupprimerAbonnement(abonnement);
                if (success)
                {
                    MessageBox.Show("Abonnement supprimé");
                    lesAbonnementsRevues = controller.GetAllAbonnements();
                    RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
                    ViderAbonnementRevue();
                }
                else
                {
                    MessageBox.Show("Erreur lors de la suppression");
                }
            }

            operationEnCours = Operation.None;
            tabCommandeRevue_Enter(null, null);
        }

        private void buttonCommandeRevueAnnuler_Click(object sender, EventArgs e)
        {
            operationEnCours = Operation.None;
            SetModeCommandeRevue(operationEnCours);
            tabCommandeRevue_Enter(null, null);

            // Repositionnement sur le premier élément
            if (lesAbonnementsRevues.Any())
            {
                bdgAbonnementsRevuesListe.Position = 0;
            }
        }

        private void buttonCommandeRevueValider_Click(object sender, EventArgs e)
        {
            if (operationEnCours != Operation.Ajouter && operationEnCours != Operation.Modifier)
            {
                MessageBox.Show("Aucune opération en cours.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string idAbonnement = textBoxLCommandeRevueNumero.Text;

            string messageErreur;
            if (!ValiderChampsCommandeRevue(out messageErreur))
            {
                MessageBox.Show(messageErreur);
                return;
            }

            // construction du command
            var cmd = new CreerCommandeCommand
            {
                Type = TypeMedia.Revue,
                Id = textBoxLCommandeRevueNumero.Text,
                DateCommande = dateTimePickerCommandeRevueDate.Value,
                DateFinAbonnement = dateTimePickerAbonnementFin.Value,
                Montant = double.Parse(textBoxCommandeRevueMontant.Text),
                IdRevue = textBoxRevueNumeroDansCommande.Text
            };

            Debug.WriteLine(
                $"controller.SauvegarderAbonnement, Id : {cmd.Id}, Revue : {cmd.IdRevue}, DateCommande : {cmd.DateCommande}, DateFinAbonnement : {cmd.DateFinAbonnement}, Montant : {cmd.Montant}"
            );

            bool success = controller.SauvegarderCommande(cmd, operationEnCours == Operation.Ajouter);

            if (success)
            {
                MessageBox.Show("Abonnement créé");
                operationEnCours = Operation.None;
                SetModeCommandeRevue(operationEnCours);
                lesAbonnementsRevues = controller.GetAllAbonnements();
                RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
                var updated = lesAbonnementsRevues.FirstOrDefault(a => a.Id == idAbonnement);
                if (updated != null)
                {
                    bdgAbonnementsRevuesListe.Position = lesAbonnementsRevues.IndexOf(updated);
                }
            }
            else
            {
                MessageBox.Show("Erreur lors de l'enregistrement");
            }
        }

        private bool ValiderChampsCommandeRevue(out string messageErreur)
        {
            if (string.IsNullOrWhiteSpace(textBoxRevueNumeroDansCommande.Text))
            {
                messageErreur = "Document obligatoire";
                return false;
            }

            var revue = lesRevues.FirstOrDefault(r => r.Id == textBoxRevueNumeroDansCommande.Text);
            if (revue == null)
            {
                messageErreur = "Cette revue n'existe pas.";
                return false;
            }

            if (dateTimePickerAbonnementFin.Value <= dateTimePickerCommandeRevueDate.Value)
            {
                messageErreur = "La date de fin doit être postérieure à la date de début.";
                return false;
            }

            if (!double.TryParse(textBoxCommandeRevueMontant.Text, out double montant) || montant < 0)
            {
                messageErreur = "Montant invalide";
                return false;
            }

            messageErreur = null;
            return true;
        }

        private void RemplirAbonnementsRevuesListe(List<Abonnement> abonnements)
        {
            bdgAbonnementsRevuesListe.DataSource = abonnements;
            dataGridViewAbonnementsListe.AutoGenerateColumns = false;
            dataGridViewAbonnementsListe.DataSource = bdgAbonnementsRevuesListe;
            dataGridViewAbonnementsListe.Columns.Clear();
            dataGridViewAbonnementsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Id",
                DataPropertyName = "Id"
            });
            dataGridViewAbonnementsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Id Revue",
                DataPropertyName = "IdRevue"
            });
            dataGridViewAbonnementsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date début",
                DataPropertyName = "DateCommande"
            });
            dataGridViewAbonnementsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date Fin",
                DataPropertyName = "DateFinAbonnement"
            });
            dataGridViewAbonnementsListe.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Montant",
                DataPropertyName = "Montant"
            });
            dataGridViewAbonnementsListe.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewAbonnementsListe.AutoResizeColumns();
        }

        private void RemplirDetailsRevue(Abonnement abonnement)
        {
            if (!lesRevues.Any())
            {
                lesRevues = controller.GetAllRevues();
            }

            var revue = lesRevues.FirstOrDefault(r => r.Id == abonnement.IdRevue);

            if (revue != null)
            {
                RemplirDetailsRevueDepuisDocument(revue);
            }
            else
            {
                ViderDetailsRevue();
            }
        }

        private void RemplirDetailsRevueDepuisDocument(Document document)
        {
            if (document is Revue revue)
            {
                textBoxRevueNumero.Text = revue.Id;
                textBoxRevueTitre.Text = revue.Titre;
                textBoxRevueGenre.Text = revue.Genre;
                textBoxRevuePublic.Text = revue.Public;
                textBoxRevueRayon.Text = revue.Rayon;
                textBoxRevuePeriodicite.Text = revue.Periodicite;
                textBoxRevueDateMiseADispo.Text = revue.DelaiMiseADispo.ToString();
                textBoxRevueImage.Text = revue.Image;
            }
        }

        private void ViderDetailsRevue()
        {
            textBoxRevueNumero.Text = "";
            textBoxRevueTitre.Text = "";
            textBoxRevueGenre.Text = "";
            textBoxRevuePublic.Text = "";
            textBoxRevueRayon.Text = "";
            textBoxRevuePeriodicite.Text = "";
            textBoxRevueDateMiseADispo.Text = "";
            textBoxRevueImage.Text = "";
        }

        private void ViderAbonnementRevue()
        {
            textBoxLCommandeRevueNumero.Text = "";
            textBoxRevueNumeroDansCommande.Text = "";
            dateTimePickerCommandeRevueDate.Value = DateTime.Now;
            dateTimePickerAbonnementFin.Value = DateTime.Now.AddMonths(12);
            textBoxCommandeRevueMontant.Text = "";
        }

        private void RemplirAbonnementRevue(Abonnement abonnement)
        {
            if (abonnement == null) return;
            textBoxLCommandeRevueNumero.Text = "";
            textBoxRevueNumeroDansCommande.Text = abonnement.IdRevue ?? "";
            dateTimePickerCommandeRevueDate.Value = abonnement.DateCommande;
            dateTimePickerAbonnementFin.Value = abonnement.DateFinAbonnement;
            textBoxCommandeRevueMontant.Text = abonnement.Montant.ToString("0.00");
        }

        private void dataGridViewAbonnementsListe_SelectionChanged(object sender, EventArgs e)
        {
            if (!(bdgAbonnementsRevuesListe.Current is Abonnement abonnement))
            {
                ViderDetailsRevue();
                return;
            }
            
            RemplirAbonnementRevue(abonnement);
            RemplirDetailsRevue(abonnement);
        }

        private void buttonAbonnementRechercher_Click(object sender, EventArgs e)
        {
            var input = textBoxAbonnementRecherche.Text.Trim();
            if (string.IsNullOrEmpty(input))
            {
                RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
                ViderDetailsRevue();
                return;
            }

            var abonnementsFiltrees = lesAbonnementsRevues
                .Where(a => a.IdRevue == input)
                .ToList();

            if (abonnementsFiltrees.Any())
            {
                RemplirAbonnementsRevuesListe(abonnementsFiltrees);
                return;
            }

            if (!lesRevues.Any())
            {
                lesRevues = controller.GetAllRevues();
            }

            var revue = lesRevues.FirstOrDefault(r => r.Id == input);

            if (revue != null) 
            {
                dataGridViewAbonnementsListe.ClearSelection();
                dataGridViewAbonnementsListe.CurrentCell = null;
                RemplirDetailsRevueDepuisDocument(revue);
            }
            else
            {
                MessageBox.Show("Aucune revue trouvée", "Information",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                RemplirAbonnementsRevuesListe(lesAbonnementsRevues);
                ViderDetailsRevue();
            }
        }

        private void dataGridViewAbonnementsListe_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string titreColonne = dataGridViewAbonnementsListe.Columns[e.ColumnIndex].HeaderText;
            List<Abonnement> sortedList = new List<Abonnement>();

            switch(titreColonne)
            {
                case "Id":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.Id).ToList();
                    break;
                case "Id Revue":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.IdRevue).ToList();
                    break;
                case "Date début":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.DateCommande).ToList();
                    break;
                case "Date Fin":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.DateFinAbonnement).ToList();
                    break;
                case "Montant":
                    sortedList = lesAbonnementsRevues.OrderBy(o => o.Montant).ToList();
                    break;
            }

            RemplirAbonnementsRevuesListe(sortedList);
        }



        #endregion

        #region Exemplaires

        private void InitGridExemplaires(DataGridView dgv, BindingSource source)
        {
            dgv.AutoGenerateColumns = false;
            dgv.Columns.Clear();

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "Numero",
                HeaderText = "Numéro"
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "LibelleEtat",
                HeaderText = "Etat"
            });

            dgv.Columns.Add(new DataGridViewTextBoxColumn
            {
                DataPropertyName = "DateAchat",
                HeaderText = "Date achat",
                DefaultCellStyle = { Format = "dd/MM/yyyy" }
            });

            dgv.RowHeadersVisible = false;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgv.DataSource = source;
        }

        private List<Exemplaire> TrierExemplaires(DataGridView dgv, List<Exemplaire> liste, int columnIndex)
        {
            string titreColonne = dgv.Columns[columnIndex].HeaderText;
            List<Exemplaire> sortedList = new List<Exemplaire>();

            switch (titreColonne)
            {
                case "Numéro":
                    sortedList = liste.OrderBy(o => o.Numero).ToList();
                    break;

                case "Etat":
                    sortedList = liste.OrderBy(o => o.LibelleEtat).ToList();
                    break;

                case "Date achat":
                    sortedList = liste.OrderBy(o => o.DateAchat).ToList();
                    break;
            }

            return sortedList;
        }

        #endregion
    }

}
