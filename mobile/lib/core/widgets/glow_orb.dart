import 'dart:ui';

import 'package:flutter/material.dart';

// Soft blurred color blob used as decorative background polish.
class GlowOrb extends StatelessWidget {
  const GlowOrb({
    super.key,
    required this.alignment,
    required this.diameter,
    required this.color,
  });

  final Alignment alignment;
  final double diameter;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return Align(
      alignment: alignment,
      child: ImageFiltered(
        imageFilter: ImageFilter.blur(sigmaX: 80, sigmaY: 80),
        child: Container(
          width: diameter,
          height: diameter,
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: color.withValues(alpha: 0.35),
          ),
        ),
      ),
    );
  }
}
