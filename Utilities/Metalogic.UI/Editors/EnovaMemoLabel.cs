using System;
using System.Drawing;
using System.Windows.Forms;
using Metaphase.Editors;

namespace Metalogic.UI.Editors
{
    public class EnovaMemoLabel : EnovaMemoEdit
    {
        public EnovaMemoLabel()
        {
            Properties.AppearanceReadOnly.BackColor = DevExpress.LookAndFeel.LookAndFeelHelper.GetSystemColor(DevExpress.LookAndFeel.UserLookAndFeel.Default.ActiveLookAndFeel, SystemColors.Control);
            Properties.ReadOnly = true;
        }

        private readonly GlassPanel _glassPanel = new GlassPanel() { Dock = DockStyle.Fill };

        private bool _doNotHandle;
        protected override void OnEnabledChanged(EventArgs e)
        {
            if (_doNotHandle)
            {
                _doNotHandle = false;
                base.OnEnabledChanged(e);
                return;
            }

            _doNotHandle = true;
            if (Enabled)
            {
                Controls.Remove(_glassPanel);
            }
            else
            {
                Controls.Add(_glassPanel);
                _glassPanel.BringToFront();
            }
            Enabled = true;
            Refresh();
        }
    }
}
