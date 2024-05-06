import 'package:flutter/material.dart';
import 'dart:developer' as debug;
import 'constants.dart' as constants;
import 'points.dart';
import 'package:vector_math/vector_math.dart' as math;
import 'dart:math' as dartmath;

void main() {
  runApp(const MyApp());
}

// *****************************************************************************
// Block of code that handles the flutter display and input tracking
// *****************************************************************************
class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'Plot and Store Points',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: const MyHomePage(title: 'Plot and Store Points'),
    );
  }
}

class MyHomePage extends StatefulWidget {
  const MyHomePage({super.key, required this.title});
  final String title;

  @override
  State<MyHomePage> createState() => _MyHomePageState();
}

class _MyHomePageState extends State<MyHomePage> {
  double _x = 0.0;
  double _y = 0.0;
  String _focusText = constants.outside;
  String _pressedText = constants.notPressed;
  GlobalKey key = GlobalKey();
  Points points = Points();
  List<int> triangles = [];

  void _inputEnter(PointerEvent details) {
    setState(() {
      _focusText = constants.inside;
      debug.log(_focusText);
    });
  }

  void _inputExit(PointerEvent details) {
    setState(() {
      _focusText = constants.outside;
      debug.log(_focusText);
    });
  }

  void _inputPositionChange(PointerEvent details) {
    setState(() {
      // Get the offset (from TL) of the container
      RenderBox box = key.currentContext?.findRenderObject() as RenderBox;
      Offset pos = box.localToGlobal(Offset.zero);

      // Set the X and Y relative to the container TL
      _x = details.position.dx - pos.dx;
      _y = details.position.dy - pos.dy;
      debug.log("Pos X: $_x, Y: $_y");
    });
  }

  void _inputPressed(PointerEvent details) {
    setState(() {
      _inputPositionChange(details);
      _pressedText = constants.pressed;
      debug.log(_pressedText);

      _storePosition(_x, _y);
    });
  }

  void _inputReleased(PointerEvent details) {
    setState(() {
      _inputPositionChange(details);
      _pressedText = constants.notPressed;
      debug.log(_pressedText);
    });
  }

  void _storePosition(double x, double y) {
    points.add(_x, _y, 0.0);
  }

  @override
  Widget build(BuildContext context) {
    var width = MediaQuery.of(context).size.width;
    const margin = EdgeInsets.only(bottom: 10.0, right: 10.0, left: 10.0);

    return Scaffold(
        appBar: AppBar(
          title: Text(widget.title),
        ),
        body: SafeArea(
          child: Column(children: [
            Expanded(
              flex: 10,
              child: Container(
                padding:
                    const EdgeInsets.symmetric(horizontal: 20, vertical: 20),
                color: Colors.blueAccent,

                /// The listener listens for various events. We're using it to
                /// track the user PointerDown and PointerUp (Mouse click or
                /// screen tap)
                child: Listener(
                  onPointerDown: _inputPressed,
                  onPointerMove: _inputPositionChange,
                  onPointerUp: _inputReleased,

                  /// Track mouse focus events and position changes
                  /// Tapping doesn't really have a focus as it's either inside
                  /// or nothing is happening
                  child: MouseRegion(
                    onEnter: _inputEnter,
                    onHover: _inputPositionChange,
                    onExit: _inputExit,
                    child: LayoutBuilder(
                      builder: (_, constraints) => Container(
                        key: key,
                        width: constraints.widthConstraints().maxWidth,
                        height: constraints.heightConstraints().maxHeight,
                        color: Colors.blueGrey,
                        child: Stack(children: [
                          /// Add a transform to show the mouse/tap position
                          Transform(
                              transform: Matrix4.translationValues(
                                  _x - 16, _y - 16, 0.0),
                              child: const CircleAvatar(
                                radius: 16,
                              )),
                          CustomPaint(
                              painter: PolygonDisplay(points, triangles)),
                        ]),
                      ),
                    ),
                  ),
                ),
              ),
            ),

            /// Update the area at the bottom of the screen where we show the
            /// stats and feedback on what's going on
            Expanded(
              flex: 2,
              child: Container(
                  width: width,
                  margin: margin, //variable
                  color: const Color.fromARGB(255, 10, 40, 53), //variable
                  child: Text(
                      "X: ${_x.toStringAsFixed(4)}, Y: ${_y.toStringAsFixed(4)}\nFocus: $_focusText\nButton/Tap: $_pressedText",
                      style: const TextStyle(
                          color: Color.fromARGB(255, 255, 255, 255)))),
            ),
            ElevatedButton(
              child: const Text('Triangulate'),
              onPressed: () {
                triangulate();
              },
            ),
            ElevatedButton(
              child: const Text('Reset'),
              onPressed: () {
                reset();
              },
            ),
          ]),
        ));
  }

// *****************************************************************************
// Block of code that handles the polygon triangulation (ear triangulation)
// See here for an overview; https://www.youtube.com/watch?v=QAdfkylpYwc
// Tutorial above didn't seem to work in all cases so adjusted the math as seen
// in the code below
// *****************************************************************************
  // Reset the flutter display so we can have another go
  void reset() {
    points.pointList.clear();
    triangles.clear();
  }

  // Triangulate the poly using an ear algorithm
  void triangulate() {
    debug.log("Starting Triangulation");

    // Build a list of indices. One for every vert
    List<int> indexList = [];
    indexList.clear();
    for (int i = 0; i < points.pointList.length; i++) {
      indexList.add(i);
    }

    // Check the orientation of the points (needs to be ccw) to make this work
    double total = 0;
    for (int i = 0; i < points.pointList.length - 1; i++) {
      total += (points.pointList[i + 1].x - points.pointList[i].x) *
          (points.pointList[i + 1].y + points.pointList[i].y);
    }
    if (total <= 0) {
      points.pointList = points.pointList.reversed.toList();
    }

    int loops =
        0; // This is just a safeguard to avoid locking in a while due to bad values in the verts
    triangles.clear();
    while (indexList.length > 3 && loops < 2000) {
      loops++;
      for (int i = 0; i < indexList.length; i++) {
        int indexA = getItem(indexList, i);
        int indexB = getItem(indexList, i - 1);
        int indexC = getItem(indexList, i + 1);

        math.Vector3 va = points.pointList[indexA];
        math.Vector3 vb = points.pointList[indexB];
        math.Vector3 vc = points.pointList[indexC];

        // If this poly is convex then don't use it (for now)
        if (isConvex(vb, va, vc)) {
          continue;
        }

        // Check to see if there are any verts inside this poly. If so then skip it
        bool isEar = true;
        for (int j = 0; j < points.pointList.length; j++) {
          if (j == indexA || j == indexB || j == indexC) {
            continue;
          }

          if (inTriangle(points.pointList[j], va, vc, vb)) {
            isEar = false;
            break;
          }
        }

        // If this is an ear then add it to the list for display
        if (isEar) {
          triangles.add(
              indexB); // A and B are switched to give the current winding order
          triangles.add(indexA);
          triangles.add(indexC);
          indexList.removeAt(i);
          break;
        }
      }
    }
    // Store the last remaining indices as these must form the final triangle
    triangles.add(indexList[0]);
    triangles.add(indexList[1]);
    triangles.add(indexList[2]);
  }

  // Use pieces of a cross product to see if the angle between two vecs is convex
  // http://www.gamedev.net/topic/542870-determine-which-side-of-a-line-a-point-is/page__view__findpost__p__4500667
  bool isConvex(math.Vector3 a, math.Vector3 b, math.Vector3 c) {
    return ((a.x * (c.y - b.y)) + (b.x * (a.y - c.y)) + (c.x * (b.y - a.y))) <
        0;
  }

  // Check to see if a point is within a triangle
  bool inTriangle(math.Vector3 pointToCheck, math.Vector3 currentVert,
      math.Vector3 nextVert, math.Vector3 previousVert) {
    // Make sure we have no dupes in the mix
    if (pointToCheck == currentVert ||
        pointToCheck == nextVert ||
        pointToCheck == previousVert) {
      return false;
    }

    // Vector from current vert to previous
    math.Vector3 v0 = math.Vector3((previousVert.x - currentVert.x),
        (previousVert.y - currentVert.y), 0.0);

    // Vector from current vert to next
    math.Vector3 v1 = math.Vector3(
        (nextVert.x - currentVert.x), (nextVert.y - currentVert.y), 0.0);

    // Vector from current vert to point to check
    math.Vector3 v2 = math.Vector3((pointToCheck.x - currentVert.x),
        (pointToCheck.y - currentVert.y), 0.0);

    // Crazy math (found here; http://www.blackpawn.com/texts/pointinpoly/default.html)
    double u = (v1.dot(v1) * v2.dot(v0) - v1.dot(v0) * v2.dot(v1)) /
        (v0.dot(v0) * v1.dot(v1) - v0.dot(v1) * v1.dot(v0));
    double v = (v0.dot(v0) * v2.dot(v1) - v0.dot(v1) * v2.dot(v0)) /
        (v0.dot(v0) * v1.dot(v1) - v0.dot(v1) * v1.dot(v0));

    // u OR v < 0 = wrong direction from triangle and thus outside
    // u OR v > 1 = went past triangle and thus outside
    // u  + v > 1  = crossed diagonal of Vi+1 and Vi-1 and thus outside

    return !(u < 0 || v < 0 || u > 1 || v > 1 || (u + v) > 1);
  }

  int getItem(List<int> indexList, int index) {
    if (index < 0) {
      return indexList[index + indexList.length];
    }
    if (index >= indexList.length) {
      return indexList[index - indexList.length];
    }
    return indexList[index];
  }
}

// *****************************************************************************
// Block of code to handle the render of the polygons to show the results
// *****************************************************************************
class PolygonDisplay extends CustomPainter {
  Points points;
  List<int> triangles;

  PolygonDisplay(this.points, this.triangles);

  @override
  void paint(Canvas canvas, Size size) {
    final pointPaint = Paint()
      ..style = PaintingStyle.stroke
      ..strokeWidth = 2.0
      ..color = Colors.red;

    if (points.pointList.isEmpty == false) {
      final pointPath = Path();
      if (points.pointList.length > 1) {
        double lastX = points.pointList[0].x;
        double lastY = points.pointList[0].y;

        pointPath.moveTo(lastX, lastY);
        for (int i = 1; i < points.pointList.length; i++) {
          pointPath.relativeLineTo(
              points.pointList[i].x - lastX, points.pointList[i].y - lastY);
          lastX = points.pointList[i].x;
          lastY = points.pointList[i].y;
        }
        pointPath.close();
        canvas.drawPath(pointPath, pointPaint);
      }

      final pointPaint2 = Paint()
        ..style = PaintingStyle.fill
        ..strokeWidth = 3.0
        ..color = Colors.red;

      for (int i = 0; i < triangles.length; i += 3) {
        var pointPath2 = Path();

        List<math.Vector3> verts = points.pointList;

        math.Vector3 a = verts[triangles[i]];
        math.Vector3 b = verts[triangles[i + 1]];
        math.Vector3 c = verts[triangles[i + 2]];

        pointPath2.moveTo(a.x, a.y);
        pointPath2.relativeLineTo(b.x - a.x, b.y - a.y);
        pointPath2.relativeLineTo(c.x - b.x, c.y - b.y);
        pointPath2.close();

        pointPaint2.color =
            Color((dartmath.Random().nextDouble() * 0xFFFFFF).toInt())
                .withOpacity(1.0);

        canvas.drawPath(pointPath2, pointPaint2);
      }
    }
  }

  @override
  bool shouldRepaint(PolygonDisplay oldDelegate) => false;
}
