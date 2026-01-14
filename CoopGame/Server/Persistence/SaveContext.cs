using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoopGame.Server.Persistence;

public class SaveContext {
	public string rootPath { get; }

	public SaveContext(string rootPath) {
		this.rootPath = rootPath;
		Directory.CreateDirectory(rootPath);
	}

	public string getPath(string relativePath) {
		return Path.Combine(rootPath, relativePath);
	}

	public void writeJson<T>(string relativePath, T data) {
		string path = getPath(relativePath);

		var options = new JsonSerializerOptions {
			WriteIndented = true
		};

		string json = JsonSerializer.Serialize(data, options);

		File.WriteAllText(path, json);
	}

	public T readJson<T>(string relativePath) {
		string path = getPath(relativePath);

		if (!File.Exists(path)) {
			return default;
		}

		string json = File.ReadAllText(path);
		return JsonSerializer.Deserialize<T>(json)!;
	}
}
