using System.Text;
using Monte.Abstractions;
using Monte.Components;
using Monte.Lib;
using Monte.Scenes;
using Monte.UI;
using static SDL2.SDL;


namespace Monte
{
    public class Debug
    {
        public static void Log(object[] args)
        {
            StringBuilder sb = new();
#if DEBUG
            args.ToList().ForEach(x => sb.Append(x.ToString()).Append(' '));
            Console.WriteLine(sb.ToString());
#endif
        }

        public static void Log(object args)
        {
#if DEBUG
            Console.WriteLine(args.ToString());
#endif
        }

        internal static void RenderUpdate(IntPtr SDLRenderer)
        {
            if (!DebugSettings.DrawDebugGizmos) return;

            foreach (Entity ent in SceneManager.CurrentScene.entities)
            {
                if (ent is Canvas)
                {
                    var buttons = ent.GetComponentsOfType<Button>();
                    foreach (var button in buttons)
                    {
                        SDL_Rect buttonRect = button.InteractionArea;
                        _ = SDL_SetRenderDrawColor(SDLRenderer, 255, 0, 0, 255);
                        _ = SDL_RenderDrawRect(SDLRenderer, ref buttonRect);
                    }
                }

                // Origin points of entities
                SDL_Rect origin = new() { x = (int)ent.Transform.Position.X - 1, y = (int)ent.Transform.Position.Y - 1, w = 2, h = 2 };
                SDL_Rect originTransformed = SceneManager.CurrentScene.Camera.TransfromDSTRect(origin);
                _ = SDL_SetRenderDrawColor(SDLRenderer, 255, 255, 0, 255);
                _ = SDL_RenderFillRect(SDLRenderer, ref originTransformed);

                List<RectangleCollider> rcs = ent.GetComponentsOfType<RectangleCollider>().ToList();
                List<CircleCollider> ccs = ent.GetComponentsOfType<CircleCollider>().ToList();
                List<PolygonCollider> pcs = ent.GetComponentsOfType<PolygonCollider>().ToList();

                List<Collider> allColliders = new();
                allColliders.AddRange(rcs);
                allColliders.AddRange(ccs);
                allColliders.AddRange(pcs);

                _ = SDL_SetRenderDrawColor(SDLRenderer, 0, 255, 0, 255);

                // WorldBoundingBoxes (excluding RectangleColliders)
                foreach (CircleCollider cc in ccs)
                {
                    SDL_Rect transformed = SceneManager.CurrentScene.Camera.TransfromDSTRect(cc.WorldBoundingBox);
                    _ = SDL_RenderDrawRect(SDLRenderer, ref transformed);
                }
                foreach (PolygonCollider pc in pcs)
                {
                    SDL_Rect transformed = SceneManager.CurrentScene.Camera.TransfromDSTRect(pc.WorldBoundingBox);
                    _ = SDL_RenderDrawRect(SDLRenderer, ref transformed);
                }

                // Rectangle Colliders
                foreach (RectangleCollider rc in rcs)
                {
                    SDL_Rect transformed = SceneManager.CurrentScene.Camera.TransfromDSTRect(rc.WorldBoundingBox);
                    _ = SDL_RenderDrawRect(SDLRenderer, ref transformed);
                }

                // CircleCollider
                foreach (CircleCollider cc in ccs)
                {
                    SDL_Point screenCenter = SceneManager.CurrentScene.Camera.TransformPoint(cc.WorldCenter);
                    Renderer.RenderCircle(SDLRenderer, screenCenter, cc.Radius, new SDL_Color() { r = 0, g = 255, b = 0, a = 255 });
                }

                // PolygonCollider
                foreach (PolygonCollider pc in pcs)
                {
                    _ = SDL_SetRenderDrawColor(SDLRenderer, 255, 0, 125, 255);
                    SDL_Rect pcsr = new() { x = pc.ShapeMeanCenter.x - 1, y = pc.ShapeMeanCenter.y - 1, w = 2, h = 2 };
                    SDL_Rect tpcsr = SceneManager.CurrentScene.Camera.TransfromDSTRect(pcsr);
                    _ = SDL_RenderFillRect(SDLRenderer, ref tpcsr);

                    foreach (Polygon p in pc.Polygons)
                    {
                        _ = SDL_SetRenderDrawColor(SDLRenderer, 0, 255, 255, 255);
                        SDL_Point pcc = p.PolygonCenter();
                        SDL_Rect tr = new() { x = pcc.x - 1, y = pcc.y - 1, w = 2, h = 2 };
                        SDL_Rect trsc = SceneManager.CurrentScene.Camera.TransfromDSTRect(tr);
                        _ = SDL_RenderFillRect(SDLRenderer, ref trsc);

                        SDL_Point transformed0 = SceneManager.CurrentScene.Camera.TransformPoint(p.Vertices[0]);
                        SDL_Point transformed1 = SceneManager.CurrentScene.Camera.TransformPoint(p.Vertices[1]);
                        SDL_Point transformed2 = SceneManager.CurrentScene.Camera.TransformPoint(p.Vertices[2]);
                        _ = SDL_SetRenderDrawColor(SDLRenderer, 0, 255, 0, 255);

                        _ = SDL_RenderDrawLine(SDLRenderer, transformed0.x, transformed0.y, transformed1.x, transformed1.y);
                        _ = SDL_RenderDrawLine(SDLRenderer, transformed1.x, transformed1.y, transformed2.x, transformed2.y);
                        _ = SDL_RenderDrawLine(SDLRenderer, transformed2.x, transformed2.y, transformed0.x, transformed0.y);
                    }
                }

                // PolygonCollider edges
                _ = SDL_SetRenderDrawColor(SDLRenderer, 0, 0, 255, 255);
                foreach (PolygonCollider pc in pcs)
                {
                    foreach (Edge p in pc.Edges)
                    {
                        SDL_Point transformed0 = SceneManager.CurrentScene.Camera.TransformPoint(p.a);
                        SDL_Point transformed1 = SceneManager.CurrentScene.Camera.TransformPoint(p.b);

                        _ = SDL_RenderDrawLine(SDLRenderer, transformed0.x, transformed0.y, transformed1.x, transformed1.y);
                    }
                }


                // All Collider worldcenters 
                if (allColliders.Count > 0)
                {
                    _ = SDL_SetRenderDrawColor(SDLRenderer, 255, 255, 0, 255);
                    foreach (Collider col in allColliders)
                    {
                        SDL_Rect center = new SDL_Rect() { x = col.WorldCenter.x - 1, y = col.WorldCenter.y - 1, w = 2, h = 2 };
                        SDL_Rect worldCenter = SceneManager.CurrentScene.Camera.TransfromDSTRect(center);
                        _ = SDL_RenderFillRect(SDLRenderer, ref worldCenter);
                    }
                }



            }

            if (!DebugSettings.DisplayInformation) return;


        }
    }
}