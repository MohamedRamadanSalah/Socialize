// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'current_user_dto.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$CurrentUserDto {

 String get id; String get userName; String get displayName; String? get bio; String? get avatarUrl; int get followerCount; int get followingCount; bool get isFollowedByMe; DateTime get createdAt;
/// Create a copy of CurrentUserDto
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$CurrentUserDtoCopyWith<CurrentUserDto> get copyWith => _$CurrentUserDtoCopyWithImpl<CurrentUserDto>(this as CurrentUserDto, _$identity);

  /// Serializes this CurrentUserDto to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is CurrentUserDto&&(identical(other.id, id) || other.id == id)&&(identical(other.userName, userName) || other.userName == userName)&&(identical(other.displayName, displayName) || other.displayName == displayName)&&(identical(other.bio, bio) || other.bio == bio)&&(identical(other.avatarUrl, avatarUrl) || other.avatarUrl == avatarUrl)&&(identical(other.followerCount, followerCount) || other.followerCount == followerCount)&&(identical(other.followingCount, followingCount) || other.followingCount == followingCount)&&(identical(other.isFollowedByMe, isFollowedByMe) || other.isFollowedByMe == isFollowedByMe)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,userName,displayName,bio,avatarUrl,followerCount,followingCount,isFollowedByMe,createdAt);

@override
String toString() {
  return 'CurrentUserDto(id: $id, userName: $userName, displayName: $displayName, bio: $bio, avatarUrl: $avatarUrl, followerCount: $followerCount, followingCount: $followingCount, isFollowedByMe: $isFollowedByMe, createdAt: $createdAt)';
}


}

/// @nodoc
abstract mixin class $CurrentUserDtoCopyWith<$Res>  {
  factory $CurrentUserDtoCopyWith(CurrentUserDto value, $Res Function(CurrentUserDto) _then) = _$CurrentUserDtoCopyWithImpl;
@useResult
$Res call({
 String id, String userName, String displayName, String? bio, String? avatarUrl, int followerCount, int followingCount, bool isFollowedByMe, DateTime createdAt
});




}
/// @nodoc
class _$CurrentUserDtoCopyWithImpl<$Res>
    implements $CurrentUserDtoCopyWith<$Res> {
  _$CurrentUserDtoCopyWithImpl(this._self, this._then);

  final CurrentUserDto _self;
  final $Res Function(CurrentUserDto) _then;

/// Create a copy of CurrentUserDto
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? id = null,Object? userName = null,Object? displayName = null,Object? bio = freezed,Object? avatarUrl = freezed,Object? followerCount = null,Object? followingCount = null,Object? isFollowedByMe = null,Object? createdAt = null,}) {
  return _then(_self.copyWith(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,userName: null == userName ? _self.userName : userName // ignore: cast_nullable_to_non_nullable
as String,displayName: null == displayName ? _self.displayName : displayName // ignore: cast_nullable_to_non_nullable
as String,bio: freezed == bio ? _self.bio : bio // ignore: cast_nullable_to_non_nullable
as String?,avatarUrl: freezed == avatarUrl ? _self.avatarUrl : avatarUrl // ignore: cast_nullable_to_non_nullable
as String?,followerCount: null == followerCount ? _self.followerCount : followerCount // ignore: cast_nullable_to_non_nullable
as int,followingCount: null == followingCount ? _self.followingCount : followingCount // ignore: cast_nullable_to_non_nullable
as int,isFollowedByMe: null == isFollowedByMe ? _self.isFollowedByMe : isFollowedByMe // ignore: cast_nullable_to_non_nullable
as bool,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as DateTime,
  ));
}

}


/// Adds pattern-matching-related methods to [CurrentUserDto].
extension CurrentUserDtoPatterns on CurrentUserDto {
/// A variant of `map` that fallback to returning `orElse`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _CurrentUserDto value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _CurrentUserDto() when $default != null:
return $default(_that);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// Callbacks receives the raw object, upcasted.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case final Subclass2 value:
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _CurrentUserDto value)  $default,){
final _that = this;
switch (_that) {
case _CurrentUserDto():
return $default(_that);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `map` that fallback to returning `null`.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case final Subclass value:
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _CurrentUserDto value)?  $default,){
final _that = this;
switch (_that) {
case _CurrentUserDto() when $default != null:
return $default(_that);case _:
  return null;

}
}
/// A variant of `when` that fallback to an `orElse` callback.
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return orElse();
/// }
/// ```

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String id,  String userName,  String displayName,  String? bio,  String? avatarUrl,  int followerCount,  int followingCount,  bool isFollowedByMe,  DateTime createdAt)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _CurrentUserDto() when $default != null:
return $default(_that.id,_that.userName,_that.displayName,_that.bio,_that.avatarUrl,_that.followerCount,_that.followingCount,_that.isFollowedByMe,_that.createdAt);case _:
  return orElse();

}
}
/// A `switch`-like method, using callbacks.
///
/// As opposed to `map`, this offers destructuring.
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case Subclass2(:final field2):
///     return ...;
/// }
/// ```

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String id,  String userName,  String displayName,  String? bio,  String? avatarUrl,  int followerCount,  int followingCount,  bool isFollowedByMe,  DateTime createdAt)  $default,) {final _that = this;
switch (_that) {
case _CurrentUserDto():
return $default(_that.id,_that.userName,_that.displayName,_that.bio,_that.avatarUrl,_that.followerCount,_that.followingCount,_that.isFollowedByMe,_that.createdAt);case _:
  throw StateError('Unexpected subclass');

}
}
/// A variant of `when` that fallback to returning `null`
///
/// It is equivalent to doing:
/// ```dart
/// switch (sealedClass) {
///   case Subclass(:final field):
///     return ...;
///   case _:
///     return null;
/// }
/// ```

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String id,  String userName,  String displayName,  String? bio,  String? avatarUrl,  int followerCount,  int followingCount,  bool isFollowedByMe,  DateTime createdAt)?  $default,) {final _that = this;
switch (_that) {
case _CurrentUserDto() when $default != null:
return $default(_that.id,_that.userName,_that.displayName,_that.bio,_that.avatarUrl,_that.followerCount,_that.followingCount,_that.isFollowedByMe,_that.createdAt);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _CurrentUserDto implements CurrentUserDto {
  const _CurrentUserDto({required this.id, required this.userName, required this.displayName, this.bio, this.avatarUrl, required this.followerCount, required this.followingCount, required this.isFollowedByMe, required this.createdAt});
  factory _CurrentUserDto.fromJson(Map<String, dynamic> json) => _$CurrentUserDtoFromJson(json);

@override final  String id;
@override final  String userName;
@override final  String displayName;
@override final  String? bio;
@override final  String? avatarUrl;
@override final  int followerCount;
@override final  int followingCount;
@override final  bool isFollowedByMe;
@override final  DateTime createdAt;

/// Create a copy of CurrentUserDto
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$CurrentUserDtoCopyWith<_CurrentUserDto> get copyWith => __$CurrentUserDtoCopyWithImpl<_CurrentUserDto>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$CurrentUserDtoToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _CurrentUserDto&&(identical(other.id, id) || other.id == id)&&(identical(other.userName, userName) || other.userName == userName)&&(identical(other.displayName, displayName) || other.displayName == displayName)&&(identical(other.bio, bio) || other.bio == bio)&&(identical(other.avatarUrl, avatarUrl) || other.avatarUrl == avatarUrl)&&(identical(other.followerCount, followerCount) || other.followerCount == followerCount)&&(identical(other.followingCount, followingCount) || other.followingCount == followingCount)&&(identical(other.isFollowedByMe, isFollowedByMe) || other.isFollowedByMe == isFollowedByMe)&&(identical(other.createdAt, createdAt) || other.createdAt == createdAt));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,id,userName,displayName,bio,avatarUrl,followerCount,followingCount,isFollowedByMe,createdAt);

@override
String toString() {
  return 'CurrentUserDto(id: $id, userName: $userName, displayName: $displayName, bio: $bio, avatarUrl: $avatarUrl, followerCount: $followerCount, followingCount: $followingCount, isFollowedByMe: $isFollowedByMe, createdAt: $createdAt)';
}


}

/// @nodoc
abstract mixin class _$CurrentUserDtoCopyWith<$Res> implements $CurrentUserDtoCopyWith<$Res> {
  factory _$CurrentUserDtoCopyWith(_CurrentUserDto value, $Res Function(_CurrentUserDto) _then) = __$CurrentUserDtoCopyWithImpl;
@override @useResult
$Res call({
 String id, String userName, String displayName, String? bio, String? avatarUrl, int followerCount, int followingCount, bool isFollowedByMe, DateTime createdAt
});




}
/// @nodoc
class __$CurrentUserDtoCopyWithImpl<$Res>
    implements _$CurrentUserDtoCopyWith<$Res> {
  __$CurrentUserDtoCopyWithImpl(this._self, this._then);

  final _CurrentUserDto _self;
  final $Res Function(_CurrentUserDto) _then;

/// Create a copy of CurrentUserDto
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? id = null,Object? userName = null,Object? displayName = null,Object? bio = freezed,Object? avatarUrl = freezed,Object? followerCount = null,Object? followingCount = null,Object? isFollowedByMe = null,Object? createdAt = null,}) {
  return _then(_CurrentUserDto(
id: null == id ? _self.id : id // ignore: cast_nullable_to_non_nullable
as String,userName: null == userName ? _self.userName : userName // ignore: cast_nullable_to_non_nullable
as String,displayName: null == displayName ? _self.displayName : displayName // ignore: cast_nullable_to_non_nullable
as String,bio: freezed == bio ? _self.bio : bio // ignore: cast_nullable_to_non_nullable
as String?,avatarUrl: freezed == avatarUrl ? _self.avatarUrl : avatarUrl // ignore: cast_nullable_to_non_nullable
as String?,followerCount: null == followerCount ? _self.followerCount : followerCount // ignore: cast_nullable_to_non_nullable
as int,followingCount: null == followingCount ? _self.followingCount : followingCount // ignore: cast_nullable_to_non_nullable
as int,isFollowedByMe: null == isFollowedByMe ? _self.isFollowedByMe : isFollowedByMe // ignore: cast_nullable_to_non_nullable
as bool,createdAt: null == createdAt ? _self.createdAt : createdAt // ignore: cast_nullable_to_non_nullable
as DateTime,
  ));
}


}

// dart format on
