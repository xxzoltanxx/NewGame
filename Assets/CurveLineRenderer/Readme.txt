CurveLineRenderer
ver. 1.3

The CurveLineRenderer script allows to draw 2D line into 3D scene. Script is written on C#.
The script is render the line with a constant width.

To start using script you need extract Unity package into created project and
attach the CurveLineRenderer script to the GameObject.
----------------------------------------------------------------------------------------------------

CHANGE LOG:
Version 2.0:
1) Added new curve mesh build mode to allow to represent more complex and smooth curves

Version 1.3.3:
1) Fix missed material for MeshRenderer component

Version 1.3.1:
1) Fixed seldom bugs

Version 1.3:
1) Physics support: component MeshCollider is attached to the curve to provide object physical
behaviour. You can enable/disable this component both in Editor and Runtime
2) If you receive an error:
"MissingComponentException: There is no 'MeshCollider' attached to the game object, but a script is trying to access it"
after updating script to version 1.3, just add manually component 'MeshCollider' to the curve by 'Add Component' menu

Version 1.2.1:
1) Added the ability to quickly add and remove vertices to the curve from the Inspector of
Unity Editor

Version 1.2:
1) Texture support: now you can build textured lines such as roads, borders and others.
2) Optimized draw calls in EditMode of Unity Editor

Version 1.1:
1) The CurveLineRenderer script is now available for editing in SceneView. You can change position
of vertices, just select vertex and move it by mouse. Also, all changes of script parameters in
Inspector will be displayed in SceneView at the same time.
2) New 'Splitted' mode has been added by request from one of the developers.

----------------------------------------------------------------------------------------------------

CONTACTS:
You can send me any questions about script, errors or ideas for new features

Igor Kostritsyn
sarfut@gmail.com
http://curvelinerenderer.blogspot.ru/p/help.html
----------------------------------------------------------------------------------------------------

ASSET DESCRIPTION:
Full version of manual available in site http://curvelinerenderer.blogspot.com/p/guide.html

Data for script there are into the Assets\CurveLineRenderer folder.
The CurveLineRenderer.cs script file is located into the CurveLineRenderer\Scripts folder.

There is example of using script into the CurveLineRenderer\Example folder. In this example object
TileManager holds CurveLineRenderer script.
Example is a prototype of game like Heroes, Civilization, King's Bounty, and shows how
CurveLineRenderer script can be used for draw path of game objects
----------------------------------------------------------------------------------------------------

PARAMETERS OF THE SCRIPT:
1) Type - defines spline type.
	Default - without smoothing of corners
    Rounded - corners are smoothed along the arc of given radius
	Splitted (Added in ver. 1.1) - each segment is independent from other segments

2) Width - width of the curve spline
3) Radius - a smoothing radius.
	Parameter is used only for Rounded spline
4) RoundedAngle - specifies a offset of smoothing angle. Defines count of the fragments of
	smoothing corners.
	Used only for Rounded spline
5) Normal - vector, which defines the orientation of the spline in 3D space
6) ReverseSideEnabled - if the parameter is enabled, the reverse side of the spline will be drawn
7) Vertices - vertices of the spline
