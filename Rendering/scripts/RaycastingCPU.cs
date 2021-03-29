using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastingCPU : MonoBehaviour
{
    public RenderTexture bufferRenderTexture;
    public ComputeShader computeShader;

    public RenderTexture destinationRenderTexture;

    float aspectRatio = 2;
    int height;
    int width;
    int totalPixels;

    Vector3[] objectiveRayDirections; // the positions of the pixels as if the player is standing on 0,0,0 with 0 rotation.
    Vector3[] rayDirections;
    public Transform playerVirtualCameraTransform; // using an empty transform for convenience
    float fieldOfView = .05f;

    int[] testVoxels;

    const int GPUbufferVoxelBufferRowSize = 32; // basically a finite world of voxels for the GPU, will chunkify later
    ComputeBuffer rayDirectionsBuffer;
    ComputeBuffer voxelBuffer;

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
                objectiveRayDirections[x + (y * width)] = new Vector3(x - coordOffsetX, y - coordOffsetY, 1);
            }
        }

        bufferRenderTexture = new RenderTexture(width, height, 24); // must be initiated in start for some reason
        bufferRenderTexture.enableRandomWrite = true;
        bufferRenderTexture.Create();
        bufferRenderTexture.filterMode = FilterMode.Point;

        computeShader.SetFloat("width", bufferRenderTexture.width);
        computeShader.SetFloat("height", bufferRenderTexture.height);
        computeShader.SetTexture(0, "Result", bufferRenderTexture);

        computeShader.SetVector("playerCameraPosition", playerVirtualCameraTransform.position);

        CalculateNewRaysDirections();
        rayDirectionsBuffer = new ComputeBuffer(totalPixels, sizeof(float) * 3);
        rayDirectionsBuffer.SetData(rayDirections);
        computeShader.SetBuffer(0, "rayDirections", rayDirectionsBuffer);
        computeShader.SetFloat("maxRayDistance", 100);

        makeTestShape();
        voxelBuffer = new ComputeBuffer(GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize, sizeof(int));
        voxelBuffer.SetData(testVoxels);
        computeShader.SetBuffer(0, "voxelMaterials", voxelBuffer);
        computeShader.SetInt("voxelBufferRowSize", GPUbufferVoxelBufferRowSize);

        /*
        int[] testArr = new int[totalPixels];
        for (int i = 0; i < totalPixels; i++)
        {
            testArr[i] = Random.Range(0, 2);
        }
        ComputeBuffer testBuffer = new ComputeBuffer(width * height, sizeof(int));
        testBuffer.SetData(testArr);
        computeShader.SetBuffer(0, "voxelMaterials", testBuffer);
        */


    }

    int i = 0;
    bool increase = true;
    private void Update()
    {
        computeShader.SetVector("playerCameraPosition", playerVirtualCameraTransform.position);

        CalculateNewRaysDirections();
        rayDirectionsBuffer = new ComputeBuffer(totalPixels, sizeof(float) * 3);
        rayDirectionsBuffer.SetData(rayDirections);
        computeShader.SetBuffer(0, "rayDirections", rayDirectionsBuffer);

        //computeShader.SetFloat("c", (float)i / 255); // for testing
        computeShader.Dispatch(0, bufferRenderTexture.width / 32, bufferRenderTexture.height / 32, 1);

        Graphics.Blit(bufferRenderTexture, destinationRenderTexture); // apparently we can't just use renderTexture



        /*
        //TESTING STUFF
        if (increase)
        {
            i++;
        }
        else
        {
            i--;
        }
        if (i > 255)
        {
            i = 255;
            increase = false;
        }
        if (i < 0)
        {
            increase = true;
            i = 0;
        }
        */
    }

    void CalculateNewRaysDirections()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int index = x + (y * width);
                rayDirections[index] = playerVirtualCameraTransform.TransformDirection(new Vector3(objectiveRayDirections[index].x * fieldOfView, objectiveRayDirections[index].y * fieldOfView, 5)).normalized;
            }
        }
    }

    void makeTestShape() // a single layer platform with vertical poles on the corners
    {
        testVoxels = new int[GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize];

        for (int i = 0; i < testVoxels.Length; i++)
        {
            testVoxels[i] = 0; // "0" for empty
            if (i < GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize) // bottom
            {
                testVoxels[i] = 1;
            }
            else
            {
                int iCornerCheck = i % (GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize);
                if (iCornerCheck == 0 || iCornerCheck == GPUbufferVoxelBufferRowSize - 1 || 
                    iCornerCheck == (GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize) - GPUbufferVoxelBufferRowSize || iCornerCheck == (GPUbufferVoxelBufferRowSize * GPUbufferVoxelBufferRowSize) - 1)
                {
                    testVoxels[i] = 1; // "1" for dirt
                }
               
            }
        }
    }

    private void OnDrawGizmos()
    {
        CalculateNewRaysDirections();
        Gizmos.color = Color.red;
        for (int i = 0; i < totalPixels; i++)
        {
            Gizmos.DrawRay(playerVirtualCameraTransform.position, rayDirections[i] * .5f);
        }
    }

}
