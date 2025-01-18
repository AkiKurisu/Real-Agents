using UnityEngine;
using UnityEngine.Playables;
namespace Kurisu.AkiAI.Playables
{
    public class FadeInTask : ITask
    {
        public TaskStatus Status { get; private set; } = TaskStatus.Enabled;
        private readonly Playable clipPlayable;
        private Playable mixerPlayable;
        private readonly float fadeInTime;
        public FadeInTask(Playable mixerPlayable, Playable clipPlayable, float fadeInTime)
        {
            this.clipPlayable = clipPlayable;
            this.mixerPlayable = mixerPlayable;
            this.fadeInTime = fadeInTime;
        }
        public void Tick()
        {
            if (!clipPlayable.IsValid())
            {
                Debug.LogWarning("Playable is already destroyed");
                Status = TaskStatus.Disabled;
                return;
            }
            clipPlayable.SetSpeed(1d);
            double current = clipPlayable.GetTime();
            float weight = (float)(current / fadeInTime);
            mixerPlayable.SetInputWeight(0, Mathf.Lerp(1, 0, weight));
            mixerPlayable.SetInputWeight(1, Mathf.Lerp(0, 1, weight));
            if (current >= fadeInTime)
            {
                mixerPlayable.SetInputWeight(0, 0);
                mixerPlayable.SetInputWeight(1, 1);
                Status = TaskStatus.Disabled;
            }
        }
    }
    public class FadeOutTask : ITask
    {
        public TaskStatus Status { get; private set; } = TaskStatus.Enabled;
        private readonly Playable clipPlayable;
        private Playable mixerPlayable;
        private readonly float fadeOutTime;
        private readonly double duration;
        public FadeOutTask(Playable mixerPlayable, Playable clipPlayable, float fadeInTime)
        {
            this.clipPlayable = clipPlayable;
            this.mixerPlayable = mixerPlayable;
            fadeOutTime = fadeInTime;
            duration = clipPlayable.GetDuration();
        }
        public void Tick()
        {
            if (!clipPlayable.IsValid())
            {
                Debug.LogWarning("Playable is already destroyed");
                Status = TaskStatus.Disabled;
                return;
            }
            double current = clipPlayable.GetTime();
            float weight = 1 - (float)((duration - current) / fadeOutTime);
            mixerPlayable.SetInputWeight(0, Mathf.Lerp(0, 1, weight));
            mixerPlayable.SetInputWeight(1, Mathf.Lerp(1, 0, weight));
            if (current >= duration)
            {
                mixerPlayable.SetInputWeight(0, 1);
                mixerPlayable.SetInputWeight(1, 0);
                Status = TaskStatus.Disabled;
            }
        }
    }
    public class WaitPlayableTask : ITask
    {
        public TaskStatus Status { get; private set; } = TaskStatus.Enabled;
        private readonly Playable clipPlayable;
        private readonly double waitTime;
        public WaitPlayableTask(Playable clipPlayable, double waitTime)
        {
            this.clipPlayable = clipPlayable;
            this.waitTime = waitTime;
        }
        public void Tick()
        {
            if (!clipPlayable.IsValid())
            {
                Debug.LogWarning("Playable is already destroyed");
                Status = TaskStatus.Disabled;
                return;
            }
            if (clipPlayable.GetTime() >= waitTime)
            {
                Status = TaskStatus.Disabled;
            }
        }
    }
}
