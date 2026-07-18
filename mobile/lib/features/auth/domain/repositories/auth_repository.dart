import '../Entities/Current_User.dart';

abstract interface class AuthRepository {
  Future<CurrentUser> login({
    required String userNameOrEmail,
    required String password,
  });

  Future<CurrentUser> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  });

  Future<CurrentUser?> getCurrentSession();

  Future<void> logout();
}
