using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopGame.Server.Persistence;

public class SaveManager {
	private readonly string baseFolder;
	private readonly List<ISaveModule> modules;

	public SaveManager(string baseFolder) {
		this.baseFolder = baseFolder;

		modules = new List<ISaveModule>();

		if(!Directory.Exists(baseFolder)) {
			Directory.CreateDirectory(baseFolder);
		}
	}

	public void registerModule(ISaveModule module) {
		if(!modules.Contains(module)) {
			modules.Add(module);
		}
	}

	public void saveAll() {
		foreach(var module in modules) {
			string moduleFolder = Path.Combine(baseFolder, module.id);

			var context = new SaveContext(moduleFolder);

			try {
				module.save(context);
				Console.WriteLine($"[Save] Saved Module: {module.id}");
			} catch(Exception e) {
				Console.WriteLine($"[Save] Failed to save module {module.id}: {e.Message}");
			}
		}
	}

	public void loadAll() {
		foreach(var module in modules) {
			string moduleFolder = Path.Combine(baseFolder, module.id);

			if(!Directory.Exists(moduleFolder)) {
				Directory.CreateDirectory(moduleFolder);
				Console.WriteLine($"[Load] Created directory for module {module.id} at {moduleFolder}");
			}

			var context = new SaveContext(moduleFolder);

			try {
				module.load(context);
				Console.WriteLine($"[Save] Loaded Module: {module.id}");
			} catch(Exception e) {
				Console.WriteLine($"[Save] Failed to load module {module.id}: {e.Message}");
			}
		}
	}
}
