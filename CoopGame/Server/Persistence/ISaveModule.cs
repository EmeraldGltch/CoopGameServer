using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoopGame.Server.Persistence;

public interface ISaveModule {
	string id { get; }

	void save(SaveContext context);
	void load(SaveContext context);
}
