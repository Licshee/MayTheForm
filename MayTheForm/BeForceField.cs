using System;
using System.Drawing;
using System.Windows.Forms;

namespace MayTheForm
{
    public enum TransparencyMode
    {
        Auto,
        Opaque,
        LayeredHack,
        NoRedirectionBitMap,
    }

    public class BeForceField : FixTheForm
    {
        internal const int WS_EX_NOREDIRECTIONBITMAP = 0x00200000;

        public TransparencyMode TransparencyMode { get; }

        public BeForceField(TransparencyMode mode = TransparencyMode.Auto)
        {
            //========//========//========//========//========//========//========//========
            // HACK: the transparency key glitch is no more if the program is running on a version
            //       of Windows above 8 while claiming itself is compatible with that version
            //       fortunately with Windows 8 we can do the trick via WS_EX_NOREDIRECTIONBITMAP
            //========//========//========//========//========//========//========//========
            switch (mode)
            {
                case TransparencyMode.Auto:
                    var ver = Environment.OSVersion.Version;
                    // NT 6.1 = Windows 7, anything above this means Windows 8 & beyond
                    mode = (ver.Major > 6 || (ver.Major == 6 && ver.Minor > 1)) ? TransparencyMode.NoRedirectionBitMap : TransparencyMode.LayeredHack;
                    break;
                case TransparencyMode.Opaque:
                case TransparencyMode.LayeredHack:
                case TransparencyMode.NoRedirectionBitMap:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }

            TransparencyMode = mode;

            //========//========//========//========//========//========//========//========
            // HACK: there is a glitch in GDI so that having R & B bytes be different value will 
            //       cause the window rendering transparent but actually not transparent for hit-testing
            //       so we are exploiting this glitch to do what we want
            //========//========//========//========//========//========//========//========
            if (TransparencyMode == TransparencyMode.LayeredHack)
            {
                var bytes = new byte[3];
                var rnd = new Random();
                rnd.NextBytes(bytes);
                if (bytes[0] == bytes[2])
                    bytes[0]++;
                var color = Color.FromArgb(bytes[0], bytes[1], bytes[2]);
                BackColor = TransparencyKey = color;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var p = base.CreateParams;
                if (TransparencyMode == TransparencyMode.NoRedirectionBitMap)
                    p.ExStyle |= WS_EX_NOREDIRECTIONBITMAP;
                return p;
            }
        }
    }
}
