import 'package:dartz/dartz.dart';
import 'package:src/features/auth/domain/failures/auth_failure.dart';

import '../Entities/Current_User.dart';

abstract interface class AuthRepository {
  Future<Either<AuthFailure, CurrentUser>> login({
    required String userNameOrEmail,
    required String password,
  });

  Future<Either<AuthFailure, CurrentUser>> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  });

  Future<CurrentUser?> getCurrentSession();

  Future<void> logout();
}
