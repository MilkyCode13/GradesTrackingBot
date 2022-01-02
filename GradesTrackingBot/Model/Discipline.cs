namespace GradesTrackingBot.Model;

public class Discipline
{
    private List<MarkElement> elements = new();

    public Discipline(string name, long userId) : this(Guid.NewGuid(), name, userId) {}

    public Discipline(string name, long userId, int? target) : this(Guid.NewGuid(), name, userId, target) {}

    public Discipline(Guid id, string name, long userId, int? target = null)
    {
        Id = id;
        Name = name;
        UserId = userId;
        Target = target;
    }

    public Guid Id { get; }

    public string Name { get; }

    public long UserId { get; }

    public int? Target { get; }

    public IReadOnlyList<MarkElement> Elements => elements;
}