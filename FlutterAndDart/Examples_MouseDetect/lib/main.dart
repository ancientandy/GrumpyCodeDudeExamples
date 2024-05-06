import 'package:flutter/material.dart';
import 'dart:developer' as debug;

import 'constants.dart' as constants;

void main() {
  runApp(const MyApp());
}

class MyApp extends StatelessWidget {
  const MyApp({super.key});

  // This widget is the root of your application.
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'User Tap/Mouse Demo',
      theme: ThemeData(
        primarySwatch: Colors.blue,
      ),
      home: const MyHomePage(title: 'User Tap/Mouse Input Test'),
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
    });
  }

  void _inputReleased(PointerEvent details) {
    setState(() {
      _inputPositionChange(details);
      _pressedText = constants.notPressed;
      debug.log(_pressedText);
    });
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
                child: Listener(
                  onPointerDown: _inputPressed,
                  onPointerMove: _inputPositionChange,
                  onPointerUp: _inputReleased,
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
                          Transform(
                              transform: Matrix4.translationValues(
                                  _x - 16, _y - 16, 0.0),
                              child: const CircleAvatar(
                                radius: 16,
                              ))
                        ]),
                      ),
                    ),
                  ),
                ),
              ),
            ),
            Expanded(
              flex: 1,
              child: Container(
                  width: width,
                  margin: margin, //variable
                  color: const Color.fromARGB(255, 10, 40, 53), //variable
                  child: Text(
                      "X: ${_x.toStringAsFixed(4)}, Y: ${_y.toStringAsFixed(4)}\nFocus: $_focusText\nButton/Tap: $_pressedText",
                      style: const TextStyle(
                          color: Color.fromARGB(255, 255, 255, 255)))),
            )
          ]),
        ));
  }
}
