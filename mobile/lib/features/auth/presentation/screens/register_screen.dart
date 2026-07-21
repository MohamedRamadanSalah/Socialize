import 'package:flutter/material.dart';
import 'package:flutter_riverpod/flutter_riverpod.dart';
import 'package:go_router/go_router.dart';
import 'package:src/core/widgets/auth_screen_scaffold.dart';
import 'package:src/core/widgets/auth_text_field.dart';
import 'package:src/core/widgets/gradient_button.dart';
import 'package:src/features/auth/domain/failures/auth_failure.dart';
import 'package:src/features/auth/presentation/controllers/auth_controller.dart';
import 'package:src/features/auth/presentation/screens/login_screen.dart';
import 'package:src/features/home/presentation/screens/home_screen.dart';

class RegisterScreen extends ConsumerStatefulWidget {
  const RegisterScreen({super.key});

  static const path = '/register';
  static const routeName = 'register';

  @override
  ConsumerState<RegisterScreen> createState() => _RegisterScreenState();
}

class _RegisterScreenState extends ConsumerState<RegisterScreen> {
  static final _emailPattern = RegExp(r'^[\w.+-]+@[\w-]+\.[\w.-]+$');
  static final _userNamePattern = RegExp(r'^[a-zA-Z0-9_]{3,20}$');

  final _formKey = GlobalKey<FormState>();
  final _fullNameController = TextEditingController();
  final _userNameController = TextEditingController();
  final _emailController = TextEditingController();
  final _passwordController = TextEditingController();
  final _confirmPasswordController = TextEditingController();

  bool _obscurePassword = true;
  bool _obscureConfirmPassword = true;

  @override
  void dispose() {
    _fullNameController.dispose();
    _userNameController.dispose();
    _emailController.dispose();
    _passwordController.dispose();
    _confirmPasswordController.dispose();
    super.dispose();
  }

  String _authFailureMessage(AuthFailure failure) {
    return failure.when(
      invalidCredentials: () => 'Invalid credentials.',
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
        .register(
          userName: _userNameController.text.trim(),
          email: _emailController.text.trim(),
          password: _passwordController.text,
          displayName: _fullNameController.text.trim(),
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
      title: 'Create account',
      subtitle: 'Join Nexus and start connecting.',
      body: SingleChildScrollView(
        child: Form(
          key: _formKey,
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.stretch,
            children: [
              AuthTextField(
                controller: _fullNameController,
                label: 'Full name',
                hint: 'Enter your full name',
                icon: Icons.person_outline,
                textInputAction: TextInputAction.next,
                validator: (value) {
                  if (value == null || value.trim().length < 2) {
                    return 'Enter your full name';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              AuthTextField(
                controller: _userNameController,
                label: 'Username',
                hint: 'Choose a username',
                icon: Icons.alternate_email,
                textInputAction: TextInputAction.next,
                autocorrect: false,
                validator: (value) {
                  if (value == null || !_userNamePattern.hasMatch(value)) {
                    return '3-20 letters, numbers, or underscores';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              AuthTextField(
                controller: _emailController,
                label: 'Email',
                hint: 'email@example.com',
                icon: Icons.mail_outline,
                keyboardType: TextInputType.emailAddress,
                textInputAction: TextInputAction.next,
                validator: (value) {
                  if (value == null || !_emailPattern.hasMatch(value)) {
                    return 'Enter a valid email address';
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
                textInputAction: TextInputAction.next,
                suffixIcon: IconButton(
                  icon: Icon(
                    _obscurePassword
                        ? Icons.visibility_outlined
                        : Icons.visibility_off_outlined,
                  ),
                  onPressed: () => setState(() => _obscurePassword = !_obscurePassword),
                ),
                validator: (value) {
                  if (value == null || value.length < 8) {
                    return 'Password must be at least 8 characters';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 16),
              AuthTextField(
                controller: _confirmPasswordController,
                label: 'Confirm password',
                hint: '••••••••',
                icon: Icons.lock_clock_outlined,
                obscureText: _obscureConfirmPassword,
                textInputAction: TextInputAction.done,
                onFieldSubmitted: (_) => _submit(),
                suffixIcon: IconButton(
                  icon: Icon(
                    _obscureConfirmPassword
                        ? Icons.visibility_outlined
                        : Icons.visibility_off_outlined,
                  ),
                  onPressed: () =>
                      setState(() => _obscureConfirmPassword = !_obscureConfirmPassword),
                ),
                validator: (value) {
                  if (value != _passwordController.text) {
                    return 'Passwords do not match';
                  }
                  return null;
                },
              ),
              const SizedBox(height: 28),
              GradientButton(
                label: 'Create account',
                isLoading: isLoading,
                onPressed: isLoading ? null : _submit,
              ),
              const SizedBox(height: 24),
              Center(
                child: TextButton(
                  onPressed: () => context.push(LoginScreen.path),
                  child: Text.rich(
                    TextSpan(
                      text: 'Already have an account? ',
                      style: TextStyle(color: Theme.of(context).colorScheme.onSurfaceVariant),
                      children: [
                        TextSpan(
                          text: 'Login',
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
