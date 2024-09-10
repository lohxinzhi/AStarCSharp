namespace PathFinding
{

    public class AStarSearch
    {
        public static float[][] getPath(float[] initialPose, float[] targetPose, float nodeDiameter, ScottPlot.Image map){
            Grid MapGrid = new Grid(nodeDiameter,  map);
            Node initialNode = MapGrid.GetNearestNodeFromPosition(initialPose);
            Node targetNode =MapGrid.GetNearestNodeFromPosition(targetPose);
            
            return [initialNode.Position,targetNode.Position];

        }
    }
}