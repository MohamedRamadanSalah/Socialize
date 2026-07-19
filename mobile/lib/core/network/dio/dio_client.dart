import 'package:dio/dio.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
import 'package:src/core/network/di/api_config.dart';
import 'package:src/core/network/interceptors/auth_interceptor.dart';
import 'package:src/core/storage/token_storage.dart';

part 'dio_client.g.dart';

@riverpod
Dio buildDio(Ref ref) {
  final TokenStorage tokenStorage = ref.watch(tokenStorageProvider);
  final dio = Dio(BaseOptions(baseUrl: apiBaseUrl));
  dio.interceptors.add(AuthInterceptor(tokenStorage: tokenStorage, dio: dio));

  return dio;
}
