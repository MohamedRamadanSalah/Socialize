import 'package:flutter/material.dart';

// Slowly shifting linear gradient backdrop shared by the splash and onboarding screens.
class AnimatedGradientBackground extends StatefulWidget {
  const AnimatedGradientBackground({
    super.key,
    required this.colors,
    required this.child,
    this.duration = const Duration(seconds: 6),
  });

  final List<Color> colors;
  final Widget child;
  final Duration duration;

  @override
  State<AnimatedGradientBackground> createState() => _AnimatedGradientBackgroundState();
}

class _AnimatedGradientBackgroundState extends State<AnimatedGradientBackground>
    with SingleTickerProviderStateMixin {
  late final AnimationController _controller;
  late final Animation<Alignment> _begin;
  late final Animation<Alignment> _end;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(vsync: this, duration: widget.duration)
      ..repeat(reverse: true);
    final curve = CurvedAnimation(parent: _controller, curve: Curves.easeInOut);
    _begin = AlignmentTween(
      begin: Alignment.topLeft,
      end: Alignment.bottomLeft,
    ).animate(curve);
    _end = AlignmentTween(
      begin: Alignment.bottomRight,
      end: Alignment.topRight,
    ).animate(curve);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return AnimatedBuilder(
      animation: _controller,
      builder: (context, child) {
        return DecoratedBox(
          decoration: BoxDecoration(
            gradient: LinearGradient(
              begin: _begin.value,
              end: _end.value,
              colors: widget.colors,
            ),
          ),
          child: child,
        );
      },
      child: widget.child,
    );
  }
}
