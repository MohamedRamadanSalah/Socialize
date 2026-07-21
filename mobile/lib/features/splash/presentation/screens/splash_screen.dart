import 'dart:ui';

import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:src/core/theme/app_theme.dart';
import 'package:src/core/widgets/animated_gradient_background.dart';
import 'package:src/core/widgets/glow_orb.dart';
import 'package:src/features/auth/presentation/controllers/auth_controller.dart';
import 'package:src/features/home/presentation/screens/home_screen.dart';
import 'package:src/features/onboarding/presentation/screens/onboarding_screen.dart';

class SplashScreen extends ConsumerStatefulWidget {
  const SplashScreen({super.key});

  static const path = '/';
  static const routeName = 'splash';

  @override
  ConsumerState<SplashScreen> createState() => _SplashScreenState();
}

class _SplashScreenState extends ConsumerState<SplashScreen>
    with TickerProviderStateMixin {
  static const _minimumDisplay = Duration(milliseconds: 2600);

  late final AnimationController _entranceController;
  late final AnimationController _floatController;

  late final Animation<double> _logoOpacity;
  late final Animation<Offset> _logoOffset;
  late final Animation<double> _float;

  @override
  void initState() {
    super.initState();

    _entranceController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1200),
    );
    final entranceCurve = CurvedAnimation(
      parent: _entranceController,
      curve: const Cubic(0.22, 1, 0.36, 1),
    );
    _logoOpacity = Tween<double>(begin: 0, end: 1).animate(entranceCurve);
    _logoOffset = Tween<Offset>(
      begin: const Offset(0, 0.05),
      end: Offset.zero,
    ).animate(entranceCurve);

    _floatController = AnimationController(
      vsync: this,
      duration: const Duration(seconds: 3),
    )..repeat(reverse: true);
    _float = Tween<double>(begin: -6, end: 6).animate(
      CurvedAnimation(parent: _floatController, curve: Curves.easeInOut),
    );

    _entranceController.forward();
    // Defer past the current build phase: checkAuthStatus() sets provider state
    // synchronously, which Riverpod forbids while the widget tree is still building.
    WidgetsBinding.instance.addPostFrameCallback((_) => _startAuthFlow());
  }

  Future<void> _startAuthFlow() async {
    final minimumDisplay = Future.delayed(_minimumDisplay);
    final authCheck = ref.read(authControllerProvider.notifier).checkAuthStatus();
    await Future.wait([minimumDisplay, authCheck]);

    if (!mounted) return;

    final isAuthenticated = ref.read(authControllerProvider).maybeWhen(
      authenticated: (_) => true,
      orElse: () => false,
    );

    context.go(isAuthenticated ? HomeScreen.path : OnboardingScreen.path);
  }

  @override
  void dispose() {
    _entranceController.dispose();
    _floatController.dispose();
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
              alignment: Alignment(-1.3, -1.2),
              diameter: 260,
              color: AppColors.primaryContainer,
            ),
            const GlowOrb(
              alignment: Alignment(1.3, 1.2),
              diameter: 320,
              color: AppColors.tertiaryContainer,
            ),
            SafeArea(
              child: Padding(
                padding: const EdgeInsets.symmetric(vertical: 48, horizontal: 16),
                child: Column(
                  children: [
                    const Spacer(),
                    FadeTransition(
                      opacity: _logoOpacity,
                      child: SlideTransition(
                        position: _logoOffset,
                        child: AnimatedBuilder(
                          animation: _float,
                          builder: (context, child) {
                            return Transform.translate(
                              offset: Offset(0, _float.value),
                              child: child,
                            );
                          },
                          child: const _BrandCluster(),
                        ),
                      ),
                    ),
                    const Spacer(),
                    const _SyncingFooter(),
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

class _BrandCluster extends StatelessWidget {
  const _BrandCluster();

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(28),
          child: BackdropFilter(
            filter: ImageFilter.blur(sigmaX: 12, sigmaY: 12),
            child: Container(
              width: 128,
              height: 128,
              decoration: BoxDecoration(
                color: Colors.white.withValues(alpha: 0.12),
                borderRadius: BorderRadius.circular(28),
                border: Border.all(color: Colors.white.withValues(alpha: 0.25)),
                boxShadow: [
                  BoxShadow(
                    color: Colors.black.withValues(alpha: 0.25),
                    blurRadius: 32,
                    offset: const Offset(0, 16),
                  ),
                ],
              ),
              child: const Center(
                child: Icon(Icons.hub_rounded, size: 72, color: Colors.white),
              ),
            ),
          ),
        ),
        const SizedBox(height: 24),
        Text(
          'Nexus',
          style: Theme.of(context).textTheme.headlineLarge?.copyWith(
            color: Colors.white,
            fontWeight: FontWeight.w600,
            letterSpacing: -0.5,
            shadows: [
              Shadow(color: Colors.black.withValues(alpha: 0.25), blurRadius: 6),
            ],
          ),
        ),
        const SizedBox(height: 8),
        Text(
          'Connect. Explore. Create.',
          style: Theme.of(context).textTheme.bodyMedium?.copyWith(
            color: Colors.white.withValues(alpha: 0.8),
            letterSpacing: 0.5,
          ),
        ),
      ],
    );
  }
}

class _SyncingFooter extends StatelessWidget {
  const _SyncingFooter();

  @override
  Widget build(BuildContext context) {
    return Column(
      mainAxisSize: MainAxisSize.min,
      children: [
        const SizedBox(
          width: 24,
          height: 24,
          child: CircularProgressIndicator(
            strokeWidth: 3,
            valueColor: AlwaysStoppedAnimation(Colors.white),
          ),
        ),
        const SizedBox(height: 16),
        Text(
          'SYNCING UNIVERSE',
          style: Theme.of(context).textTheme.labelMedium?.copyWith(
            color: Colors.white.withValues(alpha: 0.6),
            letterSpacing: 3,
          ),
        ),
        const SizedBox(height: 32),
        Text(
          'Version 1.0.0 © 2026 Nexus Social',
          style: Theme.of(context).textTheme.labelMedium?.copyWith(
            color: Colors.white.withValues(alpha: 0.4),
          ),
        ),
      ],
    );
  }
}
