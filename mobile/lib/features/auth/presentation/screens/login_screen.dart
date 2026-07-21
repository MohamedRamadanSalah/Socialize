import 'package:flutter/material.dart';
import 'package:go_router/go_router.dart';
import 'package:src/features/auth/presentation/screens/register_screen.dart';

// Placeholder destination for unauthenticated users coming from the splash screen.
class LoginScreen extends StatelessWidget {
  const LoginScreen({super.key});

  static const path = '/login';
  static const routeName = 'login';

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text('Login', style: Theme.of(context).textTheme.headlineMedium),
            TextButton(
              onPressed: () => context.push(RegisterScreen.path),
              child: const Text("Don't have an account? Create one"),
            ),
          ],
        ),
      ),
    );
  }
}
