import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:src/core/widgets/auth_screen_scaffold.dart';
import 'package:src/core/widgets/auth_text_field.dart';
import 'package:src/core/widgets/gradient_button.dart';
import 'package:src/features/auth/domain/failures/auth_failure.dart';
import 'package:src/features/auth/presentation/controllers/auth_controller.dart';
import 'package:src/features/auth/presentation/screens/register_screen.dart';
import 'package:src/features/home/presentation/screens/home_screen.dart';

class LoginScreen extends ConsumerStatefulWidget {
  const LoginScreen({super.key});

  static const path = '/login';
  static const routeName = 'login';

  @override
  ConsumerState<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends ConsumerState<LoginScreen> {
  final _formKey = GlobalKey<FormState>();
  final _identifierController = TextEditingController();
  final _passwordController = TextEditingController();

  bool _obscurePassword = true;

  @override
  void dispose() {
    _identifierController.dispose();
    _passwordController.dispose();
    super.dispose();
  }

  String _authFailureMessage(AuthFailure failure) {
    return failure.when(
      invalidCredentials: () => 'Incorrect username/email or password.',
      emailAlreadyInUse: () => 'This email is already in use.',
      usernameAlreadyInUse: () => 'This username is already taken.',
      validation: (errors) => errors.values.expand((e) => e).first,
      sessionExpired: () => 'Your session has expired. Please try again.',
      network: () => 'Network error. Check your connection and try again.',
      unknown: (message) => message,
    );
  }

  void _submit() {
    if (!_formKey.currentState!.validate()) return;

    ref
        .read(authControllerProvider.notifier)
        .login(
          userNameOrEmail: _identifierController.text.trim(),
          password: _passwordController.text,
        );
  }

  @override
  Widget build(BuildContext context) {
    final isLoading = ref
        .watch(authControllerProvider)
        .maybeWhen(loading: () => true, orElse: () => false);

    ref.listen<AuthState>(authControllerProvider, (previous, next) {
      next.whenOrNull(
        authenticated: (_) => context.go(HomeScreen.path),
        error: (failure) {
          ScaffoldMessenger.of(context)
            ..hideCurrentSnackBar()
            ..showSnackBar(SnackBar(content: Text(_authFailureMessage(failure))));
        },
      );
    });

    return AuthScreenScaffold(
      title: 'Welcome back',
      subtitle: 'Log in to keep up with your network.',
      body: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AuthTextField(
                controller: _identifierController,
                label: 'Username or email',
                hint: 'Enter your username or email',
                icon: Icons.person_outline,
                textInputAction: TextInputAction.next,
                validator: (value) {
                  if (value == null || value.trim().isEmpty) {
                    return 'Enter your username or email';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              AuthTextField(
                controller: _passwordController,
                label: 'Password',
                hint: '••••••••',
                icon: Icons.lock_outline,
                obscureText: _obscurePassword,
                textInputAction: TextInputAction.done,
                onFieldSubmitted: (_) => _submit(),
                suffixIcon: IconButton(
                  icon: Icon(
                    _obscurePassword
                        ? Icons.visibility_outlined
                        : Icons.visibility_off_outlined,
                  ),
                  onPressed: () => setState(() => _obscurePassword = !_obscurePassword),
                ),
                validator: (value) {
                  if (value == null || value.isEmpty) {
                    return 'Enter your password';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 28),
              GradientButton(
                label: 'Log in',
                isLoading: isLoading,
                onPressed: isLoading ? null : _submit,
              ),
              const SizedBox(height: 24),
              Center(
                child: TextButton(
                  onPressed: () => context.push(RegisterScreen.path),
                  child: Text.rich(
                    TextSpan(
                      text: "Don't have an account? ",
                      style: TextStyle(color: Theme.of(context).colorScheme.onSurfaceVariant),
                      children: [
                        TextSpan(
                          text: 'Create one',
                          style: TextStyle(
                            color: Theme.of(context).colorScheme.primary,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
