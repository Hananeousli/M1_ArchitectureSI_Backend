using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Dtos;
using UniversiteDomain.Entities;

namespace UniversiteDomain.UseCases.NoteUseCases.ImportFromCsv;

public class ImportNotesFromCsvUseCase(IRepositoryFactory repositoryFactory)
{
    public async Task<(int success, List<string> errors, List<EtudiantCompletDto> etudiants)> ExecuteAsync(Stream csvStream, long ueId)
{
    int successCount = 0;
    List<string> errors = new List<string>();
    List<NoteCsvDto> allRecords = new List<NoteCsvDto>();
    List<long> etudiantIdsFromCsv = new List<long>(); // ✅ TOUS les étudiants du CSV

    try
    {
        using (var reader = new StreamReader(csvStream))
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                BadDataFound = null
            };
        
            using (var csv = new CsvReader(reader, config))
            {
                allRecords = csv.GetRecords<NoteCsvDto>().ToList();
            }
        }

        // Valider TOUT avant d'enregistrer
        int lineNumber = 2;
        foreach (var record in allRecords)
        {
            try
            {
                await ValidateRecordAsync(record, ueId);
            }
            catch (Exception e)
            {
                errors.Add($"Ligne {lineNumber}: {e.Message}");
            }
            lineNumber++;
        }

        // Si erreurs, ARRÊTER
        if (errors.Any())
            return (0, errors, new List<EtudiantCompletDto>());

        // Enregistrer les notes ET tracker TOUS les étudiants du CSV
        foreach (var record in allRecords)
        {
            var etudiants = await repositoryFactory.EtudiantRepository()
                .FindByConditionAsync(e => e.NumEtud == record.NumEtud);
            var etudiantId = etudiants.First().Id;
            
            etudiantIdsFromCsv.Add(etudiantId); // ✅ Ajouter même si pas de note
            
            if (record.Valeur.HasValue)
            {
                await ProcessNoteAsync(record, ueId, etudiantId);
                successCount++;
            }
        }

        // ✅ Récupérer TOUS les étudiants du CSV avec leurs notes complètes
        List<EtudiantCompletDto> etudiantsComplets = new List<EtudiantCompletDto>();
        
        foreach (var etudiantId in etudiantIdsFromCsv)
        {
            var etudiant = await repositoryFactory.EtudiantRepository().FindEtudiantCompletAsync(etudiantId);
            if (etudiant != null)
            {
                etudiantsComplets.Add(new EtudiantCompletDto().ToDto(etudiant));
            }
        }

        return (successCount, errors, etudiantsComplets);
    }
    catch (Exception e)
    {
        errors.Add($"Erreur de lecture du CSV: {e.Message}");
        return (0, errors, new List<EtudiantCompletDto>());
    }
}

private async Task ValidateRecordAsync(NoteCsvDto noteDto, long ueId)
{
    if (string.IsNullOrEmpty(noteDto.NumEtud))
        throw new ArgumentException("NumEtud est obligatoire");

    if (noteDto.Valeur.HasValue && (noteDto.Valeur < 0 || noteDto.Valeur > 20))
        throw new ArgumentException("La note doit être entre 0 et 20");

    var etudiants = await repositoryFactory.EtudiantRepository()
        .FindByConditionAsync(e => e.NumEtud == noteDto.NumEtud);

    if (!etudiants.Any())
        throw new ArgumentException($"Étudiant '{noteDto.NumEtud}' non trouvé");
}

private async Task ProcessNoteAsync(NoteCsvDto noteDto, long ueId, long etudiantId)
{
    var notesExistantes = await repositoryFactory.NoteRepository()
        .FindByConditionAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);

    if (notesExistantes.Any())
    {
        var noteExistante = notesExistantes.First();
        noteExistante.Valeur = noteDto.Valeur!.Value;
        await repositoryFactory.NoteRepository().UpdateAsync(noteExistante);
    }
    else
    {
        Note nouvelleNote = new Note
        {
            EtudiantId = etudiantId,
            UeId = ueId,
            Valeur = noteDto.Valeur!.Value
        };
        await repositoryFactory.NoteRepository().CreateAsync(nouvelleNote);
    }
}
    public bool IsAuthorized(string role)
    {
        return role == Roles.Scolarite;
    }
}