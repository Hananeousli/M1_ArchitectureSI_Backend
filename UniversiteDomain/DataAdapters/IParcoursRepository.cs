using UniversiteDomain.Entities;

namespace UniversiteDomain.DataAdapters;

/// 
/// Interface du repository pour les parcours.
/// Hérite des opérations CRUD génériques de IRepository.
/// 
public interface IParcoursRepository : IRepository<Parcours>
{
    Task<Parcours> AddEtudiantAsync(Parcours parcours, Etudiant etudiant);
    Task<Parcours> AddEtudiantAsync(long idParcours, long idEtudiant);
    Task<Parcours> AddEtudiantAsync(Parcours? parcours, List<Etudiant> etudiants);
    Task<Parcours> AddEtudiantAsync(long idParcours, long[] idEtudiants);
    Task<Parcours> AddUeAsync(long idParcours, long idUe);
    Task<Parcours> AddUeAsync(long idParcours, long[] idUes);
}