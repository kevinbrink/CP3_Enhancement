// $Id: SheetModel.cs 1614 2008-05-30 22:29:29Z cmprince $

using System;
using System.Drawing;

namespace UW.ClassroomPresenter.Model.Presentation {
    [Serializable]
    public abstract class SheetModel : PropertyPublisher, IComparable {

        private readonly SheetDisposition m_Disposition;
        public SheetDisposition Disposition {
            get { return this.m_Disposition; }
        }

        // Published properties:
        private Rectangle m_Bounds;
        [Published] public Rectangle Bounds {
            get { return this.GetPublishedProperty("Bounds", ref this.m_Bounds); }
            set { this.SetPublishedProperty("Bounds", ref this.m_Bounds, value); }
        }

        /// <summary>
        /// Represents how high this layer is, higher layers are displayed above lower layers
        /// </summary>
        private readonly int m_Height;
        public int Height {
            get { return this.m_Height; }
        }

        private readonly Guid m_Id;
        public Guid Id {
            get { return this.m_Id; }
        }

        public SheetModel(Guid id, int height) : this(id, SheetDisposition.All, height) {}

        public SheetModel(Guid id, SheetDisposition disp, int height) : this(id, disp, Rectangle.Empty, height) {}

        public SheetModel(Guid id, SheetDisposition disp, Rectangle rect, int height) {
            this.m_Id = id;
            this.m_Bounds = rect;
            this.m_Disposition = disp;
            this.m_Height = height;
        }

        #region IComparable Members

        /// <summary>
        /// Comparison operator to determine the ordering of sheets
        /// Sheets are first ordered by type:
        /// Bottom--ImageSheetModels, Middle--TextSheetModels, Top--InkSheetModels
        /// Then within type they are ordered by height...highest to lowest 
        /// WITH the exception that slides of height 0 are always on top in their type class
        /// </summary>
        /// <param name="obj">The sheet to compare against</param>
        /// <returns>0 if the sheets belong at the same height, 1 if if belongs below, -1 if it belongs above</returns>
        public virtual int CompareTo(object obj) {
            if (!(obj is SheetModel)) {
                return -1;
            }
            int my_type = GetZType(this);
            int other_type = GetZType(obj);
            // Types are the same, need to 
            if (my_type == other_type) {
                // Same type need to sort by height
                // NOTE: 0 is always highest, then sorted in descending order
                if( this.Height == ((SheetModel)obj).Height )
                    return 0;
                else if( this.Height == 0 )
                    return 1;
                else if( this.Height < ((SheetModel)obj).Height )
                    return -1;
                else
                    return 1;
            } else if (my_type > other_type) {
                return 1;
            } else {
                /// my index less than other index
                return -1;
            }
        }
        /// <summary>
        /// Returns the type of sheet that we are comparing against
        /// This ensure the following drawing order: 
        /// Bottom--ImageSheetModels, Middle--TextSheetModels, Top--InkSheetModels
        /// </summary>
        /// <param name="obj">The object to get the sheet type of</param>
        /// <returns>Returns 0 if and ImageSheetModel, 1 if a TextSheetModel, 2 if an InkSheetModel</returns>
        public int GetZType(object obj) {
            int z_type = 0;
            if( obj is TextSheetModel || obj is QuickPollSheetModel ) {
                z_type = 1;
            } else if (obj is ImageSheetModel) {
                z_type = 0;
            } else if (obj is InkSheetModel) {
                z_type = 2;
            }
            return z_type;
        }

        #endregion
    }
}
