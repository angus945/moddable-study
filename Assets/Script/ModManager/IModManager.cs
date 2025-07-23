

using System.Collections.Generic;

public interface IModManager
{
    List<string> modIDs { get; }

    string GetModDirectory(string modId);
}
