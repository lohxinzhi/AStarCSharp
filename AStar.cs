using System.ComponentModel;
using System.Data.Common;
using System.Windows;
using OpenTK.Graphics.OpenGL;

namespace PathFinding
{
    public class AStarResult
    {
        public float[][] path;
        public HashSet<Node> closeSet;

        public AStarResult(float[][] _path, HashSet<Node> _closeSet){
            path = _path;
            closeSet = _closeSet;
        }
    }

    public class AStarSearch
    {
        public static AStarResult getPath(float[] startPose, float[] targetPose, float nodeDiameter, ScottPlot.Image map, bool simplify=false){
            Grid MapGrid = new Grid(nodeDiameter,  map);
            return getPath(startPose, targetPose,nodeDiameter, MapGrid, simplify);
        }
        public static AStarResult getPath(float[] startPose, float[] targetPose, float nodeDiameter, Grid MapGrid, bool simplify=false){
            
            Node startNode = MapGrid.GetNearestNodeFromPosition(startPose);
            Node targetNode = MapGrid.GetNearestNodeFromPosition(targetPose);

            if (startNode==null || targetNode == null){
                return null;
            }

            MinHeap<Node> openSet = new MinHeap<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Insert(startNode);
            while(openSet.Count>0){
                Node currentNode = openSet.Pop(); // get lowest in node
                closeSet.Add(currentNode);
                if(currentNode == targetNode){
                    //found path
                    var nodePath = RetracePath(targetNode);
                    float[][] path = new float[nodePath.Count][];
                    for(int i = 0; i<path.Length; i++){
                        path[i] = [nodePath[i].Position[0],nodePath[i].Position[1]];
                    }
                    if (simplify){
                        path = SimplifyPath(path);
                    }
                    return new AStarResult(path, closeSet);
                }
                var neighbours = MapGrid.GetNeighbours(currentNode);
                foreach(var node in neighbours){
                    if (!node.Walkable || closeSet.Contains(node)){
                        continue;
                    }
                    if(!openSet.Data.Contains(node) || node.gCost > currentNode.gCost+MapGrid.GetDistance(node,currentNode)) { // update the openset heap
                        node.gCost = currentNode.gCost+MapGrid.GetDistance(node, currentNode);
                        node.hCost = MapGrid.GetDistance(targetNode, node);
                        node.Parent = currentNode;
                    
                        if (!openSet.Data.Contains(node)){
                            openSet.Insert(node);
                        }
                        else{
                            openSet.UpdateHeap(node);
                        }
                    }
                }
            }
            return new AStarResult([startNode.Position,targetNode.Position],closeSet);
        }


        static List<Node> RetracePath(Node endNode){
            List<Node> path = new List<Node>();
            while (endNode!=null){
                path.Add(endNode);
                endNode = endNode.Parent;
            }
            path.Reverse();
            return path;
        }

        static float[][] SimplifyPath(float[][] path){
            List<float[]> finalPath = new();
            finalPath.Add(path[0]);
            var lineStart = path[0];
            for(int i = 1; i<path.Length; i++){
                if ((path[i][0] == path[i-1][0] && path[i][0] == lineStart[0])
                ||(path[i][1] == path[i-1][1] && path[i][1] == lineStart[1])
                ||((path[i][1]-path[i-1][1])/(path[i][0]-path[i-1][0])) == ((path[i][1]-lineStart[1])/(path[i][0]-lineStart[0]))){
                    continue;
                }
                else{
                    finalPath.Add(path[i-1]);
                    lineStart = path[i-1];
                }
            }
            finalPath.Add(path.Last());
            float[][] final = new float[finalPath.Count][];
            for(int i = 0; i<final.Length; i++){
                final[i] = finalPath[i];
            }
            return final;

        }
    }
}