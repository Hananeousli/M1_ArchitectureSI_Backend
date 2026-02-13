using UniversiteDomain.DataAdapters.DataAdaptersFactory;

namespace UniversiteDomain.JeuxDeDonnees;

public abstract class BdBuilder(IRepositoryFactory repositoryFactory)
{
    public async Task BuildUniversiteBdAsync()
    {
        // Suppression de la BD
        Console.WriteLine("Suppression et recréation de la BD");
        await RegenererBdAsync();
    
        // IMPORTANT: Créer les rôles EN PREMIER (avant tout le reste)
        Console.WriteLine("BuildRoles");
        await BuildRolesAsync();
    
        Console.WriteLine("BuildParcours");
        await BuildParcoursAsync();
    
        Console.WriteLine("BuildUes");
        await BuildUesAsync();
    
        Console.WriteLine("BuildMaquette");
        await BuildMaquetteAsync();
    
        Console.WriteLine("BuildEtudiants");
        await BuildEtudiantsAsync();
    
        Console.WriteLine("InscrireEtudiants");
        await InscrireEtudiantsAsync();
    
        Console.WriteLine("Noter");
        await NoterAsync();
    
        // Création des utilisateurs (étudiants + autres)
        // APRES avoir créé les entités Etudiant
        Console.WriteLine("BuildUsers");
        await BuildUsersAsync();
    }

    protected abstract Task RegenererBdAsync();
    protected abstract Task BuildRolesAsync();
    protected abstract Task BuildUsersAsync();
    protected abstract Task BuildParcoursAsync();
    protected abstract Task BuildEtudiantsAsync();
    protected abstract Task BuildUesAsync();
    protected abstract Task InscrireEtudiantsAsync();
    protected abstract Task BuildMaquetteAsync();
    protected abstract Task NoterAsync();
}