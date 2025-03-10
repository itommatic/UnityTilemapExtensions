using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Unity.Tilemaps {
    public sealed class TileData {
        public TileData(int x, int y, Sprite sprite, TileBase tile) {
            X = x;
            Y = y;
            Sprite = sprite;
            Tile = tile;
        }

        public int X { get; }
        public int Y { get; }
        public Sprite Sprite { get; }
        public TileBase Tile { get; }
    }
}
