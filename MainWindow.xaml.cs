using Microsoft.Win32;
using ScottPlot;
using ScottPlot.Palettes;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PathFinding;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    ScottPlot.Image Map;
    Grid MapGrid;
    public MainWindow()
    {
        InitializeComponent();
        Loaded += (s, e) =>
        {

            // Images may be loaded from files or created dynamically
            Map = new ScottPlot.Image("C:\\Users\\Administrator\\Desktop\\xinzhi\\Others\\CS App Test\\PathFinding\\map_4.bmp");

            CoordinateRect rect = new(left: 0, right: Map.Width, bottom: 0, top: Map.Height);

            Plot_1.Plot.Add.ImageRect(Map, rect);

        };
    }

    private void button_AStarMode_Click(object sender, RoutedEventArgs e)
    {
        MapGrid = new Grid(1.0f, Map);
        // List<float> dataX = new List<float>();
        // List<float> dataY = new List<float> ();
        for(int x = 0; x<MapGrid.GridSizeX;x++){
            for(int y = 0; y<MapGrid.GridSizeY;y++){
                if (!MapGrid.grid[x,y].Walkable){
                    // dataX.Add(MapGrid.grid[x,y].Position[0]);
                    // dataY.Add(MapGrid.grid[x,y].Position[1]);
                    Plot_1.Plot.Add.Marker(x,y,shape:MarkerShape.FilledSquare);
                }
            }
        }
        // Plot_1.Plot.Clear();
        // Plot_1.Plot.Add.Scatter(dataX,dataY);
        Plot_1.Refresh();
        
    }

    private void button_RRTMode_Click(object sender, RoutedEventArgs e)
    {
        var wp = AStarSearch.getPath([5,5], [100,120], 10, Map);
        Plot_1.Plot.Add.Marker(wp[0][0], wp[0][1],size:15);
        Plot_1.Plot.Add.Marker(wp[1][0], wp[1][1], size: 15);
        Plot_1.Refresh();

    }

    private void button_RRTStar_Click(object sender, RoutedEventArgs e)
    {
        ;
    }

    private void button_UploadMap_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult result = MessageBoxResult.None;
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.Filter = "Image files (*.jpg,*.jpeg,*.png,*.bmp)|*.jpg;*.jpeg;*.png;*.bmp|All files (*.*)|*.*";
        ofd.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
        if (ofd.ShowDialog()==true)
        {
            Map = new ScottPlot.Image(ofd.FileName);
            CoordinateRect rect = new(left: 0, right: Map.Width, bottom: 0, top: Map.Height);
            Plot_1.Plot.Clear();
            Plot_1.Plot.Add.ImageRect(Map, rect);
            Plot_1.Refresh();
            
        }
    }
}