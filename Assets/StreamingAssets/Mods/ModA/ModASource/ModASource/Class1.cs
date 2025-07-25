using ModArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModASource
{
    public class ModA : IModInstance
    {
        public void Initialize()
        {
            Debug.Log("ModA initialized");
        }

        public void OnGameEnd()
        {
            Debug.Log("ModA game ended");
        }

        public void OnGameStart()
        {
            Debug.Log("ModA game started");
        }
    }
}
