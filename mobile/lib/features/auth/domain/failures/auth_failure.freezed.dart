// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'auth_failure.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;
/// @nodoc
mixin _$AuthFailure {





@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is AuthFailure);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'AuthFailure()';
}


}

/// @nodoc
class $AuthFailureCopyWith<$Res>  {
$AuthFailureCopyWith(AuthFailure _, $Res Function(AuthFailure) __);
}


/// Adds pattern-matching-related methods to [AuthFailure].
extension AuthFailurePatterns on AuthFailure {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>({TResult Function( _InvalidCredentials value)?  invalidCredentials,TResult Function( _EmailAlreadyInUse value)?  emailAlreadyInUse,TResult Function( _UsernameAlreadyInUse value)?  usernameAlreadyInUse,TResult Function( _ValidationFailure value)?  validation,TResult Function( _SessionExpired value)?  sessionExpired,TResult Function( _NetworkFailure value)?  network,TResult Function( _UnknownFailure value)?  unknown,required TResult orElse(),}){
final _that = this;
switch (_that) {
case _InvalidCredentials() when invalidCredentials != null:
return invalidCredentials(_that);case _EmailAlreadyInUse() when emailAlreadyInUse != null:
return emailAlreadyInUse(_that);case _UsernameAlreadyInUse() when usernameAlreadyInUse != null:
return usernameAlreadyInUse(_that);case _ValidationFailure() when validation != null:
return validation(_that);case _SessionExpired() when sessionExpired != null:
return sessionExpired(_that);case _NetworkFailure() when network != null:
return network(_that);case _UnknownFailure() when unknown != null:
return unknown(_that);case _:
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

@optionalTypeArgs TResult map<TResult extends Object?>({required TResult Function( _InvalidCredentials value)  invalidCredentials,required TResult Function( _EmailAlreadyInUse value)  emailAlreadyInUse,required TResult Function( _UsernameAlreadyInUse value)  usernameAlreadyInUse,required TResult Function( _ValidationFailure value)  validation,required TResult Function( _SessionExpired value)  sessionExpired,required TResult Function( _NetworkFailure value)  network,required TResult Function( _UnknownFailure value)  unknown,}){
final _that = this;
switch (_that) {
case _InvalidCredentials():
return invalidCredentials(_that);case _EmailAlreadyInUse():
return emailAlreadyInUse(_that);case _UsernameAlreadyInUse():
return usernameAlreadyInUse(_that);case _ValidationFailure():
return validation(_that);case _SessionExpired():
return sessionExpired(_that);case _NetworkFailure():
return network(_that);case _UnknownFailure():
return unknown(_that);}
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>({TResult? Function( _InvalidCredentials value)?  invalidCredentials,TResult? Function( _EmailAlreadyInUse value)?  emailAlreadyInUse,TResult? Function( _UsernameAlreadyInUse value)?  usernameAlreadyInUse,TResult? Function( _ValidationFailure value)?  validation,TResult? Function( _SessionExpired value)?  sessionExpired,TResult? Function( _NetworkFailure value)?  network,TResult? Function( _UnknownFailure value)?  unknown,}){
final _that = this;
switch (_that) {
case _InvalidCredentials() when invalidCredentials != null:
return invalidCredentials(_that);case _EmailAlreadyInUse() when emailAlreadyInUse != null:
return emailAlreadyInUse(_that);case _UsernameAlreadyInUse() when usernameAlreadyInUse != null:
return usernameAlreadyInUse(_that);case _ValidationFailure() when validation != null:
return validation(_that);case _SessionExpired() when sessionExpired != null:
return sessionExpired(_that);case _NetworkFailure() when network != null:
return network(_that);case _UnknownFailure() when unknown != null:
return unknown(_that);case _:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>({TResult Function()?  invalidCredentials,TResult Function()?  emailAlreadyInUse,TResult Function()?  usernameAlreadyInUse,TResult Function( Map<String, List<String>> errors)?  validation,TResult Function()?  sessionExpired,TResult Function()?  network,TResult Function( String message)?  unknown,required TResult orElse(),}) {final _that = this;
switch (_that) {
case _InvalidCredentials() when invalidCredentials != null:
return invalidCredentials();case _EmailAlreadyInUse() when emailAlreadyInUse != null:
return emailAlreadyInUse();case _UsernameAlreadyInUse() when usernameAlreadyInUse != null:
return usernameAlreadyInUse();case _ValidationFailure() when validation != null:
return validation(_that.errors);case _SessionExpired() when sessionExpired != null:
return sessionExpired();case _NetworkFailure() when network != null:
return network();case _UnknownFailure() when unknown != null:
return unknown(_that.message);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>({required TResult Function()  invalidCredentials,required TResult Function()  emailAlreadyInUse,required TResult Function()  usernameAlreadyInUse,required TResult Function( Map<String, List<String>> errors)  validation,required TResult Function()  sessionExpired,required TResult Function()  network,required TResult Function( String message)  unknown,}) {final _that = this;
switch (_that) {
case _InvalidCredentials():
return invalidCredentials();case _EmailAlreadyInUse():
return emailAlreadyInUse();case _UsernameAlreadyInUse():
return usernameAlreadyInUse();case _ValidationFailure():
return validation(_that.errors);case _SessionExpired():
return sessionExpired();case _NetworkFailure():
return network();case _UnknownFailure():
return unknown(_that.message);}
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>({TResult? Function()?  invalidCredentials,TResult? Function()?  emailAlreadyInUse,TResult? Function()?  usernameAlreadyInUse,TResult? Function( Map<String, List<String>> errors)?  validation,TResult? Function()?  sessionExpired,TResult? Function()?  network,TResult? Function( String message)?  unknown,}) {final _that = this;
switch (_that) {
case _InvalidCredentials() when invalidCredentials != null:
return invalidCredentials();case _EmailAlreadyInUse() when emailAlreadyInUse != null:
return emailAlreadyInUse();case _UsernameAlreadyInUse() when usernameAlreadyInUse != null:
return usernameAlreadyInUse();case _ValidationFailure() when validation != null:
return validation(_that.errors);case _SessionExpired() when sessionExpired != null:
return sessionExpired();case _NetworkFailure() when network != null:
return network();case _UnknownFailure() when unknown != null:
return unknown(_that.message);case _:
  return null;

}
}

}

/// @nodoc


class _InvalidCredentials implements AuthFailure {
  const _InvalidCredentials();
  






@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _InvalidCredentials);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'AuthFailure.invalidCredentials()';
}


}




/// @nodoc


class _EmailAlreadyInUse implements AuthFailure {
  const _EmailAlreadyInUse();
  






@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _EmailAlreadyInUse);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'AuthFailure.emailAlreadyInUse()';
}


}




/// @nodoc


class _UsernameAlreadyInUse implements AuthFailure {
  const _UsernameAlreadyInUse();
  






@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UsernameAlreadyInUse);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'AuthFailure.usernameAlreadyInUse()';
}


}




/// @nodoc


class _ValidationFailure implements AuthFailure {
  const _ValidationFailure(final  Map<String, List<String>> errors): _errors = errors;
  

 final  Map<String, List<String>> _errors;
 Map<String, List<String>> get errors {
  if (_errors is EqualUnmodifiableMapView) return _errors;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(_errors);
}


/// Create a copy of AuthFailure
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ValidationFailureCopyWith<_ValidationFailure> get copyWith => __$ValidationFailureCopyWithImpl<_ValidationFailure>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ValidationFailure&&const DeepCollectionEquality().equals(other._errors, _errors));
}


@override
int get hashCode => Object.hash(runtimeType,const DeepCollectionEquality().hash(_errors));

@override
String toString() {
  return 'AuthFailure.validation(errors: $errors)';
}


}

/// @nodoc
abstract mixin class _$ValidationFailureCopyWith<$Res> implements $AuthFailureCopyWith<$Res> {
  factory _$ValidationFailureCopyWith(_ValidationFailure value, $Res Function(_ValidationFailure) _then) = __$ValidationFailureCopyWithImpl;
@useResult
$Res call({
 Map<String, List<String>> errors
});




}
/// @nodoc
class __$ValidationFailureCopyWithImpl<$Res>
    implements _$ValidationFailureCopyWith<$Res> {
  __$ValidationFailureCopyWithImpl(this._self, this._then);

  final _ValidationFailure _self;
  final $Res Function(_ValidationFailure) _then;

/// Create a copy of AuthFailure
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') $Res call({Object? errors = null,}) {
  return _then(_ValidationFailure(
null == errors ? _self._errors : errors // ignore: cast_nullable_to_non_nullable
as Map<String, List<String>>,
  ));
}


}

/// @nodoc


class _SessionExpired implements AuthFailure {
  const _SessionExpired();
  






@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _SessionExpired);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'AuthFailure.sessionExpired()';
}


}




/// @nodoc


class _NetworkFailure implements AuthFailure {
  const _NetworkFailure();
  






@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _NetworkFailure);
}


@override
int get hashCode => runtimeType.hashCode;

@override
String toString() {
  return 'AuthFailure.network()';
}


}




/// @nodoc


class _UnknownFailure implements AuthFailure {
  const _UnknownFailure(this.message);
  

 final  String message;

/// Create a copy of AuthFailure
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$UnknownFailureCopyWith<_UnknownFailure> get copyWith => __$UnknownFailureCopyWithImpl<_UnknownFailure>(this, _$identity);



@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _UnknownFailure&&(identical(other.message, message) || other.message == message));
}


@override
int get hashCode => Object.hash(runtimeType,message);

@override
String toString() {
  return 'AuthFailure.unknown(message: $message)';
}


}

/// @nodoc
abstract mixin class _$UnknownFailureCopyWith<$Res> implements $AuthFailureCopyWith<$Res> {
  factory _$UnknownFailureCopyWith(_UnknownFailure value, $Res Function(_UnknownFailure) _then) = __$UnknownFailureCopyWithImpl;
@useResult
$Res call({
 String message
});




}
/// @nodoc
class __$UnknownFailureCopyWithImpl<$Res>
    implements _$UnknownFailureCopyWith<$Res> {
  __$UnknownFailureCopyWithImpl(this._self, this._then);

  final _UnknownFailure _self;
  final $Res Function(_UnknownFailure) _then;

/// Create a copy of AuthFailure
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') $Res call({Object? message = null,}) {
  return _then(_UnknownFailure(
null == message ? _self.message : message // ignore: cast_nullable_to_non_nullable
as String,
  ));
}


}

// dart format on
