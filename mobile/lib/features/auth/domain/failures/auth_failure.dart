import 'package:freezed_annotation/freezed_annotation.dart';

part 'auth_failure.freezed.dart';

@freezed
sealed class AuthFailure with _$AuthFailure {
  const factory AuthFailure.invalidCredentials() = _InvalidCredentials;
  const factory AuthFailure.emailAlreadyInUse() = _EmailAlreadyInUse;
  const factory AuthFailure.usernameAlreadyInUse() = _UsernameAlreadyInUse;
  const factory AuthFailure.validation(Map<String, List<String>> errors) =
      _ValidationFailure;
  const factory AuthFailure.sessionExpired() = _SessionExpired;
  const factory AuthFailure.network() = _NetworkFailure;
  const factory AuthFailure.unknown(String message) = _UnknownFailure;
}
