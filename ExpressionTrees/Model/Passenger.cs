namespace ExpressionTrees.Model;

public record Passenger(
        bool Survived,
        int PClass,
        string Name,
        Gender Gender,
        decimal Age,
        int SiblingsOrSpouse,
        int ParentOrChildren,
        decimal Fare
    );

public enum Gender
{
    Male = 0,
    Female = 1
}