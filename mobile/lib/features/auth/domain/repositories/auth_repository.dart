import 'package:dartz/dartz.dart';

import '../entities/current_user.dart';
import '../failures/auth_failure.dart';

// Repository interface for authentication-related operations
abstract class AuthRepository {
  // Registers a new user with the provided details
  Future<Either<AuthFailure, CurrentUser>> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  });

  // Logs in a user with the provided credentials
  Future<Either<AuthFailure, CurrentUser>> login({
    required String userNameOrEmail,
    required String password,
  });

  // Logs out the current user
  Future<Either<AuthFailure, Unit>> logout();
  // Retrieves the current logged-in user, if any
  Future<Either<AuthFailure, CurrentUser>> getCurrentUser();
}
