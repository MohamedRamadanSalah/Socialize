import 'package:freezed_annotation/freezed_annotation.dart';

part 'current_user_dto.freezed.dart';
part 'current_user_dto.g.dart';

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

  factory CurrentUserDto.fromJson(Map<String, dynamic> json) =>
      _$CurrentUserDtoFromJson(json);
}