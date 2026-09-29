using System;
using LittleClass.Core;

internal static class Program
{
    static int Main()
    {
        var problems = Activities.Validate();
        if (problems.Count == 0)
        {
            Console.WriteLine("Little Class content ok: 5 English activities.");
            return 0;
        }
        foreach (var p in problems) Console.WriteLine(p);
        return 1;
    }
}
