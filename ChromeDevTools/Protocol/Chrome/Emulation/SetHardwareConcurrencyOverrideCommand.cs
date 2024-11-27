using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDevs.ChromeDevTools.Protocol.Chrome.Emulation
{
    [Command(ProtocolName.Emulation.SetHardwareConcurrencyOverride)]
    [SupportedBy("Chrome")]
    public class SetHardwareConcurrencyOverrideCommand : ICommand<SetHardwareConcurrencyOverrideCommandResponse>
    {
        public int HardwareConcurrency { get; set; }
    }
}
