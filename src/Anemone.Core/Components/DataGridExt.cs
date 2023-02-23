using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Anemone.Core.Components
{
    public class DataGridExt : DataGrid
    {
        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            base.OnAutoGeneratingColumn(e);
            var columnIndex = Columns.Count;
            var column = new DataGridTextColumn
            {
                Header = ColumnSource[columnIndex],
                Binding = new Binding(e.PropertyName) { Mode = BindingMode.TwoWay }
            };
            e.Column = column;
        }

    
        private static readonly DependencyProperty ColumnSourceProperty = DependencyProperty.Register(
            nameof(ColumnSource),
            typeof(IList),
            typeof(DataGridExt),
            new FrameworkPropertyMetadata(null));
        public IList ColumnSource
        {
            get => (IList)GetValue(ColumnSourceProperty);
            set => SetValue(ColumnSourceProperty, value);
        }
    }
}