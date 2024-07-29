using System.Reflection;
using System.Runtime.InteropServices;
using static SDL2.SDL;
using static SDL2.SDL_ttf;
using static SDL2.SDL_image;


namespace Monte.Resource
{
    public static class MonteResource
    {
        private static Dictionary<string, IntPtr> _fonts = new();
        private static Dictionary<string, IntPtr> _images = new();

        private static Stream GetFontStream(string font)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"monte.Resource.{font}";
            Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream != null)
                return resourceStream;
            throw new InvalidOperationException("Resource could not be found");
        }

        private static Stream GetImageStream(string image)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"monte.Resource.{image}";
            Stream? resourceStream = assembly.GetManifestResourceStream(resourceName);
            if (resourceStream != null)
                return resourceStream;
            throw new InvalidOperationException("Resource could not be found");
        }
        public static void UnloadFont(string resource)
        {
            if (_fonts.ContainsKey(resource))
            {
                TTF_CloseFont(_fonts[resource]);
                _fonts.Remove(resource);
            }
        }
        public static void LoadFont(string resourceName, int ptSize, out IntPtr font)
        {
            if (_fonts.ContainsKey(resourceName))
            {
                font = _fonts[resourceName];
            }
            else
            {
                using var fontStream = GetFontStream(resourceName) ?? throw new FileNotFoundException("Font resource not found.");
                byte[] fontData = new byte[fontStream.Length];
                fontStream.Read(fontData, 0, (int)fontStream.Length);

                IntPtr fontPtr = Marshal.AllocHGlobal(fontData.Length);
                Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

                IntPtr rwOps = SDL_RWFromMem(fontPtr, fontData.Length);
                if (rwOps == IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fontPtr);
                    throw new Exception("Failed to create RWops structure: " + SDL_GetError());
                }

                IntPtr f = TTF_OpenFontRW(rwOps, 1, ptSize);
                if (f == IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(fontPtr);
                    throw new Exception("Failed to load font: " + SDL_GetError());
                }

                _fonts.Add(resourceName, f);
                font = f;

                // We do not free the fontPtr here because SDL_ttf manages it now
            }
        }
        public static void UnloadTexture(string resource)
        {
            if (_images.ContainsKey(resource)){
                SDL_DestroyTexture(_images[resource]);
                _images.Remove(resource);
            }
        }

        public static void LoadImage(string resourceName, out IntPtr texture, IntPtr renderer)
        {
            if (_images.ContainsKey(resourceName))
            {
                texture = _images[resourceName];
            }
            else
            {
                using var imageStream = GetImageStream(resourceName) ?? throw new FileNotFoundException("Image resource not found.");
                byte[] imageData = new byte[imageStream.Length];
                imageStream.Read(imageData, 0, (int)imageStream.Length);

                IntPtr imagePtr = Marshal.AllocHGlobal(imageData.Length);
                Marshal.Copy(imageData, 0, imagePtr, imageData.Length);

                IntPtr rwOps = SDL_RWFromMem(imagePtr, imageData.Length);
                if (rwOps == IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(imagePtr);
                    throw new Exception("Failed to create RWops structure: " + SDL_GetError());
                }

                IntPtr surface = IMG_Load_RW(rwOps, 1);
                if (surface == IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(imagePtr);
                    throw new Exception("Failed to load image: " + SDL_GetError());
                }

                // Enable blending for the surface to handle alpha transparency
                if (SDL_SetSurfaceBlendMode(surface, SDL_BlendMode.SDL_BLENDMODE_BLEND) != 0)
                {
                    throw new InvalidOperationException("Could not set surface blend mode " + SDL_GetError());
                }

                texture = SDL_CreateTextureFromSurface(renderer, surface);
                SDL_FreeSurface(surface);
                Marshal.FreeHGlobal(imagePtr);
                if (texture == IntPtr.Zero)
                {
                    throw new Exception("Failed to create texture: " + SDL_GetError());
                }

                _images.Add(resourceName, texture);
            }
        }
    }
}