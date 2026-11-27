# Changelog

## 1.0.1

- Renamed from Easy Rotate to Deft Hands with Keevi's blessing
- Remove the old `BepInEx/plugins/EasyRotate` folder when updating, since both would load together
- Settings now live in `com.skeptic043.monbazou.defthands.cfg` and start from defaults

## 1.0.0

### Initial Release
- Added smooth 3-axis mouse rotation around held items while holding `Left Alt`, pausing camera look while active
- Added `Right Mouse Button` axis swap between roll and yaw while suppressing held-item throws during rotation
- Added smooth scroll-wheel push/pull distance control with an option to fall back to vanilla scroll rotation
- Added live `BepInEx Configuration Manager` settings for keybinds, speed, default horizontal axis, and per-axis inversion
