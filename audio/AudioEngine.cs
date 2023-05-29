using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace nook.audio;

sealed class AudioEngine : IDisposable
{
    private readonly IWavePlayer outputDevice;
    private readonly MixingSampleProvider mixer;

    private AudioEngine(int sampleRate = 44100, int channelCount = 2)
    {
        outputDevice = new WaveOutEvent();
        mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount))
        {
            ReadFully = true
        };
        outputDevice.Init(mixer);
        outputDevice.Play();
    }

    public void PlaySound(string filename)
    {
        var input = new AudioFileReader(filename);
        AddMixerInput(new AutoDisposeFileReader(input));
    }

    public void PlaySound(CachedSound sound)
    {
        AddMixerInput(new CachedSoundSampleProvider(sound));
    }

    private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
    {
        if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
        {
            return input;
        }

        if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
        {
            return new MonoToStereoSampleProvider(input);
        }

        throw new NotImplementedException("Not Implemented More Than 2 Channels");
    }

    private void AddMixerInput(ISampleProvider input)
    {
        mixer.AddMixerInput(ConvertToRightChannelCount(input));
    }
    
    public void Dispose()
    {
        outputDevice.Dispose();
    }

    public static readonly AudioEngine Instance = new AudioEngine();
}