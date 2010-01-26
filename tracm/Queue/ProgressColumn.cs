using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;

namespace tracm.Queue
{
	public class DataGridViewProgressColumn : DataGridViewImageColumn
	{
		public DataGridViewProgressColumn()
		{
			CellTemplate = new DataGridViewProgressCell();
		}
	}

	public class DataGridViewProgressCell : DataGridViewImageCell
	{
		// Used to make custom cell consistent with a DataGridViewImageCell
		private static Image emptyImage;

		static DataGridViewProgressCell()
		{
			emptyImage = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
		}

		public DataGridViewProgressCell()
		{
			this.ValueType = typeof(int);
		}

		// Method required to make the Progress Cell consistent with the default Image Cell. 
		// The default Image Cell assumes an Image as a value, although the value of the Progress Cell is an int.
		protected override object GetFormattedValue(object value,
							int rowIndex, ref DataGridViewCellStyle cellStyle,
							TypeConverter valueTypeConverter,
							TypeConverter formattedValueTypeConverter,
							DataGridViewDataErrorContexts context)
		{
			return emptyImage;
		}

		protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
		{
			int progressVal = 0;
			string label = String.Empty;

			// get progress values
			if (value != null && value.GetType() == typeof(ProgressState))
			{
				ProgressState s = (ProgressState)value;
				progressVal = s.CurrentValue;
				label = s.CurrentState;
				if (s.ShowProgressBar == false || String.IsNullOrEmpty(label) == false)
					progressVal = 0;
				if (s.HasError)
					this.ErrorText = s.ErrorText;
			}

			// Draws the cell grid
			base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

			// draw progress bar
			float percentage = ((float)progressVal / 100.0f); // Need to convert to float before division; otherwise C# returns int which is 0 for anything but 100%.
			if (percentage > 0.0)
			{
				using (Brush highlightBrush = new SolidBrush(SystemColors.Highlight))
					g.FillRectangle(highlightBrush, cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32((percentage * cellBounds.Width - 4)), cellBounds.Height - 4);
			}

			// draw text
			if (String.IsNullOrEmpty(label) == false)
			{
				using (Brush foreColorBrush = new SolidBrush(cellStyle.ForeColor))
					g.DrawString(label, cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 4);
			}
		}
	}
}