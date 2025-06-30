using static SDL.Helpers;
using static SDL.SDL;
using static SDL.SDL_video;
using static SDL.SDL_render;
using static SDL.SDL_events;
using static SDL.SDL_image;
using static SDL.SDL_ttf;
using static SDL.SDL_mixer;
using static SDL.SDL_surface;
using static SDL.SDL_rect;
using static SDL.SDL_Version;
using static SDL.Constants;
using static SDL.SDL_Gamecontroller;

using Monte.Settings;
using Monte.Tweening;
using System.Runtime.InteropServices;
using SDL;


namespace Monte.Core
{
    public class MonteEngine
    {
        private static IntPtr sdlw = IntPtr.Zero;

        /// <summary>
        /// SDL_Window pointer reference
        /// </summary>
        public static IntPtr SDL_Window => sdlw;
        private static IntPtr sdlr = IntPtr.Zero;

        /// <summary>
        /// SDL_Renderer pointer reference
        /// </summary>
        public static IntPtr SDL_Renderer => sdlr;

        /// <summary>
        /// Current version of this Engine framework
        /// </summary>
        public const string Version = "1.1.1";
        private bool isRunning = true;

        public MonteEngine()
        {
            Debug.Log($"MonteEngine: {Version}");
            Debug.Log("MonteEngine: Initializing SDL2");

            if (SDL_Init(EngineSettings.SDLInitFlags) != 0)
            {
                Debug.Log(SDL_GetError());
                SDL_Quit();
                return;
            }

            SDL_GetVersion(out SDL_version ver);
            Debug.Log($"SDL2 {ver.major}.{ver.minor}.{ver.patch}");

            Debug.Log("MonteEngine: Initializing Window And Renderer");
            if (SDL_CreateWindowAndRenderer(
                WindowSettings.WindowWidth,
                WindowSettings.WindowHeight,
                WindowSettings.WindowFlags,
                out IntPtr sdl_window,
                out IntPtr sdl_renderer
            ) != 0)
            {
                Debug.Log(SDL_GetError());
                SDL_Quit();
                return;
            }
            else
            {
                sdlw = sdl_window;
                sdlr = sdl_renderer;
            }

            Debug.Log("MonteEngine: Initializing IMG library");
            if (IMG_Init(RendererSettings.ImgInitFlags) == 0)
            {
                Debug.Log(SDL_GetError());
                SDL_Quit();
                return;
            }

            Debug.Log("MonteEngine: Initializing TTF library");
            if (TTF_Init() != 0)
            {
                Debug.Log(SDL_GetError());
                SDL_Quit();
                return;
            }

            Debug.Log("MonteEngine: Initializing MIX library");
            if (Mix_Init(EngineSettings.MixerInitFlags) == 0)
            {
                Debug.Log(SDL_GetError());
                SDL_Quit();
                return;
            }

            Debug.Log("MonteEngine: Initializing Renderer");
            Renderer.Initialize();

            Debug.Log("MonteEngine: Initializing AudioManager");
            AudioManager.Initialize();

            Debug.Log("MonteEngine: Initializing Input system");
            Input.Initialize();

            Debug.Log("MonteEngine: Initializing Debug System");
            Debug.Initialize();

            // ContentManager.Initialize();
        }

        private void DoLogos()
        {
            bool RunLogos = true;

            Stream logoStream = ContentManager.GetResourceStream("logo.png");
            IntPtr LogoPtr = ContentManager.StreamToImage(logoStream);

            Stream fontStream = ContentManager.GetResourceStream("SimpleStuffFixed.ttf");
            IntPtr fontPtr = ContentManager.StreamToFont(fontStream, 12);

            // Stream audioStream = ContentManager.GetResourceStream("monte_engine.wav");
            // IntPtr audioPtr = ContentManager.StreamToAudio(audioStream);

            SDL_Rect src = new(0, 0, 256, 175);
            SDL_FRect dst = new(RendererSettings.VirtualWidth / 2 - 256 / 2, RendererSettings.VirtualHeight / 2 - 175 / 2, 256, 175);
            SDL_FPoint center = new(0, 0);

            Tween alphaTween = new(0, 256, 1);
            byte alpha = 0;
            alphaTween.OnValueChangeAction += (v) => alpha = (byte)v;
            alphaTween.OnCompleteAction += () =>
            {
                alphaTween.ChangeValues(255, 255, 3);
                alphaTween.OnCompleteAction += () =>
                {
                    alphaTween.ChangeValues(255, 0, 1);
                    alphaTween.OnCompleteAction += () =>
                    {
                        RunLogos = false;
                        alphaTween.Destroy();
                    };
                };
            };

            // Play audio bit before running the logos because it's kinda annoying sounding when you restart it each frame. 
            // int channel = Mix_PlayChannel(-1, audioPtr, 0);
            // Mix_Volume(channel, MIX_MAX_VOLUME / 2);

            while (RunLogos)
            {
                PollEvents();

                if (Input.AnyKey())
                {
                    // Mix_HaltChannel(channel);
                    alphaTween.Destroy();
                    RunLogos = false;
                }

                // Logo
                if (SDL_SetRenderDrawColor(SDL_Renderer, 35, 35, 35, 255) != 0)
                    Debug.Log(SDL_GetError());
                if (SDL_RenderClear(SDL_Renderer) != 0)
                    Debug.Log(SDL_GetError());
                if (SDL_SetTextureAlphaMod(LogoPtr, alpha) != 0)
                    Debug.Log(SDL_GetError());
                if (SDL_RenderCopyExF(SDL_Renderer, LogoPtr, ref src, ref dst, 0, ref center, SDL_RendererFlip.SDL_FLIP_NONE) != 0)
                    Debug.Log(SDL_GetError());

                // Text
                if (TTF_SizeText(fontPtr, Version, out int w, out int h) != 0)
                    Debug.Log(SDL_GetError());
                SDL_Rect fsrc = new(0, 0, w, h);
                SDL_FRect fdst = new(
                    RendererSettings.VirtualWidth - h * 3,
                    RendererSettings.VirtualHeight - h * 2,
                    w,
                    h
                );
                IntPtr fSurf = TTF_RenderText_Blended(fontPtr, Version, SDL_WHITE);
                IntPtr fTex = SDL_CreateTextureFromSurface(SDL_Renderer, fSurf);
                SDL_FreeSurface(fSurf);
                SDL_FPoint rotOrigin = new()
                {
                    x = fdst.w / 2,
                    y = fdst.h / 2
                };
                if (SDL_SetTextureAlphaMod(fTex, SDL_WHITE.a) != 0)
                    Debug.Log(SDL_GetError());
                if (SDL_SetTextureColorMod(fTex, SDL_WHITE.r, SDL_WHITE.g, SDL_WHITE.b) != 0)
                    Debug.Log(SDL_GetError());
                if (SDL_RenderCopyExF(SDL_Renderer, fTex, ref fsrc, ref fdst, 0, ref rotOrigin, SDL_RendererFlip.SDL_FLIP_NONE) != 0)
                    Debug.Log($"There was an issue drawing texture. {SDL_GetError()}");

                SDL_DestroyTexture(fTex);

                SDL_RenderPresent(SDL_Renderer);

                TweenCore.Update(0.016);
                SDL_Delay(16);
            }

            TTF_CloseFont(fontPtr);
            SDL_DestroyTexture(LogoPtr);
            // Mix_FreeChunk(audioPtr);
        }

        private void PollEvents()
        {
            while (SDL_PollEvent(out SDL_Event e) != 0)
            {
                if (e.type == SDL_EventType.SDL_QUIT)
                {
                    isRunning = false;
                    break;
                }

                if (e.type == SDL_EventType.SDL_CONTROLLERDEVICEADDED)
                {
                    int deviceIndex = e.cdevice.which;
                    if (SDL_IsGameController(deviceIndex) == SDL_Bool.SDL_TRUE)
                    {
                        IntPtr controller = SDL_GameControllerOpen(deviceIndex);
                        if (controller == IntPtr.Zero)
                            Console.WriteLine("MonteEngine: Could not open gamecontroller: " + SDL_GetError());
                        else
                        {
                            Input.SetupGameController(controller, deviceIndex);
                            Console.WriteLine("MonteEngine: Gamecontroller opened!");
                        }
                    }
                }

                if (e.type == SDL_EventType.SDL_WINDOWEVENT)
                    Window.WindowEvent(e);
                else
                    Input.Update(e);
            }
        }

        /// <summary>
        /// Called to start the gameloop
        /// </summary>
        public void Run()
        {
            if (EngineSettings.ShowMonteLogo)
                DoLogos();
            try
            {
                SceneManager.LoadScene(0);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                Cleanup();
                return;
            }

            while (isRunning)
            {
                Time.FrameStart();

                // Input polls might get called twice per frame so we cannot do setups during polls.  
                Input.BeginFrame();
                PollEvents();

                // Update 
                TweenCore.Update(Time.DeltaTime);
                SceneManager.CurrentScene?.Update();

                Physics2D.PhysicsUpdate();
                Physics2D.DoCollisionUpdate();

                // Render update
                Renderer.Render();
                Debug.Render();

                SDL_RenderPresent(SDL_Renderer);

                Time.FrameEnd();

                SDL_Delay(Time.delayTime);
            }

            Cleanup();
        }

        private void Cleanup()
        {
            Debug.Log("MonteEngine: Cleanup");

            Debug.Log("MonteEngine: Destroying Window");
            SDL_DestroyWindow(SDL_Window);

            Debug.Log("MonteEngine: Destroying Renderer");
            SDL_DestroyRenderer(SDL_Renderer);

            ContentManager.CleanUp();

            Debug.Log("MonteEngine: TTF Quit");
            TTF_Quit();
            Debug.Log("MonteEngine: IMG Quit");
            IMG_Quit();
            Debug.Log("MonteEngine: MIX Quit");
            Mix_Quit();

            Debug.Log("MonteEngine: SDL Quit");
            SDL_Quit();

            Debug.Log("MonteEngine: Goodbye <3");
        }
    }
}