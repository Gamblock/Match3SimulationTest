using System;
using Features.Data;
using UnityEngine;

namespace Features.Config
{
    [Serializable]
    public struct TileSpriteConfig
    {
        public TileColor Color;
        public Sprite Sprite;
    }
}