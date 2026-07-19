//ِِApiException contain ------>  type,title,status,detail,errors

import 'package:dio/dio.dart';
import 'package:freezed_annotation/freezed_annotation.dart';

part 'api_exception.freezed.dart';
part 'api_exception.g.dart';

@freezed
abstract class ApiException with _$ApiException {
  const factory ApiException({
    required String type,
    required String title,
    required int status,
    required String detail,
    Map<String, List<String>>? errors,
  }) = _ApiException;

  factory ApiException.fromJson(Map<String, dynamic> json) =>
      _$ApiExceptionFromJson(json);
  factory ApiException.fromDioException(DioException exception) {
    final data = exception.response?.data;
    if (data is Map<String, dynamic>) {
      return ApiException.fromJson(data);
    }

    return ApiException(
      type: 'connection',
      title: 'Network Error',
      status: exception.response?.statusCode ?? 0,
      detail: exception.message ?? 'An unknown network error occurred',
    );
  }
}
