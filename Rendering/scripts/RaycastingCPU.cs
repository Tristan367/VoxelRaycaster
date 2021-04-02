using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingCPU : MonoBehaviour
{
    public RenderTexture bufferRenderTexture;
    public ComputeShader computeShader;

    public RenderTexture destinationRenderTexture;

    //float aspectRatio = 2;
    int height;
    int width;
    int totalPixels;

    Vector3[] objectiveRayDirections; // the positions of the pixels as if the player is standing on 0,0,0 with 0 rotation.
    Vector3[] rayDirections;
    public Transform playerVirtualCameraTransform; // using an empty transform for convenience
    float fieldOfView = .01f;

    int[] testVoxels;

    const int GPUbufferVoxelBufferRowSize = 128; // basically a finite world of voxels for the GPU, will chunkify later
    ComputeBuffer rayDirectionsBuffer;
    ComputeBuffer voxelBuffer;

    int gpuThreadGroupsX;
    int gpuThreadGroupsY;

    void Start()
    {
        width = destinationRenderTexture.width;
        height = destinationRenderTexture.height;
        totalPixels = width * height;
        rayDirections = new Vector3[totalPixels];
        objectiveRayDirections = new Vector3[totalPixels];
        float coordOffsetX = (float)width / 2;
        float coordOffsetY = (float)height / 2;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                objectiveRayDirections[x + (y * width)] = new Vector3((x - coordOffsetX) * fieldOfView, (y - coordOffsetY) * fieldOfView, 5).normalized; // which is also the positions considering the offset is 0,0,0
            }
        }

        gpuThreadGroupsX = width / 8;
        gpuThreadGroupsY = height / 8;

        bufferRenderTexture = new RenderTexture(width, height, 0); // must be initiated in start for some reason
        bufferRenderTexture.enableRandomWrite = true;
        bufferRenderTexture.Create();
        bufferRenderTexture.filterMode = FilterMode.Point;

        computeShader.SetFloat("width", bufferRenderTexture.width);
        computeShader.SetFloat("height", bufferRenderTexture.height);
        computeShader.SetTexture(0, "Result", bufferRenderTexture);

        rayDirectionsBuffer = new ComputeBuffer(totalPixels, sizeof(float) * 3);
        rayDirectionsBuffer.SetData(objectiveRayDirections);
        computeShader.SetBuffer(0, "rayDirections", rayDirectionsBuffer);
        computeShader.SetFloat("maxRayDistance", 256);

        makeTestShape();
        voxelBuffer = new ComputeBuffer(GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize, sizeof(int));
        voxelBuffer.SetData(testVoxels);
        computeShader.SetBuffer(0, "voxelMaterials", voxelBuffer);
        computeShader.SetInt("voxelBufferRowSize", GPUbufferVoxelBufferRowSize);
        computeShader.SetInt("voxelBufferPlaneSize", GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize);
        computeShader.SetInt("voxelBufferSize", GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize);


    }

    private void Update()
    {
        computeShader.SetVector("playerCameraPosition", playerVirtualCameraTransform.position);
        computeShader.SetVector("playerWorldForward", playerVirtualCameraTransform.forward);
        computeShader.SetVector("playerWorldRight", playerVirtualCameraTransform.right);
        computeShader.SetVector("playerWorldUp", playerVirtualCameraTransform.up);

        computeShader.Dispatch(0, gpuThreadGroupsX, gpuThreadGroupsY, 1);

        Graphics.Blit(bufferRenderTexture, destinationRenderTexture); // apparently we can't just use renderTexture
    }

    void makeTestShape() // a double layer platform with vertical poles on the corners and in the middle
    {
        testVoxels = new int[GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize];

        for (int i = 0; i < testVoxels.Length; i++)
        {
            testVoxels[i] = 0; // "0" for empty
            if (i < (GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize * 2)) // bottom
            {
                testVoxels[i] = Random.Range(1, 4);
            }
            else
            {
                int iCornerCheck = i % (GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize);
                if (iCornerCheck == 0 || iCornerCheck == GPUbufferVoxelBufferRowSize - 1 || 
                    iCornerCheck == (GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize) - GPUbufferVoxelBufferRowSize || iCornerCheck == (GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize) - 1)
                {
                    testVoxels[i] = Random.Range(1, 4);
                }
                if (iCornerCheck == (GPUbufferVoxelBufferRowSize * (GPUbufferVoxelBufferRowSize / 2)) - (GPUbufferVoxelBufferRowSize / 2))
                {
                    testVoxels[i] = Random.Range(1, 4);
                }

            }
        }
    }

    private void OnApplicationQuit()
    {
        rayDirectionsBuffer.Dispose();
        voxelBuffer.Dispose();
    }

}
