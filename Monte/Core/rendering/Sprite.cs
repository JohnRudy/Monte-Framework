using static SDL2.SDL;


namespace Monte.Rendering
{
    public class Sprite
    {
        readonly string _file;
        readonly IntPtr _texture;
        readonly int _width;
        readonly int _height;
        int _x;
        int _y;

        public IntPtr Tex { get => _texture; }
        public int Width { get => _width; }
        public int Height { get => _height; }
        public string File { get => _file; }
        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        public Sprite(string? file, int width, int height, int x, int y, IntPtr? texture = null)
        {
            _x = x;
            _y = y;

            _width = width;
            _height = height;

            if (file is null)
            {
                var loaded = ContentManager.LoadColoredTexture(width, height, new SDL_Color() { r = 255, g = 255, b = 255, a = 255 });
                _file = loaded.Item1;
                _texture = loaded.Item2;
            }
            else
            {
                if (texture is null)
                {
                    _texture = ContentManager.LoadTexture(file);
                    _file = file;
                }
                else
                {
                    _file = file;
                    _texture = (IntPtr)texture;
                }
            }
        }

        public SDL_Rect GetSRCRect() => new() { x = _x, y = _y, h = _height, w = _width };
    }
}