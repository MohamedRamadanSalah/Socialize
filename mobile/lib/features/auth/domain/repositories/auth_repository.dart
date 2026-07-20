import 'package:dartz/dartz.dart';

import '../entities/current_user.dart';
import '../failures/auth_failure.dart';

abstract class AuthRepository {
  Future<Either<AuthFailure, CurrentUser>> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  });

  Future<Either<AuthFailure, CurrentUser>> login({
    required String userNameOrEmail,
    required String password,
  });

  Future<Either<AuthFailure, Unit>> logout();

  Future<Either<AuthFailure, CurrentUser>> getCurrentUser();
}
