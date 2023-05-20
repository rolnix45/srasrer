using Irrlicht;
using Irrlicht.Core;

namespace nook.io;

class Input
{
    public static readonly bool[] keys = new bool[255];
    public static readonly Vector2Di mousePos = new Vector2Di();

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
            
            return false;
        };
    }
}