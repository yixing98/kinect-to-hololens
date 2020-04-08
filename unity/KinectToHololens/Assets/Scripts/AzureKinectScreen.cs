﻿using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AzureKinectScreen : MonoBehaviour
{
    public MeshFilter meshFilter;

    public void Setup(InitSenderPacketData initSenderPacketData)
    {
        meshFilter.mesh = CreateMesh(initSenderPacketData);
    }

    //private static Mesh CreateMesh(AzureKinectCalibration calibration)
    private static Mesh CreateMesh(InitSenderPacketData initSenderPacketData)
    {
        int width = initSenderPacketData.depthWidth;
        int height = initSenderPacketData.depthHeight;

        //var depthCamera = calibration.DepthCamera;

        var vertices = new Vector3[width * height];
        var uv = new Vector2[width * height];

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                float[] xy = new float[2];
                int valid = 0;
                if (AzureKinectIntrinsicTransformation.Unproject(initSenderPacketData.depthIntrinsics,
                                                                 initSenderPacketData.depthMetricRadius,
                                                                 new float[2] { i, j }, ref xy, ref valid))
                {
                    vertices[i + j * width] = new Vector3(xy[0], xy[1], 1.0f);
                }
                else
                {
                    vertices[i + j * width] = new Vector3(0.0f, 0.0f, 0.0f);
                }
                uv[i + j * width] = new Vector2(i / (float)(width - 1), j / (float)(height - 1));
            }
        }

        int quadWidth = width - 2;
        int quadHeight = height - 2;
        var quadVertices = new Vector3[quadWidth * quadHeight * 4];
        var quadUv = new Vector2[quadWidth * quadHeight * 4];

        for (int ii = 0; ii < quadWidth; ++ii)
        {
            for (int jj = 0; jj < quadHeight; ++jj)
            {
                int quadIndex = ii + jj * quadWidth;
                int i = ii + 1;
                int j = jj + 1;
                quadVertices[quadIndex * 4 + 0] = (vertices[i + j * width] + vertices[(i - 1) + (j - 1) * width]) * 0.5f;
                quadVertices[quadIndex * 4 + 1] = (vertices[i + j * width] + vertices[(i + 1) + (j - 1) * width]) * 0.5f;
                quadVertices[quadIndex * 4 + 2] = (vertices[i + j * width] + vertices[(i - 1) + (j + 1) * width]) * 0.5f;
                quadVertices[quadIndex * 4 + 3] = (vertices[i + j * width] + vertices[(i + 1) + (j + 1) * width]) * 0.5f;

                quadUv[quadIndex * 4 + 0] = uv[i + j * width];
                quadUv[quadIndex * 4 + 1] = uv[i + j * width];
                quadUv[quadIndex * 4 + 2] = uv[i + j * width];
                quadUv[quadIndex * 4 + 3] = uv[i + j * width];
            }
        }

        var triangles = new int[quadWidth * quadHeight * 6];
        for (int i = 0; i < quadWidth * quadHeight; ++i)
        {
            triangles[i * 6 + 0] = i * 4 + 0;
            triangles[i * 6 + 1] = i * 4 + 1;
            triangles[i * 6 + 2] = i * 4 + 2;
            triangles[i * 6 + 3] = i * 4 + 1;
            triangles[i * 6 + 4] = i * 4 + 3;
            triangles[i * 6 + 5] = i * 4 + 2;
        }

        // Without the bounds, Unity decides whether to render this mesh or not based on the vertices calculated here.
        // This causes Unity not rendering the mesh transformed by the depth texture even when the transformed one
        // belongs to the viewport of the camera.
        var bounds = new Bounds(Vector3.zero, Vector3.one * 1000.0f);

        var mesh = new Mesh()
        {
            indexFormat = IndexFormat.UInt32,
            vertices = quadVertices,
            uv = quadUv,
            bounds = bounds,
        };
        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);

        return mesh;
    }
}