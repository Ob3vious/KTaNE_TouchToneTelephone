using System.Collections.Generic;
using System.Linq;

public class KeypadRule
{
    public string Name { get; private set; }
    public ApplyCheck Applies { get; private set; }
    public bool CheckLate { get; private set; }

    public KeypadRule(string name, ApplyCheck applies, bool checkLate = false)
    {
        Name = name;
        Applies = applies;
        CheckLate = checkLate;
    }

    public delegate bool ApplyCheck(List<int> history, int[] availablePresses);
}

public class RulePair
{
    private KeypadRule _rule1;
    private KeypadRule _rule2;

    public RulePair(KeypadRule rule1, KeypadRule rule2)
    {
        _rule1 = rule1;
        _rule2 = rule2;
    }

    public bool Applies(List<int> history, int[] availablePresses)
    {
        //implies is written here as (¬A | B)
        return !_rule1.Applies(history, availablePresses) || _rule2.Applies(history, availablePresses);
    }

    public bool FullApplies(List<int> history, int[] availablePresses)
    {
        return Enumerable.Range(1, history.Count).All(x => Applies(history.Take(x).ToList(), availablePresses));
    }

    public string Name()
    {
        return _rule1.Name + _rule2.Name;
    }

    public bool CheckLate()
    {
        return _rule1.CheckLate || _rule2.CheckLate;
    }
}

public static class KeypadLogic
{
    public static int GetCurrent(this List<int> history)
    {
        if (history.Count < 2)
            return -1;
        return history[history.Count - 2];
    }

    public static int GetNext(this List<int> history)
    {
        if (history.Count < 1)
            return -1;
        return history[history.Count - 1];
    }

    public static int GetX(this int number)
    {
        if (number == 0)
            return 1;
        return (number - 1) % 3;
    }

    public static int GetY(this int number)
    {
        if (number == 0)
            return 3;
        return (number - 1) / 3;
    }
}