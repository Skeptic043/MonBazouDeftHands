# Easy Rotate

Adds smooth 3-axis mouse rotation and scroll-wheel push/pull for held items.

## Install

### Manual installation

1. Install [BepInEx 5](https://github.com/BepInEx/BepInEx/releases) in your Mon Bazou game folder, following its installation instructions.
2. Download and extract the [Easy Rotate ZIP](https://github.com/Skeptic043/MonBazouEasyRotate/releases/latest). Copy its `BepInEx/plugins/EasyRotate` folder into `BepInEx/plugins` in your Mon Bazou folder.
3. Launch Mon Bazou normally.

## How it works

While carrying an item, hold `Left Alt` and move the mouse to pitch and roll it around its center while camera look is paused. Hold `Right Mouse Button` together with `Left Alt` to swap horizontal movement between rolling and yawing without throwing the item.

Scroll the mouse wheel to smoothly move the held item closer or farther. Grid Mode, snapped car parts and bolts, equipped tools, and vehicle cameras are left untouched.

## Configuration

Settings are in `BepInEx/config/com.skeptic043.monbazou.easyrotate.cfg` after the first launch, or in game with the BepInEx 5 version of [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager).

| Setting | Default | Options |
| --- | --- | --- |
| `Enable Mouse Rotation` | `true` | Turn mouse-driven item rotation on or off. |
| `Rotate Key` | `LeftAlt` | Key held to pause camera look and rotate the held item. |
| `Swap Axis Key` | `Mouse1` | Key held with `Rotate Key` to switch horizontal movement between `Roll` and `Yaw`. |
| `Default Horizontal Axis` | `Roll` | `Roll` or `Yaw` when `Swap Axis Key` is not held. |
| `Rotation Speed` | `2` | Mouse rotation speed multiplier (`0.5` to `10`). |
| `Scroll Push/Pull` | `true` | Use the scroll wheel to move the held item closer or farther. |
| `Vanilla Scroll Rotation` | `true` | Keep Mon Bazou's default 15-degree scroll pitch when `Scroll Push/Pull` is off. |
| `Invert Pitch` | `false` | Reverse vertical mouse movement. |
| `Invert Roll` | `false` | Reverse horizontal roll. |
| `Invert Yaw` | `false` | Reverse horizontal yaw. |

## Compatibility

No known incompatibilities.

## Credits

Inspired by Keevi's [Deft Hands](https://github.com/KeeviDev/SailwindDeftHands) mod for Sailwind. Easy Rotate is a separate, original implementation written for Mon Bazou.

## AI Use

AI was used to write all of the code in this project. The original concept, design direction, testing, debugging, and release decisions are my own. If you prefer not to use mods developed with AI assistance, I understand and respect that choice.

## Links

[Report an issue](https://github.com/Skeptic043/MonBazouEasyRotate/issues) · [Source code](https://github.com/Skeptic043/MonBazouEasyRotate) · [Release notes](https://github.com/Skeptic043/MonBazouEasyRotate/blob/main/CHANGELOG.md) · [Build instructions](https://github.com/Skeptic043/MonBazouEasyRotate/blob/main/BUILDING.md) · [License](https://github.com/Skeptic043/MonBazouEasyRotate/blob/main/LICENSE) · [Support on Ko-fi](https://ko-fi.com/skeptic043) · skeptic043
