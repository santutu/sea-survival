using System;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendGrid
    {
        public static Vector3 GetAbsCellSize(this Grid grid)
        {
            return grid.GetAbsoluteCellSize();
        }

        public static Vector3 GetWorldAbsoluteCellSize(this Grid grid)
        {
            return grid.GetAbsoluteCellSize().ChangeWorldCoordinate(grid);
        }

        public static Vector3 GetAbsoluteCellSize(this Grid grid)
        {
            return grid.cellSize.Multiply(grid.transform.lossyScale);
        }

        public static Vector3 Snap(this Grid grid, Vector3 worldPos)
        {
            return grid.CellToWorld(grid.WorldToCell(worldPos));
        }

        public static bool GetCellOnMousePosition(
            this Grid grid,
            out Vector3Int cell,
            Camera camera,
            float raycastDistance,
            int layerMask = -1
        )
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, raycastDistance, layerMask))
            {
                cell = default;
                return false;
            }

            cell = grid.WorldToCell(hit.point);
            return true;
        }

        public static Vector3 CellToMaxWorld(this Grid grid, Vector3Int cell)
        {
            var pos = grid.CellToWorld(cell);
            return pos + grid.GetAbsoluteCellSize();
        }

        public static Vector3 ChangeWorldCoordinate(this Vector3 v3, Grid grid)
        {
            if (grid.cellSwizzle == GridLayout.CellSwizzle.XZY)
            {
                return v3.ChangeYZ();
            }

            throw new NotImplementedException();
        }

        public static Vector3 CellToWorldCenter(this Grid grid, Vector3Int cell)
        {
            var pos = grid.CellToWorld(cell);
            return pos + (grid.GetWorldAbsoluteCellSize() / 2);
        }

        public static Vector3 CellToWorld(this Grid grid, Vector3 cell)
        {
            Vector3Int minCellInt = cell.Floor();
            Vector3Int maxCellInt = cell.Ceil();

            if (minCellInt == maxCellInt)
            {
                maxCellInt += Vector3Int.one;
            }

            var minWorldPos = grid.CellToWorld(minCellInt);
            var maxWorldPos = grid.CellToWorld(maxCellInt);

            var direction = (maxWorldPos - minWorldPos).normalized;

            var rate = (cell - minCellInt).magnitude;
            var worldPos = minWorldPos + (direction * rate);

            return worldPos;
        }

        public static Bounds BoundsIntToBox(this Grid grid, BoundsInt gridBoundsInt)
        {
            Vector3 minWorld = grid.CellToWorld(gridBoundsInt.min);
            Vector3 maxWorld = grid.CellToWorld(gridBoundsInt.max);


            Vector3 size = maxWorld - minWorld;
            Vector3 center = minWorld + (size / 2);

            Bounds bounds = new Bounds(center, size);

            return bounds;
        }
    }
}