/************************************************************
//     文件名      : SpritePrefabGenerateTool.cs
//     功能描述    : 
//     负责人      : cai yang
//     参考文档    : 无
//     创建日期    : 12/30/2016
//     Copyright  : Copyright 2016-2017 EZFun.
**************************************************************/

using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using LitJson;

public class SpritePrefabGenerateTool {

	private static bool isStoppedByUser = false;

	[MenuItem ("Assets/GeneratePrefab_for_Sprite")]
	static private void GeneratePrefab_for_Sprite()
	{
		isStoppedByUser =  false;
		
		string configPath = "Assets/Editor/SpriteTools/config.json";
		var textData = AssetDatabase.LoadAssetAtPath<TextAsset>(configPath);

		var jsonData = JsonMapper.ToObject(textData.text);

		var tasks = jsonData["tasks"];

		for (int i = 0; i < tasks.Count; i++)
		{
			var task = tasks[i];
			var taskData = task as JsonData;
			var fromFolder = (string)taskData["data_folder"];
			var toFolder = (string)taskData["prefab_folder"];
			var taskName = (string)taskData["name"];

			ConvertPrefabs(fromFolder, toFolder, taskName);
		}

	}

	private void OnEditorUpdate()
	{

	}

	static void ConvertPrefabs(string fromFolder, string toFolder, string taskName)
	{
		if (isStoppedByUser)
		{
			EditorUtility.ClearProgressBar();
			return;
		}

		Debug.Log("CREATE SPRITE PREFABS, FROM:" + fromFolder);
		Debug.Log("CREATE SPRITE PREFABS, TO:" + toFolder);

		string fromPath = Application.dataPath + "/" +fromFolder;
		string toPath = Application.dataPath + "/" + toFolder;

		if(!Directory.Exists(toPath)){
			Directory.CreateDirectory(toPath);
		}

		DirectoryInfo rootDirInfo = new DirectoryInfo (fromPath);


		var files = rootDirInfo.GetFiles("*.png", SearchOption.TopDirectoryOnly);

		if (files.Length > 0)
		{
			for (int i = 0; i < files.Length; i++)
			{
				FileInfo pngFile = files[i];

				string des = "[" + fromFolder + "] to [" + toFolder + "], " + pngFile.Name;
				if (EditorUtility.DisplayCancelableProgressBar("制作sprite prefab:" + taskName, des, ((float)(i + 1)) / files.Length))
				{
					isStoppedByUser = true;
					EditorUtility.ClearProgressBar();
					return;
				}
				else
				{
					string allPath = pngFile.FullName;
					string assetPath = allPath.Substring(allPath.IndexOf("Assets"));

					try {
						Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);

						if (sprite == null)
						{
							Debug.LogError("get sprite failed:" + assetPath);
							continue;
						}

						GameObject go = new GameObject(sprite.name);
						go.AddComponent<UnityEngine.UI.Image>().sprite = sprite;
						allPath = toPath + "/" + sprite.name + ".prefab";
						string prefabPath = allPath.Substring(allPath.IndexOf("Assets"));

						Debug.Log("CREATE SPRITE PREFAB:" + prefabPath);
						PrefabUtility.CreatePrefab(prefabPath,go);
						GameObject.DestroyImmediate(go);
					}
					catch(System.Exception e)
					{
						Debug.LogException(e);
						continue;
					}

				}
			}
			EditorUtility.ClearProgressBar();
		}

		foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories()) {

			ConvertPrefabs(fromFolder + "/" + dirInfo.Name, toFolder + "/" + dirInfo.Name, taskName);

		}

	}

}
