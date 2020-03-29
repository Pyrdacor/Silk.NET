namespace Silk.NET.UI
{
    using Input.Common;

    public class KeyEventArgs : PropagatedEventArgs
    {
        public Key Key { get; }
        public KeyModifiers KeyModifiers { get; }

        internal KeyEventArgs(Key key, KeyModifiers modifiers = KeyModifiers.None)
        {
            Key = key;
            KeyModifiers = modifiers;
        }
    }

    public delegate void KeyEventHandler(Control sender, KeyEventArgs args);
}