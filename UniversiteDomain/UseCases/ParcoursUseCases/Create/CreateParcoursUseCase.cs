using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.ParcoursUseCases.Create;

public class CreateParcoursUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Parcours> ExecuteAsync(string nomParcours, int anneeFormation)
    {
        // Validation du nom
        if (string.IsNullOrWhiteSpace(nomParcours))
            throw new InvalidNomParcoursException("Le nom du parcours ne peut pas être vide");

        // Validation de l'année
        if (anneeFormation < 1 || anneeFormation > 5)
            throw new InvalidAnneeFormationException("L'année de formation doit être entre 1 et 5");

        // Vérifier les doublons (si vous avez cette règle)
        var existing = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.NomParcours == nomParcours && p.AnneeFormation == anneeFormation);
        if (existing.Count > 0)
            throw new DuplicateParcoursException($"Un parcours {nomParcours} M{anneeFormation} existe déjà");

        // Créer le parcours
        Parcours parcours = new Parcours
        {
            NomParcours = nomParcours,
            AnneeFormation = anneeFormation
        };

        return await repositoryFactory.ParcoursRepository().CreateAsync(parcours);
    }
    
    // Surcharge pour accepter directement un objet Parcours
    public async Task<Parcours> ExecuteAsync(Parcours parcours)
    {
        return await ExecuteAsync(parcours.NomParcours, parcours.AnneeFormation);
    }
}