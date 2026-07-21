import 'package:flutter/material.dart';

// Fades and slides content upward into place, driven by an externally owned
// animation (e.g. an Interval of a shared staggered AnimationController).
class FadeSlideIn extends StatelessWidget {
  const FadeSlideIn({
    super.key,
    required this.animation,
    required this.child,
    this.offset = const Offset(0, 0.08),
  });

  final Animation<double> animation;
  final Widget child;
  final Offset offset;

  @override
  Widget build(BuildContext context) {
    return FadeTransition(
      opacity: animation,
      child: SlideTransition(
        position: Tween<Offset>(begin: offset, end: Offset.zero).animate(animation),
        child: child,
      ),
    );
  }
}
