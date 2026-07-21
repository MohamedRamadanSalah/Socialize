import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:src/core/theme/app_theme.dart';
import 'package:src/core/widgets/animated_gradient_background.dart';
import 'package:src/core/widgets/glow_orb.dart';
import 'package:src/features/auth/presentation/screens/login_screen.dart';
import 'package:src/features/auth/presentation/screens/register_screen.dart';

class OnboardingScreen extends StatefulWidget {
  const OnboardingScreen({super.key});

  static const path = '/onboarding';
  static const routeName = 'onboarding';

  @override
  State<OnboardingScreen> createState() => _OnboardingScreenState();
}

class _OnboardingScreenState extends State<OnboardingScreen>
    with SingleTickerProviderStateMixin {
  late final AnimationController _controller;

  late final Animation<double> _iconAnimation;
  late final Animation<double> _titleAnimation;
  late final Animation<double> _subtitleAnimation;
  late final Animation<double> _actionsAnimation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1400),
    );

    _iconAnimation = _staggered(0, 0.55);
    _titleAnimation = _staggered(0.15, 0.7);
    _subtitleAnimation = _staggered(0.3, 0.85);
    _actionsAnimation = _staggered(0.5, 1);

    _controller.forward();
  }

  Animation<double> _staggered(double start, double end) {
    return CurvedAnimation(
      parent: _controller,
      curve: Interval(start, end, curve: Curves.easeOutCubic),
    );
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: AnimatedGradientBackground(
        colors: AppColors.splashGradient,
        child: Stack(
          fit: StackFit.expand,
          children: [
            const GlowOrb(
              alignment: Alignment(1.3, -1.2),
              diameter: 280,
              color: AppColors.tertiaryContainer,
            ),
            const GlowOrb(
              alignment: Alignment(-1.3, 1.3),
              diameter: 260,
              color: AppColors.primaryFixed,
            ),
            SafeArea(
              child: Padding(
                padding: const EdgeInsets.symmetric(horizontal: 24, vertical: 32),
                child: Column(
                  children: [
                    const Spacer(),
                    _FadeSlideIn(
                      animation: _iconAnimation,
                      child: Container(
                        width: 96,
                        height: 96,
                        decoration: BoxDecoration(
                          color: Colors.white.withValues(alpha: 0.15),
                          borderRadius: BorderRadius.circular(24),
                          border: Border.all(color: Colors.white.withValues(alpha: 0.25)),
                        ),
                        child: const Icon(Icons.hub_rounded, size: 52, color: Colors.white),
                      ),
                    ),
                    const SizedBox(height: 32),
                    _FadeSlideIn(
                      animation: _titleAnimation,
                      child: Text(
                        'Welcome to Nexus',
                        textAlign: TextAlign.center,
                        style: Theme.of(context).textTheme.headlineLarge?.copyWith(
                          color: Colors.white,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
                    const SizedBox(height: 12),
                    _FadeSlideIn(
                      animation: _subtitleAnimation,
                      child: Text(
                        'Where every connection matters. Join the network built for real conversations.',
                        textAlign: TextAlign.center,
                        style: Theme.of(context).textTheme.bodyLarge?.copyWith(
                          color: Colors.white.withValues(alpha: 0.8),
                        ),
                      ),
                    ),
                    const Spacer(flex: 2),
                    _FadeSlideIn(
                      animation: _actionsAnimation,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.stretch,
                        children: [
                          FilledButton(
                            onPressed: () => context.push(RegisterScreen.path),
                            style: FilledButton.styleFrom(
                              backgroundColor: Colors.white,
                              foregroundColor: AppColors.primary,
                              padding: const EdgeInsets.symmetric(vertical: 16),
                              shape: const StadiumBorder(),
                            ),
                            child: const Row(
                              mainAxisSize: MainAxisSize.min,
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                Text('Get Started'),
                                SizedBox(width: 8),
                                Icon(Icons.arrow_forward, size: 20),
                              ],
                            ),
                          ),
                          const SizedBox(height: 16),
                          TextButton(
                            onPressed: () => context.push(LoginScreen.path),
                            style: TextButton.styleFrom(foregroundColor: Colors.white),
                            child: const Text('I already have an account · Log in'),
                          ),
                        ],
                      ),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _FadeSlideIn extends StatelessWidget {
  const _FadeSlideIn({required this.animation, required this.child});

  final Animation<double> animation;
  final Widget child;

  @override
  Widget build(BuildContext context) {
    return FadeTransition(
      opacity: animation,
      child: SlideTransition(
        position: Tween<Offset>(
          begin: const Offset(0, 0.08),
          end: Offset.zero,
        ).animate(animation),
        child: child,
      ),
    );
  }
}
