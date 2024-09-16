using System.ComponentModel;
using OpenTK.Graphics.OpenGL;

namespace PathFinding
{
    public class RRTResult2
    {
        public float[][] path;
        public List<RRTNode> treeList;
        public bool[,] occupancyGrid;
        public List<RRTNode> farNodeList;

        public RRTResult2(float[][] _path, List<RRTNode> _treeList, bool[,] _occupancyGrid, List<RRTNode> _farNodeList){
            path = _path;
            treeList = _treeList;
            occupancyGrid = _occupancyGrid;
            farNodeList = _farNodeList;

        }

    }
    public class RRTSearch2
    {
        public static RRTResult2 getPath(float[] startPose, float[] targetPose, float distanceThreshold, int maxIteration, ScottPlot.Image MapImage)
        {
            var rnd = new Random();
            bool[,] occupancyGrid = GetOccupancyGridFromImage(MapImage);
            RRTNode currentNode = new RRTNode(startPose);
            List<RRTNode> treeNodes = new();
            treeNodes.Add(currentNode);
            List<RRTNode> farNodeList = new();
            RRTNode nearestNodeToTarget = currentNode;

            int currentIteration = 0;
            // while(GetDistance(targetPose, currentNode.Position) > distanceThreshold && currentIteration < maxIteration){
            while(currentIteration < maxIteration){

                RRTNode farNode;
                if(currentIteration%20==0){
                    farNode = new RRTNode([rnd.Next((int)targetPose[0]-20,(int)targetPose[0]+20) , rnd.Next((int)targetPose[1]-20,(int)targetPose[1]+20)]); // sample random node in the field
                }
                else{
                    farNode = new RRTNode([rnd.Next(-occupancyGrid.GetLength(0)/10,occupancyGrid.GetLength(0)) , rnd.Next(-occupancyGrid.GetLength(1)/10,occupancyGrid.GetLength(1))]); // sample random node in the field
                }
                farNodeList.Add(farNode);
                var nearestNode = treeNodes[0];
                foreach(var node in treeNodes){ // get nearest node to the random far node
                    if (GetDistance(node.Position, farNode.Position) < GetDistance(nearestNode.Position, farNode.Position)){
                        nearestNode = node;
                    }
                }
                var nextNode = getNextNode(farNode,nearestNode,distanceThreshold,occupancyGrid);
                if (nextNode!=null){
                    treeNodes.Add(nextNode);                
                    currentNode = treeNodes.Last();
                    if( GetDistance(currentNode.Position,targetPose) <GetDistance(nearestNodeToTarget.Position,targetPose)){
                        nearestNodeToTarget=currentNode;
                    }
                }
                currentIteration++;

                
            }

            if(GetDistance(targetPose, nearestNodeToTarget.Position) <= distanceThreshold){
                treeNodes.Add(new RRTNode(targetPose,nearestNodeToTarget));
                var pathNode = retracePath(treeNodes.Last());
                var path = new float[pathNode.Count][];
                for(int i = 0; i<path.Length; i++){
                    path[i] = pathNode[i].Position;
                }
                return new RRTResult2(path,treeNodes,occupancyGrid,farNodeList);
            }
            else{
                return new RRTResult2([startPose,targetPose],treeNodes,occupancyGrid, farNodeList);
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
            var rnd = new Random();
            float[] currentPoint = new float[2];
            nearestNode.Position.CopyTo(currentPoint,0);
            float angle = MathF.Atan2(farNode.Position[1]-nearestNode.Position[1], farNode.Position[0]-nearestNode.Position[0]);
            float multiplier = 1.0f;
            float dx = multiplier * MathF.Cos(angle);
            float dy = multiplier * MathF.Sin(angle);  
            int count = 0;
            bool reflected = false;
            while(count*multiplier<distanceThreshold){
                currentPoint[0]+=dx;
                currentPoint[1]+=dy;

                if( occupancyGrid[Math.Clamp((int)MathF.Round(currentPoint[0]),0,occupancyGrid.GetLength(0)-1) , Math.Clamp((int)MathF.Round(currentPoint[1]), 0 ,occupancyGrid.GetLength(1)-1)]){
                    // return null;
                    // break;
                    if(!reflected){
                        currentPoint[0]-=dx;
                        currentPoint[1]-=dy;
                        (dx,dy) = rnd.Next(0,2) == 1 ? (-dy,dx) : (dy,-dx);
                        reflected=true;
                    }
                    else{
                        break;
                    }

                }                
                if ( !reflected && count > GetDistance(nearestNode.Position,farNode.Position)){
                    float[] pos = [Math.Clamp(farNode.Position[0],0,occupancyGrid.GetLength(0)), Math.Clamp(farNode.Position[1], 0, occupancyGrid.GetLength(1))];
                    return new RRTNode(pos, nearestNode) ;
                }
                count++;
            }
            currentPoint[0]-=dx;
            currentPoint[0] = Math.Clamp(currentPoint[0], 0.0f, (float) occupancyGrid.GetLength(0));
            currentPoint[1]-=dy;
            currentPoint[1] = Math.Clamp(currentPoint[1], 0.0f, (float) occupancyGrid.GetLength(1));

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