## Circular Obstacle Pathfinding and A-Star

[Implementation](Assets/Scripts/Pathfinding/CircularObstacleGraph/CircularObsticleGraphGenerator.cs) of COP [algorithm](https://redblobgames.github.io/circular-obstacle-pathfinding/). It creates a [graph](Assets/Scripts/Pathfinding/Graph/Graph.cs) based on a forest of rounded obstacles. \
A-Star is also [implemented](Assets/Scripts/Pathfinding/Algorithms/AStar.cs) to find the shortest path on that graph.

### Demo gifs:

**Pathfinding with changing goal**\
![](Readme/cop-1.gif)

**Another example**\
![](Readme/cop-3.gif)

**Graph recalculation when obstacles move**\
![](Readme/cop-2.gif)

## Optimal Reciprocal Collision Avoidance

Also [ORCA](http://gamma.cs.unc.edu/ORCA/) have been integrated into a project with [RVO-2 library](https://github.com/snape/RVO2-CS). 

### Examples:

![](Readme/oca-1.gif)

<img src="Readme/oca-3.gif" alt="drawing" width="800"/>

![](Readme/oca-2.gif)

