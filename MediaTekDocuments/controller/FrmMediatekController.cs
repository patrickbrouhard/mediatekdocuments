using MediaTekDocuments.commands;
using MediaTekDocuments.dal;
using MediaTekDocuments.model;
using MediaTekDocuments.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static MediaTekDocuments.view.FrmMediatek;

namespace MediaTekDocuments.controller
{
    /// <summary>
    /// Contrôleur lié à FrmMediatek
    /// </summary>
    class FrmMediatekController
    {
        /// <summary>
        /// Objet d'accès aux données
        /// </summary>
        private readonly Access access;
        private readonly AbonnementService abonnementService = new AbonnementService();

        /// <summary>
        /// Récupération de l'instance unique d'accès aux données
        /// </summary>
        public FrmMediatekController()
        {
            access = Access.GetInstance();
        }

        /// <summary>
        /// getter sur la liste des genres
        /// </summary>
        /// <returns>Liste d'objets Genre</returns>
        public List<Categorie> GetAllGenres()
        {
            return access.GetAllGenres();
        }

        /// <summary>
        /// getter sur la liste des livres
        /// </summary>
        /// <returns>Liste d'objets Livre</returns>
        public List<Livre> GetAllLivres()
        {
            return access.GetAllLivres();
        }

        /// <summary>
        /// getter sur la liste des Dvd
        /// </summary>
        /// <returns>Liste d'objets dvd</returns>
        public List<Dvd> GetAllDvd()
        {
            return access.GetAllDvd();
        }

        /// <summary>
        /// getter sur la liste des revues
        /// </summary>
        /// <returns>Liste d'objets Revue</returns>
        public List<Revue> GetAllRevues()
        {
            return access.GetAllRevues();
        }

        /// <summary>
        /// getter sur les rayons
        /// </summary>
        /// <returns>Liste d'objets Rayon</returns>
        public List<Categorie> GetAllRayons()
        {
            return access.GetAllRayons();
        }

        /// <summary>
        /// getter sur les publics
        /// </summary>
        /// <returns>Liste d'objets Public</returns>
        public List<Categorie> GetAllPublics()
        {
            return access.GetAllPublics();
        }

        public List<CommandeDocument> GetAllCommandesDocuments(TypeMedia type)
        {
            return access.GetAllCommandesDocuments(type);
        }

        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// Récupère la liste complète des objets d'état disponibles.
        /// </summary>
        /// <returns>Une liste d'objets <see cref="Etat"/> représentant tous les états existants. La liste est vide s'il n'existe
        /// aucun état.</returns>
        public List<Etat> GetAllEtats()
        {
            return access.GetAllEtats();
        }

        /// <summary>
        /// récupère les exemplaires d'un document à partir de son id
        /// </summary>
        /// <param name="idDocument">id du document concerné</param>
        /// <returns>Liste d'objets Exemplaire</returns>
        public List<Exemplaire> GetExemplairesDocument(string idDocument)
        {
            return access.GetExemplairesDocument(idDocument);
        }

        /// <summary>
        /// Crée un exemplaire d'une revue dans la bdd
        /// </summary>
        /// <param name="exemplaire">L'objet Exemplaire concerné</param>
        /// <returns>True si la création a pu se faire</returns>
        public bool CreerExemplaire(Exemplaire exemplaire)
        {
            return access.CreerExemplaire(exemplaire);
        }

        /// <summary>
        /// Modifie les informations d'un exemplaire existant dans le système.
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si la modification a réussi ; sinon, false.</returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            return access.ModifierExemplaire(exemplaire);
        }

        /// <summary>
        /// Supprime l'exemplaire spécifié de la collection ou de la source de données sous-jacente.
        /// </summary>
        /// <param name="exemplaire"></param>
        /// <returns>true si l'exemplaire a été supprimé avec succès ; sinon, false.</returns>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            return access.SupprimerExemplaire(exemplaire);
        }

        public bool SauvegarderDocument(CreerDocumentCommand cmd, bool isNew)
        {
            System.Diagnostics.Debug.WriteLine($"isNew = {isNew}");
            Document doc = DocumentFactory.Creer(cmd);

            if (isNew)
                return access.AjouterDocument(doc);
            else
                return access.ModifierDocument(doc);
        }

        public bool SupprimerDocument(TypeMedia type, string id)
        {
            Document doc = DocumentFactory.Creer(new CreerDocumentCommand { Type = type, Id = id });
            return access.SupprimerDocument(doc);
        }

        public bool SauvegarderCommande(CreerCommandeCommand cmd, bool isNew)
        {
            Commande commande = CommandeFactory.Creer(cmd);

            if (isNew)
                return access.AjouterCommande(commande);
            else
                return access.ModifierCommande(commande);
        }

        public bool SupprimerCommande(TypeMedia type, string id)
        {
            Commande commande = CommandeFactory.Creer(new CreerCommandeCommand { Type = type, Id = id });
            return access.SupprimerCommande(commande);
        }

        public List<Abonnement> GetAllAbonnements()
        {
            return access.GetAllAbonnements();
        }

        public List<AlerteAbonnement> GetAbonnementsArrivantAExpiration()
        {
            return access.getAbonnementsExpirantDans(30);
        }

        public bool AJouterAbonnement(Abonnement abonnement)
        {
            return access.AjouterAbonnement(abonnement);
        }

        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            return access.SupprimerAbonnement(abonnement);
        }

        public bool PeutSupprimerAbonnement(Abonnement abonnement)
        {
            var exemplaires = GetExemplairesDocument(abonnement.IdRevue);
            return abonnementService.PeutSupprimerAbonnement(
                abonnement,
                exemplaires
            );
        }

        public Utilisateur GetUtilisateur(string login, string password)
        {
            return access.Authentifier(login, password);
        }
    }
}