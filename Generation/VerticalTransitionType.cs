using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheBindingOfV1.Generation
{
    public enum VerticalTransitionType
    {
        Vent,
        WallJump,
        JumpPad,
        FreeFall,
        Elevator,
        DirectFall     // non-adjacent stacked rooms
    }
}
