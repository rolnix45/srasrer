using Irrlicht;
using Irrlicht.Core;

namespace nook.io;

class Input
{
    public static readonly bool[] keys = new bool[255];
    public static readonly Vector2Di mousePos = new Vector2Di();
    public static bool mouseLeft { get; private set;  }
    public static bool mouseRight { get; private set;  }
    public static bool mouseMiddle { get; private set;  }

    public Input(IrrlichtDevice device)
    {
        device.OnEvent += evnt =>
        {
            if (evnt.Type == EventType.Key)
            {
                keys[(int)evnt.Key.Key] = evnt.Key.PressedDown;
                return true;
            }
            
            if (evnt.Type == EventType.Mouse && evnt.Mouse.Type == MouseEventType.Move)
            {
                mousePos.Set(evnt.Mouse.X, evnt.Mouse.Y);
                return true;
            }

            if (evnt.Type == EventType.Mouse)
            {
                mouseLeft = evnt.Mouse.IsLeftPressed();
                mouseRight = evnt.Mouse.IsRightPressed();
                mouseMiddle = evnt.Mouse.IsMiddlePressed();
                return true;
            }
            
            return false;
        };
    }
}