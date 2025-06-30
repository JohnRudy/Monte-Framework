using static SDL.SDL_render;
using static SDL.SDL_video;
using static SDL.SDL;
using static SDL.SDL_pixels;
using static SDL.SDL_rect;
using static SDL.SDL_ttf;
using static SDL.Constants;
using static SDL.SDL_surface;

using System.Numerics;
using Monte.Settings;
using Monte.Abstractions;
using System.Runtime.InteropServices;


namespace Monte.Core
{
    /// <summary>
    /// Renderorder enumerator 
    /// </summary>
    public enum RenderOrder
    {
        ONLY_PRIORITY, // fastest sort
        LEFTDOWN, // topdown - isometric view from top left
    }

    /// <summary>
    /// Renderobjects renderspace enumerator
    /// </summary>
    public enum RenderSpace
    {
        SCREEN,
        CAMERA
    }

    /// <summary>
    /// Main renderer class with Helper methods and handles renderobject handling and Render calls.
    /// </summary>
    public static class Renderer
    {
        public static List<RenderObject> RenderObjects = [];

        internal static void Initialize()
        {
            if (SDL_SetRenderDrawBlendMode(MonteEngine.SDL_Renderer, RendererSettings.BlendMode) != 0)
                Debug.Log($"There was an issue settings blend mode. {SDL_GetError()}");

            if (SDL_RenderSetLogicalSize(MonteEngine.SDL_Renderer, RendererSettings.VirtualWidth, RendererSettings.VirtualHeight) != 0)
                Debug.Log($"There was an issue settings logical size. {SDL_GetError()}");
        }

        private static void PreDraw()
        {
            if (SceneManager.CurrentScene != null)
            {
                SDL_Color bg = SceneManager.CurrentScene.BackgroundColor;
                if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, bg.r, bg.g, bg.b, bg.a) != 0)
                    Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");
            }
            else
            {
                if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, 35, 35, 35, 255) != 0)
                    Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");
            }

            if (SceneManager.CurrentScene != null)
                _ = SDL_SetWindowOpacity(MonteEngine.SDL_Window, SceneManager.CurrentScene.BackgroundColor.a);

            if (SDL_RenderClear(MonteEngine.SDL_Renderer) != 0)
                Debug.Log($"There was an issue clearing renderer. {SDL_GetError()}");
        }

        internal static void Render()
        {
            PreDraw();

            List<RenderObject> sorted = [];

            if (RendererSettings.RenderOrder == RenderOrder.ONLY_PRIORITY)
                sorted = RenderObjects.OrderBy(x => x.Priority).ToList();

            else if (RendererSettings.RenderOrder == RenderOrder.LEFTDOWN)
                sorted = RenderObjects.OrderBy(x => x.Priority)
                .ThenBy(x => x.Transform.Position.X)
                .ThenBy(x => x.Transform.Position.Y)
                .ToList();

            foreach (RenderObject ro in sorted)
                if (ro.Enabled)
                    ro.Render();

            // Moved to engine loop
            // SDL_RenderPresent(MonteEngine.SDL_Renderer);
        }

        /// <summary>
        /// Use the SDL internal method and draw a point with SDL color in screen space
        /// </summary>
        /// <param name="point">point to draw</param>
        /// <param name="color">Color to use</param>
        public static void DrawPoint(SDL_FPoint point, SDL_Color color)
        {
            if (point.x < 0 || point.x > RendererSettings.Margin + RendererSettings.VirtualWidth || point.y < 0 || point.y > RendererSettings.Margin + RendererSettings.VirtualHeight)
                return;

            if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, color.r, color.g, color.b, color.a) != 0)
                Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");

            if (SDL_RenderDrawPointF(MonteEngine.SDL_Renderer, point.x, point.y) != 0)
                Debug.Log($"There was an issue drawing a pixel. {SDL_GetError()}");
        }

        /// <summary>
        /// Use the SDL internal method and draw a point with SDL color in screen space
        /// </summary>
        /// <param name="point">point to draw</param>
        /// <param name="color">Color to use</param>
        public static void DrawPoint(Vector2 point, SDL_Color color)
        {
            if (point.X < 0 || point.X > RendererSettings.Margin + RendererSettings.VirtualWidth || point.Y < 0 || point.Y > RendererSettings.Margin + RendererSettings.VirtualHeight)
                return;

            SDL_FPoint p = new()
            {
                x = point.X,
                y = point.Y
            };

            if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, color.r, color.g, color.b, color.a) != 0)
                Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");

            if (SDL_RenderDrawPointF(MonteEngine.SDL_Renderer, p.x, p.y) != 0)
                Debug.Log($"There was an issue drawing a pixel. {SDL_GetError()}");
        }

        /// <summary>
        /// Use the SDL internal method and draw a line with SDL color in screen space
        /// </summary>
        /// <param name="a">start of line</param>
        /// <param name="b">end of line</param>
        /// <param name="color">Color to use</param>
        public static void DrawLine(SDL_FPoint a, SDL_FPoint b, SDL_Color color)
        {
            if (float.IsNaN(a.x) || float.IsNaN(a.y) || float.IsNaN(b.x) || float.IsNaN(b.y)) return;
            if (float.IsInfinity(a.x) || float.IsInfinity(b.x) || float.IsInfinity(a.y) || float.IsInfinity(b.y)) return;

            if (
                a.x < 0 && b.x < 0 ||
                a.y < 0 && b.y < 0 ||
                a.x > RendererSettings.Margin + RendererSettings.VirtualWidth && b.x > RendererSettings.Margin + RendererSettings.VirtualWidth ||
                a.y > RendererSettings.Margin + RendererSettings.VirtualHeight && b.y > RendererSettings.Margin + RendererSettings.VirtualHeight
            )
                return;

            if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, color.r, color.g, color.b, color.a) != 0)
                Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");

            if (SDL_RenderDrawLineF(
                MonteEngine.SDL_Renderer,
                a.x,
                a.y,
                b.x,
                b.y
            ) != 0)
                Debug.Log($"There was an issue drawing a line. {SDL_GetError()}");
        }

        /// <summary>
        /// Use the SDL internal method and draw a line with SDL color in screen space
        /// </summary>
        /// <param name="a">start of line</param>
        /// <param name="b">end of line</param>
        /// <param name="color">Color to use</param>
        public static void DrawLine(Vector2 a, Vector2 b, SDL_Color color)
        {
            if (float.IsNaN(a.X) || float.IsNaN(a.Y) || float.IsNaN(b.X) || float.IsNaN(b.Y)) return;
            if (float.IsInfinity(a.X) || float.IsInfinity(b.X) || float.IsInfinity(a.Y) || float.IsInfinity(b.Y)) return;

            SDL_FPoint af = new() { x = a.X, y = a.Y };
            SDL_FPoint bf = new() { x = b.X, y = b.Y };

            if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, color.r, color.g, color.b, color.a) != 0)
                Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");

            if (SDL_RenderDrawLineF(MonteEngine.SDL_Renderer, af.x, af.y, bf.x, bf.y) != 0)
                Debug.Log($"There was an issue drawing a line. {SDL_GetError()}");
        }

        /// <summary>
        /// Draw a polygon object using the SDL internal logic
        /// </summary>
        /// <param name="texture">Texture to use</param>
        /// <param name="vertices">Vertices of the polygon</param>
        /// <param name="indices">indices of the vertices</param>
        public static void DrawPolygon(IntPtr texture, SDL_Vertex[] vertices, int[] indices)
        {
            int num_indicies = indices.Length;
            int num_verticies = vertices.Length;

            if (SDL_RenderGeometry(MonteEngine.SDL_Renderer, texture, vertices, num_verticies, indices, num_indicies) != 0)
                Debug.Log($"There was an issue drawing a polygon. {SDL_GetError()}");
        }


        public static void DrawFRect(SDL_FRect rect, SDL_Color color) => DrawFRect(rect.x, rect.y, rect.w, rect.h, color);
        public static void DrawFRect(float x, float y, float w, float h, SDL_Color color)
        {
            if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(w) || float.IsNaN(h) || float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(w) || float.IsInfinity(h)) return;

            // Debug.Log($"h: {h}");

            if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, color.r, color.g, color.b, color.a) != 0)
                Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");

            SDL_FRect rect = new()
            {
                x = x,
                y = y,
                w = w,
                h = h
            };

            // Debug.Log($"x: {rect.x}, y: {rect.y}, w: {rect.w}, h: {rect.h}");

            if (SDL_RenderDrawRectF(MonteEngine.SDL_Renderer, ref rect) != 0)
                Debug.Log($"There was an issue drawing a fRect. {SDL_GetError()}");
        }


        /// <summary>
        /// Draws a filled rectangle in screen space using SDL internals
        /// </summary>
        /// <param name="x">x position on screen</param>
        /// <param name="y">y position on screen</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="color">Color</param>
        public static void DrawFilledRect(float x, float y, float w, float h, SDL_Color color)
        {
            if (float.IsNaN(x) || float.IsNaN(y) || float.IsNaN(w) || float.IsNaN(h) || float.IsInfinity(x) || float.IsInfinity(y) || float.IsInfinity(w) || float.IsInfinity(h)) return;

            if (SDL_SetRenderDrawColor(MonteEngine.SDL_Renderer, color.r, color.g, color.b, color.a) != 0)
                Debug.Log($"There was an issue setting renderer draw color. {SDL_GetError()}");

            SDL_FRect rect = new()
            {
                x = x,
                y = y,
                w = w,
                h = h
            };

            if (SDL_RenderFillRectF(MonteEngine.SDL_Renderer, ref rect) != 0)
                Debug.Log($"There was an issue drawing a fRect. {SDL_GetError()}");
        }

        public static void RenderText(string text, IntPtr font, SDL_FPoint position, SDL_Color color, SDL_RendererFlip renderFlip = SDL_RendererFlip.SDL_FLIP_NONE)
        {
            if (TTF_SizeText(font, text, out int w, out int h) != 0)
                Debug.Log(SDL_GetError());

            SDL_Rect fsrc = new(0, 0, w, h);
            SDL_FRect fdst = new(
                position.x,
                position.y,
                w,
                h
            );

            IntPtr fSurf = TTF_RenderText_Blended(font, text, color);
            IntPtr fTex = SDL_CreateTextureFromSurface(MonteEngine.SDL_Renderer, fSurf);
            SDL_FPoint rotOrigin = new()
            {
                x = fdst.w / 2,
                y = fdst.h / 2
            };
            if (SDL_SetTextureAlphaMod(fTex, SDL_WHITE.a) != 0)
                Debug.Log(SDL_GetError());
            if (SDL_SetTextureColorMod(fTex, SDL_WHITE.r, SDL_WHITE.g, SDL_WHITE.b) != 0)
                Debug.Log(SDL_GetError());
            if (SDL_RenderCopyExF(MonteEngine.SDL_Renderer, fTex, ref fsrc, ref fdst, 0, ref rotOrigin, renderFlip) != 0)
                Debug.Log($"There was an issue drawing texture. {SDL_GetError()}");

            SDL_FreeSurface(fSurf);
            SDL_DestroyTexture(fTex);
        }

        /// <summary>
        /// Normalize a SDL point in screenspace by making 0,0 center of screen and x+ right y- up
        /// </summary>
        /// <param name="a">point to center</param>
        /// <returns></returns>
        public static SDL_FPoint FNormalizeScreenSpace(SDL_FPoint a)
        {
            return new SDL_FPoint()
            {
                x = a.x + RendererSettings.VirtualWidth / 2,
                y = a.y + RendererSettings.VirtualHeight / 2
            };
        }
    }
}