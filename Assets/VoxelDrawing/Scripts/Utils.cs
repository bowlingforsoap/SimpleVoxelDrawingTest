using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelDrawing
{

    public class Utils
    {
        /// <summary>Corner points + center.</summary>
        public static Vector3[] GetBoundingBoxPoints(Vector3 center, float size)
        {
            Vector3[] points = new Vector3[9];
            Vector3 cornerPoint;
            int count = 0;
            points[count++] = center;
            float halfSize = size / 2f;

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    float iIndexMagic = -1 * i * 2 + 1;
                    float jIndexMagic = -1 * j * 2 + 1;
                    cornerPoint = new Vector3(center.x + halfSize * iIndexMagic, center.y + halfSize, center.z + halfSize * jIndexMagic);

                    points[count++] = cornerPoint; // Upper corner point
                    points[count++] = new Vector3(cornerPoint.x, cornerPoint.y - size, cornerPoint.z); // Lower corner point
                }
            }

            return points;
        }

        public static Vector3 PositionToVoxelIndex(Vector3 position, float voxelSize)
        {
            Vector3 voxelIndex;

            voxelIndex = position / voxelSize;
            voxelIndex.x = Mathf.Floor(voxelIndex.x);
            voxelIndex.y = Mathf.Floor(voxelIndex.y);
            voxelIndex.z = Mathf.Floor(voxelIndex.z);
            voxelIndex *= voxelSize;

            voxelIndex += new Vector3(voxelSize / 2f, voxelSize / 2f, voxelSize / 2f); // Move half a voxel left

            return voxelIndex;
        }

    }
}