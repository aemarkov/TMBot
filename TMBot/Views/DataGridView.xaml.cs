using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TMBot.Views
{
    /// <summary>
    /// DataGrid с несколькими дополнительными возможностями:
    ///  - подсветка текущей строки (текущая != выделенная)
    ///  - подсветка изменений    
    /// </summary>
    public partial class DataGridView : DataGrid
    {
        //Элемент ItemSource, чья строка должна подсвечиваться
        #region HighlightedRow
        public static DependencyProperty HighlightedRowProperty;

        public object HighlightedRow
        {
            get { return GetValue(HighlightedRowProperty); }
            set { SetValue(HighlightedRowProperty, value);}
        }
        #endregion

        //Цвет подсветки строки
        public static DependencyProperty HighlihgtedBrushProperty;

        public Brush HighlihgtedBrush
        {
            get { return (Brush)GetValue(HighlihgtedBrushProperty); }
            set { SetValue(HighlihgtedBrushProperty, value);}
        }

        public DataGridView()
        {
            HighlightedRowProperty = DependencyProperty.Register("HighlightedRow", typeof(object), typeof(DataGridView), 
                new FrameworkPropertyMetadata(null,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnRowChanged), new CoerceValueCallback(CoerceText)));

            HighlihgtedBrushProperty = DependencyProperty.Register("HighlihgtedBrush", typeof(Brush), typeof(DataGridView),
                new FrameworkPropertyMetadata(Brushes.Tomato,
                FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsRender,
                new PropertyChangedCallback(OnRowChanged), new CoerceValueCallback(CoerceText)));

            InitializeComponent();
        }

        private object CoerceText(DependencyObject d, object basevalue)
        {
            //throw new NotImplementedException();
            var grid = (DataGridView) d;
            var items = grid.ItemContainerGenerator.Items;

            /*int index = find_item(items, basevalue);
            if (index == -1)
                return 0;

            
            var row = (DataGridRow) grid.ItemContainerGenerator.ContainerFromIndex(index);
            if (row != null)
                row.Background = HighlihgtedBrush;*/

            int index = 1;
            foreach (var item in items)
            {
                var row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromIndex(index);
                if(row==null)
                    continue;

                if (item == basevalue)
                    row.Background = HighlihgtedBrush;
                else
                    row.Background = Background;

                index++;
            }

            return 0;
        }

        //Находит индекс строки по значению ItemSource
        private int find_item(IReadOnlyCollection<object> items, object current_item)
        {
            int index = 0;
            foreach (var item in items)
            {
                if (item == current_item)
                    return index;

                index++;
            }

            return -1;
        }

        private void OnRowChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
