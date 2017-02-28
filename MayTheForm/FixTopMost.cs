using System.Windows.Forms;

namespace MayTheForm
{
    public class FixTopMost : Form
    {
        internal const int WS_EX_TOPMOST = 8;

        protected override CreateParams CreateParams
        {
            get
            {
                var p = base.CreateParams;
                // HACK: for mysterious reasons, TopMost won't work if a break point is triggered before showing the form
                if (TopMost)
                    p.ExStyle |= WS_EX_TOPMOST;
                //else
                //    p.ExStyle &= ~WS_EX_TOPMOST;
                return p;
            }
        }
    }
}
