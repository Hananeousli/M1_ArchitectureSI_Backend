using UniversiteDomain.Entities;
using System.Linq.Expressions;

namespace UniversiteDomain.DataAdapters;

public interface IUeRepository : IRepository<Ue>
{
    // Recherche d'une UE par son numéro
    Task<Ue?> FindByNumeroAsync(string numeroUe);
    
    // Recherche des UEs d'un parcours spécifique
    Task<List<Ue>> FindByParcoursAsync(long parcoursId);
    
    // Recherche des UEs par année de formation
    Task<List<Ue>> FindByAnneeFormationAsync(int anneeFormation);
    
    // Vérifier si un numéro d'UE existe déjà
    Task<bool> ExistsNumeroUeAsync(string numeroUe);
    
    // Obtenir les UEs avec leurs étudiants inscrits
    Task<List<Ue>> GetUesWithStudentsAsync();
    
    // Obtenir une UE avec toutes ses notes
    Task<Ue?> GetUeWithNotesAsync(long ueId);
}