using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEditor;

public class ExpoterWindow : EditorWindow
{
#if UNITY_EDITOR
    [DllImport("libFontArt", EntryPoint = "?exportMeshData@@YAHPEAM0PEAHHHPEADH@Z")]
    private static extern int exportMeshData(UIntPtr vertBuffer, UIntPtr norBuffer, UIntPtr meshBuffer, 
        int vertCount, int meshCount, UIntPtr path, int pathLength);

    private string  objName = "3DText1";
    private string exportPath = "/3DTextPulginAsset/Mesh";

    [MenuItem("Window/Export as...")]
    public static void ShowWindow()
    {
        GetWindow<ExpoterWindow>("Export as a Static Object File");
    }

    public string GetAbsoluteExportPath(string exportPath, string objName)
    {
        string ret = "";
        if (exportPath.Length == 0) return ret;
        if (exportPath.Substring(0, 1) != "/")
        {
            exportPath = "/" + exportPath;
        }
        if (exportPath.Substring(exportPath.Length - 1, 1) != "/")
        {
            exportPath += "/";
        }

        ret = Application.dataPath + exportPath + objName + ".obj";
        return ret;
    }
    void OnGUI()
    {
        GameObject[] objs;
        exportPath = EditorGUILayout.TextField("Export Path", exportPath);
        objName = EditorGUILayout.TextField("Name", objName);
        EditorGUILayout.Separator();

        if(GUILayout.Button("Export as Static Object"))
        {
            objs = Selection.gameObjects;
            int ret = 0;

            string path = GetAbsoluteExportPath(exportPath, objName);

            int vCount = objs[0].transform.GetComponent<MeshFilter>().sharedMesh.vertexCount;

            Vector3[] verts = objs[0].transform.GetComponent<MeshFilter>().sharedMesh.vertices;
            Vector3[] norms = objs[0].transform.GetComponent<MeshFilter>().sharedMesh.normals;
            int[] tr = objs[0].transform.GetComponent<MeshFilter>().sharedMesh.triangles;
            int tCount = tr.Length / 3;
            float[] vertsBuffer = new float[vCount * 3];
            float[] normsBuffer = new float[vCount * 3];
            for (int i = 0; i < vCount; i++)
            {
                vertsBuffer[i * 3] = verts[i].x;
                vertsBuffer[i * 3 + 1] = verts[i].y;
                vertsBuffer[i * 3 + 2] = verts[i].z;

                normsBuffer[i * 3] = norms[i].x;
                normsBuffer[i * 3 + 1] = norms[i].y;
                normsBuffer[i * 3 + 2] = norms[i].z;
            }
                
            unsafe
            {
                fixed (float* vBuf = vertsBuffer, nBuf = normsBuffer)
                {
                    fixed (int* tBuf = tr)
                    {
                        fixed (byte* pBuf = System.Text.Encoding.ASCII.GetBytes(path.ToCharArray()))
                        {
                            ret = exportMeshData(new UIntPtr(vBuf), new UIntPtr(nBuf), new UIntPtr(tBuf), 
                                vCount, tCount, new UIntPtr(pBuf), path.Length);
                        }
                    }
                }
            }
            if (ret == 0)
            {
                Debug.Log("Successfully exported to obj! (" + path + ")");
            }
            else
            {
                Debug.LogWarning("Unknown error occured! (" + path + ")");
            }
        }
    }
#endif

}
