import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:src/core/theme/app_theme.dart';
import 'package:src/core/widgets/animated_gradient_background.dart';
import 'package:src/core/widgets/fade_slide_in.dart';
import 'package:src/core/widgets/glow_orb.dart';

// Shared shell for the Register/Login screens: an animated gradient hero
// carrying the brand mark and a headline, above a rounded sheet that slides
// up to hold the form.
class AuthScreenScaffold extends StatefulWidget {
  const AuthScreenScaffold({
    super.key,
    required this.title,
    required this.subtitle,
    required this.body,
  });

  final String title;
  final String subtitle;
  final Widget body;

  @override
  State<AuthScreenScaffold> createState() => _AuthScreenScaffoldState();
}

class _AuthScreenScaffoldState extends State<AuthScreenScaffold>
    with SingleTickerProviderStateMixin {
  late final AnimationController _controller;
  late final Animation<double> _headerAnimation;
  late final Animation<double> _sheetAnimation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 900),
    );
    _headerAnimation = CurvedAnimation(
      parent: _controller,
      curve: const Interval(0, 0.6, curve: Curves.easeOutCubic),
    );
    _sheetAnimation = CurvedAnimation(
      parent: _controller,
      curve: const Interval(0.25, 1, curve: Curves.easeOutCubic),
    );
    _controller.forward();
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final colorScheme = Theme.of(context).colorScheme;

    return Scaffold(
      body: AnimatedGradientBackground(
        colors: AppColors.splashGradient,
        child: Stack(
          fit: StackFit.expand,
          children: [
            const GlowOrb(
              alignment: Alignment(-1.4, -1),
              diameter: 240,
              color: AppColors.tertiaryContainer,
            ),
            const GlowOrb(
              alignment: Alignment(1.4, -0.6),
              diameter: 200,
              color: AppColors.primaryFixed,
            ),
            SafeArea(
              bottom: false,
              child: Column(
                children: [
                  if (Navigator.of(context).canPop())
                    Align(
                      alignment: Alignment.centerLeft,
                      child: IconButton(
                        onPressed: () => context.pop(),
                        icon: const Icon(Icons.arrow_back, color: Colors.white),
                      ),
                    ),
                  Padding(
                    padding: const EdgeInsets.fromLTRB(24, 8, 24, 28),
                    child: FadeSlideIn(
                      animation: _headerAnimation,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Container(
                            width: 56,
                            height: 56,
                            decoration: BoxDecoration(
                              color: Colors.white.withValues(alpha: 0.15),
                              borderRadius: BorderRadius.circular(18),
                              border: Border.all(color: Colors.white.withValues(alpha: 0.25)),
                            ),
                            child: const Icon(Icons.hub_rounded, color: Colors.white, size: 30),
                          ),
                          const SizedBox(height: 20),
                          Text(
                            widget.title,
                            style: Theme.of(context).textTheme.headlineLarge?.copyWith(
                              color: Colors.white,
                              fontWeight: FontWeight.w600,
                            ),
                          ),
                          const SizedBox(height: 8),
                          Text(
                            widget.subtitle,
                            style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                              color: Colors.white.withValues(alpha: 0.85),
                            ),
                          ),
                        ],
                      ),
                    ),
                  ),
                  Expanded(
                    child: FadeSlideIn(
                      animation: _sheetAnimation,
                      offset: const Offset(0, 0.15),
                      child: Container(
                        width: double.infinity,
                        decoration: BoxDecoration(
                          color: colorScheme.surface,
                          borderRadius: const BorderRadius.vertical(top: Radius.circular(32)),
                        ),
                        padding: const EdgeInsets.fromLTRB(24, 32, 24, 16),
                        child: widget.body,
                      ),
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
      ),
    );
  }
}
