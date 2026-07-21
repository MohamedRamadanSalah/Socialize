import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:src/features/auth/presentation/controllers/auth_controller.dart';

// Placeholder destination for authenticated users coming from the splash screen.
class HomeScreen extends ConsumerWidget {
  const HomeScreen({super.key});

  static const path = '/home';
  static const routeName = 'home';

  @override
  Widget build(BuildContext context, WidgetRef ref) {
    final authState = ref.watch(authControllerProvider);

    return Scaffold(
      body: Center(
        child: authState.maybeWhen(
          authenticated: (user) => Text(
            'Welcome, ${user.displayName}',
            style: Theme.of(context).textTheme.headlineMedium,
          ),
          orElse: () => Text('Home', style: Theme.of(context).textTheme.headlineMedium),
        ),
      ),
    );
  }
}
