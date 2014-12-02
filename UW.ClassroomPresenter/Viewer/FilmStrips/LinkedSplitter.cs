// $Id: LinkedSplitter.cs 924 2006-04-02 08:13:41Z cmprince $

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Windows.Forms;

using System.Runtime.InteropServices;

using UW.ClassroomPresenter.Model;
using UW.ClassroomPresenter.Model.Presentation;

namespace UW.ClassroomPresenter.Viewer.FilmStrips {
    /// <summary>
    /// A utility class that keeps its <see cref="Dock"/> property in sync
    /// with the associated <see cref="FilmStrip"/>.
    /// </summary>
    internal class LinkedSplitter : Splitter {
        private readonly Control m_Linked;
        private bool m_Disposed;

        public LinkedSplitter(Control linked) {
            this.m_Linked = linked;

            // Make the splitter very wide so it's easier to grab with a stylus.
            this.Name = "LinkedSplitter";
            this.Width *= 4;

            this.MinExtra *= 16;
            this.MinSize *= 3;

            this.m_Linked.DockChanged += new EventHandler(this.HandleLinkedDockChanged);
            this.m_Linked.VisibleChanged += new EventHandler(this.HandleLinkedVisibleChanged);
            this.m_Linked.ParentChanged += new EventHandler(this.HandleLinkedParentChanged);

            // Initialize the linked state.
            this.HandleLinkedDockChanged(this, EventArgs.Empty);
            this.HandleLinkedVisibleChanged(this, EventArgs.Empty);
            // Do not bother calling HandleLinkedParentChanged, since this.Parent hasn't set initialized yet.
        }

        protected override void Dispose(bool disposing) {
            if(this.m_Disposed) return;
            try {
                if(disposing) {
                    this.m_Linked.DockChanged -= new EventHandler(this.HandleLinkedDockChanged);
                    this.m_Linked.VisibleChanged -= new EventHandler(this.HandleLinkedVisibleChanged);
                    this.m_Linked.ParentChanged -= new EventHandler(this.HandleLinkedParentChanged);
                }
            } finally {
                base.Dispose(disposing);
            }
            this.m_Disposed = true;
        }

        private void HandleLinkedDockChanged(object sender, EventArgs args) {
            // Keep the dock in sync with the parent.
            this.Dock = this.m_Linked.Dock;
        }

        private void HandleLinkedVisibleChanged(object sender, EventArgs args) {
            // Keep the visible state in sync with the parent.
            this.Visible = this.m_Linked.Visible;
        }

        protected override void OnVisibleChanged(EventArgs e) {
            if(this.Visible) {
                // If this.Visible and m_Linked.Visible were both false when the parent control was first displayed,
                // the Z-orders won't be correct anymore.  So whenever the control is made visible, we need to
                // readjust the Z-order so that the splitter is always just "above" m_Linked.
                this.UpdateLinkedZOrder();
            }

            base.OnVisibleChanged(e);
        }

        private void HandleLinkedParentChanged(object sender, EventArgs args) {
            this.UpdateLinkedZOrder();
        }

        private void UpdateLinkedZOrder() {
            if(this.m_Linked.Parent != this.Parent)
                return;

            if(this.Parent != null) {
                int index = this.Parent.Controls.GetChildIndex(this.m_Linked, false);

                // Since both controls have the same parent, m_Linked must be a child of this.Parent.
                Debug.Assert(index >= 0);

                // Reinsert the LinkedSplitter to the index *before* m_Linked in the parent's control list.
                // This makes the LinkedSplitter *higher* than m_Linked in the Z-order,
                // so it will be docked farther away from the edge of the parent,
                // and thus the splitter will be "attached" to m_Linked.
                this.Parent.Controls.SetChildIndex(this, index - 1);
            }
        }
    }
}
