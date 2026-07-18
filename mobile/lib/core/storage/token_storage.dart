import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:riverpod_annotation/riverpod_annotation.dart';
part 'token_storage.g.dart';


class TokenStorage{
  final FlutterSecureStorage _storage ;
  static const String accessTokenKey = 'access_token';
  static const String refreshTokenKey = 'refresh_token';
  TokenStorage(this._storage);


  Future<void> saveTokens({required String accessToken, required String refreshToken}) async {
    await _storage.write(key: accessTokenKey, value: accessToken);
    await _storage.write(key: refreshTokenKey, value: refreshToken);
  }

  Future<String?> getAccessToken() async {
    return await _storage.read(key: accessTokenKey);
  }

  Future<String?> getRefreshToken() async {
    return await _storage.read(key: refreshTokenKey);
  }

  Future<void> deleteTokens() async {
    await _storage.delete(key: accessTokenKey);
    await _storage.delete(key: refreshTokenKey);
  }

}


@Riverpod(keepAlive: true)
TokenStorage tokenStorage(Ref ref) {
  return TokenStorage(const FlutterSecureStorage());
}