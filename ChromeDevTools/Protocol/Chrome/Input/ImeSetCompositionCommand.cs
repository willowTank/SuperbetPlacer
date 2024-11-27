using MasterDevs.ChromeDevTools;
using MasterDevs.ChromeDevTools.Protocol.Chrome;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChromeDevTools.Protocol.Chrome.Input
{
	[Command(ProtocolName.Input.ImeSetComposition)]
	[SupportedBy("Chrome")]
	public class ImeSetCompositionCommand : ICommand<ImeSetCompositionCommandResponse>
	{
		/// <summary>
		/// Gets or sets The text to insert
		/// </summary>
		public string Text { get; set; }
		/// <summary>
		/// Gets or sets selection start
		/// </summary>
		public long SelectionStart { get; set; }
		/// <summary>
		/// Gets or sets selection end
		/// </summary>
		public long SelectionEnd { get; set; }
		/// <summary>
		/// Gets or sets replacement start
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? ReplacementStart { get; set; }
		/// <summary>
		/// Gets or sets replacement end
		/// </summary>
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public long? ReplacementEnd { get; set; }
	}
}
