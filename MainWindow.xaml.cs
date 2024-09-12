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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PathFinding;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    ScottPlot.Image Map;
    Grid MapGrid;
    IPalette palette1 = new ScottPlot.Palettes.Category20();

    float InitialX=0, InitialY=0, TargetX=0, TargetY=0;
    float nodeDiameter = 10f;
    bool showExpanded = false;
    bool simplifyPath = false;
    int RRTIternation = 5000;
    SearchAlgo searchAlgo = SearchAlgo.ASTAR;

    AStarResult aStarResult;
    RRTResult rRTResult;

    DispatcherTimer TimerObj;

    enum SearchAlgo{
        ASTAR,
        RRT,
        RRTSTAR
    }

    public MainWindow()
    {
        InitializeComponent();

        // Initialise Dispatcher
        TimerObj = new DispatcherTimer();
        TimerObj.Interval = TimeSpan.FromMilliseconds(100);
        TimerObj.Tick += new EventHandler(timer_tick);
        Loaded += (s, e) =>
        {

            // Images may be loaded from files or created dynamically
            Map = new ScottPlot.Image("C:\\Users\\Administrator\\Desktop\\xinzhi\\Others\\CS App Test\\PathFinding\\map_2.bmp");

            CoordinateRect rect = new(left: 0, right: Map.Width, bottom: 0, top: Map.Height);

            Plot_1.Plot.Add.ImageRect(Map, rect);

        };
    }

    private void timer_tick(object sender, EventArgs e)
    {
        ;
    }

    void ShowMap(Grid MapGrid)
    {
        for(int x = 0; x<MapGrid.GridSizeX;x++){
            for(int y = 0; y<MapGrid.GridSizeY;y++){
                if (!MapGrid.grid[x,y].Walkable){
                    Node currentNode = MapGrid.grid[x,y];
                    ScottPlot.Plottables.Rectangle rect = new(){
                    X1 = currentNode.Position[0] - MapGrid.nodeRadius,
                    X2 = currentNode.Position[0] + MapGrid.nodeRadius,
                    Y1 = currentNode.Position[1] - MapGrid.nodeRadius,
                    Y2 = currentNode.Position[1] + MapGrid.nodeRadius,
                    LineColor = palette1.GetColor(14),
                    FillColor = palette1.GetColor(15),
                    };
                    Plot_1.Plot.PlottableList.Add(rect);
                    Plot_1.Refresh();
                }
            }
        }
        Plot_1.Refresh();
    }
    void ShowPath(float[][] wp){
        if (wp!=null){
            Plot_1.Plot.Clear();
            ShowMap(MapGrid);
            var wp_coord = new Coordinates[wp.Length];
            for(int i = 0; i< wp.Length; i++){
                wp_coord[i] = new Coordinates(wp[i][0],wp[i][1]);
            }
            Plot_1.Plot.Add.Scatter(wp_coord,color:palette1.GetColor(5));
            Plot_1.Plot.Add.Marker(wp[0][0], wp[0][1],size:15,color:palette1.GetColor(7));
            Plot_1.Plot.Add.Marker(wp.Last()[0], wp.Last()[1], size: 15,color:palette1.GetColor(19));
            Plot_1.Refresh(); 
            if (showExpanded)
            {
                switch(searchAlgo)
                {
                    case SearchAlgo.ASTAR:{
                        ShowExpanded(aStarResult.closeSet);
                        break;
                    }
                    case SearchAlgo.RRT:{
                        ShowExpanded(rRTResult.treeList);
                        break;
                    }
                }
            }
        }
    }

    void ShowExpanded(HashSet<Node> closeSet){
        foreach (var node in closeSet){
            ScottPlot.Plottables.Rectangle rect = new(){
                    X1 = node.Position[0] - MapGrid.nodeRadius,
                    X2 = node.Position[0] + MapGrid.nodeRadius,
                    Y1 = node.Position[1] - MapGrid.nodeRadius,
                    Y2 = node.Position[1] + MapGrid.nodeRadius,
                    LineColor = palette1.GetColor(6),
                    FillColor = palette1.GetColor(6).WithAlpha(0.5),
            };
            Plot_1.Plot.PlottableList.Add(rect);
            Plot_1.Refresh();
        }
    }
    void ShowExpanded(List<RRTNode> treeList){
        foreach(var node in treeList){
            Plot_1.Plot.Add.Marker(node.Position[0], node.Position[1], size:15, color:palette1.GetColor(6));
        }
        Plot_1.Refresh();
    }

    void Update()
    {
        switch (searchAlgo)
        {
            default:
            case SearchAlgo.ASTAR:
                {
                    textBox_CurrentMethod.Text = "A*";
                    UpdateASTAR();
                    break;
                }
            case SearchAlgo.RRT:
                {
                    textBox_CurrentMethod.Text = "RRT";
                    UpdateRRT();
                    break;
                }
            case SearchAlgo.RRTSTAR:
                {
                    textBox_CurrentMethod.Text = "RRT*";
                    UpdateRRTSTAR();
                    break;
                }

        }
    }
    void UpdateASTAR() 
    {
        aStarResult = AStarSearch.getPath([InitialX, InitialY], [TargetX, TargetY], nodeDiameter, Map, simplifyPath);
        if (aStarResult != null)
        {
            ShowPath(aStarResult.path);
        }
    }
    
    void UpdateRRT()
    {
        rRTResult = RRTSearch.getPath([InitialX, InitialY], [TargetX, TargetY],nodeDiameter,1000,Map);
        if (rRTResult!=null){
            ShowPath(rRTResult.path);
        }
    }

    void UpdateRRTSTAR()
    {
        ;
    }

    private void slider_NodeDiameter_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        nodeDiameter = (float)slider_NodeDiameter.Value;
        MapGrid = new Grid (nodeDiameter, Map);
        if (MapGrid != null) 
        {
            Update();
        }

    }

    private void textBox_InitialX_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(textBox_InitialX.Text, out InitialX) && MapGrid!=null)
        {
            Update();
        }
    }

    private void textBox_InitialY_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(float.TryParse(textBox_InitialY.Text, out InitialY) && MapGrid!=null)
        {
            Update();
        }
    }

    private void textBox_TargetX_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (float.TryParse(textBox_TargetX.Text, out TargetX) && MapGrid!=null)
        {
            Update();
        }
    }
    private void textBox_TargetY_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(float.TryParse(textBox_TargetY.Text, out TargetY) && MapGrid!=null)
        {
            Update();
        }
    }

    private void checkBox_ShowExplored_Checked(object sender, RoutedEventArgs e)
    {
        showExpanded = true;
        Update();

    }

    private void checkBox_ShowExplored_Unchecked(object sender, RoutedEventArgs e)
    {
        Plot_1.Plot.Clear();
        showExpanded = false;
        Update();
    }
    private void checkBox_SimplifyPath_Checked(object sender, RoutedEventArgs e)
    {
        simplifyPath = true;
        Update();
    }
    private void checkBox_SimplifyPath_Unchecked(object sender, RoutedEventArgs e)
    {
        simplifyPath = false;
        Update();
    }
    private void button_AStarMode_Click(object sender, RoutedEventArgs e)
    {
        searchAlgo = SearchAlgo.ASTAR;
        Update();
    }

    private void slider_RRTIteration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        RRTIternation = (int)slider_RRTIteration.Value;
        Update();
    }

    private void button_RRTMode_Click(object sender, RoutedEventArgs e)
    {
        searchAlgo = SearchAlgo.RRT;
        Update();
    }

    private void button_RRTStar_Click(object sender, RoutedEventArgs e)
    {
        searchAlgo = SearchAlgo.RRTSTAR;
        Update();
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