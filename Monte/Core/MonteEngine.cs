using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;
using Monte.Scenes;
using Monte.Physics;
using Monte.Audio;
using SDL;
using Monte.Tweening;
using Monte.Lib;
using System.Runtime.InteropServices;


namespace Monte
{
    public class MonteEngine
    {
        readonly Window _window;
        readonly Renderer _renderer;

        bool _isRunning = true;
        uint _desiredMS = 16;
        public bool IsRunning => _isRunning;

        public int SetFPS
        {
            set => _desiredMS = (uint)(1 / value * 1000);
        }
        public Window Window
        {
            get => _window;
        }
        public Renderer Renderer
        {
            get => _renderer;
        }

        public MonteEngine(Window window, Renderer renderer)
        {
            Debug.Log("Initializing Monte Engine!\n");
            SDL_GetVersion(out SDL_version vers);
            SDL_MIXER_VERSION(out SDL_version mix_ver);
            SDL_version ttf_Vers = SDL2TtfFixes.TTF_Linked_Version();
            SDL_version img_vers = IMG_Linked_Version();

            Debug.Log($"SDL Version: {vers.major}.{vers.minor}.{vers.patch}");
            Debug.Log($"SDL_Mixer Version: {mix_ver.major}.{mix_ver.minor}.{mix_ver.patch}");
            Debug.Log($"SDL_TTF Version: {ttf_Vers.major}.{ttf_Vers.minor}.{ttf_Vers.patch}");
            Debug.Log($"SDL_IMG Version: {img_vers.major}.{img_vers.minor}.{img_vers.patch}");

            _ = SDL_GetRendererInfo(renderer.SDL_Renderer, out SDL_RendererInfo info);
            Debug.Log($"Renderer tech: {Marshal.PtrToStringAnsi(info.name)}");

            int ramAmount = SDL_GetSystemRAM();
            var osNameAndVersion = RuntimeInformation.OSDescription;
            string gpuInfo = InternalLib.GetGPUInfo();
            string cpuInfo = InternalLib.GetCPUInfo();

            Debug.Log($"SystemRam: {ramAmount / 1000} GB");
            Debug.Log($"OS Architecture: {RuntimeInformation.OSArchitecture}");
            Debug.Log($"Process Architecture: {RuntimeInformation.ProcessArchitecture}");
            Debug.Log($"OS: {osNameAndVersion}");
            Debug.Log($"CPU info: {cpuInfo}");
            Debug.Log($"GPU info: {gpuInfo}");


            if (SDL_Init(EngineSettings.SDLInitFlags) != 0)
            {
                Debug.Log($"Could not initialize SDL: {SDL_GetError()}");
                CloseApplication();
                Cleanup();
            }

            if (IMG_Init(RendererSettings.ImgInitFlags) == 0)
            {
                Debug.Log($"Could not initialize PNG: {SDL_GetError()}; {IMG_GetError()}");
            }

            if (TTF_Init() != 0)
            {
                Debug.Log($"Could not initialize ttf: {SDL_GetError()}; {TTF_GetError()}");
            }


            SetFPS = EngineSettings.DesiredFPS;

            _window = window;
            _renderer = renderer;

            Renderer.AssignRenderer(_renderer);

            AudioManager.Init();
            Input.Setup();
            Application.Initialize(this);

            OnInit();

        }
        private void Logos()
        {
            MonteIntroScene mis = new();
            SceneManager.LoadScene(mis);
        }

        public virtual void OnInit() { }

        public void Run()
        {
            Debug.Log(" ==== Starting Game loop ====");

            uint loopStart = SDL_GetTicks();

            if (EngineSettings.ShowLogoAtStart)
                Logos();
            else
                SceneManager.LoadScene(0);


            while (_isRunning)
            {
                uint startTick = SDL_GetTicks();

                Events(startTick);

                OnUpdate();

                // Might need to start using threading.
                AudioManager.Update();

                TweenCore.Update(Time.DeltaTime);
                SceneManager.CurrentScene.Update();

                if (PhysicsSettings.UsePhysics)
                    Physics2D.DoPhysicsUpdate();
                else
                    Physics2D.DoCollisionUpdate();

                OnRender();
                Renderer.Render();

                SceneManager.CurrentScene.RenderUpdate(Renderer);

                uint deltaTicks = SDL_GetTicks() - startTick;

                if (deltaTicks < _desiredMS)
                {
                    Time.SetDelta(_desiredMS);
                    uint wait = _desiredMS - deltaTicks;
                    SDL_Delay(wait);
                }
                else
                    Time.SetDelta(deltaTicks);
            }
            uint lastedTicks = SDL_GetTicks() - loopStart;
            Debug.Log(new object[] { " = Engine ran for ", lastedTicks, "ticks" });
            Debug.Log(" ==== Game loop stopped  ====");

            Cleanup();
        }

        private void Events(uint startTick)
        {
            while (SDL_PollEvent(out SDL_Event e) == 1)
            {
                Input.HandleKeyboard(e, startTick);
                Input.HandleMouse();

                switch (e.type)
                {
                    case SDL_EventType.SDL_QUIT:
                        _isRunning = false;
                        break;
                    case SDL_EventType.SDL_WINDOWEVENT:
                        _window.HandleWindowEvent(e);
                        break;
                }

                OnEvents(e);
            }
        }

        public void CloseApplication() => _isRunning = false;

        private void Cleanup()
        {
            SceneManager.CurrentScene.Unload();
            SDL_DestroyRenderer(Renderer.SDL_Renderer);
            SDL_DestroyWindow(Window.SDLWindow);

            ContentManager.Cleanup();

            IMG_Quit();
            TTF_Quit();
            Mix_Quit();
            SDL_Quit();

            Debug.Log("Goodbye!");
        }

        public virtual void OnEvents(SDL_Event e) { }
        public virtual void OnRender() { }
        public virtual void OnUpdate() { }
    }
}