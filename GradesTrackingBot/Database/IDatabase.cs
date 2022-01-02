using GradesTrackingBot.Model;

namespace GradesTrackingBot.Database;

public interface IDatabase
{
    public Task AddDiscipline(Discipline discipline);

    public Task<Discipline?> GetDiscipline(Guid id);

    public Task<List<Discipline>> GetDisciplines(long userId);
    
    public Task AddMarkElement(MarkElement discipline);
    
    public Task<MarkElement?> GetMarkElement(Guid id);

    public Task<List<MarkElement>> GetMarkElements(Discipline discipline);
}