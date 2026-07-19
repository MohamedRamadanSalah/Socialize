import 'package:dio/dio.dart';
import 'package:src/core/network/di/api_config.dart';
import 'package:src/core/storage/token_storage.dart';

class AuthInterceptor extends Interceptor {
  // We need the token storage to get and save tokens
  final TokenStorage tokenStorage;
  // We need the dio instance to retry the original request after refreshing the token
  final Dio dio;
  // We create a separate dio instance for refreshing the token to avoid infinite loops
  final Dio _refreshDio;

  // The constructor takes the token storage and the dio instance as parameters
  // We initialize the _refreshDio with the base URL of the API
  AuthInterceptor({required this.tokenStorage, required this.dio})
    : _refreshDio = Dio(BaseOptions(baseUrl: apiBaseUrl));

  //............... The onRequest method is called before the request is sent ........................
  @override
  void onRequest(
    RequestOptions options,
    RequestInterceptorHandler handler,
  ) async {
    final accessToken = await tokenStorage.getAccessToken();
    if (accessToken != null) {
      options.headers['Authorization'] = 'Bearer $accessToken';
    }
    return handler.next(options);
  }
  // ....................................................................................................

  // The onError method is called when an error occurs during the request
  @override
  void onError(DioException err, ErrorInterceptorHandler handler) async {
    // We make sure this problem from the authentication and not from another problem
    if (err.response?.statusCode != 401) {
      return handler.next(err);
    }

    final refreshToken = await tokenStorage.getRefreshToken();
    // If there is no refresh token, we cannot refresh the access token, so we just forward the error
    if (refreshToken == null) {
      return handler.next(err);
    }

    try {
      // We try to refresh the access token using the refresh token
      final response = await _refreshDio.post(
        '/api/auth/refresh',
        data: {'refreshToken': refreshToken},
      );
      // If the refresh is successful, we save the new tokens and retry the original request
      final newAccessToken = response.data['accessToken'];
      final newRefreshToken = response.data['refreshToken'];
      await tokenStorage.saveTokens(
        accessToken: newAccessToken,
        refreshToken: newRefreshToken,
      );
      // We retry the original request with the new access token
      final retryRequest = err.requestOptions;
      retryRequest.headers['Authorization'] = 'Bearer $newAccessToken';
      // We use the original dio instance to retry the request,
      //so that it goes through the same interceptors
      final retryResponse = await dio.fetch(retryRequest);
      // We return the response of the retried request to the original caller
      return handler.resolve(retryResponse);
    } catch (_) {
      // If the refresh fails, we delete the tokens and forward the error
      await tokenStorage.deleteTokens();
      // We can also redirect the user to the login page here if needed
      return handler.next(err);
    }
  }
}
