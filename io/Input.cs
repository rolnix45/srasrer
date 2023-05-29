using Irrlicht;
using Irrlicht.Core;

namespace nook.io;

sealed class Input
{
    public static readonly Vector2Di mousePos = new Vector2Di();
    private static readonly bool[] keys = new bool[255];
    private static bool mouseLeft;
    private static bool mouseRight;
    private static bool mouseMiddle;

    public static bool IsKeyDown(KeyCode keyCode) =>
        keys[(int)keyCode];

    public static bool IsLMBPressed() =>
        mouseLeft;

    public static bool IsRMBPressed() =>
        mouseRight;

    public static bool IsMMBPressed() =>
        mouseMiddle;

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