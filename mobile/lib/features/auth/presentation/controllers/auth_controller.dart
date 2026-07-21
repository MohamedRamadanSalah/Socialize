import 'package:freezed_annotation/freezed_annotation.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:src/features/auth/data/repositories/auth_repository_impl.dart';
import 'package:src/features/auth/domain/entities/current_user.dart';
import 'package:src/features/auth/domain/failures/auth_failure.dart';

part 'auth_controller.freezed.dart';
part 'auth_controller.g.dart';

// Controller for managing authentication state
@freezed
sealed class AuthState with _$AuthState {
  const factory AuthState.initial() = _Initial;
  const factory AuthState.loading() = _Loading;
  const factory AuthState.authenticated(CurrentUser user) = _Authenticated;
  const factory AuthState.unauthenticated() = _Unauthenticated;
  const factory AuthState.error(AuthFailure failure) = _Error;
}

@Riverpod(keepAlive: true)
class AuthController extends _$AuthController {
  @override
  AuthState build() {
    return const AuthState.initial();
  }

  // Method to check the current authentication status of the user
  Future<void> checkAuthStatus() async {
    state = const AuthState.loading();
    final repository = ref.read(authRepositoryProvider);
    final result = await repository.getCurrentUser();

    result.fold(
      (failure) => state = const AuthState.unauthenticated(),
      (user) => state = AuthState.authenticated(user),
    );
  }

  // Method to handle user login
  Future<void> login({
    required String userNameOrEmail,
    required String password,
  }) async {
    state = const AuthState.loading();
    final repository = ref.read(authRepositoryProvider);
    final result = await repository.login(
      userNameOrEmail: userNameOrEmail,
      password: password,
    );

    result.fold(
      (failure) => state = AuthState.error(failure),
      (user) => state = AuthState.authenticated(user),
    );
  }

  // Method to handle user registration
  Future<void> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  }) async {
    state = const AuthState.loading();
    final repository = ref.read(authRepositoryProvider);
    final result = await repository.register(
      userName: userName,
      email: email,
      password: password,
      displayName: displayName,
    );

    result.fold(
      (failure) => state = AuthState.error(failure),
      (user) => state = AuthState.authenticated(user),
    );
  }

  // Method to handle user logout
  Future<void> logout() async {
    final repository = ref.read(authRepositoryProvider);
    await repository.logout();
    state = const AuthState.unauthenticated();
  }
}
