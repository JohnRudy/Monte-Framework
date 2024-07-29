using Monte.Scenes;
using Monte.Abstractions;
using Monte.Rendering;
using System.Numerics;
using Monte.Tweening;
using System.Collections;
using static SDL2.SDL;
using Monte.Resource;


namespace Monte.Lib
{
    internal class MonteLogoManager : Entity
    {
        public SpriteRenderer sr;
        Tween? SpriteRendererColorTween;

        public MonteLogoManager()
        {
            sr = new SpriteRenderer(this);
            CoroutineHandler.StartCoroutine(Intro());
        }
        public override void OnUpdate()
        {
            if (SpriteRendererColorTween != null)
            {
                sr.Color = new SDL_Color() { r = sr.Color.r, b = sr.Color.b, g = sr.Color.g, a = (byte)Math.Round(SpriteRendererColorTween.Value) };
            }
        }

        public IEnumerator Intro()
        {
            List<Sprite> logos = new();
            if (EngineSettings.ShowEngineLogo)
            {
                MonteResource.LoadImage("MonteLogo.png", out IntPtr logoImage, Renderer.Instance.SDL_Renderer);
                logos.Add(
                    new Sprite(
                        file: "MonteLogo.png",
                        width: 256,
                        height: 128,
                        x: 0,
                        y: 0,
                        texture: logoImage

                ));
            }

            foreach (Tuple<string, int, int> s in EngineSettings.LogoFiles)
            {
                logos.Add(
                    new Sprite(
                        file: s.Item1,
                        width: s.Item2,
                        height: s.Item3,
                        x: 0,
                        y: 0
                    )
                );
            }

            float timing = EngineSettings.LogoDurationsInSeconds / 3;
            foreach (Sprite sprite in logos)
            {
                sr.Sprite = sprite;
                SpriteRendererColorTween = new Tween(0, 255, timing, Easing.QuartOut);
                yield return new WaitForSeconds(timing * 2);
                SpriteRendererColorTween = new Tween(255, 0, timing, Easing.QuartOut);
                yield return new WaitForSeconds(timing);
            }

            SceneManager.LoadScene(0);
        }
    }

    internal class MonteIntroScene : Scene
    {
        MonteLogoManager lm;
        public MonteIntroScene(int sceneIndex = int.MinValue, string sceneName = "MONTE_INTROSCENE") : base(sceneIndex, sceneName)
        {
            BackgroundColor = EngineSettings.LogoBackgroundColor;
            lm = new();
            entities.Add(lm);
        }

        public override void OnUpdate()
        {
            int x = 256 / 2;
            int y = 128 / 2;
            Camera.Transform.Position = new Vector3(x, y, 0);
        }
    }
}