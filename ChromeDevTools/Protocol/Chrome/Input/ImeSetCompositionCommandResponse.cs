using MasterDevs.ChromeDevTools;
using MasterDevs.ChromeDevTools.Protocol.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDevTools.Protocol.Chrome.Input
{
	[CommandResponse(ProtocolName.Input.ImeSetComposition)]
	[SupportedBy("Chrome")]
	public class ImeSetCompositionCommandResponse
	{
	}
}
