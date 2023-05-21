using Irrlicht;

namespace nook.io;

class Timer
{
    private double lastLoopTime;
    private float timeCount;

    private UInt16 fps;
    private UInt16 fpsCount;

    private IrrlichtDevice device;

    public Timer(IrrlichtDevice device)
    {
        lastLoopTime = device.Timer.Time;
        timeCount = 0f;

        fps = 0;
        fpsCount = 0;

        this.device = device;
    }

    public double Delta()
    {
        var time = device.Timer.Time;
        var delta = time - lastLoopTime;
        lastLoopTime = time;
        timeCount += (float)delta;
        return delta;
    }

    public void Update()
    {
        if (timeCount > 1)
        {
            fps = fpsCount;
            fpsCount = 0;

            timeCount -= 1;
        }
    }
    
    public UInt16 FPS()
    {
        return fps > 0 ? fps : fpsCount;
    }

    public void UpdateFPS()
    {
        fpsCount++;
    }
}