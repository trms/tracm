using System;
using System.Windows.Forms;
using System.Drawing;

namespace tracm.Queue
{
	public class DataGridViewDisableButtonColumn : DataGridViewButtonColumn
	{
		public DataGridViewDisableButtonColumn()
		{
			this.CellTemplate = new DataGridViewDisableButtonCell();
		}
	}

	public class DataGridViewDisableButtonCell : DataGridViewButtonCell
	{
		protected override void Paint(Graphics graphics,
			Rectangle clipBounds, Rectangle cellBounds, int rowIndex,
			DataGridViewElementStates elementState, object value,
			object formattedValue, string errorText,
			DataGridViewCellStyle cellStyle,
			DataGridViewAdvancedBorderStyle advancedBorderStyle,
			DataGridViewPaintParts paintParts)
		{
			if(value.GetType() == typeof(string) && String.IsNullOrEmpty((string)value))
			{
				// Draw the cell background, if specified.
				if ((paintParts & DataGridViewPaintParts.Background) ==
					DataGridViewPaintParts.Background)
				{
					using (SolidBrush cellBackground = new SolidBrush(cellStyle.BackColor))
					{
						graphics.FillRectangle(cellBackground, cellBounds);
						cellBackground.Dispose();
					}
				}

				// Draw the cell borders, if specified.
				if ((paintParts & DataGridViewPaintParts.Border) ==
					DataGridViewPaintParts.Border)
				{
					PaintBorder(graphics, clipBounds, cellBounds, cellStyle,
						advancedBorderStyle);
				}
			}
			else
			{
				// The button cell is enabled, so let the base class 
				// handle the painting.
				base.Paint(graphics, clipBounds, cellBounds, rowIndex,
					elementState, value, formattedValue, errorText,
					cellStyle, advancedBorderStyle, paintParts);
			}
		}
	}
}
