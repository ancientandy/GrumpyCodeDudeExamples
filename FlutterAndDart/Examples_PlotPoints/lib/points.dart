import 'dart:convert';
import 'package:vector_math/vector_math.dart';

class Points {
  List<Vector3> pointList = [];

  Points();

  void add(double x, double y, double z) {
    // If the list is null then initialise it with a filled list. If not then
    // add another point to the list
    pointList.add(Vector3(x, y, z));
  }

  // Handle json encode and decode
  Points.fromJson(Map<String, dynamic> json)
      : pointList = List<Vector3>.from(json['pointList']);
  Map<String, dynamic> toJson() => {'pointList': pointList};
}

// class Vector3 {
//   double x;
//   double y;
//   double z;

//   Vector3(this.x, this.y, this.z);

//   // Handle json decode and encode
//   Vector3.fromJson(Map<String, dynamic> json)
//       : x = json['x'],
//         y = json['y'],
//         z = json['z'];

//   Map<String, dynamic> toJson() => {'x': x, 'y': y, 'z': z};
// }
