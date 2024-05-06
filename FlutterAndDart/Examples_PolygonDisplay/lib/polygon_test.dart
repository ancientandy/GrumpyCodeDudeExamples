import 'package:flutter/material.dart';

class PolygonTest extends CustomPainter {
  final double _width;
  final double _height;

  PolygonTest(this._width, this._height);

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..style = PaintingStyle.fill
      ..strokeWidth = 2.0
      ..color = Colors.black;

    // Create a polygon path
    final polygon = Path();

    polygon.moveTo(_width / 2, _height / 2);
    polygon.relativeLineTo(100, 100);
    polygon.relativeLineTo(-200, 0);
    polygon.close();

    canvas.drawPath(polygon, paint);
  }

  @override
  bool shouldRepaint(PolygonTest oldDelegate) => false;
}
