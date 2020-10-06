﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;
using VRKeys;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Combine : MonoBehaviour
{

    public GameObject engine;
    private GameObject third;
    private string name;
    private string path, ass;
    private List<string> carList = new List<string>();
    private string test;
    private GameObject CameraRig;
    public GameObject demo;

    void Awake()
    {
        StreamReader sr = new StreamReader("Assets/Resources/CarList/list.txt");
        test = sr.ReadLine();
        while (test != null)
        {
            carList.Add(test);
            test = sr.ReadLine();
        }
        sr.Close();

        CameraRig = (GameObject)Resources.Load("[CameraRig]");
        if(CameraRig == null)
        {
            print("No CareraRig");
        }
        //demo = GameObject.Find("DemoScene");
    }

    public void combine()
    {
        close();
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        StreamWriter sw = new StreamWriter("Assets/Resources/CarList/list.txt", true);
        path = "Assets/Resources/Prefabs/" + name + ".Prefab";
        ass = "Assets/Resources/Models/" + name + ".asset";
        sw.WriteLine(name);
        sw.Close();
        
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }


        giveComponent(CameraRig);
        Mesh msh = engine.GetComponent<MeshFilter>().sharedMesh;
        AssetDatabase.CreateAsset(msh, ass);
        AssetDatabase.SaveAssets();
        PrefabUtility.SaveAsPrefabAsset(engine, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        SceneManager.LoadScene(0);
        
    }

    

    public void giveComponent(GameObject cameraRig)
    {

        if (engine == null)
        {
            Debug.Log("null engine");
        }
        Debug.Log("scale = " + engine.transform.lossyScale);
        BoxCollider bc = this.GetComponent<BoxCollider>();
        BuildBlock bb = this.GetComponent<BuildBlock>();

        bc.enabled = false;
        DestroyImmediate(bc);
        DestroyImmediate(bb);
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        engine.AddComponent<BoxCollider>();
        BoxCollider box = engine.GetComponent<BoxCollider>();

        engine.AddComponent<CarController>();
        CarController car = engine.GetComponent<CarController>();
        car.enabled = false;

        engine.AddComponent<Rigidbody>();
        Rigidbody rb = engine.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotationZ;
        rb.centerOfMass = box.center - new Vector3(0, box.center.y/2, 0);
        print(rb.centerOfMass);

        Vector3 PosOfCam = new Vector3(box.size.x / 2, box.size.y / 2, box.size.z / 2) + new Vector3(0, box.size.y / 2 + 0.25f, -(box.size.z / 2 + 2));
        GameObject t = Instantiate(cameraRig, PosOfCam, Quaternion.Euler(0, 0, 0), engine.transform);
        t.transform.SetAsFirstSibling();
        t.SetActive(false);
        //選好賽車再啟動所有component

        engine.tag = "Player";
        //engine.AddComponent<Lap>();
    }

    public void SetName(string name)
    {
        this.name = name;
        combine();
    }

    public void close()
    {
        demo.SetActive(false);
        
    }
}
