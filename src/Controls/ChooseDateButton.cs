// epi: SyncroSim Base Package for modeling epidemic infections and deaths.
// Copyright © 2007-2020 Apex Resource Management Solutions Ltd. (ApexRMS). All rights reserved.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SyncroSim.Epi
{
    class ChooseDateButton : Panel
    {
        protected override void OnPaint(PaintEventArgs pevent)
        {
            base.OnPaint(pevent);

            if (this.Enabled)
            {
                pevent.Graphics.DrawImage(Properties.Resources.DatePicker, new Point(0, 2));
            }
            else
            {
                ControlPaint.DrawImageDisabled(pevent.Graphics, Properties.Resources.DatePicker, 0, 2, Color.White);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Default;
        }
    }
}
