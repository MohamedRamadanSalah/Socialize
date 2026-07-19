//ِِApiException contain ------>  type,title,status,detail,errors

import 'package:dio/dio.dart';
import 'package:freezed_annotation/freezed_annotation.dart';

part 'api_exception.freezed.dart';
part 'api_exception.g.dart';

// The ApiException class represents an exception that occurs when making API requests
@freezed
abstract class ApiException with _$ApiException {
  // The factory constructor takes the type, title, status, detail, and errors as parameters
  const factory ApiException({
    required String type,
    required String title,
    required int status,
    required String detail,
    Map<String, List<String>>? errors,
  }) = _ApiException;

  // The fromJson method is used to deserialize the exception from JSON
  factory ApiException.fromJson(Map<String, dynamic> json) =>
      _$ApiExceptionFromJson(json);

  // The fromDioException method is used to create an ApiException from a DioException
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
