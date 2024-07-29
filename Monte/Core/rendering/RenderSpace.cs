namespace Monte.Rendering
{
    public enum RenderSpace
    {
        /// <summary>
        /// World space layer, first to be rendered
        /// </summary>
        WORLD,

        /// <summary>
        /// UI layer will be rendered ontop of screenspace and world
        /// </summary>
        UI,

        /// <summary>
        /// Screen space, in world. Renders ontop World but under UI
        /// </summary>
        SCREEN,
    }
}