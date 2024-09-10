using System.Windows.Documents;
using OpenTK.Graphics.OpenGL;

namespace PathFinding
{
    public class Node
    {
        public float[] Position {get;set;}
        public Node Parent {get; set;}
        public bool Walkable {get;set;}

        public Node(float[] position, bool walkable = true, Node parent = null){
            Position = position;
            Parent = parent;
            Walkable = walkable;
        }
    }

    public class Grid
    {
        public int[] mapSize{get;set;} // size of image
        public byte[,] occupancyGrid{get;set;} // from an image
        public float nodeRadius{get;set;}
        public float nodeDiameter{get;set;}
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
                    grid[x,y] = new Node(point, walkable,null);
                }
            }

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
            int x =(int)Math.Round(position[0]/nodeDiameter);
            int y =(int)Math.Round(position[1]/nodeDiameter);
            return grid[x,y];
        }

        

    }
}