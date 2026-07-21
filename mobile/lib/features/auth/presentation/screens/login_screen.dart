import 'package:flutter/material.dart';

// Placeholder destination for unauthenticated users coming from the splash screen.
class LoginScreen extends StatelessWidget {
  const LoginScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Center(
        child: Text('Login', style: Theme.of(context).textTheme.headlineMedium),
      ),
    );
  }
}
