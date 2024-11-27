using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterDevs.ChromeDevTools.Protocol.Chrome.Emulation
{
    [Command(ProtocolName.Emulation.SetIdleOverride)]
    [SupportedBy("Chrome")]
    public class SetIdleOverrideCommand : ICommand<SetIdleOverrideCommandResponse>
    {
        public bool IsUserActive { get; set; }
        public bool IsScreenUnlocked { get; set; }
    }
}
