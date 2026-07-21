import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:src/features/auth/presentation/screens/login_screen.dart';
import 'package:src/features/auth/presentation/screens/register_screen.dart';
import 'package:src/features/home/presentation/screens/home_screen.dart';
import 'package:src/features/onboarding/presentation/screens/onboarding_screen.dart';
import 'package:src/features/splash/presentation/screens/splash_screen.dart';

final appRouterProvider = Provider<GoRouter>((ref) {
  return GoRouter(
    initialLocation: SplashScreen.path,
    routes: [
      GoRoute(
        path: SplashScreen.path,
        name: SplashScreen.routeName,
        pageBuilder: (context, state) => _fadeTransitionPage(const SplashScreen(), state),
      ),
      GoRoute(
        path: OnboardingScreen.path,
        name: OnboardingScreen.routeName,
        pageBuilder: (context, state) =>
            _fadeTransitionPage(const OnboardingScreen(), state),
      ),
      GoRoute(
        path: LoginScreen.path,
        name: LoginScreen.routeName,
        pageBuilder: (context, state) => _fadeTransitionPage(const LoginScreen(), state),
      ),
      GoRoute(
        path: RegisterScreen.path,
        name: RegisterScreen.routeName,
        pageBuilder: (context, state) => _fadeTransitionPage(const RegisterScreen(), state),
      ),
      GoRoute(
        path: HomeScreen.path,
        name: HomeScreen.routeName,
        pageBuilder: (context, state) => _fadeTransitionPage(const HomeScreen(), state),
      ),
    ],
  );
});

CustomTransitionPage<void> _fadeTransitionPage(Widget child, GoRouterState state) {
  return CustomTransitionPage<void>(
    key: state.pageKey,
    child: child,
    transitionDuration: const Duration(milliseconds: 400),
    transitionsBuilder: (context, animation, secondaryAnimation, child) {
      return FadeTransition(opacity: animation, child: child);
    },
  );
}
