using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
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
using SQL_DB_DataTables;

namespace Grocery_Store_With_IMG
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes)
            {
                using (MemoryStream stream = new MemoryStream(bytes))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    return image;
                }
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public partial class MainWindow : Window
    {
        
        
        SQL_DB DB = new SQL_DB(ConfigurationManager.ConnectionStrings["SQL_Server"].ConnectionString);

            public MainWindow()
            {
            InitializeComponent();

            DataTable dataTable = DB.GetDataTable("select * from Product");

            foreach (DataColumn column in dataTable.Columns)
            {
                DataGridTemplateColumn dataGridColumn = new DataGridTemplateColumn();
                dataGridColumn.Header = column.ColumnName;


                if (column.DataType == typeof(byte[]))
                {
                    // Шаблон для отображения изображений
                    FrameworkElementFactory imageFactory = new FrameworkElementFactory(typeof(Image));
                    Binding imageBinding = new Binding(column.ColumnName);
                    imageBinding.Converter = new ByteArrayToImageSourceConverter();
                    imageFactory.SetBinding(Image.SourceProperty, imageBinding);

                    DataTemplate dataTemplate = new DataTemplate();
                    dataTemplate.VisualTree = imageFactory;
                    dataGridColumn.CellTemplate = dataTemplate;
                }
                else
                {
                    // Шаблон для отображения текстовых значений
                    FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(column.ColumnName));

                    DataTemplate dataTemplate = new DataTemplate();
                    dataTemplate.VisualTree = textBlockFactory;
                    dataGridColumn.CellTemplate = dataTemplate;
                }


                main_grid.Columns.Add(dataGridColumn);
            }

            main_grid.ItemsSource = dataTable.DefaultView;
        }
    }
}
