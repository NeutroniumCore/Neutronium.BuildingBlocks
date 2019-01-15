<p align="center"><img <p align="center"><img width="200"src="./images/logo.png"></p>

<h1 align="center">Neutronium Building Blocks</h1>

[![Build status](https://img.shields.io/appveyor/ci/David-Desmaisons/neutronium-buildingblocks.svg)](https://ci.appveyor.com/project/David-Desmaisons/neutronium-buildingblocks)
[![NuGet Badge](https://buildstats.info/nuget/Neutronium.BuildingBlocks.Standard)](https://www.nuget.org/packages/Neutronium.BuildingBlocks.Standard/)
[![MIT License](https://img.shields.io/github/license/NeutroniumCore/ViewModel.Tools.svg)](https://github.com/NeutroniumCore/ViewModel.Tools/blob/master/LICENSE)

## Description

Neutronium.BuildingBlocks provides opinionated solutions to build [Neutronium](https://github.com/NeutroniumCore/Neutronium) application:

- [`ApplicationTools`](./applicationTools) provides interfaces for common application features such as native message box, native file and directory picker...
- [`Wpf`](./wpf) provides an implementation for `ApplicationTools` interfaces based on Wpf framework.
- [`Application`](./application) provides solution for application architecture including:
  - routing (integrated with vue via [vue-cli-plugin-neutronium](https://github.com/NeutroniumCore/vue-cli-plugin-neutronium)).
  - Dependency injection for main View-models
  - API for modal and notifications
- [`SetUp`](./setup) aims at making it easy to switch between different debug modes and make the usage of `live reload` easy. It provides utility to run npm scripts and to manage application mode.

Neutronium [Visual Studio templates](https://marketplace.visualstudio.com/manage/publishers/daviddes?src=DavidDes.NeutroniumApplicationTemplates) show cases usages of Neutronium.BuildingBlocks.

See also [Neutronium.SPA.Demo](https://github.com/NeutroniumCore/Neutronium.SPA.Demo) and [Neutronium.Simple.Template](https://github.com/NeutroniumCore/Neutronium.Simple.Template) for corresponding sample usage.

## Context

By provided MVVM bindings compatible with Wpf, Neutronium is an unopinionated framework that leaves the users free to build its application logics.


Neutronium.BuildingBlocks fills the gap between an hello-world example and a complete application by solving some common challenges such as routing, dependency injection or layer isolation.
