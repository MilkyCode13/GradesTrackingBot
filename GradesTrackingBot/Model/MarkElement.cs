namespace GradesTrackingBot.Model;

public class MarkElement
{
    private Guid id;

    private string name;

    private Discipline discipline;

    private int weight;

    private double? grade;

    public MarkElement(string name, Discipline discipline, int weight, double? grade = null)
    {
        this.name = name;
        this.discipline = discipline;
        this.weight = weight;
        this.grade = grade;
    }

    public Guid Id => id;

    public string Name => name;

    public Discipline Discipline => discipline;

    public int Weight => weight;

    public double? Grade => grade;
}