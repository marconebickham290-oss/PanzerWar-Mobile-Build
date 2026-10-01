using UnityEngine;
using UnityEditor;
using System.IO;

public static class Leopard1PanzerWarBuilder
{
    const string Base = "Assets/Leopard1";

    [MenuItem("Tools/Leopard1/1 - Build Final Vehicle Prefab")]
    public static void BuildPrefab()
    {
        AssetDatabase.Refresh();
        var old = GameObject.Find("Vehicle-Leopard1");
        if (old) Object.DestroyImmediate(old);

        var root = new GameObject("Vehicle-Leopard1");
        var rb = root.AddComponent<Rigidbody>();
        rb.mass = 42400f; rb.interpolation = RigidbodyInterpolation.Interpolate;

        var chassis = new GameObject("Chassis"); chassis.transform.SetParent(root.transform,false);
        AddModel(Base+"/Models/Leopard1_Chassis.obj", chassis.transform, "ChassisVisual");
        var hullCol=chassis.AddComponent<BoxCollider>();
        hullCol.center=new Vector3(0,1.12f,-0.30f); hullCol.size=new Vector3(3.35f,1.45f,6.7f);

        var turretPivot = new GameObject("TurretPivot"); turretPivot.transform.SetParent(root.transform,false);
        turretPivot.transform.localPosition=new Vector3(0,1.72f,0);
        var turret = new GameObject("Turret"); turret.transform.SetParent(turretPivot.transform,false);
        AddModel(Base+"/Models/Leopard1_Turret.obj",turret.transform,"TurretVisual");
        var turretCol=turret.AddComponent<BoxCollider>();
        turretCol.center=new Vector3(0,0.55f,0.10f); turretCol.size=new Vector3(2.75f,1.65f,3.25f);

        var gunPivot = new GameObject("GunPivot"); gunPivot.transform.SetParent(turretPivot.transform,false);
        gunPivot.transform.localPosition=new Vector3(0,0.45f,1.45f); // world source pivot converted minus turret pivot
        var gun = new GameObject("Gun"); gun.transform.SetParent(gunPivot.transform,false);
        AddModel(Base+"/Models/Leopard1_Gun.obj",gun.transform,"GunVisual");

        var muzzle=new GameObject("Muzzle"); muzzle.transform.SetParent(gunPivot.transform,false);
        muzzle.transform.localPosition=new Vector3(0,-0.15f,4.48f);
        var recoil=new GameObject("RecoilRoot"); recoil.transform.SetParent(gunPivot.transform,false);
        var camera=new GameObject("CameraPivot"); camera.transform.SetParent(turretPivot.transform,false); camera.transform.localPosition=new Vector3(0,1.4f,-0.2f);
        var driver=new GameObject("DriverView"); driver.transform.SetParent(root.transform,false); driver.transform.localPosition=new Vector3(0,1.85f,1.35f);
        var exhaustL=new GameObject("Exhaust_L"); exhaustL.transform.SetParent(root.transform,false); exhaustL.transform.localPosition=new Vector3(-1.15f,1.25f,-3.2f);
        var exhaustR=new GameObject("Exhaust_R"); exhaustR.transform.SetParent(root.transform,false); exhaustR.transform.localPosition=new Vector3(1.15f,1.25f,-3.2f);

        // Useful Panzer War / tracked vehicle markers. These do not replace SDK components.
        CreateWheelMarkers(root.transform);
        CreateTrackMarkers(root.transform);

        Directory.CreateDirectory(Base+"/Prefabs");
        PrefabUtility.SaveAsPrefabAsset(root,Base+"/Prefabs/Vehicle-Leopard1.prefab");
        Selection.activeGameObject=root;
        Debug.Log("Leopard 1 final geometry prefab built. Next use Tools/Leopard1/2 - SDK Readiness Check.");
    }

    static void AddModel(string path, Transform parent, string name){
        var p=AssetDatabase.LoadAssetAtPath<GameObject>(path); if(!p){Debug.LogError("Missing "+path);return;}
        var o=(GameObject)PrefabUtility.InstantiatePrefab(p); o.name=name; o.transform.SetParent(parent,false);
    }
    static void CreateWheelMarkers(Transform root){
        var w=new GameObject("WheelMarkers");w.transform.SetParent(root,false);
        float[] z={2.91f,2.24f,1.56f,0.82f,0.15f,-0.58f,-1.27f,-1.97f,-2.74f};
        foreach(float x in new[]{-1.42f,1.42f}) foreach(float zz in z){var g=new GameObject((x<0?"Wheel_L_":"Wheel_R_")+zz.ToString("0.00"));g.transform.SetParent(w.transform,false);g.transform.localPosition=new Vector3(-x,0.41f,zz);}
    }
    static void CreateTrackMarkers(Transform root){
        var t=new GameObject("TrackMarkers");t.transform.SetParent(root,false);
        foreach(float x in new[]{-1.35f,1.35f}){var g=new GameObject(x<0?"Track_L":"Track_R");g.transform.SetParent(t.transform,false);g.transform.localPosition=new Vector3(-x,0.58f,0);}
    }

    [MenuItem("Tools/Leopard1/2 - SDK Readiness Check")]
    public static void CheckSDK(){
        bool modManager=Directory.Exists("Assets/ModManager");
        int pipeline=AssetDatabase.FindAssets("BuildPipline").Length;
        int modPackage=AssetDatabase.FindAssets("ModPackage").Length;
        Debug.Log("Panzer War SDK readiness: Assets/ModManager="+modManager+", BuildPipline assets="+pipeline+", ModPackage assets="+modPackage+". Official SDK is required to generate a game-recognized .modpack.");
        EditorUtility.DisplayDialog("Leopard 1 SDK Check", modManager?"检测到 ModManager。请使用官方 Vehicle Wizard/BuildPipline 关联 Vehicle-Leopard1.prefab，然后构建 Android AssetBundle 与 ModPackage。":"当前工程没有检测到 Panzer War Mod SDK 的 Assets/ModManager，因此不能生成游戏可识别的 .modpack。模型/节点已经准备完成。","OK");
    }
}
