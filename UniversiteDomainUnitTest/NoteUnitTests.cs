using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.NoteUseCases;
using UniversiteDomain.UseCases.NoteUseCases.Create;

namespace UniversiteDomainUnitTest;

public class NoteUnitTests
{
    [Test]
    public async Task AddNoteUseCase_Success()
    {
        // Arrange
        long etudiantId = 1;
        long ueId = 2;
        float valeur = 15.5f;
        long parcoursId = 3;
        
        var etudiant = new Etudiant 
        { 
            Id = etudiantId,
            ParcoursSuivi = new Parcours { Id = parcoursId }
        };
        
        var ue = new Ue { Id = ueId };
        
        var parcours = new Parcours
        {
            Id = parcoursId,
            UesEnseignees = new List<Ue> { ue }
        };
        
        var note = new Note
        {
            Id = 1,
            EtudiantId = etudiantId,
            UeId = ueId,
            Valeur = valeur
        };
        
        // Mocks
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        var mockNote = new Mock<INoteRepository>();
        
        mockEtudiant.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Etudiant, bool>>>()))
            .ReturnsAsync(new List<Etudiant> { etudiant });
            
        mockUe.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue> { ue });
            
        mockParcours.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(new List<Parcours> { parcours });
            
        mockNote.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Note, bool>>>()))
            .ReturnsAsync(new List<Note>()); // Pas de note existante
            
        mockNote.Setup(repo => repo.AddNoteAsync(etudiantId, ueId, valeur))
            .ReturnsAsync(note);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(f => f.EtudiantRepository()).Returns(mockEtudiant.Object);
        mockFactory.Setup(f => f.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(f => f.ParcoursRepository()).Returns(mockParcours.Object);
        mockFactory.Setup(f => f.NoteRepository()).Returns(mockNote.Object);
        
        // Act
        var useCase = new AddNoteUseCase(mockFactory.Object);
        var result = await useCase.ExecuteAsync(etudiantId, ueId, valeur);
        
        // Assert
        Assert.That(result.Id, Is.EqualTo(note.Id));
        Assert.That(result.EtudiantId, Is.EqualTo(etudiantId));
        Assert.That(result.UeId, Is.EqualTo(ueId));
        Assert.That(result.Valeur, Is.EqualTo(valeur));
    }
    
    [Test]
    public async Task AddNoteUseCase_InvalidValue_ThrowsException()
    {
        // Arrange
        var mockFactory = new Mock<IRepositoryFactory>();
        var useCase = new AddNoteUseCase(mockFactory.Object);
        
        // Act & Assert
        var exception = Assert.ThrowsAsync<UniversiteDomain.Exceptions.NoteExceptions.InvalidNoteValueException>(
            async () => await useCase.ExecuteAsync(1, 1, 25) // Note > 20
        );
        
        Assert.That(exception.Message, Does.Contain("entre 0 et 20"));
    }
}