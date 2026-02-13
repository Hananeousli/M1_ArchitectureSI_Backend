using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteEFDataProvider.Data;
using Microsoft.EntityFrameworkCore;

namespace UniversiteEFDataProvider.Repositories;

public class NoteRepository(UniversiteDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<Note> AddNoteAsync(long etudiantId, long ueId, float valeur)
    {
        // Vérifier si la note existe déjà
        var noteExistante = await context.Notes!
            .FirstOrDefaultAsync(n => n.EtudiantId == etudiantId && n.UeId == ueId);

        if (noteExistante != null)
        {
            // Mettre à jour la note existante
            noteExistante.Valeur = valeur;
            context.Notes!.Update(noteExistante);
        }
        else
        {
            // Créer une nouvelle note
            noteExistante = new Note
            {
                EtudiantId = etudiantId,
                UeId = ueId,
                Valeur = valeur
            };
            await context.Notes!.AddAsync(noteExistante);
        }

        await context.SaveChangesAsync();
        return noteExistante;
    }
}