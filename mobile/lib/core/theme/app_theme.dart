import 'package:flutter/material.dart';

// Material 3 color scheme for the app, generated from the Nexus brand palette.
class AppColors {
  const AppColors._();

  static const primary = Color(0xFF004394);
  static const onPrimary = Color(0xFFFFFFFF);
  static const primaryContainer = Color(0xFF005AC1);
  static const onPrimaryContainer = Color(0xFFC8D8FF);
  static const primaryFixed = Color(0xFFD8E2FF);
  static const primaryFixedDim = Color(0xFFADC6FF);

  static const secondary = Color(0xFF535F70);
  static const onSecondary = Color(0xFFFFFFFF);
  static const secondaryContainer = Color(0xFFD7E3F8);
  static const onSecondaryContainer = Color(0xFF596576);

  static const tertiary = Color(0xFF533A73);
  static const onTertiary = Color(0xFFFFFFFF);
  static const tertiaryContainer = Color(0xFF6C528C);
  static const onTertiaryContainer = Color(0xFFE6CEFF);

  static const error = Color(0xFFBA1A1A);
  static const onError = Color(0xFFFFFFFF);
  static const errorContainer = Color(0xFFFFDAD6);
  static const onErrorContainer = Color(0xFF93000A);

  static const surface = Color(0xFFF9F9FC);
  static const onSurface = Color(0xFF1A1C1E);
  static const onSurfaceVariant = Color(0xFF424753);
  static const surfaceDim = Color(0xFFDADADC);
  static const surfaceBright = Color(0xFFF9F9FC);
  static const surfaceContainerLowest = Color(0xFFFFFFFF);
  static const surfaceContainerLow = Color(0xFFF3F3F6);
  static const surfaceContainer = Color(0xFFEEEEF0);
  static const surfaceContainerHigh = Color(0xFFE8E8EA);
  static const surfaceContainerHighest = Color(0xFFE2E2E5);

  static const outline = Color(0xFF727784);
  static const outlineVariant = Color(0xFFC2C6D5);
  static const inverseSurface = Color(0xFF2F3133);
  static const onInverseSurface = Color(0xFFF0F0F3);
  static const inversePrimary = Color(0xFFADC6FF);
  static const surfaceTint = Color(0xFF015AC1);

  // Splash screen background gradient (mirrors the Stitch design's animated gradient).
  static const splashGradient = [primary, primaryContainer, primaryFixed, primaryFixedDim];
}

class AppTheme {
  const AppTheme._();

  static final ColorScheme _lightScheme = ColorScheme(
    brightness: Brightness.light,
    primary: AppColors.primary,
    onPrimary: AppColors.onPrimary,
    primaryContainer: AppColors.primaryContainer,
    onPrimaryContainer: AppColors.onPrimaryContainer,
    secondary: AppColors.secondary,
    onSecondary: AppColors.onSecondary,
    secondaryContainer: AppColors.secondaryContainer,
    onSecondaryContainer: AppColors.onSecondaryContainer,
    tertiary: AppColors.tertiary,
    onTertiary: AppColors.onTertiary,
    tertiaryContainer: AppColors.tertiaryContainer,
    onTertiaryContainer: AppColors.onTertiaryContainer,
    error: AppColors.error,
    onError: AppColors.onError,
    errorContainer: AppColors.errorContainer,
    onErrorContainer: AppColors.onErrorContainer,
    surface: AppColors.surface,
    onSurface: AppColors.onSurface,
    surfaceContainerLowest: AppColors.surfaceContainerLowest,
    surfaceContainerLow: AppColors.surfaceContainerLow,
    surfaceContainer: AppColors.surfaceContainer,
    surfaceContainerHigh: AppColors.surfaceContainerHigh,
    surfaceContainerHighest: AppColors.surfaceContainerHighest,
    surfaceDim: AppColors.surfaceDim,
    surfaceBright: AppColors.surfaceBright,
    onSurfaceVariant: AppColors.onSurfaceVariant,
    outline: AppColors.outline,
    outlineVariant: AppColors.outlineVariant,
    inverseSurface: AppColors.inverseSurface,
    onInverseSurface: AppColors.onInverseSurface,
    inversePrimary: AppColors.inversePrimary,
    surfaceTint: AppColors.surfaceTint,
    shadow: const Color(0xFF000000),
    scrim: const Color(0xFF000000),
  );

  static ThemeData get light => ThemeData(
    useMaterial3: true,
    colorScheme: _lightScheme,
    scaffoldBackgroundColor: _lightScheme.surface,
    fontFamily: 'Inter',
  );
}
