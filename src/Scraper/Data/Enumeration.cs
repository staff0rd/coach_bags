using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace coach_bags_selenium.Data
{
public abstract class Enumeration : IComparable
{
    private readonly int _id;
    private readonly string _displayName;

    protected Enumeration()
    {
    }

    protected Enumeration(int id, string displayName)
    {
        _id = id;
        _displayName = displayName;
    }

    public int Id
    {
        get { return _id; }
    }

    public string DisplayName
    {
        get { return _displayName; }
    }

    public override string ToString()
    {
        return DisplayName;
    }

    public static IEnumerable<T> GetAll<T>() where T : Enumeration, new()
    {
        var type = typeof(T);
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);

        foreach (var info in fields)
        {
            var instance = new T();
            var locatedValue = info.GetValue(instance) as T;

            if (locatedValue != null)
            {
                yield return locatedValue;
            }
        }
    }

    public override bool Equals(object obj)
    {
        var otherValue = obj as Enumeration;

        if (otherValue == null)
        {
            return false;
        }

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = _id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode()
    {
        return _id.GetHashCode();
    }

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    public static T FromId<T>(int value) where T : Enumeration, new()
    {
        var matchingItem = parse<T, int>(value, "value", item => item.Id == value);
        return matchingItem;
    }

    public static T FromDisplayName<T>(string displayName) where T : Enumeration, new()
    {
        var matchingItem = parse<T, string>(displayName, "display name", item => item.DisplayName == displayName);
        return matchingItem;
    }

    private static T parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration, new()
    {
        var matchingItem = GetAll<T>().FirstOrDefault(predicate);

        if (matchingItem == null)
        {
            var message = string.Format("'{0}' is not a valid {1} in {2}", value, description, typeof(T));
            throw new ApplicationException(message);
        }

        return matchingItem;
    }

    public int CompareTo(object other)
    {
        return Id.CompareTo(((Enumeration)other).Id);
    }
}
}