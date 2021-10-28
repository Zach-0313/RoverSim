using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Terrain_Modify : MonoBehaviour
{
    public Terrain terrain; //get terrain to manipulate
    public float strength = 0.001f; //keep the strength low as we are working in [0, 1] increments as 1 is maximum
    public float Ray_distance = 2f; // get raycast distance from Gameobject

    private int heightmapWidth; //map width
    private int heightmapHeight; //map height
    private float[,] heights; //still don't know what this really does
    private TerrainData terrainData; //all the data ever needed for terrains


    // Baisically call everything so we can work with the terrain in the start function
    void Start()
    {
        terrainData = terrain.terrainData;

        heightmapHeight = terrainData.heightmapResolution;
        heightmapWidth = terrainData.heightmapResolution;
        heights = terrainData.GetHeights(0, 0, heightmapWidth, heightmapHeight);
    }

    // I used fix so it is more smooth with how the terrain is leveled (as this costs cpu resources)
    void FixedUpdate()
    {
        RaycastHit hit;

        //if raycast from object is either above/below find what it hits
        //AND make sure we hit a tag for "Terrain"
        if (Physics.Raycast(this.gameObject.transform.position, -transform.up, out hit, Ray_distance) || Physics.Raycast(this.gameObject.transform.position, Vector3.up, out hit))
        {
            if (hit.point.y < (this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2)) && hit.collider.tag == "Terrain")
            {
                //raise terrain
                RaiseTerrain(hit.point);
            }


            if (hit.point.y > (this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2)) + 0.5f && hit.collider.tag == "Terrain")
            {
                //lower terrain
                LowerTerrain(hit.point);
            }

            // if the terrain hit spot is not too high or low turn off script so we don't waste resources on manipulating the terrain anymore
            if (!(hit.point.y < (this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2))) && (!(hit.point.y > (this.gameObject.transform.position.y - (this.gameObject.transform.localScale.y / 2)) + 0.5f)) && hit.collider.tag == "Terrain")
            {
                this.gameObject.GetComponent<Object_Terrain_Modify>().enabled = false;
            }

        }

        //see in real time when we move the box where we are attempting to hit
        Debug.DrawRay(this.gameObject.transform.position, -Vector3.up, Color.red, Ray_distance);
    }

    public void RaiseTerrain(Vector3 point)
    {
        //get the world coordinate of our hit location and convert it to terrain location
        int mouseX = (int)((point.x / terrainData.size.x) * heightmapWidth);
        int mouseZ = (int)((point.z / terrainData.size.z) * heightmapHeight);

        Debug.Log("what is X: " + mouseX); //check to see if we are in the right area
        Debug.Log("what is Z: " + mouseZ); //check to see if we are in the right area

        float[,] modifiedHeights = new float[1, 1]; //set the maximum heights to 1 as we want to make sure we can manipulate the terrain at any height level
        float y = heights[mouseX, mouseZ]; //unsure still of what heights[,] really accomplishes


        //raise terrain
        y += strength * Time.deltaTime;



        //if object is higher than terrain max height spot increasing terrain.
        if (y > terrainData.size.y)
        {
            y = terrainData.size.y;
        }

        //get our lowest point that we want to start in
        modifiedHeights[0, 0] = y;


        // this is where the magic terrain manipulation magic starts to happen //
        // I set both i & z to a negative scale value to create a starting point behind and away from the box, and keep adjusting terrain till it is positively over the box's area
        for (int i = -(int)this.gameObject.transform.localScale.x; i < (this.gameObject.transform.localScale.x * 2); i++)
        {
            for (int z = -(int)this.gameObject.transform.localScale.z; z < (this.gameObject.transform.localScale.z * 2); z++)
            {
                heights[mouseX - i, mouseZ - z] = y;
                terrainData.SetHeights(mouseX - i, mouseZ - z, modifiedHeights);
            }
        }


    }

    //does pretty much the opposite but it does it very very fast (still the work in progress part of the script)
    public void LowerTerrain(Vector3 point)
    {
        int mouseX = (int)((point.x / terrainData.size.x) * heightmapWidth);
        int mouseZ = (int)((point.z / terrainData.size.z) * heightmapHeight);

        float[,] modifiedHeights = new float[1, 1];
        float y = heights[mouseX, mouseZ];
        y -= strength * Time.deltaTime;

        if (y < terrainData.size.y)
        {
            y = 0;
        }

        modifiedHeights[0, 0] = y;

        for (int i = -(int)this.gameObject.transform.localScale.x; i < (this.gameObject.transform.localScale.x * 2); i++)
        {
            for (int z = -(int)this.gameObject.transform.localScale.z; z < (this.gameObject.transform.localScale.z * 2); z++)
            {
                heights[mouseX - i, mouseZ - z] = y;
                terrainData.SetHeights(mouseX - i, mouseZ - z, modifiedHeights);
            }
        }

    }
}