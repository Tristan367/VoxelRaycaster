using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctreeRaycastingCPU : MonoBehaviour
{
    struct OctantParent
    {
        public bool empty;
        public OctantParent[] children;
        public int material; // only used at the voxel level
        public Color lodColor; // only used at the OctantParent level, an average of the colors of its children.

        public void initParent()
        {
            children = new OctantParent[8];
        }
    };

    // convert position to integer
    // look at the first OctantParent and ask if it is empty (if the entire world is empty)
    // if not empty, look at the children of that OctantParent and determine which one the point is inside of
    // if it is empty, calculate the distance in the ray's direction to the end of the Octant, check if we are exiting just a child or exiting a parent

    // an empty OctantParent will have no children
    // a non-empty OctantParent will go down to a single voxel, of which those OctantParents will have no children.

    // convert voxel data into Octtree:
    // start with an initial OctantParent, check if it is empty
    // if not, break down it's octants but using X,Y,Z and converting them into flat coords and check if those Octants are empty

    // recursively check emptiness of octants until one is either empty or goes down to the voxel level

    // voxel data rowSizes must always be factor of 2. So 2,4,6,8,16,32,64,128,256 et cetera

    void convertVoxelArrayToOctreeRecursively(ref OctantParent octPar, int[] voxels, int rowSizeInVoxels)
    {
        octPar.empty = true;
        int planeSizeInVoxels = rowSizeInVoxels * rowSizeInVoxels;
        int sizeInVoxels = planeSizeInVoxels * rowSizeInVoxels;
        for (int i = 0; i < sizeInVoxels; i++)
        {
            if (voxels[i] != 0) // if it is not an empty voxel
            {
                octPar.empty = false;

                if (sizeInVoxels == 1) // meaning we went down to the voxel level and cannot recurse any further
                {
                    // assign the material and lodColor for parents to average
                    octPar.material = voxels[0];
                }
                else
                {
                    octPar.initParent();

                    int octantChildrenSize = sizeInVoxels / 8;
                    int octantRowSize = rowSizeInVoxels / 2;

                    int[] octVoxelMaterials = new int[octantChildrenSize];
                    int octVoxelCount = 0;

                    // just keeping it simple and making 8 loops

                    for (int y = 0; y < octantRowSize; y++)
                    {
                        for (int z = 0; z < octantRowSize; z++)
                        {
                            for (int x = 0; x < octantRowSize; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[0], octVoxelMaterials, octantRowSize); // recursion

                    octVoxelCount = 0;
                    for (int y = 0; y < octantRowSize; y++)
                    {
                        for (int z = 0; z < octantRowSize; z++)
                        {
                            for (int x = octantRowSize; x < rowSizeInVoxels; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[1], octVoxelMaterials, octantRowSize); // recursion

                    octVoxelCount = 0;
                    for (int y = 0; y < octantRowSize; y++)
                    {
                        for (int z = octantRowSize; z < rowSizeInVoxels; z++)
                        {
                            for (int x = 0; x < octantRowSize; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[2], octVoxelMaterials, octantRowSize); // recursion

                    octVoxelCount = 0;
                    for (int y = 0; y < octantRowSize; y++)
                    {
                        for (int z = octantRowSize; z < rowSizeInVoxels; z++)
                        {
                            for (int x = octantRowSize; x < rowSizeInVoxels; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[3], octVoxelMaterials, octantRowSize); // recursion

                    octVoxelCount = 0;
                    for (int y = octantRowSize; y < rowSizeInVoxels; y++)
                    {
                        for (int z = 0; z < octantRowSize; z++)
                        {
                            for (int x = 0; x < octantRowSize; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[4], octVoxelMaterials, octantRowSize); // recursion

                    octVoxelCount = 0;
                    for (int y = octantRowSize; y < rowSizeInVoxels; y++)
                    {
                        for (int z = 0; z < octantRowSize; z++)
                        {
                            for (int x = octantRowSize; x < rowSizeInVoxels; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[5], octVoxelMaterials, octantRowSize); // recursion

                    octVoxelCount = 0;
                    for (int y = octantRowSize; y < rowSizeInVoxels; y++)
                    {
                        for (int z = octantRowSize; z < rowSizeInVoxels; z++)
                        {
                            for (int x = 0; x < octantRowSize; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[6], octVoxelMaterials, octantRowSize); // recursion

                    octVoxelCount = 0;
                    for (int y = octantRowSize; y < rowSizeInVoxels; y++)
                    {
                        for (int z = octantRowSize; z < rowSizeInVoxels; z++)
                        {
                            for (int x = octantRowSize; x < rowSizeInVoxels; x++)
                            {
                                int voxelIndex = x + (z * rowSizeInVoxels) + (y * planeSizeInVoxels);
                                octVoxelMaterials[octVoxelCount] = voxels[voxelIndex];
                                octVoxelCount++;
                            }
                        }
                    }
                    convertVoxelArrayToOctreeRecursively(ref octPar.children[7], octVoxelMaterials, octantRowSize); // recursion
                }
                break;
            }
        }
    }

}
