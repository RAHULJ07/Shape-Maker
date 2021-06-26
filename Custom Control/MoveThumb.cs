using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace AssignmentD
{
    public class MoveThumb: Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Thumb thumb = e.Source as Thumb;
            double left = Canvas.GetLeft(thumb) + e.HorizontalChange;
            double top = Canvas.GetTop(thumb) + e.VerticalChange;
            Canvas.SetLeft(thumb, left);
            Canvas.SetTop(thumb, top);
            if (this.DataContext is Control item)
            {
                if ((item as ContentControl).Content is Line line)
                {
                    line.X2 += e.HorizontalChange;
                    line.Y2 += e.VerticalChange;
                    line.X1 += e.HorizontalChange;
                    line.Y1 += e.VerticalChange;
                }
                else
                {
                    left = Canvas.GetLeft(item);
                    top = Canvas.GetTop(item);

                    Canvas.SetLeft(item, left + e.HorizontalChange);
                    Canvas.SetTop(item, top + e.VerticalChange);
                }
            }
        }
    }
}
