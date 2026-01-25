<div align="center">
  <img src="gitassets/blowoutbox.svg" width="80px" alt="b&box logo">

  [Facepunch Website] | [Getting Started S&Box] | [S&Box Forums] | [S&Box Documentation] | [Contributing]
</div>

[Facepunch Website]: https://sbox.game/
[Getting Started S&Box]: https://sbox.game/dev/doc/about/getting-started/first-steps/
[S&Box Forums]: https://sbox.game/f/
[S&Box Documentation]: https://sbox.game/dev/doc/
[Contributing]: CONTRIBUTING.md

# About BlowoutTeamSoft Engine
<b>BlowoutTeamSoft Engine</b> is a cross-engine framework and architectural layer designed to develop games and interactive applications independently of a specific game engine. BlowoutTeamSoft Project is created as an engine-mimic or engine-virtualbox. You can develop gameplay, systems, and logic once, and then use them in other engines. 

## Basic idea of BlowoutTeamSoft
Basic idea that engine is not a GameObject, Actor, or Node. An engine is an <b>architecture</b>. BlowoutTeamSoft Engine takes the core architecture, mathematics, game systems, and API beyond a specific engine and provides uniform code style, unified interfaces, uniform engine loops and uniform engine API.

## 🧱 Architecture
To develop a universal system, BlowoutTeamSoft uses <b>game systems</b> instead of components <i>(Not to be confused with the ECS architecture, we still adhere to the GameObject game system)</i>. Instead of being directly dependent on components (like a <i>UnityEngine.Component</i>, <i>Godot.Node</i> and else.) Game Systems concept is used.

```CSharp
public interface IBlowoutPhysicsBody : IBlowoutGameSystem
{
    float Mass { get; set; }
}
```

And then we are using AddGameSystem/GetGameSystem API.

```CSharp
entity.AddGameSystem<IBlowoutPhysicsBody>();

entity.GetGameSystem<IBlowoutPhysicsBody>();
entity.GetGameSystems<IWeaponModifier>();

entity.GetGameSystemInChildren<IAudioSource>();

```

<br/>

Game doesn't know what's under the hood of Rigidbody, RigidBody3D or something else. All dependencies remain at the interface level. This system is more flexible, which allows you to interact with game objects as you like.

BlowoutTeamSoft using uniform math system (`System.Numerics` for <b>vector</b> math and `BlowoutMath` for engine math). BlowoutTeamSoft also supports `Vector3Int` and `Vector2Int`.

```CSharp
BlowoutMath.Lerp(x, y, t);
```

### BlowoutTeamSoft SDK
BlowoutTeamSoft also supports a <b>full-fledged flexible SDK</b> for developing modifications, addons, or systems in other languages <i>(currently only the `LUA` language is supported, but others will be added in the future)</i>.

## ❤️ Motivation
<b>BlowoutTeamSoft Engine</b> is largely inspired by the Valve ecosystem and their approach to creating engines, primarily Source and Source 2. We sincerely love architectural pragmatism Source, an engineering approach to gameplay and tools and also... <b>respect</b> for long-lived code and backward compatibility.

We <b>truly love Valve engine and games</b>, first of all <b>Source</b> and <b>Source 2</b>.

# About core engine: s&box

<b>s&box</b> is a modern game engine, built on <b>Valve's Source 2</b> and the latest .NET technology, it provides a modern intuitive editor for creating games.

![s&box editor](https://files.facepunch.com/matt/1b2211b1/sbox-dev_FoZ5NNZQTi.jpg)

If your goal is to create games using s&box, please start with the [getting started guide](https://sbox.game/dev/doc/about/getting-started/first-steps/).
This repository is for building the engine from source for those who want to contribute to the development of the engine.

## Getting the Engine

### Steam

You can download and install the s&box editor directly from [Steam](https://sbox.game/give-me-that).

### Compiling from Source

If you want to build from source, this repository includes all the necessary files to compile the engine yourself.

#### Prerequisites

* [Git](https://git-scm.com/install/windows)
* [Visual Studio 2026](https://visualstudio.microsoft.com/)
* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download)

#### Building

```bash
# Clone the repo
git clone https://github.com/Facepunch/sbox-public.git
```

Once you've cloned the repo simply run `Bootstrap.bat` which will download dependencies and build the engine.

The game and editor can be run from the binaries in the game folder.

## Contributing

If you would like to contribute to the engine, please see the [contributing guide](CONTRIBUTING.md).

If you want to report bugs or request new features, see [sbox-issues](https://github.com/Facepunch/sbox-issues/).

## Documentation

Full documentation, tutorials, and API references are available at [sbox.game/dev/](https://sbox.game/dev/).

## License

The s&box engine source code is licensed under the [MIT License](LICENSE.md).

Certain native binaries in `game/bin` are not covered by the MIT license. These binaries are distributed under the s&box EULA. You must agree to the terms of the EULA to use them.

This project includes third-party components that are separately licensed.
Those components are not covered by the MIT license above and remain subject
to their original licenses as indicated in `game/thirdpartylegalnotices`.