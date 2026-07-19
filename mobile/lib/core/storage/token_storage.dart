import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
part 'token_storage.g.dart';

// This class is responsible for storing and retrieving the access
//and refresh tokens securely using FlutterSecureStorage
class TokenStorage {
  // We use FlutterSecureStorage to store the tokens securely
  final FlutterSecureStorage _storage;
  // We define the keys for the access and refresh tokens
  static const String accessTokenKey = 'access_token';
  // We define the keys for the access and refresh tokens
  static const String refreshTokenKey = 'refresh_token';
  // The constructor takes a FlutterSecureStorage instance as a parameter
  TokenStorage(this._storage);

  // This method saves the access and refresh tokens securely
  Future<void> saveTokens({
    required String accessToken,
    required String refreshToken,
  }) async {
    await _storage.write(key: accessTokenKey, value: accessToken);
    await _storage.write(key: refreshTokenKey, value: refreshToken);
  }

  // This method retrieves the access token securely
  Future<String?> getAccessToken() async {
    return await _storage.read(key: accessTokenKey);
  }

  // This method retrieves the refresh token securely
  Future<String?> getRefreshToken() async {
    return await _storage.read(key: refreshTokenKey);
  }

  // This method deletes the access and refresh tokens securely
  Future<void> deleteTokens() async {
    await _storage.delete(key: accessTokenKey);
    await _storage.delete(key: refreshTokenKey);
  }
}

// This provider creates a singleton instance of TokenStorage
@Riverpod(keepAlive: true)
TokenStorage tokenStorage(Ref ref) {
  return TokenStorage(const FlutterSecureStorage());
}
