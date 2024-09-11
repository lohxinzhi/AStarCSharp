using System.ComponentModel;
using OpenTK.Graphics.OpenGL;

namespace PathFinding
{

    public class AStarSearch
    {
        public static float[][] getPath(float[] startPose, float[] targetPose, float nodeDiameter, ScottPlot.Image map){
            Grid MapGrid = new Grid(nodeDiameter,  map);
            return getPath(startPose, targetPose,nodeDiameter, MapGrid);
        }
        public static float[][] getPath(float[] startPose, float[] targetPose, float nodeDiameter, Grid MapGrid){
            
            Node startNode = MapGrid.GetNearestNodeFromPosition(startPose);
            Node targetNode = MapGrid.GetNearestNodeFromPosition(targetPose);

            if (startNode==null || targetNode == null){
                return null;
            }

            List<Node> openSet = new List<Node>();
            HashSet<Node> closeSet = new HashSet<Node>();
            openSet.Add(startNode);
            while(openSet.Count>0){
                Node currentNode = openSet[0];
                for(int i = 1; i<openSet.Count; i++){ // get lowest cost node
                    if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost) ){
                        currentNode = openSet[i];
                    }
                }
                closeSet.Add(currentNode);
                openSet.Remove(currentNode);
                if(currentNode == targetNode){
                    //found path
                    var nodePath = RetracePath(targetNode);
                    float[][] path = new float[nodePath.Count][];
                    for(int i = 0; i<path.Length; i++){
                        path[i] = [nodePath[i].Position[0],nodePath[i].Position[1]];
                    }
                    return path;
                }
                var neighbours = MapGrid.GetNeighbours(currentNode);
                foreach(var node in neighbours){
                    if (!node.Walkable || closeSet.Contains(node)){
                        continue;
                    }
                    if(!openSet.Contains(node) || node.gCost > currentNode.gCost+MapGrid.GetDistance(node,currentNode)) {
                        node.gCost = currentNode.gCost+MapGrid.GetDistance(node, currentNode);
                        node.hCost = MapGrid.GetDistance(targetNode, node);
                        node.Parent = currentNode;
                    
                        if (!openSet.Contains(node)){
                            openSet.Add(node);
                        }
                    }
                }
            }
            return [startNode.Position,targetNode.Position];
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
    }
}