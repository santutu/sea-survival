using Santutu.Core.DynamicArray.Runtime;
using UnityEngine;

namespace Santutu.Core.Extensions.Runtime.UnityExtensions
{
    public static class ExtendBoundsInt
    {
        public static void SetCenter(ref this BoundsInt boundsInt, Vector3Int center)
        {
            boundsInt.position = center - boundsInt.SizeHalf();
        }

        public static Vector3 GetWorldCenter(this BoundsInt boundsInt,Grid grid)
        {
            var min =grid.CellToWorld(boundsInt.min);
            var max = grid.CellToWorld(boundsInt.max);

            return min.GetCenter(max);

        }

        public static Vector3Int CenterInt(this BoundsInt boundsInt)
        {
            return boundsInt.center.Floor();
        }

        public static Vector3Int SizeHalf(this BoundsInt boundsInt)
        {
            return boundsInt.size / 2;
        }


        public static Vector3 GetMinPoint(this BoundsInt boundsInt)
        {
            return new Vector3(boundsInt.xMin, boundsInt.yMin, 0);
        }

        public static Vector3 GetMaxPoint(this BoundsInt boundsInt)
        {
            return new Vector3(boundsInt.xMax, boundsInt.yMax, 0);
        }

        public static IReadonlyDynamicArray<Vector3Int> GetAllPositionsWithinNonAlloc(
            this BoundsInt bounds, DynamicArray<Vector3Int> results
        )
        {
            int index = 0;

            int totalSize = bounds.size.x * bounds.size.y * bounds.size.z;

            results.ResizeNewIfLessThan(totalSize);

            results.Length = totalSize;

            for (int z = 0; z < bounds.size.z; z++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    for (int x = 0; x < bounds.size.x; x++)
                    {
                        results[index++] = new Vector3Int(
                            bounds.min.x + x,
                            bounds.min.y + y,
                            bounds.min.z + z
                        );
                    }
                }
            }

            return results;
        }


        public static Vector3Int[] GetAllPositionsWithin(this BoundsInt bounds)
        {
            int totalSize = bounds.size.x * bounds.size.y * bounds.size.z;
            var vector3Ints = new Vector3Int[totalSize];
            int index = 0;

            for (int z = 0; z < bounds.size.z; z++)
            {
                for (int y = 0; y < bounds.size.y; y++)
                {
                    for (int x = 0; x < bounds.size.x; x++)
                    {
                        vector3Ints[index++] = new Vector3Int(
                            bounds.min.x + x,
                            bounds.min.y + y,
                            bounds.min.z + z
                        );
                    }
                }
            }

            return vector3Ints;
        }
    }
}