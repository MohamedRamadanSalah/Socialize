import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:src/core/network/dio/dio_client.dart';
import 'package:src/features/auth/data/dto/auth_response_dto.dart';
import 'package:src/features/auth/data/dto/current_user_dto.dart';
part 'auth_remote_data_source.g.dart';

// This abstract class defines the contract for the authentication remote data source.
abstract class AuthRemoteDataSource {
  Future<AuthResponseDto> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  });

  // This method is responsible for logging in a user with the provided username/email and password.
  Future<AuthResponseDto> login({
    required String userNameOrEmail,
    required String password,
  });
  // This method is responsible for logging out a user by invalidating the provided refresh token.
  Future<void> logout({required String refreshToken});
  // This method is responsible for fetching the current user's data from the remote source.
  Future<CurrentUserDto> getMe();
}

// This class implements the AuthRemoteDataSource interface
//and provides the actual implementation for the authentication
//remote data source using the Dio package for making HTTP requests.
class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final Dio _dio;
  AuthRemoteDataSourceImpl(this._dio);
  @override
  Future<AuthResponseDto> register({
    required String userName,
    required String email,
    required String password,
    required String displayName,
  }) async {
    // Make a POST request to the '/api/auth/register' endpoint with the provided user details
    final response = await _dio.post(
      '/api/auth/register',
      // Send the user details in the request body as JSON
      data: {
        'userName': userName,
        'email': email,
        'password': password,
        'displayName': displayName,
      },
    );
    // Deserialize the response data into an AuthResponseDto object and return it
    return AuthResponseDto.fromJson(response.data);
  }

  @override
  Future<AuthResponseDto> login({
    required String userNameOrEmail,
    required String password,
  }) async {
    final response = await _dio.post(
      // Make a POST request to the '/api/auth/login' endpoint
      //with the provided username/email and password
      '/api/auth/login',
      // Send the login credentials in the request body as JSON
      data: {'userNameOrEmail': userNameOrEmail, 'password': password},
    );
    // Deserialize the response data into an AuthResponseDto object and return it
    return AuthResponseDto.fromJson(response.data);
  }

  @override
  Future<void> logout({required String refreshToken}) async {
    // Make a POST request to the '/api/auth/logout' endpoint
    //with the provided refresh token to log out the user
    await _dio.post(
      '/api/auth/logout',
      // Send the refresh token in the request body as JSON
      data: {'refreshToken': refreshToken},
    );
  }

  @override
  Future<CurrentUserDto> getMe() async {
    final response = await _dio.get('/api/auth/me');
    // Deserialize the response data into a CurrentUserDto object and return it
    return CurrentUserDto.fromJson(response.data);
  }
}

@Riverpod(keepAlive: true)
AuthRemoteDataSource authRemoteDataSource(Ref ref) {
  final dio = ref.watch(buildDioProvider);
  return AuthRemoteDataSourceImpl(dio);
}
