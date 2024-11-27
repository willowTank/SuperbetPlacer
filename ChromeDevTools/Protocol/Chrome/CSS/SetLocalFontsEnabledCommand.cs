using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDevs.ChromeDevTools.Protocol.Chrome.CSS
{
    [CommandResponse(ProtocolName.CSS.SetLocalFontsEnabled)]
    [SupportedBy("Chrome")]
    public class SetLocalFontsEnabledCommand : ICommand<SetLocalFontsEnabledCommandResponse>
    {
        public bool Enabled { get; set; }
    }
}
