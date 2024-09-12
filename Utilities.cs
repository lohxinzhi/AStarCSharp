using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Documents;
using System.Windows.Input;
using OpenTK.Graphics.OpenGL;

namespace PathFinding
{
    public class Node: IHeapItem<Node>
    {
        public float[] Position;
        public Node Parent;
        public bool Walkable;
        public int gCost;
        public int hCost;
        public int gridX;
        public int gridY;
        int heapIndex;

        public Node(float[] position, int _gridX, int _gridY, bool walkable = true, Node parent = null, int _heapIndex=-1){
            Position = position;
            Parent = parent;
            Walkable = walkable;
            gridX = _gridX;
            gridY = _gridY;
            heapIndex = _heapIndex;
        }

        public int fCost{
            get{
                return gCost+hCost;
            }
        }

        public int HeapIndex{
            get{
                return heapIndex;
            }
            set{
                heapIndex = value;
            }
        }
        public int CompareTo(Node other){
            if (other==null){
                return 1;
            }
            int Compare = this.fCost.CompareTo(other.fCost);
            if (Compare == 0){
                Compare = this.hCost.CompareTo(other.hCost);
            }
            return Compare;
            
        }
    }

    public class Grid
    {
        public int[] mapSize{get;set;} // size of image
        public byte[,] occupancyGrid{get;set;} // from an image
        public float nodeRadius{get;set;}
        public float nodeDiameter { get; set;}
        public int GridSizeX {get;set;} 
        public int GridSizeY {get;set;} // number of nodes along x and y axis

        public Node[,] grid{get;set;}



        public Grid(float _nodeDiameter= 1, ScottPlot.Image map = null){
            if (map!=null){
                occupancyGrid = GetOccupancyGridFromImage(map);
                mapSize = new int[]{occupancyGrid.GetLength(0),occupancyGrid.GetLength(1)};
            }
            else{
                occupancyGrid = new byte[100,100];
                for(int i = 0; i<100; i++){
                    for(int j = 0; j<100; j++){
                        occupancyGrid[i,j] = 0;
                    }
                }
                mapSize = new int[]{100,100};
            }
            nodeDiameter = _nodeDiameter;
            nodeRadius = nodeDiameter/2;
            GridSizeX = (int)MathF.Round(mapSize[0]/nodeDiameter);
            GridSizeY = (int)MathF.Round(mapSize[1]/nodeDiameter);
            CreateGrid();        
        }

        void CreateGrid(){
            grid = new Node[GridSizeX,GridSizeY];
            for(int x = 0; x<GridSizeX; x++){
                for(int y = 0; y<GridSizeY; y++){
                    float[] point = {0,0};
                    point[0] = x*nodeDiameter+nodeRadius;
                    point[1] = y*nodeDiameter+nodeRadius;
                    bool walkable = Walkable(point);
                    grid[x,y] = new Node(point,x,y, walkable,null);
                }
            }

        }

        public List<Node> GetNeighbours(Node node){
            var neighbour = new List<Node> ();
            int x = node.gridX;
            int y = node.gridY;
            for(int i = -1; i<=1; i++){
                for(int j = -1; j<=1; j++){
                    if (i==0 && j==0){
                        continue;
                    }
                    int checkX = x+i;
                    int checkY= y+j;
                    if (checkX>=0 && checkX<GridSizeX && checkY>=0 && checkY<GridSizeY){
                        neighbour.Add(grid[checkX,checkY]);
                    } 
                }
            }
            return neighbour;


            return neighbour;

        }

        bool Walkable(float[] position){
            int lowerboundx = Math.Max( (int)MathF.Round(position[0]-nodeRadius),0);
            int upperboundx = Math.Min( (int)MathF.Round(position[0]+nodeRadius), mapSize[0]-1);
            int lowerboundy = Math.Max( (int)MathF.Round(position[1]-nodeRadius),0);
            int upperboundy = Math.Min( (int)MathF.Round(position[1]+nodeRadius),mapSize[1]-1);
            for(int x = lowerboundx; x<=upperboundx; x++){
                for(int y = lowerboundy; y<=upperboundy; y++){
                    if (occupancyGrid[x,y] <=  230 ){
                        return false;
                    }
                }
            }
            return true;
        }

        byte[,] GetOccupancyGridFromImage(ScottPlot.Image img){
            byte[,] temp_img = img.GetArrayGrayscale();
            byte[,] temp = new byte[img.Width,img.Height];
            for(int x =0; x < img.Width; x++){
                for(int y=0; y < img.Height; y++){
                    temp[x,y] = temp_img[img.Height-1-y,x];
                }
            }
            return temp;
        }

        public Node GetNearestNodeFromPosition(float[] position){
            if (position[0] > mapSize[0] || position[1] > mapSize[1]){
                return null;
            }
            int x =(int)Math.Round(position[0]/nodeDiameter);
            int y =(int)Math.Round(position[1]/nodeDiameter);
            return grid[x,y];
        }

        public int GetDistance(Node start, Node end){
            int XDist = Math.Abs(start.gridX-end.gridX);
            int YDist = Math.Abs(start.gridY-end.gridY);
            int max = Math.Max(XDist,YDist);
            int min = Math.Min(XDist,YDist);
            return (max-min)*10 + min*14;
        }

        

    }
}