namespace Monte.Rendering
{
    public class TTFFont
    {
        readonly string _file = "";

        public string File => _file;
        internal int PtSize;
        internal IntPtr Font;

        public TTFFont(string file, int ptSize, IntPtr? font = null)
        {
            _file = file;
            PtSize = ptSize;

            if (font is null)
            {
                Debug.Log("No font given");
                ContentManager.LoadFont(file, ptSize, out IntPtr _f);
                Font = _f;
            }
            else
                Font = (IntPtr)font;
        }
    }
}