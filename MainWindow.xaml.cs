using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AssignmentD
{
    #region Enum

    public enum Shapes
    {
        Selection,
        Rectangle,
        Ellipse,
        Line,
        Draw
    } 

    #endregion

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            myResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("Resources/MainWindowResources.xaml",
                                  UriKind.RelativeOrAbsolute)
            };

            selectedButtonColor = Color.FromRgb(207, 230, 253);
        }

        #region Constants
        
        private const double strokeThickness = 2;
        private const string resourceKey = "DesignerItemTemplate";

        #endregion

        #region Members

        private Point startPoint;
        private Shape shape;
        private readonly ResourceDictionary myResourceDictionary;
        private ControlTemplate previousTemplate;
        private Color selectedButtonColor;
        private bool isSelected;

        #endregion

        #region Properties

        public Shapes SelectedShape { get; set; }

        public Shape SelectedShapeCanvas { get; set; }

        #endregion


        #region Event Handlers

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!(sender is Canvas canvas))
                    return;

                switch (SelectedShape)
                {
                    case Shapes.Selection:
                        if (SelectedShapeCanvas != null && SelectedShapeCanvas.Parent is ContentControl contentControl)
                        {
                            contentControl.Template = previousTemplate;
                            isSelected = false;
                        }
                        break;
                    case Shapes.Rectangle:
                        shape = new Rectangle();
                        IntializeShape(e, canvas);
                        break;
                    case Shapes.Ellipse:
                        shape = new Ellipse();
                        IntializeShape(e, canvas);
                        break;
                    case Shapes.Line:
                        shape = new Line();
                        IntializeShape(e, canvas);
                        break;
                    case Shapes.Draw:
                        if (e.ButtonState == MouseButtonState.Pressed)
                            startPoint = e.GetPosition(canvas);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> Canvas_MouseDown", ex.Message);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Released 
                    || (shape == null && SelectedShape != Shapes.Selection && SelectedShape != Shapes.Draw))
                    return;

                Point pos = e.GetPosition(canvas);

                switch (SelectedShape)
                {
                    case Shapes.Selection:
                        SelectShape(pos);
                        break;
                    case Shapes.Rectangle:
                        MakeRectangleEllipse(pos);
                        break;
                    case Shapes.Ellipse:
                        MakeRectangleEllipse(pos);
                        break;
                    case Shapes.Line:
                        MakeLine(pos);
                        break;
                    case Shapes.Draw:
                        MakeShape(pos, e.LeftButton);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> Canvas_MouseMove", ex.Message);
            }
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                shape = null;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> Canvas_MouseUp", ex.Message);
            }
        }

        private void ShapeButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender == null)
                {
                    return;
                }
                SelectedShape = (Shapes)(sender as Button).Tag;

                btnRectangle.Background = SelectedShape == Shapes.Rectangle
                                          ? new SolidColorBrush(selectedButtonColor) : new SolidColorBrush(Colors.Transparent);
                btnEllipse.Background = SelectedShape == Shapes.Ellipse
                                        ? new SolidColorBrush(selectedButtonColor) : new SolidColorBrush(Colors.Transparent);
                btnLine.Background = SelectedShape == Shapes.Line
                                     ? new SolidColorBrush(selectedButtonColor) : new SolidColorBrush(Colors.Transparent);
                btnSelect.Background = SelectedShape == Shapes.Selection
                                       ? new SolidColorBrush(selectedButtonColor) : new SolidColorBrush(Colors.Transparent);
                btnDraw.Background = SelectedShape == Shapes.Draw
                                       ? new SolidColorBrush(selectedButtonColor) : new SolidColorBrush(Colors.Transparent);

                if (SelectedShapeCanvas != null && SelectedShapeCanvas.Parent is ContentControl contentControl)
                {
                    contentControl.Template = previousTemplate;
                    isSelected = false;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> ShapeButton_Click", ex.Message);
            }

        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (canvas == null)
                {
                    return;
                }
                canvas.Children.Clear();
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> ClearButton_Click", ex.Message);
            }
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            try
            {
                if (SelectedShapeCanvas == null)
                {
                    return;
                }

                if (isSelected)
                {
                    Color selectedColor = Color.FromRgb(0, 0, 0);
                    if (colorPicker.SelectedColor.HasValue)
                    {
                        selectedColor = colorPicker.SelectedColor.Value;
                    }
                    SelectedShapeCanvas.Stroke = new SolidColorBrush(selectedColor);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> ColorPicker_SelectedColorChanged", ex.Message);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Intialize Rectangle, Ellipse and circle
        /// </summary>
        /// <param name="e"></param>
        /// <param name="canvas"></param>
        private void IntializeShape(MouseButtonEventArgs e, Canvas canvas)
        {
            try
            {
                ContentControl content = new ContentControl();
                startPoint = e.GetPosition(canvas);

                Color selectedColor = Color.FromRgb(255, 255, 255);
                if (colorPicker.SelectedColor.HasValue)
                {
                    selectedColor = colorPicker.SelectedColor.Value;
                }

                shape.Stroke = new SolidColorBrush(selectedColor);
                shape.StrokeThickness = strokeThickness;

                content.Content = shape;

                if (!(shape is Line))
                {
                    Canvas.SetLeft(content, startPoint.X);
                    Canvas.SetTop(content, startPoint.Y);
                }

                canvas.Children.Add(content);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> IntializeShape", ex.Message);
            }
        }

        /// <summary>
        /// Free hand drawing logic
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="leftButtonState"></param>
        private void MakeShape(Point pos, MouseButtonState leftButtonState)
        {
            try
            {
                if (leftButtonState == MouseButtonState.Pressed)
                {
                    Line line = new Line();
                    Color selectedColor = Color.FromRgb(255, 255, 255);
                    if (colorPicker.SelectedColor.HasValue)
                    {
                        selectedColor = colorPicker.SelectedColor.Value;
                    }
                    line.Stroke = new SolidColorBrush(selectedColor);
                    line.StrokeThickness = strokeThickness;
                    line.X1 = startPoint.X;
                    line.Y1 = startPoint.Y;
                    line.X2 = pos.X;
                    line.Y2 = pos.Y;

                    startPoint = pos;

                    canvas.Children.Add(line);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> MakeShape", ex.Message);
            }
        }

        /// <summary>
        /// Setting properties of Rectangle, ellipse according to position
        /// </summary>
        /// <param name="pos"></param>
        private void MakeRectangleEllipse(Point pos)
        {
            try
            {
                double horizontalStart = Math.Min(pos.X, startPoint.X);
                double verticalStart = Math.Min(pos.Y, startPoint.Y);

                double deltaHorizontal = Math.Max(pos.X, startPoint.X) - horizontalStart;
                double deltaVertical = Math.Max(pos.Y, startPoint.Y) - verticalStart;

                shape.Width = deltaHorizontal;
                shape.Height = deltaVertical;

                Canvas.SetLeft(shape, horizontalStart);
                Canvas.SetTop(shape, verticalStart);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> MakeRectangleEllipse", ex.Message);
            }
        }

        /// <summary>
        /// Setting properties of Line according to position
        /// </summary>
        /// <param name="pos"></param>
        private void MakeLine(Point pos)
        {
            try
            {
                (shape as Line).X1 = startPoint.X;
                (shape as Line).Y1 = startPoint.Y;
                (shape as Line).X2 = pos.X;
                (shape as Line).Y2 = pos.Y;

                Canvas.SetLeft(shape, startPoint.X);
                Canvas.SetTop(shape, startPoint.Y);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> MakeLine", ex.Message);
            }
        }

        /// <summary>
        /// Select shape from canvas logic
        /// </summary>
        /// <param name="pos"></param>
        private void SelectShape(Point pos)
        {
            try
            {
                HitTestResult SelectedCanvasItem = VisualTreeHelper.HitTest(canvas, pos);
                if (SelectedCanvasItem.VisualHit is Shape selectedShape
                    && selectedShape.Parent != null)
                {
                    ControlTemplate template = myResourceDictionary[resourceKey] as ControlTemplate;
                    SelectedShapeCanvas = selectedShape;
                    if (selectedShape.Parent is ContentControl contentControl)
                    {
                        previousTemplate = contentControl.Template;
                        contentControl.Template = template;
                        isSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteToLog("MainWindow.xaml.cs -> SelectShape", ex.Message);
            }
        } 

        #endregion
    }
}
