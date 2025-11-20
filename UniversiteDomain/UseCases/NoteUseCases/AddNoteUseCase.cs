using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.Exceptions.EtudiantExceptions;
using UniversiteDomain.Exceptions.UeExceptions;
using UniversiteDomain.Exceptions.NoteExceptions;
using UniversiteDomain.Exceptions.ParcoursExceptions;

namespace UniversiteDomain.UseCases.NoteUseCases;

public class AddNoteUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<Note> ExecuteAsync(long etudiantId, long ueId, float valeur)
    {
        await CheckBusinessRules(etudiantId, ueId, valeur);
        return await repositoryFactory.NoteRepository().AddNoteAsync(etudiantId, ueId, valeur);
    }
    
    private async Task CheckBusinessRules(long etudiantId, long ueId, float valeur)
    {
        // Vérification de la valeur de la note (entre 0 et 20)
        if (valeur < 0 || valeur > 20)
            throw new InvalidNoteValueException($"La note doit être comprise entre 0 et 20. Valeur fournie : {valeur}");
        
        // Vérification que l'étudiant existe
        var etudiants = await repositoryFactory.EtudiantRepository().FindByConditionAsync(e => e.Id == etudiantId);
        if (etudiants.Count == 0)
            throw new EtudiantNotFoundException($"Etudiant {etudiantId} non trouvé");
        
        var etudiant = etudiants[0];
        
        // Vérification que l'UE existe
        var ues = await repositoryFactory.UeRepository().FindByConditionAsync(u => u.Id == ueId);
        if (ues.Count == 0)
            throw new UeNotFoundException($"UE {ueId} non trouvée");
        
        // Vérifier que l'étudiant est inscrit dans un parcours
        if (etudiant.ParcoursSuivi == null)
            throw new EtudiantNotFoundException($"L'étudiant {etudiantId} n'est inscrit dans aucun parcours");
        
        // Récupérer le parcours de l'étudiant avec ses UEs
        var parcours = await repositoryFactory.ParcoursRepository().FindByConditionAsync(p => p.Id == etudiant.ParcoursSuivi.Id);
        if (parcours.Count == 0)
            throw new ParcoursNotFoundException($"Parcours de l'étudiant non trouvé");
        
        // Vérifier que l'UE fait partie du parcours de l'étudiant
        if (parcours[0].UesEnseignees == null || !parcours[0].UesEnseignees.Any(u => u.Id == ueId))
            throw new UeNotInParcoursException($"L'UE {ueId} ne fait pas partie du parcours de l'étudiant {etudiantId}");
        
        // Vérifier qu'une note n'existe pas déjà pour cet étudiant dans cette UE
        var notesExistantes = await repositoryFactory.NoteRepository().FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);
        if (notesExistantes.Count > 0)
            throw new DuplicateNoteException($"L'étudiant {etudiantId} a déjà une note dans l'UE {ueId}");
    }
}