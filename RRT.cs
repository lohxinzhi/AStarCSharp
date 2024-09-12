using System.ComponentModel;
using OpenTK.Graphics.OpenGL;

namespace PathFinding
{
    public class RRTResult
    {
        public float[][] path;
        public List<RRTNode> treeList;

        public RRTResult(float[][] _path, List<RRTNode> _treeList){
            path = _path;
            treeList = _treeList;
        }

    }
    public class RRTSearch
    {
        public static RRTResult getPath(float[] startPose, float[] targetPose, float distanceThreshold, int maxIteration, ScottPlot.Image MapImage)
        {
            var rnd = new Random();
            bool[,] occupancyGrid = GetOccupancyGridFromImage(MapImage);
            RRTNode currentNode = new RRTNode(startPose);
            List<RRTNode> treeNodes = new();
            treeNodes.Add(currentNode);

            int currentIteration = 0;
            while(GetDistance(targetPose, currentNode.Position) > distanceThreshold && currentIteration < maxIteration){
                RRTNode farNode = new RRTNode([rnd.Next(0,occupancyGrid.GetLength(0)) , rnd.Next(0,occupancyGrid.GetLength(1))]); // sample random node in the field
                var nearestNode = treeNodes[0];
                foreach(var node in treeNodes){ // get nearest node to the random far node
                    if (GetDistance(node.Position, farNode.Position) < GetDistance(nearestNode.Position, farNode.Position)){
                        nearestNode = node;
                    }
                }
                treeNodes.Add(getNextNode(farNode,nearestNode,distanceThreshold,occupancyGrid));

                currentIteration++;
            }

            if(GetDistance(targetPose, currentNode.Position) <= distanceThreshold){
                treeNodes.Add(new RRTNode(targetPose,currentNode));
                var pathNode = retracePath(treeNodes.Last());
                var path = new float[pathNode.Count][];
                for(int i = 0; i<path.Length; i++){
                    path[i] = pathNode[i].Position;
                }
                return new RRTResult(path,treeNodes);
            }
            else{
                return new RRTResult([startPose,targetPose],treeNodes);
            }


        }
        static List<RRTNode> retracePath(RRTNode node){
            var path = new List<RRTNode>();
            while(node!=null){
                path.Add(node);
                node=node.Parent;
            }
            path.Reverse();
            return path;
        }

        static RRTNode getNextNode(RRTNode farNode, RRTNode nearestNode,float distanceThreshold, bool[,] occupancyGrid){
            float[] currentPoint = new float[2];
            nearestNode.Position.CopyTo(currentPoint,0);
            float gradient = (farNode.Position[1]-nearestNode.Position[1])/(farNode.Position[0]-nearestNode.Position[0]);
            float yIntercept = farNode.Position[1]-gradient*farNode.Position[0];
            float dx = 1/MathF.Sqrt(1+gradient*gradient);
            float dy = gradient/MathF.Sqrt(1+gradient*gradient);  
            while(GetDistance(nearestNode.Position, currentPoint)<distanceThreshold){
                currentPoint[0]+=dx;
                currentPoint[1]+=dy;
                if(occupancyGrid[Math.Clamp((int)MathF.Round(currentPoint[0]),0,occupancyGrid.GetLength(0)-1) , Math.Clamp((int)MathF.Round(currentPoint[1]), 0 ,occupancyGrid.GetLength(1)-1)]){
                    break;
                }
            }
            currentPoint[0]-=dx;
            currentPoint[1]-=dy;
            return new RRTNode(currentPoint,nearestNode);

        }

        static bool[,] GetOccupancyGridFromImage(ScottPlot.Image img){
            byte[,] temp_img = img.GetArrayGrayscale();
            bool[,] temp = new bool[img.Width,img.Height];
            for(int x =0; x < img.Width; x++){
                for(int y=0; y < img.Height; y++){
                    temp[x,y] = temp_img[img.Height-1-y,x]<230;
                }
            }
            return temp;
        } 
        
        static float GetDistance(float[] point1, float[] point2){
            return MathF.Sqrt(MathF.Pow(point1[0]-point2[0] , 2) + MathF.Pow(point1[1]-point2[1] , 2));
        }
    }
}