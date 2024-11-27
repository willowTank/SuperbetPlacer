using MasterDevs.ChromeDevTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDevs.ChromeDevTools.Protocol.Chrome.Emulation
{
	[SupportedBy("Chrome")]
	public class DisplayFeature
	{
		/// <summary>
		/// Gets or sets Orientation
		/// </summary>
		public string Orientation { get; set; }
		/// <summary>
		/// Gets or sets Offset
		/// </summary>
		public int Offset { get; set; }
		/// <summary>
		/// Gets or sets MaskLength
		/// </summary>
		public int MaskLength { get; set; }
	}

}
