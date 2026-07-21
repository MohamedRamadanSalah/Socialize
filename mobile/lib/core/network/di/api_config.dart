import 'package:flutter/foundation.dart' show TargetPlatform, defaultTargetPlatform, kIsWeb;

// A physical device can't reach the dev machine via `localhost` or `10.0.2.2`
// (those only make sense for the AVD emulator's virtual NAT) — it needs the
// dev machine's real LAN IP instead. Pass it at run time, e.g.:
//   flutter run --dart-define=API_HOST=192.168.0.107
const String _apiHostOverride = String.fromEnvironment('API_HOST');

// The Android emulator can't reach the host machine via `localhost` — it has
// its own loopback. `10.0.2.2` is the emulator's special alias back to the
// host's `localhost`. Every other target (iOS simulator, desktop, web) runs
// on/alongside the host directly, so `localhost` resolves correctly there.
String get apiBaseUrl {
  if (_apiHostOverride.isNotEmpty) {
    return 'http://$_apiHostOverride:8081';
  }
  if (!kIsWeb && defaultTargetPlatform == TargetPlatform.android) {
    return 'http://10.0.2.2:8081';
  }
  return 'http://localhost:8081';
}
