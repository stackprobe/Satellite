using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Charlotte.Htt
{
	public interface HttService
	{
		bool Interlude();
		HttResponse Service(HttRequest req);
	}
}
