using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_ttf;
using static SDL2.SDL_mixer;
using Monte.Animation;
using System.Xml;
using Monte.Audio;


namespace Monte
{
    public sealed class ContentManager
    {
        readonly static string MONTE_TEX = "MONTE_PLAIN_TEX_";
        readonly static Dictionary<string, IntPtr> _textures = new();
        readonly static Dictionary<string, IntPtr> _fonts = new();
        readonly static Dictionary<string, AnimationClip[]> _animations = new();
        readonly static Dictionary<string, AudioClip> _audio = new();
        readonly static Dictionary<string, XmlDocument> _maps = new();
        readonly static Dictionary<string, XmlDocument> _mapTiles = new();


        public static XmlDocument LoadMapTiles(string MapTiles)
        {
            if (!_mapTiles.ContainsKey(MapTiles))
            {
                XmlDocument mapTiles = new();
                mapTiles.Load($"Content/{MapTiles}");
                _mapTiles[MapTiles] = mapTiles;
            }
            return _mapTiles[MapTiles];
        }

        public static XmlDocument LoadMapFile(string MapFile)
        {
            if (!_maps.ContainsKey(MapFile))
            {
                XmlDocument mapFile = new();
                mapFile.Load($"Content/{MapFile}");
                _maps[MapFile] = mapFile;
            }
            return _maps[MapFile];
        }

        public static void UnloadMapTiles(string MapTiles)
        {
            if (_mapTiles.ContainsKey(MapTiles))
                _mapTiles.Remove(MapTiles);
        }
        public static void UnloadMap(string MapFile)
        {
            if (_maps.ContainsKey(MapFile))
                _maps.Remove(MapFile);
        }

        public static AudioClip MIXLoadAudio(string file)
        {
            if (!_audio.ContainsKey(file))
            {

                IntPtr wav = Mix_LoadWAV($"Content/{file}");
                AudioClip ac = new(file, null, IntPtr.Zero, 0, wav);
                _audio[file] = ac;
            }
            return _audio[file];
        }

        public static void UnloadAudio(string file)
        {
            if (_audio.ContainsKey(file))
            {
                SDL_FreeWAV(_audio[file].buf);
                _audio.Remove(file);
            }
        }

        // Unnecessary warnings as this xml file is done in accordance to MonteSplitter
        // exception raised is enough for debugging purposes
#pragma warning disable CS8600
#pragma warning disable CS8602
        public static AnimationClip[] LoadAnimations(string file)
        {
            if (_animations.ContainsKey(file))
            {
                return _animations[file];
            }

            List<AnimationClip> animationClips = new();
            XmlDocument xmlDoc = new();
            xmlDoc.Load($"Content/{file}");

            XmlNode animations = xmlDoc.SelectSingleNode("/animations");
            foreach (XmlNode animation_node in animations.ChildNodes)
            {
                string anim_name = animation_node.LocalName;

                XmlNode frames_node = animation_node.SelectSingleNode("frames");

                List<AnimationFrame> frames = new();
                foreach (XmlNode frame in frames_node.ChildNodes)
                {
                    int index = Convert.ToInt32(frame.LocalName.Trim().Replace("frame_", ""));

                    int x = Convert.ToInt32(frame.SelectSingleNode("x").InnerText.Trim());
                    int y = Convert.ToInt32(frame.SelectSingleNode("y").InnerText.Trim());

                    int width = Convert.ToInt32(frame.SelectSingleNode("width").InnerText.Trim());
                    int height = Convert.ToInt32(frame.SelectSingleNode("height").InnerText.Trim());

                    AnimationFrame af = new(index, x, y, width, height);
                    frames.Add(af);

                }
                AnimationClip ac = new(anim_name, frames.ToArray());
                animationClips.Add(ac);
            }
            _animations.Add(file, animationClips.ToArray());
            return animationClips.ToArray();
        }
#pragma warning restore CS8600
#pragma warning restore CS8602

        public static void LoadFont(string file, int ptzSize, out IntPtr font)
        {
            if (_fonts.ContainsKey(file))
                font = _fonts[file];
            else
            {
                IntPtr f = TTF_OpenFont($"Content/{file}", ptzSize);
                _fonts.Add(file, f);
                font = f;
            }
        }

        public static void UnloadFont(string file)
        {
            if (_fonts.ContainsKey(file))
            {
                TTF_CloseFont(_fonts[file]);
                _fonts.Remove(file);
            }
        }

        public static void UnloadFont(IntPtr font)
        {
            if (_fonts.ContainsValue(font))
            {
                TTF_CloseFont(font);
                string key = _fonts.FirstOrDefault(x => x.Value.Equals(font)).Key;
                if (key is not null)
                    _fonts.Remove(key);
            }
        }

        public static IntPtr LoadTexture(string file)
        {
            if (!_textures.ContainsKey(file))
            {
                IntPtr surf = IMG_Load($"Content/{file}");
                IntPtr tex = SDL_CreateTextureFromSurface(Renderer.Instance.SDL_Renderer, surf);
                SDL_FreeSurface(surf);

                _textures.Add(file, tex);
            }
            return _textures[file];
        }

        public static Tuple<string, IntPtr> LoadColoredTexture(int width, int height, SDL_Color color)
        {
            string texName = $"{MONTE_TEX}{width}{height}";
            if (_textures.ContainsKey(texName)) return new(texName, _textures[texName]);

            uint Rmask = (uint)(color.r << 16);
            uint Gmask = (uint)(color.g << 8);
            uint Bmask = (uint)(color.b);
            uint Amask = (uint)(color.a << 24);

            IntPtr surf = SDL_CreateRGBSurface(0, width, height, 32, Rmask, Gmask, Bmask, Amask);

            SDL_Rect rect = new() { x = 0, y = 0, w = width, h = height };

            uint white = unchecked((uint)((255 << 24) | (255 << 16) | (255 << 8) | 255));
            if (SDL_FillRect(surf, ref rect, white) != 0) throw new Exception($"could not fill rect: {SDL_GetError()}");

            IntPtr tex = SDL_CreateTextureFromSurface(Renderer.Instance.SDL_Renderer, surf);
            SDL_FreeSurface(surf);
            _textures.Add(texName, tex);
            return new(texName, tex);
        }

        public static void UnloadTexture(string file)
        {
            if (_textures.ContainsKey(file))
            {
                SDL_DestroyTexture(_textures[file]);
                _textures.Remove(file);
            }
        }

        public static void UnloadTexture(IntPtr texture)
        {
            if (_textures.ContainsValue(texture))
            {
                SDL_DestroyTexture(texture);
                string key = _textures.FirstOrDefault(x => x.Value.Equals(texture)).Key;
                if (key != null)
                    _textures.Remove(key);
            }
        }

        internal static void Cleanup()
        {
            foreach (IntPtr tex in _textures.Values)
                SDL_DestroyTexture(tex);

            foreach (IntPtr font in _fonts.Values)
                TTF_CloseFont(font);

            foreach (AudioClip ac in _audio.Values)
            {
                SDL_FreeWAV(ac.buf);
                if (ac.Wav != IntPtr.Zero)
                {
                    try
                    {
                        Mix_FreeChunk(ac.Wav);
                    }
                    catch
                    {
                        Mix_FreeMusic(ac.Wav);
                    }
                }
            }
            _textures.Clear();
            _fonts.Clear();
            _audio.Clear();
        }

        internal static bool IsContentManagerResource(string resource)
        {
            return  _textures.ContainsKey(resource) ||
                    _fonts.ContainsKey(resource) ||
                    _animations.ContainsKey(resource) ||
                    _audio.ContainsKey(resource) ||
                    _maps.ContainsKey(resource) ||
                    _mapTiles.ContainsKey(resource);
        }
    }
}