using static SDL.SDL_mixer;
using static SDL.SDL_surface;
using static SDL.SDL_render;
using static SDL.SDL_Rwops;
using static SDL.SDL_blendmode;
using static SDL.SDL_image;
using static SDL.SDL;
using static SDL.SDL_ttf;

using Monte.Components;

using System.Runtime.InteropServices;
using System.Reflection;
using Monte.Settings;


namespace Monte.Core
{
    public static class ContentManager
    {
        private static Dictionary<string, IntPtr> _customTextures = [];
        private static Dictionary<string, IntPtr> _images = [];
        private static Dictionary<string, IntPtr> _surfaceByFile = new();
        private static Dictionary<string, IntPtr> _fonts = [];
        private static Dictionary<string, IntPtr> _audio = [];

        private static List<string> _tempFiles = [];
        internal static bool UnloadItem(string item)
        {
            if (_images.ContainsKey(item))
            {
                SDL_DestroyTexture(_images[item]);
                _images.Remove(item);
                return true;
            }
            else if (_fonts.ContainsKey(item))
            {
                TTF_CloseFont(_fonts[item]);
                _fonts.Remove(item);
                return true;
            }
            else if (_audio.ContainsKey(item))
            {
                Mix_FreeChunk(_audio[item]);
                _audio.Remove(item);
                return true;
            }
            return false;
        }
        internal static IntPtr GetCustomTexture(string name)
        {
            if (!_customTextures.ContainsKey(name))
            {
                return IntPtr.Zero;
            }
            return _customTextures[name];
        }
        internal static void SetCustomTexture(string name, IntPtr customTex)
        {
            _customTextures.TryAdd(name, customTex);
        }

        internal static Stream GetResourceStream(string resource, string resourceFolder = "Resources", string assemblyName = "MonteEngine")
        {                                                           // Monte Engine library       // Game assembly
            var assembly = assemblyName == "MonteEngine" ? Assembly.GetCallingAssembly() : Assembly.GetEntryAssembly();
            var resourceName = $"{assemblyName}.{resourceFolder}.{resource}";
            Debug.Log($"ContentManager: {resourceName}");

            if (assembly is null)
                throw new Exception("Assembly reference not gotten");

            Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream != null)
                return resourceStream;
            throw new InvalidOperationException("Resource could not be found");
        }
        
        private static IntPtr StreamToPointer(Stream stream)
        {
            byte[] imageData = new byte[stream.Length];
            stream.Read(imageData, 0, (int)stream.Length);
            IntPtr ptr = Marshal.AllocHGlobal(imageData.Length);
            Marshal.Copy(imageData, 0, ptr, imageData.Length);
            IntPtr rwOps = SDL_RWFromMem(ptr, imageData.Length);
            if (rwOps == IntPtr.Zero)
            {
                throw new InvalidOperationException("Failed to create SDL_RWops from memory.");
            }
            return rwOps;
        }

        internal static IntPtr StreamToImage(Stream imageStream)
        {
            IntPtr rwOps = StreamToPointer(imageStream);
            IntPtr surface = IMG_Load_RW(rwOps, 1);

            if (SDL_SetSurfaceBlendMode(surface, SDL_BlendMode.SDL_BLENDMODE_BLEND) != 0)
                Debug.Log(SDL_GetError());

            IntPtr sdl_image = SDL_CreateTextureFromSurface(MonteEngine.SDL_Renderer, surface);
            // SDL_FreeSurface(surface);

            if (sdl_image == IntPtr.Zero)
                throw new Exception("Failed to create texture: " + SDL_GetError());

            return sdl_image;
        }

        internal static IntPtr StreamToFont(Stream fontStream, int ptSize)
        {
            IntPtr rwOps = StreamToPointer(fontStream);
            IntPtr sdl_font = TTF_OpenFontRW(rwOps, 1, ptSize);
            if (sdl_font == IntPtr.Zero)
            {
                Marshal.FreeHGlobal(sdl_font);
                throw new Exception("Failed to load font: " + SDL_GetError());
            }
            return sdl_font;
        }

        internal static IntPtr StreamToAudio(Stream audioStream)
        {
            IntPtr ptr = StreamToPointer(audioStream);
            IntPtr mixChunk = Mix_LoadWAV_RW(ptr, 1);
            return mixChunk;
        }


        internal static string ExtractResourceToTemp(string path)
        {
            string tempFile = Path.GetTempFileName();
            Debug.Log($"ContentManager: {tempFile} created");

#pragma warning disable CS8600
#pragma warning disable CS8602
            string callingAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
#pragma warning restore CS8600
#pragma warning restore CS8602

            if (string.IsNullOrEmpty(callingAssemblyName))
                throw new Exception("Assembly reference not gotten");

            _tempFiles.Add(tempFile);
            Stream stream = GetResourceStream(path, EngineSettings.ContentFolder, callingAssemblyName);
            Debug.Log($"ContentManager: stream fetched for {path}");

            using (FileStream fs = new(tempFile, FileMode.Create, FileAccess.Write))
                stream.CopyTo(fs);

            Debug.Log($"ContentManager: stream written to temp file {tempFile}");
            return tempFile;
        }

        // /// <summary>
        // /// Returns an IntPtr to a single color texture of width and height
        // /// </summary>
        // /// <param name="width">width of the texture</param>
        // /// <param name="height">height of the texture</param>
        // /// <param name="color">SDL_Color of the texture</param>
        // /// <returns>IntPtr Texture</returns>
        // /// <exception cref="Exception"></exception>
        // public static IntPtr LoadColoredTexture(int width, int height, SDL_Color color)
        // {
        //     string texName = $"TEX_{color.r}{color.g}{color.b}{width}{height}";

        //     Debug.Log($"ContentManager: loading  custom colored texture {texName}");

        //     if (_images.ContainsKey(texName)) return _images[texName];

        //     uint Rmask = (uint)(color.r << 16);
        //     uint Gmask = (uint)(color.g << 8);
        //     uint Bmask = (uint)(color.b);
        //     uint Amask = (uint)(color.a << 24);

        //     IntPtr surf = SDL_CreateRGBSurface(0, width, height, 32, Rmask, Gmask, Bmask, Amask);

        //     Debug.Log($"ContentManager: Created RGB Surface");

        //     SDL_Rect rect = new() { x = 0, y = 0, w = width, h = height };

        //     uint white = unchecked((uint)((255 << 24) | (255 << 16) | (255 << 8) | 255));
        //     if (SDL_FillRect(surf, ref rect, white) != 0) throw new Exception($"could not fill rect: {SDL_GetError()}");

        //     Debug.Log($"ContentManager: filled rect");

        //     IntPtr tex = SDL_CreateTextureFromSurface(MonteEngine.SDL_Renderer, surf);

        //     Debug.Log($"ContentManager: Created Texture from surface");

        //     SDL_FreeSurface(surf);

        //     if (SDL_SetTextureBlendMode(tex, RendererSettings.BlendMode) != 0)
        //         Debug.Log(SDL_GetError());

        //     Debug.Log($"ContentManager: Set Texture blend mode");

        //     _images.Add(texName, tex);
        //     return tex;
        // }

        /// <summary>
        /// Method to load SDL_Texture pointers to memroy and let ContentManager manage them
        /// </summary>
        /// <param name="file">path to file</param>
        /// <returns>IntPtr of texture</returns>
        /// <exception cref="Exception"></exception>
        public static IntPtr LoadImage(string file)
        {
            Debug.Log($"ContentManager: load {file}");
            if (!_images.ContainsKey(file))
            {
                string path = ExtractResourceToTemp(file);
                IntPtr surface = IMG_Load(path);
                if (surface == IntPtr.Zero)
                    throw new Exception($"IMG_Load failed: {SDL_GetError()}");

                Debug.Log($"ContentManager: {file} texture creation from surface");
                IntPtr texture = SDL_CreateTextureFromSurface(MonteEngine.SDL_Renderer, surface);
                if (texture == IntPtr.Zero)
                    throw new Exception($"SDL_CreateTextureFromSurface failed: {SDL_GetError()}");

                // Debug.Log($"ContentManager: {file} freeing surface");
                // SDL_FreeSurface(surface);

                if (SDL_SetTextureBlendMode(texture, RendererSettings.BlendMode) != 0)
                    Debug.Log(SDL_GetError());

                _images[file] = texture;
                _surfaceByFile[file] = surface;
            }
            return _images[file];
        }

        public static IntPtr? GetTextureSurface(string file)
            => _surfaceByFile.TryGetValue(file, out var surf) ? surf : null;

        /// <summary>
        /// Method to load SDL_Font pointers to memory and let content manager manage it
        /// </summary>
        /// <param name="file">path to file</param>
        /// <param name="ptSize">font size wanted</param>
        /// <returns>IntPtr of font</returns>
        /// <exception cref="Exception"></exception>
        public static IntPtr LoadFont(string file, int ptSize)
        {
            Debug.Log($"ContentManager: load {file}");
            if (!_fonts.ContainsKey(file))
            {
                string path = ExtractResourceToTemp(file);
                IntPtr fontPtr = TTF_OpenFont(path, ptSize);
                _fonts[file] = fontPtr;
            }
            return _fonts[file];
        }

        /// <summary>
        /// Method to load Mix_chunk pointers to memory and let content manager manage it.
        /// </summary>
        /// <param name="file">path to file</param>
        /// <returns>AudioClip object with chunk reference</returns>
        public static AudioClip LoadAudio(string file)
        {
            Debug.Log($"ContentManager: load {file}");
            if (!_audio.ContainsKey(file))
            {
                string path = ExtractResourceToTemp(file);
                IntPtr audioPtr = Mix_LoadWav(path);
                _audio[file] = audioPtr;
            }
            return new AudioClip(file, null, IntPtr.Zero, 0, _audio[file]);
        }

        internal static void CleanUp()
        {
            Debug.Log("ContentManager: Destroying textures");
            foreach (IntPtr ptr in _images.Values)
                SDL_DestroyTexture(ptr);

            Debug.Log("ContentManager: Destroying fonts");
            foreach (IntPtr ptr in _fonts.Values)
                TTF_CloseFont(ptr);

            Debug.Log("ContentManager: Destroying audio");
            foreach (IntPtr ptr in _audio.Values)
                Mix_FreeChunk(ptr);

            Debug.Log("ContentManager: cleaning temp files");
            foreach (string tmp in _tempFiles)
                File.Delete(tmp);

            Debug.Log("ContentManager: Clearning up surfaces");
            foreach (IntPtr ptr in _surfaceByFile.Values)
                SDL_FreeSurface(ptr);

            Debug.Log("ContentManager: Clearning up surfaces");
            foreach (IntPtr ptr in _customTextures.Values)
                SDL_FreeSurface(ptr);
        }
    }
}
