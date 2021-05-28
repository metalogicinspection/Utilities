using System.Windows.Forms;

namespace Metalogic.UI.Editors
{
    public class GlassPanel : Panel
    {
        //need to add a class to cover the title area...since the color for the disabled control does not look good
        //so make a glass to cover it
        //http://www.c-sharpcorner.com/UploadFile/Nildo%20Soares%20de%20Araujo/TransparentControls11152005074108AM/TransparentControls.aspx
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // do nothing
        }
    }
}
