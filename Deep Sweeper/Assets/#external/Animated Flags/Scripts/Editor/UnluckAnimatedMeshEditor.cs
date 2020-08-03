using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FlagAnimator))]
[CanEditMultipleObjects]
[System.Serializable]
public class UnluckAnimatedMeshEditor: Editor {	
    public override void OnInspectorGUI() {
		FlagAnimator target_cs = (FlagAnimator) target;
        DrawDefaultInspector();
		
		if(GUILayout.Button("Force Change Mesh")){
			target_cs.FillCacheArray();
		}
        if (GUI.changed){ 
	        target_cs.CheckIfMeshHasChanged();
	        EditorUtility.SetDirty(target_cs);
        }
    }
}