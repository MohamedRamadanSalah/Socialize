import '../../domain/entities/current_user.dart';
import '../dto/current_user_dto.dart';

// Mapper extension from DTO to Entity
extension CurrentUserDtoMapper on CurrentUserDto {
  // The toEntity method converts the CurrentUserDto instance to a CurrentUser entity.
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
