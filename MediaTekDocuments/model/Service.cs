namespace MediaTekDocuments.model
{
    /// <summary>
    /// Enum représentant les services métier de l'application.
    /// Correspond exactement à la table SQL service.
    /// Les valeurs doivent rester synchronisées avec la BDD.
    /// </summary>
    public enum Service
    {
        Administrateur = 0,
        Administratif = 1,
        Prets = 2,
        Culture = 3
    }
}