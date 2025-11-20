namespace UniversiteDomain.Entities;

public class Note
{
    public long Id { get; set; }
    public float Valeur { get; set; }
    
    // Relations
    public long EtudiantId { get; set; }
    public Etudiant? Etudiant { get; set; }
    
    public long UeId { get; set; }
    public Ue? Ue { get; set; }
    
    public override string ToString()
    {
        return $"Note: {Valeur}/20 - Etudiant {EtudiantId} - UE {UeId}";
    }
}