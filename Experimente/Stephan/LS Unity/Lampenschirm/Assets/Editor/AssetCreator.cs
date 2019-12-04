using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


//Create a folder (Right click in the Assets directory, click Create>New Folder) and name it “Editor” if one doesn’t exist already.
//Place this script in that folder

//This script creates a new menu and a new menu item in the Editor window
// Use the new menu item to create a prefab at the given path. If a prefab already exists it asks if you want to replace it
//Click on a GameObject in your Hierarchy, then go to Examples>Create Prefab to see it in action.



public class AssetCreator : EditorWindow
{
    const float radius = 0.35f;
    const float height = 0.4f;
    //Creates a new menu (Examples) with a menu item (Create Prefab)
    [MenuItem("AssetCreator/Create Prefab")]
    static void CreatePrefab()
    {
        //Keep track of the currently selected GameObject(s)
        GameObject zylindere = new GameObject("Zylinder");
        zylindere.AddComponent<MeshFilter>();
        zylindere.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        zylindere.GetComponent<MeshFilter>().mesh = mesh;

        // Randomly change vertices
        Vector3[] vertices = new Vector3[720+2];
        int p = 0;
        float winkel = 0;
        while (p < vertices.Length)
        {
            float rad_winkel = convertToRadians(winkel);
            
            vertices[p] += new Vector3(radius*Mathf.Sin(rad_winkel), -height/2f, radius*Mathf.Cos(rad_winkel));
            p++;
            vertices[p] += new Vector3(radius*Mathf.Sin(rad_winkel), height/2f, radius*Mathf.Cos(rad_winkel));
            p++;
            winkel+=1;
        }

        int[] newTriangles = new int[ (vertices.Length-2)*3];

        int vert_count=0;

        bool even = false;

        for (int i = 0; i < (newTriangles.Length) ; i+=3)
        {

            if(!even)
            {
                newTriangles[i]   = vert_count;
                newTriangles[i+1] = vert_count+2;
                newTriangles[i+2] = vert_count+1;
            }
            else
            {
                newTriangles[i]   = vert_count;
                newTriangles[i+1] = vert_count+1;
                newTriangles[i+2] = vert_count+2;
            }
            
            vert_count+=1;
            even=!even;
        }

        


        Vector2[] uvs = new Vector2[vertices.Length];

        float uv_offs =1f / ((((float)vertices.Length)/2f)-1f);
        for (int i = 0; i < uvs.Length; i+=2)
        {
            uvs[i] = new Vector2(1f-((i/2)*uv_offs), 0);
            uvs[i+1] = new Vector2(1f-((i/2)*uv_offs), 1);
        }
        uvs[uvs.Length-2] = new Vector2(0, 0);
        uvs[uvs.Length-1] = new Vector2(0, 1);

        // float uv_offs =1f / ((((float)vertices.Length)/2f)-1f);
        // for (int i = 0; i > uvs.Length; i+=2)
        // {
        //     uvs[i] = new Vector2( 1f-((i/2)*uv_offs), 0);
        //     uvs[i+1] = new Vector2( 1f-((i/2)*uv_offs), 1);
        // }
        // uvs[uvs.Length-2] = new Vector2(0, 0);
        // uvs[uvs.Length-1] = new Vector2(0, 1);

        mesh.vertices = vertices; 
        mesh.triangles = newTriangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();


        

       // CreateNew(zylindere,"Assets/myngo");

    }

    public static float convertToRadians(float angle)
    {
        return (Mathf.PI / 180f) * angle;
    }

    static void CreateNew(GameObject obj, string localPath)
    {
        //Create a new prefab at the path given
        Object prefab = PrefabUtility.SaveAsPrefabAsset (obj,localPath);
        //PrefabUtility.ReplacePrefab(obj, prefab, ReplacePrefabOptions.ConnectToPrefab);
    }
}