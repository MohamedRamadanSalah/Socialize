import 'package:freezed_annotation/freezed_annotation.dart';

part 'current_user_dto.freezed.dart';
part 'current_user_dto.g.dart';

// This class is used to represent the current user data transfer object (DTO)
//in the application. It is annotated with @freezed to generate immutable value classes
//and provides a factory constructor for creating instances of CurrentUserDto.
//The fromJson method allows for deserialization from JSON format.
@freezed
abstract class CurrentUserDto with _$CurrentUserDto {
  const factory CurrentUserDto({
    required String id,
    required String userName,
    required String displayName,
    String? bio,
    String? avatarUrl,
    required int followerCount,
    required int followingCount,
    required bool isFollowedByMe,
    required DateTime createdAt,
  }) = _CurrentUserDto;
  // The fromJson method allows for deserialization from JSON format.
  factory CurrentUserDto.fromJson(Map<String, dynamic> json) =>
      _$CurrentUserDtoFromJson(json);
}
