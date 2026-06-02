using Godot;
using System;
using System.Linq;

public partial class Erosion : NodeFunctionality
{
    float[,] PrimaryMap = {};
    public int seed;
    //[Range (2, 8)]
    public int erosionRadius = 5;
    //[Range (0, 1)]
    public float inertia = .05f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 
    public float sedimentCapacityFactor = 4; // Multiplier for how much sediment a droplet can carry
    public float minSedimentCapacity = .01f; // Used to prevent carry capacity getting too close to zero on flatter terrain
    //[Range (0, 1)]
    public float erodeSpeed = .3f;
    //[Range (0, 1)]
    public float depositSpeed = .3f;
    //[Range (0, 1)]
    public float evaporateSpeed = .01f;
    public float gravity = 4;
    public int maxDropletLifetime = 30;

    public float initialWaterVolume = 1;
    public float initialSpeed = 1;

    Vector2I[][] erosionBrushIndices;
    float[][] erosionBrushWeights;
    System.Random prng;

    int currentSeed;
    int currentErosionRadius;
    int currentMapSize;

    public override void Initialize()
    {
        properties["Iterations"] = 50000;
        properties["ErosionRadius"] = 5;
    }


    
    public void InitData () {
        int mapSize = PrimaryMap.GetLength(0);
        currentSeed = seed;
        prng = new System.Random (currentSeed);
        erosionRadius = (int)properties["ErosionRadius"];
        if (erosionBrushIndices == null || currentErosionRadius != erosionRadius || currentMapSize != mapSize) {
            InitializeBrushIndices (mapSize, erosionRadius);
            currentErosionRadius = erosionRadius;
            currentMapSize = mapSize;
        }
    }

        public override T[,] Evaluate<T>(int port){
            PrimaryMap = getFromInput<float>(0);
            InitData();
                if (PrimaryMap.GetLength(0) == 0) {
                    Vector2 terrain_size = (Vector2)GetNode<Node>("/root/Globals").Get("terrain_size");
                    float[,] CreatedPrimaryMap = new float[(int)terrain_size.X, (int)terrain_size.Y];
                    for (int i = 0; i < CreatedPrimaryMap.GetLength(0); i++) {
                        for (int j = 0; j < CreatedPrimaryMap.GetLength(1); j++) {
                            CreatedPrimaryMap[i, j] = 0f;
                        }
                    }
                    PrimaryMap = CreatedPrimaryMap;
                } 
                else {
                    Erode(PrimaryMap, PrimaryMap.GetLength(0), (int)properties["Iterations"]);
                }
                return (T[,])(object)PrimaryMap;
            }

    public void Erode (float[,] map, int mapSize, int numIterations = 1) {
        for (int iteration = 0; iteration < numIterations; iteration++) {
            // Create water droplet at random point on map
            float posX = prng.Next (0, mapSize - 1);
            float posY = prng.Next (0, mapSize - 1);
            float dirX = 0;
            float dirY = 0;
            float speed = initialSpeed;
            float water = initialWaterVolume;
            float sediment = 0;

            for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++) {
                int nodeX = (int) posX;
                int nodeY = (int) posY;
                int dropletIndex = nodeY * mapSize + nodeX;
                // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                float cellOffsetX = posX - nodeX;
                float cellOffsetY = posY - nodeY;

                
                HeightAndGradient heightAndGradient = CalculateHeightAndGradient (map, mapSize, posX, posY);

                
                dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));
                // Normalize direction
                float len = Mathf.Sqrt (dirX * dirX + dirY * dirY);
                if (len != 0) {
                    dirX /= len;
                    dirY /= len;
                }
                posX += dirX;
                posY += dirY;

                // Stop simulating droplet if it's not moving or has flowed over edge of map
                if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posY < 0 || posY >= mapSize - 1) {
                    break;
                }

                // Find the droplet's new height and calculate the deltaHeight
                float newHeight = CalculateHeightAndGradient (map, mapSize, posX, posY).height;
                float deltaHeight = newHeight - heightAndGradient.height;

                // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                float sedimentCapacity = Mathf.Max (-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                // If carrying more sediment than capacity, or if flowing uphill:
                if (sediment > sedimentCapacity || deltaHeight > 0) {
                    // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                    float amountToDeposit = (deltaHeight > 0) ? Mathf.Min (deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                    sediment -= amountToDeposit;

                    
                    map[nodeX, nodeY] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                    map[nodeX + 1, nodeY] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                    map[nodeX, nodeY+1] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                    map[nodeX + 1, nodeY + 1] += amountToDeposit * cellOffsetX * cellOffsetY;

                } else {
                    // Erode a fraction of the droplet's current carry capacity.
                    // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain
                    float amountToErode = Mathf.Min ((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                    // Erosion brush
                    for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[dropletIndex].Length; brushPointIndex++) {
                        Vector2I nodeIndex = erosionBrushIndices[dropletIndex][brushPointIndex];
                        float weighedErodeAmount = amountToErode * erosionBrushWeights[dropletIndex][brushPointIndex];

                        float deltaSediment = (map[nodeIndex.X, nodeIndex.Y] < weighedErodeAmount) ? map[nodeIndex.X, nodeIndex.Y] : weighedErodeAmount;
                        map[nodeIndex.X, nodeIndex.Y] -= deltaSediment;
                        sediment += deltaSediment;
                    }
                }

                
                speed = Mathf.Sqrt (Math.Clamp(speed * speed - deltaHeight * gravity, 0, float.MaxValue));
                water *= (1 - evaporateSpeed);
            }
        }
    }

    HeightAndGradient CalculateHeightAndGradient (float[,] nodes, int mapSize, float posX, float posY) {
        int coordX = (int) posX;
        int coordY = (int) posY;

        // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
        float x = posX - coordX;
        float y = posY - coordY;

        // Calculate heights of the four nodes of the droplet's cell
        int nodeIndexNW = coordY * mapSize + coordX;
        //float heightNW = nodes[nodeIndexNW];
        float heightNW = nodes[coordX, coordY];
        //float heightNE = nodes[nodeIndexNW + 1];
        float heightNE = nodes[coordX + 1, coordY];
        //float heightSW = nodes[nodeIndexNW + mapSize];
        float heightSW = nodes[coordX, coordY + 1];
        //float heightSE = nodes[nodeIndexNW + mapSize + 1];
        float heightSE = nodes[coordX + 1, coordY + 1];

        // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges
        float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
        float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;

        // Calculate height with bilinear interpolation of the heights of the nodes of the cell
        float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;

        return new HeightAndGradient () { height = height, gradientX = gradientX, gradientY = gradientY };
    }

    void InitializeBrushIndices (int mapSize, int radius) {
        erosionBrushIndices = new Vector2I[mapSize * mapSize][];
        erosionBrushWeights = new float[mapSize * mapSize][];

        int[] xOffsets = new int[radius * radius * 4];
        int[] yOffsets = new int[radius * radius * 4];
        float[] weights = new float[radius * radius * 4];
        float weightSum = 0;
        int addIndex = 0;

        for (int i = 0; i < erosionBrushIndices.GetLength (0); i++) {
            int centreX = i % mapSize;
            int centreY = i / mapSize;

            if (centreY <= radius || centreY >= mapSize - radius || centreX <= radius + 1 || centreX >= mapSize - radius) {
                weightSum = 0;
                addIndex = 0;
                for (int y = -radius; y <= radius; y++) {
                    for (int x = -radius; x <= radius; x++) {
                        float sqrDst = x * x + y * y;
                        if (sqrDst < radius * radius) {
                            int coordX = centreX + x;
                            int coordY = centreY + y;

                            if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize) {
                                float weight = 1 - Mathf.Sqrt (sqrDst) / radius;
                                weightSum += weight;
                                weights[addIndex] = weight;
                                xOffsets[addIndex] = x;
                                yOffsets[addIndex] = y;
                                addIndex++;
                            }
                        }
                    }
                }
            }

            int numEntries = addIndex;
            erosionBrushIndices[i] = new Vector2I[numEntries];
            erosionBrushWeights[i] = new float[numEntries];

            for (int j = 0; j < numEntries; j++) {
                erosionBrushIndices[i][j] = new Vector2I(centreX + xOffsets[j], centreY + yOffsets[j]);
                //erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                erosionBrushWeights[i][j] = weights[j] / weightSum;
            }
        }
    }

    struct HeightAndGradient {
        public float height;
        public float gradientX;
        public float gradientY;
    }
    
}
