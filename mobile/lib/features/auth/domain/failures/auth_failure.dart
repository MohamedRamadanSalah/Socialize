sealed class AuthFailure {}

final class InvalidCredentialsFailure extends AuthFailure {}

final class UsernameAlreadyExistsFailure extends AuthFailure {}

final class EmailAlreadyExistsFailure extends AuthFailure {}

final class NetworkFailure extends AuthFailure {}

final class UnknownFailure extends AuthFailure {}
