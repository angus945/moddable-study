using ModArchitecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AuthorA
{
    public class ModAEntry : IModEntry
    {
        public void Initialize()
        {
            Debug.Log("ModA initialized");
        }
        public void OnGameStart()
        {
            Debug.Log("ModA game started");

            var texture = ModAssetsDatabase.GetAsset<Texture2D>("ModA/Avator.png");
            if (texture != null)
            {
                Debug.Log("Texture loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to load texture");
            }

            var sound = ModAssetsDatabase.GetAsset<AudioClip>("TestSound.mp3");
            if (sound != null)
            {
                Debug.Log("Sound loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to load sound");
            }
        }
        public void OnGameEnd()
        {
            Debug.Log("ModA game ended");
        }
    }

}
