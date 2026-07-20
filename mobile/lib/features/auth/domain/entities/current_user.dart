import 'package:equatable/equatable.dart';

class CurrentUser extends Equatable {
  const CurrentUser({
    required this.id,
    required this.userName,
    required this.displayName,
    this.bio,
    this.avatarUrl,
    required this.followerCount,
    required this.followingCount,
    required this.isFollowedByMe,
    required this.createdAt,
  });

  final String id;
  final String userName;
  final String displayName;
  final String? bio;
  final String? avatarUrl;
  final int followerCount;
  final int followingCount;
  final bool isFollowedByMe;
  final DateTime createdAt;

  //copyWith method to create a new instance of CurrentUser with updated values
  CurrentUser copyWith({
    String? id,
    String? userName,
    String? displayName,
    String? bio,
    String? avatarUrl,
    int? followerCount,
    int? followingCount,
    bool? isFollowedByMe,
    DateTime? createdAt,
  }) {
    return CurrentUser(
      id: id ?? this.id,
      userName: userName ?? this.userName,
      displayName: displayName ?? this.displayName,
      bio: bio ?? this.bio,
      avatarUrl: avatarUrl ?? this.avatarUrl,
      followerCount: followerCount ?? this.followerCount,
      followingCount: followingCount ?? this.followingCount,
      isFollowedByMe: isFollowedByMe ?? this.isFollowedByMe,
      createdAt: createdAt ?? this.createdAt,
    );
  }

  @override
  List<Object?> get props => [
    id,
    userName,
    displayName,
    bio,
    avatarUrl,
    followerCount,
    followingCount,
    isFollowedByMe,
    createdAt,
  ];
}
