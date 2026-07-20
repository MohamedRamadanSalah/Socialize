import 'package:freezed_annotation/freezed_annotation.dart';

import 'current_user_dto.dart';

part 'auth_response_dto.freezed.dart';
part 'auth_response_dto.g.dart';

// This class is used to represent the authentication response data transfer object (DTO)
// in the application. It is annotated with @freezed to generate immutable value classes
@freezed
abstract class AuthResponseDto with _$AuthResponseDto {
  const factory AuthResponseDto({
    required String accessToken,
    required String refreshToken,
    required DateTime accessTokenExpiresAt,
    // The user field represents the current user data transfer object (DTO)
    //associated with the authentication response.
    required CurrentUserDto user,
  }) = _AuthResponseDto;
  // The fromJson method allows for deserialization from JSON format.
  factory AuthResponseDto.fromJson(Map<String, dynamic> json) =>
      _$AuthResponseDtoFromJson(json);
}
