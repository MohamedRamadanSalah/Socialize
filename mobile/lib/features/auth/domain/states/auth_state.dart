import '../entities/current_user.dart';

sealed class AuthState {}

final class AuthInitial extends AuthState {}

final class AuthUnauthenticated extends AuthState {}

final class AuthAuthenticated extends AuthState {
  final CurrentUser user;

  AuthAuthenticated(this.user);
}
