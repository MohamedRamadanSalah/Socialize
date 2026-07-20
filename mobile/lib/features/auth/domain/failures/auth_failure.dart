import 'package:freezed_annotation/freezed_annotation.dart';

part 'auth_failure.freezed.dart';

// Failure types for authentication-related errors
@freezed
sealed class AuthFailure with _$AuthFailure {
  // Represents a failure due to invalid credentials (e.g., wrong password)
  const factory AuthFailure.invalidCredentials() = _InvalidCredentials;
  // Represents a failure when the email is already in use
  const factory AuthFailure.emailAlreadyInUse() = _EmailAlreadyInUse;
  // Represents a failure when the username is already in use
  const factory AuthFailure.usernameAlreadyInUse() = _UsernameAlreadyInUse;
  // Represents a failure due to validation errors
  const factory AuthFailure.validation(Map<String, List<String>> errors) =
      _ValidationFailure;
  // Represents a failure when the session has expired
  const factory AuthFailure.sessionExpired() = _SessionExpired;
  // Represents a failure due to network issues
  const factory AuthFailure.network() = _NetworkFailure;
  // Represents an unknown failure
  const factory AuthFailure.unknown(String message) = _UnknownFailure;
}
