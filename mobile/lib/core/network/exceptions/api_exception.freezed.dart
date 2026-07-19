// GENERATED CODE - DO NOT MODIFY BY HAND
// coverage:ignore-file
// ignore_for_file: type=lint
// ignore_for_file: unused_element, deprecated_member_use, deprecated_member_use_from_same_package, use_function_type_syntax_for_parameters, unnecessary_const, avoid_init_to_null, invalid_override_different_default_values_named, prefer_expression_function_bodies, annotate_overrides, invalid_annotation_target, unnecessary_question_mark

part of 'api_exception.dart';

// **************************************************************************
// FreezedGenerator
// **************************************************************************

// dart format off
T _$identity<T>(T value) => value;

/// @nodoc
mixin _$ApiException {

 String get type; String get title; int get status; String get detail; Map<String, List<String>>? get errors;
/// Create a copy of ApiException
/// with the given fields replaced by the non-null parameter values.
@JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
$ApiExceptionCopyWith<ApiException> get copyWith => _$ApiExceptionCopyWithImpl<ApiException>(this as ApiException, _$identity);

  /// Serializes this ApiException to a JSON map.
  Map<String, dynamic> toJson();


@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is ApiException&&(identical(other.type, type) || other.type == type)&&(identical(other.title, title) || other.title == title)&&(identical(other.status, status) || other.status == status)&&(identical(other.detail, detail) || other.detail == detail)&&const DeepCollectionEquality().equals(other.errors, errors));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,type,title,status,detail,const DeepCollectionEquality().hash(errors));

@override
String toString() {
  return 'ApiException(type: $type, title: $title, status: $status, detail: $detail, errors: $errors)';
}


}

/// @nodoc
abstract mixin class $ApiExceptionCopyWith<$Res>  {
  factory $ApiExceptionCopyWith(ApiException value, $Res Function(ApiException) _then) = _$ApiExceptionCopyWithImpl;
@useResult
$Res call({
 String type, String title, int status, String detail, Map<String, List<String>>? errors
});




}
/// @nodoc
class _$ApiExceptionCopyWithImpl<$Res>
    implements $ApiExceptionCopyWith<$Res> {
  _$ApiExceptionCopyWithImpl(this._self, this._then);

  final ApiException _self;
  final $Res Function(ApiException) _then;

/// Create a copy of ApiException
/// with the given fields replaced by the non-null parameter values.
@pragma('vm:prefer-inline') @override $Res call({Object? type = null,Object? title = null,Object? status = null,Object? detail = null,Object? errors = freezed,}) {
  return _then(_self.copyWith(
type: null == type ? _self.type : type // ignore: cast_nullable_to_non_nullable
as String,title: null == title ? _self.title : title // ignore: cast_nullable_to_non_nullable
as String,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as int,detail: null == detail ? _self.detail : detail // ignore: cast_nullable_to_non_nullable
as String,errors: freezed == errors ? _self.errors : errors // ignore: cast_nullable_to_non_nullable
as Map<String, List<String>>?,
  ));
}

}


/// Adds pattern-matching-related methods to [ApiException].
extension ApiExceptionPatterns on ApiException {
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

@optionalTypeArgs TResult maybeMap<TResult extends Object?>(TResult Function( _ApiException value)?  $default,{required TResult orElse(),}){
final _that = this;
switch (_that) {
case _ApiException() when $default != null:
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

@optionalTypeArgs TResult map<TResult extends Object?>(TResult Function( _ApiException value)  $default,){
final _that = this;
switch (_that) {
case _ApiException():
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

@optionalTypeArgs TResult? mapOrNull<TResult extends Object?>(TResult? Function( _ApiException value)?  $default,){
final _that = this;
switch (_that) {
case _ApiException() when $default != null:
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

@optionalTypeArgs TResult maybeWhen<TResult extends Object?>(TResult Function( String type,  String title,  int status,  String detail,  Map<String, List<String>>? errors)?  $default,{required TResult orElse(),}) {final _that = this;
switch (_that) {
case _ApiException() when $default != null:
return $default(_that.type,_that.title,_that.status,_that.detail,_that.errors);case _:
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

@optionalTypeArgs TResult when<TResult extends Object?>(TResult Function( String type,  String title,  int status,  String detail,  Map<String, List<String>>? errors)  $default,) {final _that = this;
switch (_that) {
case _ApiException():
return $default(_that.type,_that.title,_that.status,_that.detail,_that.errors);case _:
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

@optionalTypeArgs TResult? whenOrNull<TResult extends Object?>(TResult? Function( String type,  String title,  int status,  String detail,  Map<String, List<String>>? errors)?  $default,) {final _that = this;
switch (_that) {
case _ApiException() when $default != null:
return $default(_that.type,_that.title,_that.status,_that.detail,_that.errors);case _:
  return null;

}
}

}

/// @nodoc
@JsonSerializable()

class _ApiException implements ApiException {
  const _ApiException({required this.type, required this.title, required this.status, required this.detail, final  Map<String, List<String>>? errors}): _errors = errors;
  factory _ApiException.fromJson(Map<String, dynamic> json) => _$ApiExceptionFromJson(json);

@override final  String type;
@override final  String title;
@override final  int status;
@override final  String detail;
 final  Map<String, List<String>>? _errors;
@override Map<String, List<String>>? get errors {
  final value = _errors;
  if (value == null) return null;
  if (_errors is EqualUnmodifiableMapView) return _errors;
  // ignore: implicit_dynamic_type
  return EqualUnmodifiableMapView(value);
}


/// Create a copy of ApiException
/// with the given fields replaced by the non-null parameter values.
@override @JsonKey(includeFromJson: false, includeToJson: false)
@pragma('vm:prefer-inline')
_$ApiExceptionCopyWith<_ApiException> get copyWith => __$ApiExceptionCopyWithImpl<_ApiException>(this, _$identity);

@override
Map<String, dynamic> toJson() {
  return _$ApiExceptionToJson(this, );
}

@override
bool operator ==(Object other) {
  return identical(this, other) || (other.runtimeType == runtimeType&&other is _ApiException&&(identical(other.type, type) || other.type == type)&&(identical(other.title, title) || other.title == title)&&(identical(other.status, status) || other.status == status)&&(identical(other.detail, detail) || other.detail == detail)&&const DeepCollectionEquality().equals(other._errors, _errors));
}

@JsonKey(includeFromJson: false, includeToJson: false)
@override
int get hashCode => Object.hash(runtimeType,type,title,status,detail,const DeepCollectionEquality().hash(_errors));

@override
String toString() {
  return 'ApiException(type: $type, title: $title, status: $status, detail: $detail, errors: $errors)';
}


}

/// @nodoc
abstract mixin class _$ApiExceptionCopyWith<$Res> implements $ApiExceptionCopyWith<$Res> {
  factory _$ApiExceptionCopyWith(_ApiException value, $Res Function(_ApiException) _then) = __$ApiExceptionCopyWithImpl;
@override @useResult
$Res call({
 String type, String title, int status, String detail, Map<String, List<String>>? errors
});




}
/// @nodoc
class __$ApiExceptionCopyWithImpl<$Res>
    implements _$ApiExceptionCopyWith<$Res> {
  __$ApiExceptionCopyWithImpl(this._self, this._then);

  final _ApiException _self;
  final $Res Function(_ApiException) _then;

/// Create a copy of ApiException
/// with the given fields replaced by the non-null parameter values.
@override @pragma('vm:prefer-inline') $Res call({Object? type = null,Object? title = null,Object? status = null,Object? detail = null,Object? errors = freezed,}) {
  return _then(_ApiException(
type: null == type ? _self.type : type // ignore: cast_nullable_to_non_nullable
as String,title: null == title ? _self.title : title // ignore: cast_nullable_to_non_nullable
as String,status: null == status ? _self.status : status // ignore: cast_nullable_to_non_nullable
as int,detail: null == detail ? _self.detail : detail // ignore: cast_nullable_to_non_nullable
as String,errors: freezed == errors ? _self._errors : errors // ignore: cast_nullable_to_non_nullable
as Map<String, List<String>>?,
  ));
}


}

// dart format on
