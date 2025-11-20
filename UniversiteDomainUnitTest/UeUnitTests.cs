using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteDomain.UseCases.ParcoursUseCases.UeDansParcours;

namespace UniversiteDomainUnitTest;

public class UeUnitTests
{
    [Test]
    public async Task CreateUeUseCase()
    {
        // Arrange
        long id = 1;
        string numeroUe = "UE101";
        string intitule = "Programmation Avancée";
        
        Ue ueSansId = new Ue
        {
            NumeroUe = numeroUe,
            Intitule = intitule
        };
        
        var mockUe = new Mock<IUeRepository>();
        
        mockUe.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(new List<Ue>());
            
        Ue ueCree = new Ue
        {
            Id = id,
            NumeroUe = numeroUe,
            Intitule = intitule
        };
        
        mockUe.Setup(repo => repo.CreateAsync(It.IsAny<Ue>()))
            .ReturnsAsync(ueCree);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(factory => factory.UeRepository()).Returns(mockUe.Object);
        
        // Act
        CreateUeUseCase useCase = new CreateUeUseCase(mockFactory.Object);
        var ueTeste = await useCase.ExecuteAsync(ueSansId);
        
        // Assert
        Assert.That(ueTeste.Id, Is.EqualTo(ueCree.Id));
        Assert.That(ueTeste.NumeroUe, Is.EqualTo(ueCree.NumeroUe));
        Assert.That(ueTeste.Intitule, Is.EqualTo(ueCree.Intitule));
    }
    
    [Test]
    public async Task AddUeDansParcoursUseCase()
    {
        // Arrange
        long idUe = 1;
        long idParcours = 2;
        
        Ue ue = new Ue { Id = 1, NumeroUe = "UE101", Intitule = "Programmation Avancée" };
        Parcours parcours = new Parcours { Id = 2, NomParcours = "MIAGE", AnneeFormation = 1 };
        
        var mockUe = new Mock<IUeRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        
        List<Ue> ues = new List<Ue> { ue };
        mockUe.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Ue, bool>>>()))
            .ReturnsAsync(ues);
        
        List<Parcours> parcourses = new List<Parcours> { parcours };
        mockParcours.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>()))
            .ReturnsAsync(parcourses);
        
        Parcours parcoursFinal = new Parcours 
        { 
            Id = 2, 
            NomParcours = "MIAGE", 
            AnneeFormation = 1 
        };
        parcoursFinal.UesEnseignees.Add(ue);
        
        mockParcours.Setup(repo => repo.AddUeAsync(idParcours, idUe))
            .ReturnsAsync(parcoursFinal);
        
        var mockFactory = new Mock<IRepositoryFactory>();
        mockFactory.Setup(factory => factory.UeRepository()).Returns(mockUe.Object);
        mockFactory.Setup(factory => factory.ParcoursRepository()).Returns(mockParcours.Object);
        
        // Act
        AddUeDansParcoursUseCase useCase = new AddUeDansParcoursUseCase(mockFactory.Object);
        var parcoursTest = await useCase.ExecuteAsync(idParcours, idUe);
        
        // Assert
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.UesEnseignees, Is.Not.Null);
        Assert.That(parcoursTest.UesEnseignees.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.UesEnseignees[0].Id, Is.EqualTo(idUe));
    }
}