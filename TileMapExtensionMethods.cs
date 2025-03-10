using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Assets.Scripts.Unity.Tilemaps {
    public static class TileMapExtensionMethods {

        // Directions for checking neighbour tiles
        public static readonly Vector3Int[] OrthogonalDirections = new Vector3Int[] {
        new Vector3Int(1, 0, 0),   // Right
        new Vector3Int(-1, 0, 0),  // Left
        new Vector3Int(0, 1, 0),   // Up
        new Vector3Int(0, -1, 0)   // Down
        };

        public static readonly Vector3Int[] DiagonalDirections = new Vector3Int[] {
        new Vector3Int(1, 1, 0),   // Top Right
        new Vector3Int(-1, 1, 0),  // Top Left
        new Vector3Int(1, -1, 0),  // Bottom Right
        new Vector3Int(-1, -1, 0)  // Bottom Left
         };

        private static Vector3Int[] GetDirections(bool orthogonal) {
            return orthogonal ? OrthogonalDirections : OrthogonalDirections.Concat(DiagonalDirections).ToArray();
        }

        // Shout out to: https://www.youtube.com/watch?v=oUUfwK_OOjg for making this method!
        // Use .Any to get any tile from the tilemap (stops checking once it detects a tile)
        // Use .ToList() to store as a list

        /// <summary>
        /// An IEnumerable method to get all tiles from a TileMap.
        /// </summary>
        /// <param name="tilemap"></param>
        /// <returns></returns>     
        public static IEnumerable<TileData> GetAllTiles(this Tilemap tilemap) {
            var bounds = tilemap.cellBounds;
            //tilemap.CompressBounds(); for updating the compression of the tilemap's boundaries

            for(int x = bounds.min.x; x < bounds.max.x; x++) {
                for(int y = bounds.min.y; y < bounds.max.y; y++) {
                    var cellPosition = new Vector3Int(x, y, 0);
                    var sprite = tilemap.GetSprite(cellPosition);
                    var tile = tilemap.GetTile(cellPosition);

                    if(tile == null && sprite == null) continue;

                    var tileData = new TileData(x, y, sprite, tile);                       
                    yield return tileData;
                }
            }
        }

        public static TileData GetRandomTile(this Tilemap tilemap) {
            List<TileData> tileData = new List<TileData>();
            tileData = GetAllTiles(tilemap).ToList();

            // Generate a random number and select from the list
            int i = Random.Range(0, tileData.Count);
            return tileData[i];
        }

        public static List<TileData> GetNeighborTiles(this Tilemap tilemap, Vector3Int cellPosition, bool orthogonal = false) {
            List<TileData> neighbors = new List<TileData>();

            Vector3Int[] directions = GetDirections(orthogonal);

            foreach(var dir in directions) {
                Vector3Int neighborPos = cellPosition + dir;
                var tile = tilemap.GetTile(neighborPos);
                var sprite = tilemap.GetSprite(neighborPos);

                if(tile != null || sprite != null) {
                    neighbors.Add(new TileData(neighborPos.x, neighborPos.y, sprite, tile));
                }
            }

            return neighbors;
        }

        // Handy for path finding?
        public static TileData GetRandomNeighborTiles(this Tilemap tilemap, Vector3Int cellPosition, bool orthogonal = false) {
            List<TileData> neighbors = new List<TileData>();

            Vector3Int[] directions = GetDirections(orthogonal);

            foreach(var dir in directions) {
                Vector3Int neighborPos = cellPosition + dir;
                var tile = tilemap.GetTile(neighborPos);
                var sprite = tilemap.GetSprite(neighborPos);

                if(tile != null || sprite != null) {
                    neighbors.Add(new TileData(neighborPos.x, neighborPos.y, sprite, tile));
                }
            }

            int i = Random.Range(0, neighbors.Count);
            return neighbors[i];
        }

        public static bool IsTileOfType<T>(this Tilemap tilemap, Vector3Int position) where T : TileBase {
            TileBase targetTile = tilemap.GetTile(position);

            if(targetTile != null && targetTile is T) {
                return true;
            }

            return false;
        }
        
        public static List<TileData> GetTilesInRange(this Tilemap tilemap, Vector3Int center, int range) {
            List<TileData> tiles = new List<TileData>();

            for(int x = -range; x <= range; x++) {
                for(int y = -range; y <= range; y++) {
                    Vector3Int pos = new Vector3Int(center.x + x, center.y + y, center.z);
                    var tile = tilemap.GetTile(pos);
                    var sprite = tilemap.GetSprite(pos);

                    if(tile != null || sprite != null) {
                        tiles.Add(new TileData(pos.x, pos.y, sprite, tile));
                    }
                }
            }
            return tiles;
        }
       
        public static GameObject GetGameObjectOnTile(this Tilemap tilemap, Vector3Int tilePosition, string objectName) {
            Vector3 worldPosition = tilemap.GetCellCenterWorld(tilePosition);

            // Check for colliders at the tile position
            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, 0.1f);

            foreach(Collider2D collider in colliders) {
                if(collider.gameObject.name == objectName) {
                    return collider.gameObject;
                }
            }

            return null;
        }

        public static GameObject GetGameObjectOnTileByTag(this Tilemap tilemap, Vector3Int tilePosition, string objectTag) {
            Vector3 worldPosition = tilemap.GetCellCenterWorld(tilePosition);

            // Check for colliders at the tile position
            Collider2D[] colliders = Physics2D.OverlapCircleAll(worldPosition, 0.1f);

            foreach(Collider2D collider in colliders) {
                if(collider.gameObject.tag == objectTag) {
                    return collider.gameObject;
                }
            }

            return null;
        }


        public static bool HasTile(this Tilemap tilemap, Vector3 position) {          
            Vector3Int cellPosition = tilemap.WorldToCell(position);
            return tilemap.GetTile(cellPosition) != null;
        }

        public static bool HasTile(this Tilemap tilemap, Vector3Int position) {
            return tilemap.GetTile(position) != null;
        }
    }
}
