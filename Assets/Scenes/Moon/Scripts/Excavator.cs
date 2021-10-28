using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

public class Excavator : MonoBehaviour
{
    public float ExcavationSizeX;
    public float ExcavationSizeY;
    [Tooltip("How fast the ground is excavated")]
    public float ExcavationDepth;
    [Tooltip("How far down the 'Hole' can go before bottoming out")]
    public float ExcavationMaxDepth;
    float StartHeight;
    TerrainData CurrentTerrainData;
    Terrain CurrentTerrain;
    Ray AimRay;
    Vector3 ExcavationPoint;
    public UnityEvent Excavated;
    public float depth;
    public RoverProfileSO RoverProfileSO;

    void Start()
    {
        AimRay = new Ray(transform.position, -transform.up * ExcavationMaxDepth);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawRay(AimRay);
        Gizmos.DrawWireCube(ExcavationPoint, new Vector3(ExcavationSizeX, 0, ExcavationSizeY));
    }
    public void Excavate()
    {
        ExcavationSizeX = RoverProfileSO.ExcavationSizeX;
        ExcavationSizeY = RoverProfileSO.ExcavatoreSizeY;
        ExcavationDepth = RoverProfileSO.ExcavationSpeed;

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 50f, NavMesh.AllAreas))
        {
            depth = hit.distance;
            Debug.DrawLine(hit.position, transform.position);
            if (depth > ExcavationMaxDepth) return;
        }
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit ExcavationRaycast,ExcavationMaxDepth))
        {
            if (ExcavationRaycast.collider.GetComponent<Terrain>())
            {
                ExcavationPoint = ExcavationRaycast.point;
                CurrentTerrainData = ExcavationRaycast.collider.GetComponent<Terrain>().terrainData;
                CurrentTerrain = ExcavationRaycast.collider.GetComponent<Terrain>();
                EffectTerrain(ExcavationRaycast.point, ExcavationSizeX, ExcavationSizeY, CurrentTerrain, (int)CurrentTerrainData.heightmapResolution, (int)CurrentTerrainData.heightmapResolution);
                Excavated.Invoke();
            }
        }
    }
    void EffectTerrain(Vector3 pos, float HoleSizeX, float HoleSizeY, Terrain terrain, int w, int h)
    {
        Vector3 Tpos = RelativeTPos(pos, terrain, w, h);
        int heightMapCraterWidth = (int)(HoleSizeX * (w / terrain.terrainData.size.x));
        int heightMapCraterLength = (int)(HoleSizeY * (h / terrain.terrainData.size.z));
        int heightMapStartPosX = (int)(Tpos.x - (heightMapCraterWidth / 2));
        int heightMapStartPosZ = (int)(Tpos.z - (heightMapCraterLength / 2));
        Debug.Log("Relative Position: " + Tpos.ToString());
        Debug.Log("Set From: " + heightMapStartPosX + ", " + heightMapStartPosZ + " to " + (heightMapStartPosX + heightMapCraterWidth) + ", " + (heightMapStartPosZ + heightMapCraterLength) + " Total Size = " + CurrentTerrainData.size.x + " x " + CurrentTerrainData.size.z);

        float[,] heights = terrain.terrainData.GetHeights(heightMapStartPosX, heightMapStartPosZ, heightMapCraterWidth, heightMapCraterLength);
        float circlePosX;
        float circlePosY;
        float distanceFromCenter;

        float deformationDepth = ExcavationDepth / terrain.terrainData.size.y;
        for (int x = 0; x < heightMapCraterLength; x++)
        {
            for (int y = 0; y < heightMapCraterWidth; y++)
            {
                circlePosX = (y - (heightMapCraterWidth / 2)) / (CurrentTerrainData.heightmapResolution / terrain.terrainData.size.x);
                circlePosY = (x - (heightMapCraterLength / 2)) / (CurrentTerrainData.heightmapResolution / terrain.terrainData.size.z);
                distanceFromCenter = Mathf.Abs(Mathf.Sqrt(circlePosX * circlePosX + circlePosY * circlePosY));
                //convert back to values without skew

                if (distanceFromCenter < (Mathf.Lerp(HoleSizeX,HoleSizeY,.5f) / 2.0f))
                {

                    heights[x, y] = Mathf.Clamp(heights[x, y] - (deformationDepth * Time.fixedDeltaTime), 0, 1);
                }
            }
        }

        // set the new height
        CurrentTerrainData.SetHeights(heightMapStartPosX, heightMapStartPosZ, heights);
    }
    Vector3 GetNormalizedTPos(Vector3 Position, Terrain terrain)
    {
        Vector3 temp = (Position - terrain.gameObject.transform.position);
        Vector3 result;
        result.x = temp.x / terrain.terrainData.size.x;
        result.y = temp.y / terrain.terrainData.size.y;
        result.z = temp.z / terrain.terrainData.size.z;
        return result;
    }
    Vector3 RelativeTPos(Vector3 Position, Terrain terrain, int width, int height)
    {
        Vector3 normalizedPos = GetNormalizedTPos(Position, terrain);
        Vector3 result = new Vector3((normalizedPos.x * terrain.terrainData.heightmapResolution), 0, (normalizedPos.z * terrain.terrainData.heightmapResolution));

        return result;
    }
}
