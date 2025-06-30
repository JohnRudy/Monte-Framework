using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Core;
using static SDL.SDL_rect;

namespace Monte.Components
{
    /// <summary>
    /// Wraper of SDL_Rect references to animation frames of a sprite sheet. 
    /// You shouldnt ever need to create animation clips yourself or edit them. 
    /// Animator component will deal with everything for you. 
    /// </summary>
    public struct AnimationClip
    {
        /// <summary>
        /// Name of the animation clip
        /// </summary>
        public string Name;

        /// <summary>
        /// SDL_Rect objets as frames of a spritesheet. In order.
        /// </summary>
        public SDL_Rect[] Frames = Array.Empty<SDL_Rect>();

        /// <summary>
        /// The length of the animation in frames.
        /// </summary>
        public readonly int FrameCount => Frames.Length;

        /// <summary>
        /// Which spritefile is assosiated with this animation.
        /// </summary>
        public string SpriteFile;

        /// <summary>
        /// Is this animation clip looping. 
        /// </summary>
        public bool Loop = true;

        public AnimationClip(string name, SDL_Rect[] frames, string spriteFile)
        {
            Name = name;
            Frames = frames;
            SpriteFile = spriteFile;
        }
    }

    /// <summary>
    /// Main animator component to animate spritesheet animations for you. 
    /// </summary>
    public class Animator : Enablable, IComponent
    {
        /// <summary>
        /// Current collection of animation clips made.
        /// </summary>
        public AnimationClip[] AnimationClips = Array.Empty<AnimationClip>();
        
        /// <summary>
        /// Current playing animaiton
        /// </summary>
        public AnimationClip? CurrentAnimationClip;

        /// <summary>
        /// NOT IMPLEMENTED FROM ICOMPONENT
        /// </summary>
        public string? File {get => null; set => throw new NotImplementedException(); }
        MonteBehaviour? _parent;
        public MonteBehaviour? Parent { get => _parent; set => _parent = value; }

        /// <summary>
        /// Spriterenderer to change SRC_Rect from to animation frame.
        /// </summary>
        public SpriteRenderer? SpriteRenderer;

        /// <summary>
        /// Current animation frame index of animation clip.
        /// </summary>
        public int CurrentFrameIndex {get; private set;}

        /// <summary>
        /// Get the current animation frame SDL_Rect
        /// </summary>
        public SDL_Rect? GetFrame => CurrentAnimationClip?.Frames[CurrentFrameIndex];
        float _fps = 12;

        /// <summary>
        /// Animation Frames per second speed. Automatically changes intervals for ease of use. 
        /// </summary>
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
        public Animator()
        {
            _speed = 1 / _fps;
        }

        public void Initialize()
        {
            if (Parent != null && SpriteRenderer == null)
            {
                SpriteRenderer? srend = Parent.GetComponentInstance<SpriteRenderer>();
                SpriteRenderer = srend;
            }

            if (SpriteRenderer != null && CurrentAnimationClip != null)
                if (SpriteRenderer.File != CurrentAnimationClip?.SpriteFile)
                {
                    SpriteRenderer.File = CurrentAnimationClip?.SpriteFile;
                    SpriteRenderer.Initialize();
                }
        }

        /// <summary>
        /// Adds animation clip to the animators animation clips list. 
        /// </summary>
        /// <param name="clip">Clip to add</param>
        /// <exception cref="Exception">Raised if two animation clips share the same name.</exception>
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


        /// <summary>
        /// Assing a new spriterenderer reference to this animator
        /// </summary>
        /// <param name="spriteRenderer"></param>
        public void AssignSpriteRenderer(SpriteRenderer spriteRenderer)
        {
            SpriteRenderer = spriteRenderer;
        }

        public void Update()
        {
#pragma warning disable CS8629
            if (!Enabled || SpriteRenderer is null) return;

            if (CurrentAnimationClip != null && SpriteRenderer != null)
            {
                animationTime += Time.DeltaTime;
                if (SpriteRenderer.File != CurrentAnimationClip?.SpriteFile)
                {
                    SpriteRenderer.File = CurrentAnimationClip?.SpriteFile;
                    SpriteRenderer.Initialize();
                }

                int nextFrame = (int)(animationTime / _speed);

                if (CurrentFrameIndex == nextFrame) return;

                if (!CurrentAnimationClip.Value.Loop && nextFrame == CurrentAnimationClip.Value.FrameCount) return;

                CurrentFrameIndex = nextFrame % (int)CurrentAnimationClip?.FrameCount;
                SDL_Rect af = (SDL_Rect)CurrentAnimationClip?.Frames[CurrentFrameIndex];

                SpriteRenderer.X = af.x;
                SpriteRenderer.Y = af.y;
                SpriteRenderer.Width = af.w;
                SpriteRenderer.Height = af.h;
#pragma warning restore
            }
        }


        /// <summary>
        /// Play the animation that shares the same "name" as in signature. 
        /// Safe to call multiple times. Will only start animation once. 
        /// Returns true once then false for subsequent calls to the same animation clip.
        /// </summary>
        /// <param name="name">name of the animation clips parameter name</param>
        /// <returns>True if succesfully changed animation, false if cannot change animation</returns>
        /// <exception cref="Exception">If no named clip can be found.</exception>
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

        /// <summary>
        /// Overloaded method of Play. Takes straight animation clip and compares it to the current animation clip. 
        /// If they share the same name, returns false. Otherwise true and changes the CurrentAnimation to the clip. 
        /// Does not add the clip to the animation list. 
        /// </summary>
        /// <param name="clip">AnimationClip to play</param>
        /// <returns>true if success, false if failure</returns>
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

        public void Destroy() { }
    }
}