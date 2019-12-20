using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

//Create a folder (Right click in the Assets directory, click Create>New Folder) and name it “Editor” if one doesn’t exist already.
//Place this script in that folder

//This script creates a new menu and a new menu item in the Editor window
// Use the new menu item to create a prefab at the given path. If a prefab already exists it asks if you want to replace it
//Click on a GameObject in your Hierarchy, then go to Examples>Create Prefab to see it in action.



public class AssetCreator : EditorWindow
{
    

    
    [MenuItem("AssetCreator/Create Texture Masks")]
    static void makeTextures()
    {
        initMasken();
    }

    private static void initMasken()
    {
           // init Masken
        
        // Material mat1 = GameObject.Find("/D2_Welt/ZylinderMaske1").GetComponent<Renderer>().material;
        // Material mat2 = GameObject.Find("/D2_Welt/ZylinderMaske2").GetComponent<Renderer>().material;
        // Material mat3 = GameObject.Find("/D2_Welt/ZylinderMaske3").GetComponent<Renderer>().material;
 
        Texture2D texture1 = new Texture2D(6598, 1200, TextureFormat.RGBA32,  false,  false); 
        Texture2D texture2 = new Texture2D(6598, 1200, TextureFormat.RGBA32,  false,  false); 
        Texture2D texture3 = new Texture2D(6598, 1200, TextureFormat.RGBA32,  false,  false); 
        

        // mat1.mainTexture = texture1;
        // mat2.mainTexture = texture2;
        // mat3.mainTexture = texture3;

        float tex_div_3 = texture1.width/3f;
        Color color_transparent = new Color(0,0,0,0f);
        Color color_black = new Color(0,0,0,1f);

        for (int y = 0; y < texture1.height; y++)
        {
            for (int x = 0; x < texture1.width; x++) // texture1.width
            {
                texture1.SetPixel(x, y, color_black);
                texture2.SetPixel(x, y, color_black);
                texture3.SetPixel(x, y, color_black);
            }
        }

        for (int y = 0; y < texture1.height; y++)
        {
            for (int x = 0; x < tex_div_3/2f; x++) // texture1.width
            {
               
                texture1.SetPixel(x, y, color_transparent);
            }
            for (int x = texture1.width-1; x >= texture1.width-(tex_div_3/2f); x--) // texture1.width
            {
                texture1.SetPixel(x, y, color_transparent);
            }
        }
        for (int y = 0; y < texture2.height; y++)
        {
            for (int x = (int)(tex_div_3/2f); x < (tex_div_3)+(tex_div_3/2f); x++) // texture1.width
            {
               
                texture2.SetPixel(x, y, color_transparent);
            }
           
        }
        for (int y = 0; y < texture2.height; y++)
        {
            for (int x = (int)((tex_div_3)+(tex_div_3/2f)); x < texture3.width-(tex_div_3/2f); x++) // texture1.width
            {
               
                texture3.SetPixel(x, y, color_transparent);
            }
           
        }

        
        var dirPath = Application.dataPath+"/" ;
        if(!Directory.Exists(dirPath)) 
        {
            Directory.CreateDirectory(dirPath);
        }
        byte[] bytes1 = texture1.EncodeToPNG();
        File.WriteAllBytes(dirPath + "MaskeCamera1" + ".png", bytes1);

        byte[] bytes2 = texture2.EncodeToPNG();
        File.WriteAllBytes(dirPath + "MaskeCamera2" + ".png", bytes2);

        byte[] bytes3 = texture3.EncodeToPNG();
        File.WriteAllBytes(dirPath + "MaskeCamera3" + ".png", bytes3);

        
        // texture1.Apply();
        // texture2.Apply();
        // texture3.Apply();
    }

    [MenuItem("AssetCreator/Create smaller Zylinder")]
    static void makeZylinder()
    {

        float radius = Const.radius;
        float height = Const.height_zylinder-0.02f;
     
        GameObject zylindere = new GameObject("Zylinder");
        zylindere.AddComponent<MeshFilter>();
        zylindere.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        zylindere.GetComponent<MeshFilter>().mesh = mesh;

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


        mesh.vertices = vertices; 
        mesh.triangles = newTriangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

    }

    [MenuItem("AssetCreator/Create Zylinder")]
    static void makeZylinderSmall()
    {
        float radius = Const.radius;
        float height = Const.height_zylinder;

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