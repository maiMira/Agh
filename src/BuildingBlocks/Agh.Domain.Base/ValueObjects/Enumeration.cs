using System.Reflection;

namespace Agh.Domain.Base.ValueObjects;


public abstract class Enumeration : IComparable
{
    public string Name { get; private set; }
    public int Id { get; private set; }

    protected Enumeration(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public override string ToString() => Name;

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (obj is not Enumeration otherValue) return false;

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static IEnumerable<T> GetAll<T>() where T : Enumeration
    {
        return typeof(T).GetFields(BindingFlags.Public |
                                 BindingFlags.Static |
                                 BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();
    }

    public int CompareTo(object other)
    {
        if (other is null) return 1;

        var otherValue = other as Enumeration;
        if (otherValue == null)
            throw new ArgumentException("Object is not an Enumeration");

        return Id.CompareTo(otherValue.Id);
    }

    public static bool operator ==(Enumeration left, Enumeration right)
    {
        if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
            return true;

        if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
            return false;

        return left.Equals(right);
    }

    public static bool operator !=(Enumeration left, Enumeration right)
    {
        return !(left == right);
    }

    public static bool operator <(Enumeration left, Enumeration right)
    {
        return ReferenceEquals(left, null) ? !ReferenceEquals(right, null) : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Enumeration left, Enumeration right)
    {
        return ReferenceEquals(left, null) || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Enumeration left, Enumeration right)
    {
        return !ReferenceEquals(left, null) && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Enumeration left, Enumeration right)
    {
        return ReferenceEquals(left, null) ? ReferenceEquals(right, null) : left.CompareTo(right) >= 0;
    }

    // Helper methods for creating from value
    public static T FromValue<T>(int value) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Id == value);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{value}' is not a valid value in {typeof(T)}");

        return matchingItem;
    }

    public static T FromName<T>(string name) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (matchingItem == null)
            throw new InvalidOperationException($"'{name}' is not a valid name in {typeof(T)}");

        return matchingItem;
    }
}