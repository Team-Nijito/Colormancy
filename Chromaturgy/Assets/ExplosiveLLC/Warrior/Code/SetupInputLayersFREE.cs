using UnityEditor;
using UnityEngine;

namespace WarriorAnimsFREE
{

	public class SetupMessageWindow:EditorWindow
	{
		void OnGUI()
		{
			EditorGUILayout.LabelField("Before attempting to use the Warrior Mecanim Animation Packs, you must first ensure that the layers and inputs are correctly defined.  There is an included InputManager.preset and LayerManager.preset which contains all the settings that you can load in.\n \nYou can remove this message by deleting the SetupInputLayersFREE.cs script.", EditorStyles.wordWrappedLabel);
		}
	}

	class SetupInputLayersFREE:AssetPostprocessor
	{
		static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			SetupMessageWindow window  = (SetupMessageWindow)EditorWindow.GetWindow(typeof(SetupMessageWindow), true, "Load Input and Tag Presets");
			window.maxSize = new Vector2(330f, 120f);
			window.minSize = window.maxSize;
		}
	}
}