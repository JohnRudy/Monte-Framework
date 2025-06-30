using System.Numerics;
using Monte;
using Monte.Components;
using Monte.Core;
using Monte.Settings;

using static SDL.SDL_pixels;
using static SDL.SDL_rect;
using static SDL.Constants;
using static SDL.SDL_mouse;

using Monte.Extensions;

public static class Debug
{
    static IntPtr fontPtr = IntPtr.Zero;
    internal static bool AllowDebugMessages
    {
        get => DebugSettings.ShowDebugMessages;
    }

    public static void Initialize()
    {
        Stream fontStream = ContentManager.GetResourceStream("SimpleStuffFixed.ttf");
        fontPtr = ContentManager.StreamToFont(fontStream, 12);
    }

    public static void Log(object message)
    {
        if (message == null) return;

        if (string.IsNullOrEmpty(message.ToString())) return;

        if (AllowDebugMessages)
            Console.WriteLine(message.ToString());
    }

    public static void Render()
    {
        if (DebugSettings.ShowColliders)
            RenderColliders();

        if (DebugSettings.DebugOrigin)
            RenderWorldOrigin();

        if (DebugSettings.CursorDebug)
            CursorDebug();

        if (DebugSettings.FrameInfo)
            FrameInfo();
    }

    private static void FrameInfo()
    {
        Renderer.RenderText(
            Time.CurrentFPS.ToString(),
            fontPtr,
            new SDL_FPoint(RendererSettings.VirtualWidth - 60, 12),
            SDL_WHITE
        );
        Renderer.RenderText(
            Math.Round(Time.DeltaTime, 4).ToString(),
            fontPtr,
            new SDL_FPoint(RendererSettings.VirtualWidth - 60, 24 + 2),
            SDL_WHITE
        );
        Renderer.RenderText(
            Time.LastFrameTook.ToString(),
            fontPtr,
            new SDL_FPoint(RendererSettings.VirtualWidth - 60, 36 + 2),
            SDL_WHITE
        );
    }

    static Vector2 lastPos = Vector2.Zero;
    private static void CursorDebug()
    {
        string mousePos = Input.MouseLogicalPosition.ToVector2().ToString();
        Renderer.RenderText(
            mousePos,
            fontPtr,
            new SDL_FPoint(0, 0),
            SDL_WHITE
        );

        Renderer.RenderText(
            Input.MouseWindowPosition.ToVector2().ToString(),
            fontPtr,
            new SDL_FPoint(0, 14),
            SDL_WHITE
        );

        Renderer.RenderText(
            Input.MouseWorldPosition.ToVector2().ToString(),
            fontPtr,
            new SDL_FPoint(0, 28),
            SDL_WHITE
        );

        if (Input.GetMouseButtonUp(SDL_BUTTON_RIGHT))
        {
            lastPos = Input.MouseLogicalPosition;
        }

        if (Input.GetMouseButton(SDL_BUTTON_LEFT))
        {
            SDL_FPoint start = new(
                lastPos.X,
                lastPos.Y
            );
            SDL_FPoint end = new(
                Input.MouseLogicalPosition.x,
                Input.MouseLogicalPosition.y
            );
            string dist = ((int)Vector2.Distance(lastPos, Input.MouseLogicalPosition)).ToString();
            Renderer.DrawLine(start, end, SDL_BLUE);
            Renderer.RenderText(dist, fontPtr, new SDL_FPoint(0, 16), SDL_WHITE);

            string XDIF = (end.x - start.x).ToString();
            string YDIF = (end.y - start.y).ToString();

            Renderer.RenderText($"X: {XDIF}", fontPtr, Input.MouseLogicalPosition.ToVector2() + new SDL_Point(10, 0), SDL_WHITE, SDL.SDL_render.SDL_RendererFlip.SDL_FLIP_NONE);
            Renderer.RenderText($"Y: {YDIF}", fontPtr, Input.MouseLogicalPosition.ToVector2() + new SDL_Point(10, 14), SDL_WHITE, SDL.SDL_render.SDL_RendererFlip.SDL_FLIP_NONE);
        }
    }

    private static void RenderWorldOrigin()
    {
        SDL_FPoint origin = Renderer.FNormalizeScreenSpace(new(0 - Camera.Transform.Position.X, 0 - Camera.Transform.Position.Y));
        SDL_FPoint right = Renderer.FNormalizeScreenSpace(new(10 - Camera.Transform.Position.X, 0 - Camera.Transform.Position.Y));
        SDL_FPoint up = Renderer.FNormalizeScreenSpace(new(0 - Camera.Transform.Position.X, 10 - Camera.Transform.Position.Y));

        Renderer.DrawLine(origin, right, new(255, 0, 0, 255));
        Renderer.DrawLine(origin, up, new(0, 255, 0, 255));
        Renderer.DrawFilledRect(origin.x - 1, origin.y - 1, 2, 2, new(255, 255, 0, 255));
    }

    private static void RenderColliders()
    {
        List<RectangleCollider> rectangs = [];
#pragma warning disable
        SceneManager.CurrentScene?.Behaviours.ForEach(x => rectangs.AddRange(x.GetComponentsOfType<RectangleCollider>().ToList().Where(x => x.Parent.Enabled == true && x.Enabled == true)));
#pragma warning restore

        foreach (RectangleCollider rc in rectangs)
        {
            SDL_FRect cam = Camera.GetDestinationFRect(rc.WorldBoundingBox);
            Renderer.DrawFRect(cam, new SDL_Color(0, 255, 0, 255));

            SDL_FPoint center = Camera.GetDestinationFPoint(rc.WorldCenter);
            Renderer.DrawFilledRect(
                center.x - 1,
                center.y - 1,
                2, 2,
                new SDL_Color(255, 255, 0, 255)
            );
        }
    }
}