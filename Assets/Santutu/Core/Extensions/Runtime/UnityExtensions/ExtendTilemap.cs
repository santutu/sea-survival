using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendTilemap
    {
        public static TileBase[] GetTilesInArea(this Tilemap tilemap, Vector3Int start, Vector3Int end)
        {
            Vector3Int min = Vector3Int.Min(start, end);
            Vector3Int max = Vector3Int.Max(start, end);

            var area = new BoundsInt(min, max - min + Vector3Int.one);

            return tilemap.GetTilesInArea(area);
        }

        public static TileBase[] GetTilesInArea(this Tilemap tilemap, BoundsInt area)
        {
            TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
            int counter = 0;

            foreach (var v in area.allPositionsWithin)
            {
                Vector3Int pos = new Vector3Int(v.x, v.y, 0);
                array[counter] = tilemap.GetTile(pos);
                counter++;
            }

            return array;
        }


        public static void SetTilesBlock(this Tilemap tilemap, BoundsInt area, TileBase tile)
        {
            TileBase[] tiles = tilemap.GetTilesInArea(area);
            Array.Fill(tiles,tile);

            tilemap.SetTilesBlock(area, tiles);
        }

        public static void SetTilesBlock(this Tilemap tilemap, Vector3Int startCell, Vector3Int endCell, TileBase tile)
        {
            Vector3Int minCell = Vector3Int.Min(startCell, endCell);
            Vector3Int maxCell = Vector3Int.Max(startCell, endCell);

            for (int x = minCell.x; x <= maxCell.x; x++)
            {
                for (int y = minCell.y; y <= maxCell.y; y++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, 0);
                    tilemap.SetTile(cellPos, tile);
                }
            }
        }

        public static void SetTilesBlockToNull(this Tilemap tilemap, BoundsInt area)
        {
            var empty = System.Buffers.ArrayPool<TileBase>.Shared.Rent(area.size.x * area.size.y * area.size.z);

            Array.Fill(empty,null);
            tilemap.SetTilesBlock(area, empty);
            
            System.Buffers.ArrayPool<TileBase>.Shared.Return(empty);
            
        }
    }
}