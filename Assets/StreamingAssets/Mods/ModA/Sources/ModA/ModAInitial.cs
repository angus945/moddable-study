using UnityEngine;
using ModArchitecture;

namespace ModA;

public class ModAInitial : IModInitializer
{
    public void Initialize()
    {
        Debug.Log("ModA initialized successfully!");
    }
}

