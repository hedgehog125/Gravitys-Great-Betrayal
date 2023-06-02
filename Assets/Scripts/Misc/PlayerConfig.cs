using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

public class PlayerConfig {
	private const string fileName = "config.json";

	public int version = 1;
	public float mouseSensitivity = 0.05f;

	public static PlayerConfig Load() {
		string configPath = Application.persistentDataPath + fileName;

		if (! File.Exists(configPath)) return null;

		string stringContents = File.ReadAllText(configPath);
		return JsonConvert.DeserializeObject<PlayerConfig>(stringContents);
	}
	public static void Save(PlayerConfig parsedContents) {
		string configPath = Application.persistentDataPath + fileName;

		string stringContents = JsonConvert.SerializeObject(parsedContents);
		File.WriteAllText(configPath, stringContents);
	}
}
