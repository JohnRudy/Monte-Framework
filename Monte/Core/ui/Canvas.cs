using Monte.Abstractions;
using Monte.Interfaces;
using static SDL2.SDL;


namespace Monte.UI
{
    public class Canvas : Entity
    {
        public override void OnInitialize() { }

       

        public override void OnUpdate()
        {
            List<IUi> uiElements = Components.OfType<IUi>().ToList();

            foreach (IUi element in uiElements)
            {
                bool isWithin = Input.IsMouseWithinLogical(element.InteractionArea);
                bool isPressed = Input.GetMouseButton(SDL_BUTTON_LEFT);

                element.Select(isWithin);
                element.Pressed(isWithin && isPressed);
            }
        }
    }
}