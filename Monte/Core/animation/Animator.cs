using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Rendering;


namespace Monte.Animation
{
    public class Animator : Enablable, IComponent
    {
        public AnimationClip[] AnimationClips = Array.Empty<AnimationClip>();
        public AnimationClip? CurrentAnimationClip;
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }
        public SpriteRenderer? SpriteRenderer;
        public int CurrentFrameIndex = 0;
        public AnimationFrame? GetFrame => CurrentAnimationClip?.Frames[CurrentFrameIndex];
        float _fps = 12;
        public float FPS
        {
            get => _fps;
            set
            {
                _fps = value;
                _speed = 1 / value;
            }
        }
        double animationTime = 0;
        float _speed;
        public Animator(Entity parent)
        {
            _speed = 1 / _fps;
            _parent = parent;

            parent.Components.Add(this);
        }

        public void AddAnimationsFromFile(string file)
        {
            AnimationClips = ContentManager.LoadAnimations(file);
        }

        public void AddAnimationClip(AnimationClip clip)
        {
            IEnumerable<AnimationClip> _clips = AnimationClips.Where(x => x.Name == clip.Name);
            if (!_clips.Any())
            {
                AnimationClips = AnimationClips.Append(clip).ToArray();
            }
            else
            {
                throw new Exception($"Unable to add animations with the same name twice: {clip.Name}");
            }
        }

        public void AssignSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            SpriteRenderer = spriteRenderer;
        }

        void IComponent.Update()
        {
            if (!Enabled || SpriteRenderer is null) return;

            animationTime += Time.DeltaTime;

            if (CurrentAnimationClip != null && SpriteRenderer != null && SpriteRenderer.Sprite != null)
            {
                int nextFrame = (int)(animationTime / _speed);

                if (CurrentFrameIndex == nextFrame) return;

                // All are checkd above...
#pragma warning disable CS8629
                CurrentFrameIndex = nextFrame % (int)CurrentAnimationClip?.FrameCount;
                AnimationFrame af = (AnimationFrame)CurrentAnimationClip?.Frames[CurrentFrameIndex];
                SpriteRenderer.SRCRect = new(){x = af.X, y = af.Y, w = af.Width, h = af.Height};
#pragma warning restore
            }
        }

        public bool Play(string name)
        {
            if (CurrentAnimationClip is not null)
            {
                if (CurrentAnimationClip?.Name == name)
                {
                    return false;
                }
            }

            AnimationClip? clip = AnimationClips.FirstOrDefault(x => x.Name == name);
            if (clip != null)
            {
                return Play((AnimationClip)clip);
            }
            else
            {
                throw new Exception($"Clip with name {name} could not be found");
            }
        }


        public bool Play(AnimationClip clip)
        {
            if (CurrentAnimationClip?.Name != clip.Name)
            {
                CurrentAnimationClip = clip;
                CurrentFrameIndex = 0;
                animationTime = 0;
                return true;
            }
            return false;
        }

        public void Restart()
        {
            animationTime = 0;
            CurrentFrameIndex = 0;
        }

        void IComponent.Destroy() { }
        void IComponent.Initialize()
        {
            if (Parent != null && SpriteRenderer == null)
            {
                SpriteRenderer? srend = Parent.GetComponentInstance<SpriteRenderer>();
                SpriteRenderer = srend;
            }
        }
    }
}