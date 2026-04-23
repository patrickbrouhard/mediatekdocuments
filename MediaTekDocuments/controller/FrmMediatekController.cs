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
        /// <returns>Liste d'objets Dvd</returns>
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

        /// <summary>
        /// Récupère la liste des commandes pour un type de média donné.
        /// </summary>
        /// <param name="type">Le type de média (Livre, Dvd, Revue).</param>
        /// <returns>Liste des commandes correspondantes.</returns>
        public List<CommandeDocument> GetAllCommandesDocuments(TypeMedia type)
        {
            return access.GetAllCommandesDocuments(type);
        }

        /// <summary>
        /// Récupère la liste de tous les états de suivi possibles.
        /// </summary>
        /// <returns>Liste d'objets Suivi.</returns>
        public List<Suivi> GetAllSuivis()
        {
            return access.GetAllSuivis();
        }

        /// <summary>
        /// Récupère la liste complète des objets d'état disponibles.
        /// </summary>
        /// <returns>Une liste d'objets Etat représentant tous les états existants.</returns>
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
        /// Crée un exemplaire dans la bdd
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
        /// <param name="exemplaire">L'exemplaire contenant les modifications.</param>
        /// <returns>true si la modification a réussi ; sinon, false.</returns>
        public bool ModifierExemplaire(Exemplaire exemplaire)
        {
            return access.ModifierExemplaire(exemplaire);
        }

        /// <summary>
        /// Supprime l'exemplaire spécifié de la source de données.
        /// </summary>
        /// <param name="exemplaire">L'exemplaire à supprimer.</param>
        /// <returns>true si l'exemplaire a été supprimé avec succès ; sinon, false.</returns>
        public bool SupprimerExemplaire(Exemplaire exemplaire)
        {
            return access.SupprimerExemplaire(exemplaire);
        }

        /// <summary>
        /// Crée ou met à jour un document dans la base de données.
        /// </summary>
        /// <param name="cmd">Commande contenant les données du document.</param>
        /// <param name="isNew">Indique s'il s'agit d'un nouveau document (true) ou d'une modification (false).</param>
        /// <returns>True si la sauvegarde a réussi.</returns>
        public bool SauvegarderDocument(CreerDocumentCommand cmd, bool isNew)
        {
            System.Diagnostics.Debug.WriteLine($"isNew = {isNew}");
            Document doc = DocumentFactory.Creer(cmd);

            if (isNew)
                return access.AjouterDocument(doc);
            else
                return access.ModifierDocument(doc);
        }

        /// <summary>
        /// Supprime un document de la base de données.
        /// </summary>
        /// <param name="type">Le type du média.</param>
        /// <param name="id">L'identifiant du document à supprimer.</param>
        /// <returns>True si la suppression a réussi.</returns>
        public bool SupprimerDocument(TypeMedia type, string id)
        {
            Document doc = DocumentFactory.Creer(new CreerDocumentCommand { Type = type, Id = id });
            return access.SupprimerDocument(doc);
        }

        /// <summary>
        /// Crée ou met à jour une commande dans la base de données.
        /// </summary>
        /// <param name="cmd">Commande contenant les informations d'achat.</param>
        /// <param name="isNew">Indique s'il s'agit d'une nouvelle commande (true) ou d'une modification (false).</param>
        /// <returns>True si la commande a été sauvegardée.</returns>
        public bool SauvegarderCommande(CreerCommandeCommand cmd, bool isNew)
        {
            Commande commande = CommandeFactory.Creer(cmd);

            if (isNew)
                return access.AjouterCommande(commande);
            else
                return access.ModifierCommande(commande);
        }

        /// <summary>
        /// Supprime une commande de la base de données.
        /// </summary>
        /// <param name="type">Le type de média concerné par la commande.</param>
        /// <param name="id">L'identifiant de la commande à supprimer.</param>
        /// <returns>True si la suppression a réussi.</returns>
        public bool SupprimerCommande(TypeMedia type, string id)
        {
            Commande commande = CommandeFactory.Creer(new CreerCommandeCommand { Type = type, Id = id });
            return access.SupprimerCommande(commande);
        }

        /// <summary>
        /// Récupère l'intégralité des abonnements aux revues.
        /// </summary>
        /// <returns>Liste des abonnements en base.</returns>
        public List<Abonnement> GetAllAbonnements()
        {
            return access.GetAllAbonnements();
        }

        /// <summary>
        /// Récupère la liste des abonnements expirant dans les 30 prochains jours.
        /// </summary>
        /// <returns>Liste d'objets AlerteAbonnement.</returns>
        public List<AlerteAbonnement> GetAbonnementsArrivantAExpiration()
        {
            return access.GetAbonnementsExpirantDans(30);
        }

        /// <summary>
        /// Ajoute un nouvel abonnement dans la base de données.
        /// </summary>
        /// <param name="abonnement">L'abonnement à ajouter.</param>
        /// <returns>True si l'ajout a réussi.</returns>
        public bool AJouterAbonnement(Abonnement abonnement)
        {
            return access.AjouterAbonnement(abonnement);
        }

        /// <summary>
        /// Supprime un abonnement existant de la base de données.
        /// </summary>
        /// <param name="abonnement">L'abonnement à supprimer.</param>
        /// <returns>True si la suppression a réussi.</returns>
        public bool SupprimerAbonnement(Abonnement abonnement)
        {
            return access.SupprimerAbonnement(abonnement);
        }

        /// <summary>
        /// Vérifie s'il est possible de supprimer l'abonnement spécifié en vérifiant ses éventuels exemplaires liés.
        /// </summary>
        /// <param name="abonnement">L'abonnement à vérifier.</param>
        /// <returns>True si l'abonnement peut être supprimé en toute sécurité.</returns>
        public bool PeutSupprimerAbonnement(Abonnement abonnement)
        {
            var exemplaires = GetExemplairesDocument(abonnement.IdRevue);
            return AbonnementService.PeutSupprimerAbonnement(
                abonnement,
                exemplaires
            );
        }
    }
}