// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'current_user_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

_CurrentUserDto _$CurrentUserDtoFromJson(Map<String, dynamic> json) =>
    _CurrentUserDto(
      id: json['id'] as String,
      userName: json['userName'] as String,
      displayName: json['displayName'] as String,
      bio: json['bio'] as String?,
      avatarUrl: json['avatarUrl'] as String?,
      followerCount: (json['followerCount'] as num).toInt(),
      followingCount: (json['followingCount'] as num).toInt(),
      isFollowedByMe: json['isFollowedByMe'] as bool,
      createdAt: DateTime.parse(json['createdAt'] as String),
    );

Map<String, dynamic> _$CurrentUserDtoToJson(_CurrentUserDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'userName': instance.userName,
      'displayName': instance.displayName,
      'bio': instance.bio,
      'avatarUrl': instance.avatarUrl,
      'followerCount': instance.followerCount,
      'followingCount': instance.followingCount,
      'isFollowedByMe': instance.isFollowedByMe,
      'createdAt': instance.createdAt.toIso8601String(),
    };
