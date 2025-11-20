using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.UeExceptions;

namespace UniversiteDomain.UseCases.UeUseCases.Create;

public class CreateUeUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Ue> ExecuteAsync(string numeroUe, string intitule)
    {
        // Validation du numéro
        if (string.IsNullOrWhiteSpace(numeroUe))
            throw new InvalidNumeroUeException("Le numéro de l'UE ne peut pas être vide");

        // Validation de l'intitulé (> 3 caractères)
        if (string.IsNullOrWhiteSpace(intitule) || intitule.Length <= 3)
            throw new InvalidIntituleUeException("L'intitulé de l'UE doit contenir plus de 3 caractères");

        // Vérifier que le numéro n'existe pas déjà
        var existing = await repositoryFactory.UeRepository().FindByConditionAsync(u => u.NumeroUe == numeroUe);
        if (existing.Count > 0)
            throw new DuplicateUeException($"Une UE avec le numéro {numeroUe} existe déjà");

        // Créer l'UE
        Ue ue = new Ue
        {
            NumeroUe = numeroUe,
            Intitule = intitule
        };

        return await repositoryFactory.UeRepository().CreateAsync(ue);
    }
    
    public async Task<Ue> ExecuteAsync(Ue ue)
    {
        return await ExecuteAsync(ue.NumeroUe, ue.Intitule);
    }
}