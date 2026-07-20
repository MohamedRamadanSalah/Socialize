import '../../domain/entities/current_user.dart';
import '../dto/current_user_dto.dart';

extension CurrentUserDtoMapper on CurrentUserDto {
  CurrentUser toEntity() {
    return CurrentUser(
      id: id,
      userName: userName,
      displayName: displayName,
      bio: bio,
      avatarUrl: avatarUrl,
      followerCount: followerCount,
      followingCount: followingCount,
      isFollowedByMe: isFollowedByMe,
      createdAt: createdAt,
    );
  }
}
