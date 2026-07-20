import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:src/core/network/exceptions/api_exception.dart';
import 'package:src/core/storage/token_storage.dart';
import 'package:src/features/auth/data/datasources/auth_remote_data_source.dart';
import 'package:src/features/auth/data/mappers/current_user_mapper.dart';
import 'package:src/features/auth/domain/entities/current_user.dart';
import 'package:src/features/auth/domain/failures/auth_failure.dart';
import 'package:src/features/auth/domain/repositories/auth_repository.dart';

part 'auth_repository_impl.g.dart';

class AuthRepositoryImpl implements AuthRepository {
  // The AuthRepositoryImpl class is responsible for implementing the AuthRepository interface.
  // It takes an instance of AuthRemoteDataSource and TokenStorage as dependencies.
  AuthRepositoryImpl(this._remoteDataSource, this._tokenStorage);

  // The _remoteDataSource is used to interact with the remote authentication API,
  // while the _tokenStorage is used to store and retrieve authentication tokens securely.
  final AuthRemoteDataSource _remoteDataSource;
  final TokenStorage _tokenStorage;
  // The getCurrentUser method is responsible for
  //retrieving the current logged-in user, if any.
  @override
  Future<Either<AuthFailure, CurrentUser>> getCurrentUser() async {
    try {
      final dto = await _remoteDataSource.getMe();
      return Right(dto.toEntity());
    } on DioException catch (e) {
      return Left(_mapGetCurrentUserError(e));
    }
  }

  // The login method is responsible for logging in a user with the provided
  // username/email and password.
  @override
  Future<Either<AuthFailure, CurrentUser>> login({
    required String userNameOrEmail,
    required String password,
  }) async {
    try {
      // Call the login method of the AuthRemoteDataSource to perform the login operation
      final response = await _remoteDataSource.login(
        userNameOrEmail: userNameOrEmail,
        password: password,
      );
      // Save the access and refresh tokens securely using the TokenStorage
      await _tokenStorage.saveTokens(
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
      );
      // Convert the CurrentUserDto to a CurrentUser entity and return it wrapped in a Right
      return Right(response.user.toEntity());
    } on DioException catch (e) {
      return Left(_mapLoginError(e));
    }
  }

  // The logout method is responsible for logging out the current
  //user by invalidating the refresh token and deleting the stored tokens.
  @override
  Future<Either<AuthFailure, Unit>> logout() async {
    try {
      final refreshToken = await _tokenStorage.getRefreshToken();
      if (refreshToken != null) {
        await _remoteDataSource.logout(refreshToken: refreshToken);
      }
    } on DioException catch (_) {
      // Ignored on purpose: we still log the user out locally below,
      // even if the server couldn't be reached.
    }
    await _tokenStorage.deleteTokens();
    return const Right(unit);
  }

  // The register method is responsible for registering a new user with the provided details.
  @override
  Future<Either<AuthFailure, CurrentUser>> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  }) async {
    try {
      final response = await _remoteDataSource.register(
        userName: userName,
        email: email,
        password: password,
        displayName: displayName,
      );
      // Save the access and refresh tokens securely using the TokenStorage
      await _tokenStorage.saveTokens(
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
      );
      // Convert the CurrentUserDto to a CurrentUser entity and return it wrapped in a Right
      return Right(response.user.toEntity());
    } on DioException catch (e) {
      // If a DioException occurs during the registration process, map the error to an AuthFailure
      return Left(_mapRegisterError(e));
    }
  }

  // The _mapRegisterError method maps DioExceptions to AuthFailure instances
  //based on the error details.
  AuthFailure _mapRegisterError(DioException exception) {
    final apiException = ApiException.fromDioException(exception);
    // Check the status code and error details to determine the appropriate AuthFailure to return.
    if (apiException.status == 409) {
      if (apiException.errors?.containsKey('email') ?? false) {
        return const AuthFailure.emailAlreadyInUse();
      }
      if (apiException.errors?.containsKey('userName') ?? false) {
        return const AuthFailure.usernameAlreadyInUse();
      }
    }
    // Check for validation errors and return an AuthFailure.validation instance if applicable.
    if (apiException.status == 400 && apiException.errors != null) {
      return AuthFailure.validation(apiException.errors!);
    }
    // Check for network-related errors and return an AuthFailure.network instance if applicable.
    if (apiException.type == 'connection') {
      return const AuthFailure.network();
    }
    // If none of the above conditions match, return an AuthFailure.unknown
    //instance with the error details.
    return AuthFailure.unknown(apiException.detail);
  }

  AuthFailure _mapLoginError(DioException exception) {
    // The _mapLoginError method maps DioExceptions to AuthFailure instances
    //based on the error details during the login process.
    final apiException = ApiException.fromDioException(exception);
    // Check the status code and error details to determine the appropriate AuthFailure to return.
    if (apiException.status == 401) {
      return const AuthFailure.invalidCredentials();
    }
    // Check for network-related errors and return an AuthFailure.network instance if applicable.
    if (apiException.type == 'connection') {
      return const AuthFailure.network();
    }
    // If none of the above conditions match, return an AuthFailure.unknown
    //instance with the error details.
    return AuthFailure.unknown(apiException.detail);
  }

  AuthFailure _mapGetCurrentUserError(DioException exception) {
    final apiException = ApiException.fromDioException(exception);

    if (apiException.status == 401) {
      return const AuthFailure.sessionExpired();
    }

    if (apiException.type == 'connection') {
      return const AuthFailure.network();
    }

    return AuthFailure.unknown(apiException.detail);
  }
}

@Riverpod(keepAlive: true)
AuthRepository authRepository(Ref ref) {
  final remoteDataSource = ref.watch(authRemoteDataSourceProvider);
  final tokenStorage = ref.watch(tokenStorageProvider);
  return AuthRepositoryImpl(remoteDataSource, tokenStorage);
}
